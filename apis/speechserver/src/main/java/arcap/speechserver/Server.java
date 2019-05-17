package arcap.speechserver;

import org.java_websocket.WebSocket;
import org.java_websocket.handshake.ClientHandshake;
import org.java_websocket.server.WebSocketServer;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.ByteBuffer;

public class Server extends WebSocketServer {

    public Server(InetSocketAddress address) {
        super(address);
    }

    @Override
    public void onOpen(WebSocket conn, ClientHandshake handshake) {
        conn.send("Welcome to the server!"); //This method sends a message to the new client
//        broadcast("new connection: " + handshake.getResourceDescriptor()); //This method sends a message to all clients connected
        System.out.println("new connection to " + conn.getRemoteSocketAddress());
    }

    @Override
    public void onClose(WebSocket conn, int code, String reason, boolean remote) {
        System.out.println("closed " + conn.getRemoteSocketAddress() + " with exit code " + code + " additional info: " + reason);
    }

    @Override
    public void onMessage(WebSocket conn, String message) {
        System.out.println("received message from " + conn.getRemoteSocketAddress() + ": " + message);
        if (message.startsWith("mic")) {
            String senderId = message.substring(0, message.indexOf(':')).trim();
            String msg = message.substring(message.indexOf(':') + 1);
            for (WebSocket ws : this.getConnections()) {
                String addr = ws.getRemoteSocketAddress().toString();
                String id = Launcher.connToId.getOrDefault(addr, null);
                String cm = message;
                if (id != null) {
                    String lang = Launcher.tlLanguages.getOrDefault(id, null);
                    System.out.println("lang: " + lang);
                    if (lang != null && lang.length() != 0) {
                        try {
                            System.out.println("Translating " + msg + " for " + id);
                            cm = senderId + ":" + Translator.translate(msg, "auto", lang);
                            System.out.println("translated to " + cm);
                        } catch (IOException e) {
                            e.printStackTrace();
                        }
                    }
                } else {
                    System.out.println("no id for " + addr + ' ' + Launcher.connToId.toString());
                    String lang = Launcher.defaultLang;
                    System.out.println("lang: " + lang);
                    if (lang != null && lang.length() != 0) {
                        try {
                            System.out.println("Translating " + msg + " for " + id);
                            cm = senderId + ":" + Translator.translate(msg, "auto", lang);
                            System.out.println("translated to " + cm);
                        } catch (IOException e) {
                            e.printStackTrace();
                        }
                    }
                }
                ws.send(cm);
            }
        } else if (message.startsWith("tl")) {
            String addr = conn.getRemoteSocketAddress().toString();
            String id = Launcher.connToId.getOrDefault(addr, null);
            String langId = message.substring(message.indexOf(' ') + 1).trim();
            if (id != null) {
                Launcher.tlLanguages.put(id, langId);

            }
        } else if (message.startsWith("open")) {
            String id = message.substring(message.indexOf(' ') + 1).trim();
            Launcher.tlLanguages.put(id, null);
            Launcher.connToId.put(conn.getRemoteSocketAddress().toString(), id);
            System.out.println("Registering " + id + " with " + conn.getRemoteSocketAddress().toString());
        }
    }

    @Override
    public void onMessage(WebSocket conn, ByteBuffer message) {
        System.out.println("received ByteBuffer from " + conn.getRemoteSocketAddress());
    }

    @Override
    public void onError(WebSocket conn, Exception ex) {
        System.err.println("an error occured on connection " + conn.getRemoteSocketAddress() + ":" + ex);
    }

    @Override
    public void onStart() {
        System.out.println("server started successfully");
    }


    public static void main(String[] args) {
    }
}
