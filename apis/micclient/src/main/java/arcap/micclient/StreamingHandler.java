package arcap.micclient;

import arcap.micclient.aws.StreamTranscriptionBehavior;
import arcap.micclient.aws.TranscribeStreamingClientWrapper;
import software.amazon.awssdk.services.transcribestreaming.model.Result;
import software.amazon.awssdk.services.transcribestreaming.model.StartStreamTranscriptionResponse;
import software.amazon.awssdk.services.transcribestreaming.model.TranscriptEvent;
import software.amazon.awssdk.services.transcribestreaming.model.TranscriptResultStream;

import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;

public class StreamingHandler {

    private TranscribeStreamingClientWrapper client;
    private CompletableFuture<Void> streamingReq;
    private String finalTranscript = "";

    /**
     * Performs microphone streaming speech recognition with a duration of 1 minute.
     */
    public void streamingMicRecognize() throws Exception {
        System.out.println("starting mic recognizer");
        new Thread(() -> {
            client = new TranscribeStreamingClientWrapper();
            streamingReq = client.startTranscription(getBehavior());
        }).start();
    }

    private StreamTranscriptionBehavior getBehavior() {
        return new StreamTranscriptionBehavior() {
            @Override
            public void onError(Throwable e) {
                System.out.println(e.getMessage());
                Throwable cause = e.getCause();
                while (cause != null) {
                    System.out.println("Cause: " + cause.getMessage());
                    System.out.println(Arrays.toString(cause.getStackTrace()));
                    if (cause.getCause() != cause) {
                        cause = cause.getCause();
                    } else {
                        cause = null;
                    }
                }
                e.printStackTrace();
            }

            @Override
            public void onStream(TranscriptResultStream event) {
                List<Result> results = ((TranscriptEvent) event).transcript().results();
                if (results.size() > 0) {
                    Result firstResult = results.get(0);
                    if (firstResult.alternatives().size() > 0 && !firstResult.alternatives().get(0).transcript().isEmpty()) {
                        String transcript = firstResult.alternatives().get(0).transcript();
                        if (!transcript.isEmpty()) {
                            System.out.println("Transcript: " + transcript);
                            cleanAndMessage(transcript);
                            if (!firstResult.isPartial()) {
                                finalTranscript += transcript + " ";
                            }
                        }
                    }
                }
            }

            @Override
            public void onResponse(StartStreamTranscriptionResponse r) {
                System.out.println("request opened: " + r.requestId());
            }

            @Override
            public void onComplete() {
                System.out.println("final transcript: " + finalTranscript);
            }
        };
    }

    private static final int MAX_LEN = 30;

    private void cleanAndMessage(String transcript) {
        // todo: better algo (save last trim idx)
        int lastLen = 0;
        while (transcript.length() > MAX_LEN) {
            System.out.println("trimming " + transcript);
            if (transcript.length() == lastLen) {
                // edge case infinite loop
                break;
            }
            int last = transcript.indexOf(' ', MAX_LEN - 1);
            if (last > -1) {
                transcript = transcript.substring(last);
            }
            lastLen = transcript.length();
            System.out.println("trimmed to " + last + ": " + transcript);
        }
        Launcher.message(transcript);
    }


    public static void main(String[] args) {
        StreamingHandler sh = new StreamingHandler();
        try {
            sh.streamingMicRecognize();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

}
