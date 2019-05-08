package arcap.micclient;

import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.Scanner;

public class Launcher {

    private static MicWSClient client;

    public static void main(String[] args) throws IOException, URISyntaxException {
        /*
        TODO: run StreamingHandler on a timer of ~55 seconds
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
        startClient("ws://localhost:10000");
    }


    public static void startClient(String addr) throws URISyntaxException {
        URI uri = new URI(addr);
        client = new MicWSClient(uri);
        client.connect();
    }

}
