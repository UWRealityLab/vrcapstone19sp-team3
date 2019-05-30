using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ChatList : MonoBehaviour
{
    public GameObject prefab;
    public Transform contentPanel;
    public List<Text> list = new List<Text>();
    // Start is called before the first frame update
    void Start()
    {
    }

    public void AddChatBox(string text)
    {
        GameObject textBox = (GameObject)GameObject.Instantiate(prefab, contentPanel);
        textBox.GetComponent<Text>().text = text;
        list.Add(textBox.GetComponent<Text>());
        if (list.Count > 75)
        {
            List<Text> newList = new List<Text>();
            for (int i = 50; i < 75; i++)
            {
                newList.Add(list[i]);
            }
            list = newList;
        }
    }
}
