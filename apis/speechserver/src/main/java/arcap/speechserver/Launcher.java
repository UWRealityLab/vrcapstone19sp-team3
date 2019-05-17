package arcap.speechserver;

import org.java_websocket.server.WebSocketServer;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.util.HashMap;
import java.util.Map;
import java.util.Scanner;

public class Launcher {

    public static Map<String, String> tlLanguages = new HashMap<>();
    public static Map<String, String> connToId = new HashMap<>();

    public static void main(String[] args) throws IOException {
//        SpeechHandler.request("./brooklyn.flac");
        new Thread(() -> {
            Scanner scan = new Scanner(System.in);
            while (scan.hasNextLine()) {
                String s = scan.nextLine();
                for (String key : tlLanguages.keySet()) {
                    tlLanguages.put(key, s);
                }
                System.out.println("set langs to " + s);
//                server.broadcast(s);
            }
        }).start();
        startServer();
    }

    static WebSocketServer server;

    public static void startServer() {
        int port = 10000;

        server = new Server(new InetSocketAddress(port));
        server.run();
    }

}
