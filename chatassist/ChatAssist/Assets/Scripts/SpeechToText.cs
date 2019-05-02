using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeechToText : MonoBehaviour
{
    // Script to grab text from Google API and display text to user via TextMeshPro component
    // Note: Following is just an example, still need to hook up Google API via getSpeechToText call on Update
    // Can check with parent if we are looking at the target, and start grabbing text from there via text.tranform.parent
    public TextMeshProUGUI text;
    public int count = 0;
    private string currString;

    // Start is called before the first frame update
    void Start()
    {
        currString = "Lorem ipsum dolor sit amet consectetur adipiscing elit nisi, platea at neque porta nostra scelerisque a eleifend ornare, cum accumsan cursus torquent parturient laoreet condimentum. Suspendisse congue faucibus cum sodales sem euismod, mollis dis augue primis dui nullam, erat conubia massa posuere fames. Justo varius erat netus id libero auctor malesuada cum dapibus, hac dis conubia enim ligula bibendum tincidunt nullam, vulputate integer eu non commodo vitae congue lobortis." +
            "Metus euismod sodales nibh nascetur duis parturient eu commodo, sed mauris per urna quis laoreet suspendisse, fames neque magnis mus a leo suscipit.Interdum nisi natoque porttitor nulla ridiculus molestie ut, arcu pretium ligula augue mollis ac velit, parturient faucibus hac porta et tristique.Pulvinar dui in gravida dis sociis mollis semper ornare, pellentesque turpis a eu quis himenaeos ut inceptos, felis senectus lobortis taciti arcu habitant nulla.Nullam fringilla etiam tempus aliquam pellentesque sociosqu iaculis cubilia arcu, posuere cras praesent ligula sem parturient lacinia lectus penatibus sodales, duis facilisis porttitor varius diam egestas conubia velit.";
    }

    // Update is called once per frame
    void Update()
    {
        getSpeechToText();
    }

    void getSpeechToText()
    {
        if ((count / 10) * 80 + 80 < currString.Length)
        {
            //count++;
            text.text = currString.Substring((count / 10) * 80, 80);
            count++;
        }
    }
}
