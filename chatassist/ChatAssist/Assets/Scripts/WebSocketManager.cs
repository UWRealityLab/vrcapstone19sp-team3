using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System.Timers;
using System;


public class WebSocketManager 
{
    public static WebSocket ws;
    public static List<SpeechToText> ls;
    public static bool initialized;
    private static Timer timer;
    private static bool ack;
    private static string languageCode;

    // Start is called before the first frame update
    public static void initialize()
    {
        if (initialized)
        {
            return;
        }
        ls = new List<SpeechToText>();
        ws = new WebSocket("ws://45.33.55.95:10000/");
        ws.OnMessage += response_get;
        ws.Connect();
        initialized = true;

        // Set how frequent you want to set the timer.
        timer = new Timer(50);
        timer.Elapsed += onTimer;
        timer.AutoReset = false;
        ack = true;
        languageCode = "tl en";
        send(languageCode);
    }


    public static void Register(SpeechToText st)
    {
        ls.Add(st);
    }

    private static void response_get(object sender, MessageEventArgs mssg)
    {
        string data = mssg.Data;
        int index = data.IndexOf(":");
        int head = 0;
        Int32.TryParse(data.Substring(index - 1, 1), out head);
        head -= 1;
        string msg = data.Substring(index + 1);
        ls[head].updateCurrText(msg);
    }

    public static void send(string languageCode)
    {
        ws.Send(languageCode);
        WebSocketManager.languageCode = languageCode;
    }


    public static void onTimer(object source, System.Timers.ElapsedEventArgs e)
    {
        if (!ack)
        {
            // Maybe add some prepended thing so that you know it's a language code
            ws.Send(languageCode);
            timer.Enabled = true;
        }
    }

}
