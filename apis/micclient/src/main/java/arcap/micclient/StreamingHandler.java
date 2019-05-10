package arcap.micclient;

import com.google.api.gax.core.FixedCredentialsProvider;
import com.google.api.gax.rpc.ClientStream;
import com.google.api.gax.rpc.ResponseObserver;
import com.google.api.gax.rpc.StreamController;
import com.google.auth.oauth2.GoogleCredentials;
import com.google.cloud.speech.v1.*;
import com.google.protobuf.ByteString;

import javax.sound.sampled.*;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class StreamingHandler {
    /**
     * Performs microphone streaming speech recognition with a duration of 1 minute.
     */
    public static void streamingMicRecognize() throws Exception {
        System.out.println("starting mic recognizer");
        new Thread(() -> {
            FileInputStream credentialsStream = null;
            try {
                credentialsStream = new FileInputStream("./arcap.json");
                GoogleCredentials credentials = GoogleCredentials.fromStream(credentialsStream);
                FixedCredentialsProvider credentialsProvider = FixedCredentialsProvider.create(credentials);

                SpeechSettings speechSettings =
                        SpeechSettings.newBuilder()
                                .setCredentialsProvider(credentialsProvider)
                                .build();

                ResponseObserver<StreamingRecognizeResponse> responseObserver = null;
//        try (SpeechClient client = SpeechClient.create()) {
                try (SpeechClient client = SpeechClient.create(speechSettings)) {

                    responseObserver =
                            new ResponseObserver<StreamingRecognizeResponse>() {
                                ArrayList<StreamingRecognizeResponse> responses = new ArrayList<>();

                                public void onStart(StreamController controller) {
                                }

                                public void onResponse(StreamingRecognizeResponse response) {
                                    System.out.println("got resp " + response.getResultsList().get(0).getAlternativesList().get(0).getTranscript());
                                    responses.add(response);
                                    String transcript = response.getResultsList().get(0).getAlternativesList().get(0).getTranscript();
//                                    Launcher.client.send(transcript);
                                }

                                public void onComplete() {
                                    for (StreamingRecognizeResponse response : responses) {
                                        StreamingRecognitionResult result = response.getResultsList().get(0);
                                        SpeechRecognitionAlternative alternative = result.getAlternativesList().get(0);
                                        System.out.printf("Transcript : %s\n", alternative.getTranscript());
                                    }
                                }

                                public void onError(Throwable t) {
                                    System.out.println(t);
                                }
                            };

                    ClientStream<StreamingRecognizeRequest> clientStream =
                            client.streamingRecognizeCallable().splitCall(responseObserver);

                    RecognitionConfig recognitionConfig =
                            RecognitionConfig.newBuilder()
                                    .setEncoding(RecognitionConfig.AudioEncoding.LINEAR16)
                                    .setLanguageCode("en-US")
                                    .setSampleRateHertz(16000)
                                    .build();
                    StreamingRecognitionConfig streamingRecognitionConfig =
                            StreamingRecognitionConfig.newBuilder().setConfig(recognitionConfig).build();

                    StreamingRecognizeRequest request =
                            StreamingRecognizeRequest.newBuilder()
                                    .setStreamingConfig(streamingRecognitionConfig)
                                    .build(); // The first request in a streaming call has to be a config

                    clientStream.send(request);
                    // SampleRate:16000Hz, SampleSizeInBits: 16, Number of channels: 1, Signed: true,
                    // bigEndian: false
                    AudioFormat audioFormat = new AudioFormat(16000, 16, 1, true, false);
                    // Set the system information to read from the microphone audio stream
                    DataLine.Info targetInfo = new DataLine.Info(TargetDataLine.class, audioFormat);

                    if (!AudioSystem.isLineSupported(targetInfo)) {
                        System.out.println("Microphone not supported");
                        System.exit(0);
                    }
                    // Target data line captures the audio stream the microphone produces.
//            TargetDataLine targetDataLine = (TargetDataLine) AudioSystem.getLine(targetInfo);
                    TargetDataLine targetDataLine = (TargetDataLine) AudioSystem.getLine(targetInfo);
//            targetDataLine.open(audioFormat);
                    targetDataLine.open();
                    targetDataLine.start();
                    System.out.println(targetDataLine.getLineInfo().toString());
                    System.out.println("Start speaking");
                    long startTime = System.currentTimeMillis();
                    // Audio Input Stream
                    AudioInputStream audio = new AudioInputStream(targetDataLine);
                    while (true) {
                        long estimatedTime = System.currentTimeMillis() - startTime;
                        byte[] data = new byte[6400];
                        audio.read(data);
                        if (estimatedTime > 60000) { // 60 seconds
                            System.out.println("Stop speaking.");
                            targetDataLine.stop();
                            targetDataLine.close();
                            break;
                        }
                        request =
                                StreamingRecognizeRequest.newBuilder()
                                        .setAudioContent(ByteString.copyFrom(data))
                                        .build();
                        clientStream.send(request);
                    }
                } catch (Exception e) {
                    System.out.println(e);
                    e.printStackTrace();
                }
                responseObserver.onComplete();
            } catch (Exception e) {
                e.printStackTrace();
            }

        }).start();
    }

    public static List<AudioFormat> audioFormats() throws LineUnavailableException {
        Mixer.Info[] mi = AudioSystem.getMixerInfo();
        List<AudioFormat> audioFormats = new ArrayList<AudioFormat>();
        for (Mixer.Info info : mi) {
            System.out.println("info: " + info);
            Mixer m = AudioSystem.getMixer(info);
            System.out.println("mixer " + m);
            Line.Info[] sl = m.getSourceLineInfo();
            for (Line.Info info2 : sl) {
                System.out.println("    info: " + info2);
                Line line = AudioSystem.getLine(info2);
                if (line instanceof SourceDataLine) {
                    SourceDataLine source = (SourceDataLine) line;

                    DataLine.Info i = (DataLine.Info) source.getLineInfo();
                    for (AudioFormat format : i.getFormats()) {
                        audioFormats.add(format);
                        System.out.println("    format: " + format);
                    }
                }
            }
        }
        return audioFormats;
    }

    public static void main(String[] args) {
        try {
            streamingMicRecognize();
        } catch (Exception e) {
            e.printStackTrace();
        }
//        try {
//            AudioFormat audioFormat = new AudioFormat(8000, 16, 1, true, true);
//            // Set the system information to read from the microphone audio stream
//            DataLine.Info targetInfo = new DataLine.Info(TargetDataLine.class, audioFormat);
//
//            System.out.println(Arrays.toString(AudioSystem.getTargetLineInfo(targetInfo)));
////            System.out.println(Arrays.toString(AudioSystem.getAudioFileTypes()));
//            System.out.println(Arrays.toString(AudioSystem.getMixerInfo()));
//            System.out.println(AudioSystem.getSourceDataLine(audioFormat));
//
//            if (!AudioSystem.isLineSupported(targetInfo)) {
//                System.out.println("Microphone not supported");
//                System.exit(0);
//            }
//            // Target data line captures the audio stream the microphone produces.
////            TargetDataLine targetDataLine = (TargetDataLine) AudioSystem.getLine(targetInfo);
//            TargetDataLine targetDataLine = (TargetDataLine) AudioSystem.getLine(targetInfo);
////            targetDataLine.open(audioFormat);
//            targetDataLine.open();
//            targetDataLine.start();
//            AudioInputStream audio = new AudioInputStream(targetDataLine);
//            System.out.println("now parsing stream");
//            while (true) {
//                byte[] data = new byte[6400];
//                audio.read(data);
//            }
////        audioFormats();
////        streamingMicRecognize();
//        } catch (Exception e) {
//            e.printStackTrace();
//        }
    }

}
