/*
 * Copyright 2018 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this
 * software and associated documentation files (the "Software"), to deal in the Software
 * without restriction, including without limitation the rights to use, copy, modify,
 * merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

package arcap.micclient.aws;

import software.amazon.awssdk.auth.credentials.AwsBasicCredentials;
import software.amazon.awssdk.auth.credentials.AwsCredentials;
import software.amazon.awssdk.auth.credentials.AwsCredentialsProvider;
import software.amazon.awssdk.auth.credentials.StaticCredentialsProvider;
import software.amazon.awssdk.regions.Region;
import software.amazon.awssdk.services.transcribestreaming.TranscribeStreamingAsyncClient;
import software.amazon.awssdk.services.transcribestreaming.model.LanguageCode;
import software.amazon.awssdk.services.transcribestreaming.model.MediaEncoding;
import software.amazon.awssdk.services.transcribestreaming.model.StartStreamTranscriptionRequest;

import javax.sound.sampled.*;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.Scanner;
import java.util.concurrent.CompletableFuture;

/**
 * This wraps the TranscribeStreamingAsyncClient with easier to use methods for quicker integration with the GUI. This
 * also provides examples on how to handle the various exceptions that can be thrown and how to implement a request
 * stream for input to the streaming service.
 */
public class TranscribeStreamingClientWrapper {

    private TranscribeStreamingRetryClient client;
    private AudioStreamPublisher requestStream;

    private static boolean loadedCredentials = false;
    private static String id = null;
    private static String secret = null;


    public TranscribeStreamingClientWrapper() {
        String endpoint = "https://transcribestreaming.us-west-2.amazonaws.com";
        try {
            client = new TranscribeStreamingRetryClient(
                    TranscribeStreamingAsyncClient.builder()
                            .credentialsProvider(getCredentials())
                            .endpointOverride(new URI(endpoint))
                            .region(Region.US_WEST_2)
                            .build());
        } catch (URISyntaxException e) {
            e.printStackTrace();
        }
    }

    /**
     * Start real-time speech recognition. Transcribe streaming java client uses Reactive-streams interface.
     * For reference on Reactive-streams: https://github.com/reactive-streams/reactive-streams-jvm
     *
     * @param responseHandler StartStreamTranscriptionResponseHandler that determines what to do with the response
     *                        objects as they are received from the streaming service
     */
    public CompletableFuture<Void> startTranscription(StreamTranscriptionBehavior responseHandler) {
        if (requestStream != null) {
            throw new IllegalStateException("Stream is already open");
        }
        try {
            System.out.println("setting up completable future");
            int sampleRate = 16_000; //default
            requestStream = new AudioStreamPublisher(getStreamFromMic());
            System.out.println("returning completable future");
            return client.startStreamTranscription(
                    //Request parameters. Refer to API documentation for details.
                    getRequest(sampleRate),
                    //AudioEvent publisher containing "chunks" of audio data to transcribe
                    requestStream,
                    //Defines what to do with transcripts as they arrive from the service
                    responseHandler);
        } catch (LineUnavailableException e) {
            e.printStackTrace();
            CompletableFuture<Void> failedFuture = new CompletableFuture<>();
            failedFuture.completeExceptionally(e);
            return failedFuture;
        }
    }

    /**
     * Stop in-progress transcription if there is one in progress by closing the request stream
     */
    public void stopTranscription() {
        if (requestStream != null) {
            try {
                requestStream.inputStream.close();
            } catch (IOException ex) {
                System.out.println("Error stopping input stream: " + ex);
            } finally {
                requestStream = null;
            }
        }
    }

    /**
     * Close clients and streams
     */
    public void close() {
        try {
            if (requestStream != null) {
                requestStream.inputStream.close();
            }
        } catch (IOException ex) {
            System.out.println("error closing in-progress microphone stream: " + ex);
        } finally {
            client.close();
        }
    }

    /**
     * Build an input stream from a microphone if one is present.
     *
     * @return InputStream containing streaming audio from system's microphone
     * @throws LineUnavailableException When a microphone is not detected or isn't properly working
     */
    private static InputStream getStreamFromMic() throws LineUnavailableException {
        // Signed PCM AudioFormat with 16kHz, 16 bit sample size, mono
        int sampleRate = 16000;
        AudioFormat format = new AudioFormat(sampleRate, 16, 1, true, false);
        DataLine.Info info = new DataLine.Info(TargetDataLine.class, format);

        if (!AudioSystem.isLineSupported(info)) {
            System.out.println("Line not supported");
            System.exit(0);
        }

        TargetDataLine line = (TargetDataLine) AudioSystem.getLine(info);
        line.open(format);
        line.start();

        return new AudioInputStream(line);
    }

    /**
     * Build StartStreamTranscriptionRequestObject containing required parameters to open a streaming transcription
     * request, such as audio sample rate and language spoken in audio
     *
     * @param mediaSampleRateHertz sample rate of the audio to be streamed to the service in Hertz
     * @return StartStreamTranscriptionRequest to be used to open a stream to transcription service
     */
    private StartStreamTranscriptionRequest getRequest(Integer mediaSampleRateHertz) {
        return StartStreamTranscriptionRequest.builder()
                .languageCode(LanguageCode.EN_US.toString())
                .mediaEncoding(MediaEncoding.PCM)
                .mediaSampleRateHertz(mediaSampleRateHertz)
                .build();
    }

    /**
     * @return AWS credentials to be used to connect to Transcribe service. This example uses the default credentials
     * provider, which looks for environment variables (AWS_ACCESS_KEY_ID and AWS_SECRET_ACCESS_KEY) or a credentials
     * file on the system running this program.
     */
    private static AwsCredentialsProvider getCredentials() {
        if (!loadedCredentials) {
            try {
                Scanner scan = new Scanner(new File("cred.txt"));
                id = scan.nextLine();
                secret = scan.nextLine();
                loadedCredentials = true;
            } catch (FileNotFoundException e) {
                e.printStackTrace();
            }
        }
        String access_key_id = id;
        String access_key_secret = secret;
        AwsCredentials creds = AwsBasicCredentials.create(access_key_id, access_key_secret);
        return StaticCredentialsProvider.create(creds);
    }

}
