using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechToText : MonoBehaviour
{
    public Text text;
    public int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "Hello World";
    }

    // Update is called once per frame
    void Update()
    {
        if (count < 10)
        {
            count++;
            text.text = count.ToString();
        }
        else
        {
            text.text = "Goodbye World";
        }
    }
}
