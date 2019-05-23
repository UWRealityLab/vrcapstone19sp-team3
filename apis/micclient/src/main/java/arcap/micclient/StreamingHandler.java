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
import java.util.concurrent.atomic.AtomicInteger;

public class StreamingHandler {

    private TranscribeStreamingClientWrapper client;
    private CompletableFuture<Void> streamingReq;

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
                try {

                    List<Result> results = ((TranscriptEvent) event).transcript().results();
                    if (results.size() > 0) {
                        System.out.println("res 2size: " + results.size());
                        Result firstResult = results.get(0);
                        if (firstResult.alternatives().size() > 0 && !firstResult.alternatives().get(0).transcript().isEmpty()) {
//                        String transcript = firstResult.alternatives().get(0).transcript();
//                        Alternative alternative = firstResult.alternatives().get(0);
//                        for (SdkField<?> sdk : alternative.sdkFields()) {
//                           System.out.println("sdk: " + sdk.locationName());
//                        }
//                        System.out.println(alternative.getValueForField("Items", String.class));
                            String transcript = firstResult.alternatives().get(0).transcript();
                            if (!transcript.isEmpty()) {
                                System.out.println("Transcript: " + transcript + " " + firstResult.isPartial());
//                            System.out.println("now clean and msg");
//                            System.out.flush();
                                cleanAndMessage(transcript, firstResult.isPartial());
                            }
                            System.out.println("next");
                        }
                        System.out.println("finished res2");
                    }
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }

            @Override
            public void onResponse(StartStreamTranscriptionResponse r) {
                System.out.println("request opened: " + r.requestId());
            }

            @Override
            public void onComplete() {
                System.out.println("stopped streaming.");
            }

        };
    }

    private static final int MAX_LEN = 30;

    private long lastMessageTime = 0;
    private String lastMessage = "";
    private int lastMessageIdx = 0;

    private AtomicInteger messageRepeatCounter = new AtomicInteger();

    private synchronized void send(String transcript) {
        System.out.println("Sending " + transcript);
        if (transcript.length() > MAX_LEN) {
            if (transcript.length() - lastMessageIdx > MAX_LEN) {
                lastMessageIdx += MAX_LEN;
            }
            int idx = transcript.indexOf(' ', lastMessageIdx - 1);
            if (idx > -1) {
                transcript = transcript.substring(idx);
            }
        }
        System.out.println("Trimmed to: " + transcript);
        Launcher.message(transcript);
//        int lastLen = 0;
//        while (transcript.length() > MAX_LEN) {
//            System.out.println("trimming " + transcript);
//            if (transcript.length() == lastLen) {
//                // edge case infinite loop
//                break;
//            }
//            int last = transcript.indexOf(' ', MAX_LEN - 1);
//            if (last > -1) {
//                transcript = transcript.substring(last);
//            }
//            lastLen = transcript.length();
//            System.out.println("trimmed to " + last + ": " + transcript);
//        }
//        Launcher.message(transcript);
    }

    private synchronized void cleanAndMessage(String transcript, boolean partial) {
//        System.out.println("clean and msg " + transcript + " " + partial);
        if (partial) {
            // filter out some partials
            // we never filter non-partials
            if (transcript.length() < 3) {
//                System.out.println("skip 1");
                // ignore short partials
                return;
            }
            if (transcript.equals(lastMessage)) {
//                System.out.println("skip 2");
                // exactly the same, skip lol
                return;
            }
            if (lastMessage.length() >= 3 && transcript.substring(0, 3).equals(lastMessage.substring(0, 3))) {
                // seems to be same message
                // let's wait half a second for another...
                final int myId = messageRepeatCounter.incrementAndGet();
                final String myTranscript = transcript;
//                System.out.println("launching thraed");
                new Thread(() -> {
                    try {
//                        System.out.println("Waiting on " + myTranscript);
                        Thread.sleep(100L);
                        if (messageRepeatCounter.get() == myId) {
                            // didn't receive another partial!
                            send(myTranscript);
                        } else {
                            System.out.println("Skipping: " + myTranscript);
                        }
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }
                }).start();
                return;
            }
//            System.out.println("not matched: " + transcript);
            lastMessage = transcript;
            send(transcript);
        } else {
//            System.out.println("down here");
            // reset message counter
            messageRepeatCounter.set(0);
            send(transcript);
        }
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
