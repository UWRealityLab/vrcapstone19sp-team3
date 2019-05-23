package arcap.micclient;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.Scanner;

public class Launcher {

    public static MicWSClient client;

    public static final String ID = "mic1";

    public static void main(String[] args) throws Exception {
        /*
        TODO: run OldStreamingHandler on a timer of ~55 seconds
        the goog streaming speech api has a limit of 60 seconds.
        so we must restat the streaming when it stops
         */
        new Thread(() -> {
            Scanner scan = new Scanner(System.in);
            while (scan.hasNextLine()) {
                String s = scan.nextLine();
                System.out.println(s);
//                server.broadcast(s);
                client.send(s);
            }
        }).start();
//        startClient("ws://45.33.55.95:10000");
        startClient("ws://127.0.0.1:10000");
        StreamingHandler sh = new StreamingHandler();
        sh.streamingMicRecognize();
    }

    public static void message(String msg) {
        client.send(ID + ":" + msg);
    }

    public static void startClient(String addr) throws URISyntaxException {
        URI uri = new URI(addr);
        client = new MicWSClient(uri);
        client.connect();
        System.out.println("finished startClient()");
    }

}
