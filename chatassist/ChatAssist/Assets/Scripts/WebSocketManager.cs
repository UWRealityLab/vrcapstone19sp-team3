using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


public class WebSocketManager 
{
    public static WebSocket ws;
    public static List<SpeechToText> ls;
    public static bool initialized;
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
    }


    public static void Register(SpeechToText st)
    {
        Debug.Log("registering!!!!!!!!!!!! " + st);
        ls.Add(st);
        Debug.Log("list size: " + ls.Count);
    }

    private static void response_get(object sender, MessageEventArgs mssg)
    {
        string data = mssg.Data;
        Debug.Log("speech server says2: " + data);
        Debug.Log("step1: " + data);
        //this.mydude.text = data;
        Debug.Log("step2: " + data);
        //this.currString = data;
        Debug.Log("step3: " + data);
        Debug.Log("set currtext to " + data);
        //this.mydude.text = "helloabc!!!";
        //this.mydude.text = data;
        string msg = data.Substring(data.IndexOf(":") + 1);
        //Debug.Log("preparing sending to 5" + msg);

        Debug.Log("This is an in-between message");
        Debug.Log("list size2: " + ls.Count);

        foreach (SpeechToText st in ls)
        {
            Debug.Log("sending to something 10");
            st.updateCurrText(msg);
        }
    }
}
