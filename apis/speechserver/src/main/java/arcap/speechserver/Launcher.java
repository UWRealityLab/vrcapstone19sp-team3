package arcap.speechserver;

import org.java_websocket.server.WebSocketServer;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.util.Scanner;

public class Launcher {

    public static void main(String[] args) throws IOException {
        SpeechHandler.request("./brooklyn.flac");
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

}
