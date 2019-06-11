using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WebSocketSharp;


public class SpeechToText : MonoBehaviour
{
    // Script to grab text from AWS Transcribe API and display text to user via TextMeshPro component
    // Starts grabbing text via updateCurrText call once connection is established
    public TextMeshProUGUI text;
    private string currString;
    private WebSocket ws;

    // Start is called before the first frame update
    void Start()
    {
        currString =  "Hello! This chat bubble should have speech from the person speaking to you in just a moment";
        Debug.Log("connecting to ws...");
        WebSocketManager.initialize();
        WebSocketManager.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.text.text != this.currString)
        {
            this.text.text = currString;
            MagicLeap.ControllerSelection cs = this.transform.parent.gameObject.GetComponentInChildren<MagicLeap.ControllerSelection>();
            cs.addText(currString);
        }
    }

    public void updateCurrText(string currText)
    {
        this.currString = currText;
    }
}