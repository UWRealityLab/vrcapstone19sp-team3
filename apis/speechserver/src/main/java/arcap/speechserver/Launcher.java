package arcap.speechserver;

import com.google.api.gax.core.CredentialsProvider;
import com.google.api.gax.core.FixedCredentialsProvider;
import com.google.auth.oauth2.GoogleCredentials;
import com.google.auth.oauth2.ServiceAccountCredentials;
import com.google.cloud.speech.v1.*;
import com.google.protobuf.ByteString;
import org.java_websocket.server.WebSocketServer;

import java.io.FileInputStream;
import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.List;
import java.util.Scanner;

public class Launcher {

    public static void main(String[] args) throws IOException {
        request("./brooklyn.flac");
        new Thread(() -> {
            Scanner scan = new Scanner(System.in);
            while (scan.hasNextLine()) {
                String s = scan.nextLine();
                System.out.println(s);
                server.broadcast(s);
            }
        }).start();
        startServer();
    }

    static WebSocketServer server;

    public static void startServer() {
        String host = "localhost";
        int port = 8887;

        server = new Server(new InetSocketAddress(host, port));
        server.run();
    }

    public static String request(String fileName) throws IOException {
        FileInputStream credentialsStream = new FileInputStream("./arcap.json");
        GoogleCredentials credentials = GoogleCredentials.fromStream(credentialsStream);
        FixedCredentialsProvider credentialsProvider = FixedCredentialsProvider.create(credentials);

        SpeechSettings speechSettings =
                SpeechSettings.newBuilder()
                        .setCredentialsProvider(credentialsProvider)
                        .build();

        try (SpeechClient speech = SpeechClient.create(speechSettings)) {
            // The path to the audio file to transcribe
//            String fileName = "./brooklyn.flac";

            // Reads the audio file into memory
            Path path = Paths.get(fileName);
            byte[] data = Files.readAllBytes(path);
            ByteString audioBytes = ByteString.copyFrom(data);

            // Builds the sync recognize request
            RecognitionConfig config = RecognitionConfig.newBuilder()
//                    .setEncoding(RecognitionConfig.AudioEncoding.FLAC)
//                    .setSampleRateHertz(16000)
                    .setLanguageCode("en-US")
                    .build();
            RecognitionAudio audio = RecognitionAudio.newBuilder()
                    .setContent(audioBytes)
                    .build();

            // Performs speech recognition on the audio file
            RecognizeResponse response = speech.recognize(config, audio);
            List<SpeechRecognitionResult> results = response.getResultsList();

            for (SpeechRecognitionResult result : results) {
                // There can be several alternative transcripts for a given chunk of speech. Just use the
                // first (most likely) one here.
                SpeechRecognitionAlternative alternative = result.getAlternativesList().get(0);
                String s = alternative.getTranscript();
                System.out.printf("Transcription: %s%n", s);
                return s;
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        return "@transcription_failed@";
    }

}
