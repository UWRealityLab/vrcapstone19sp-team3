using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WebSocketSharp;


public class SpeechToText : MonoBehaviour
{
    // Script to grab text from Google API and display text to user via TextMeshPro component
    // Note: Following is just an example, still need to hook up Google API via getSpeechToText call on Update
    // Can check with parent if we are looking at the target, and start grabbing text from there via text.tranform.parent
    public TextMeshProUGUI text;
    private string currString;
    private WebSocket ws;

    // Start is called before the first frame update
    void Start()
    {
        currString =  "Hello! This chat bubble should have speech from the person speaking to you in just a moment";
        Debug.Log("connecting to ws...");
        //ws.Send("./peter.flac");
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