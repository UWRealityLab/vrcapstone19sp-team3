using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LanguageScrollList : MonoBehaviour
{
    private List<System.String> languageList = new List<System.String>();
    public Transform contentPanel;
    public GameObject prefab;
    public ToggleGroup toggleGroup;

    // Start is called before the first frame update
    void Start()
    {
        populateList();
        AddButtons();
    }

    private void populateList()
    {
        StreamReader stream = new StreamReader("./languageList.txt");
        while (!stream.EndOfStream)
        {
            languageList.Add(stream.ReadLine());
        }
        stream.Close();
        Debug.Log(languageList.Count);
    }
    
    private void AddButtons()
    {
        Debug.Log("Adding buttons");
        for (int i = 0; i < languageList.Count; i++)
        {
            // Make object
            GameObject newBtn = (GameObject)GameObject.Instantiate(prefab, contentPanel);
            newBtn.SetActive(true);
            Debug.Log(languageList[i]);
            Toggle tog = newBtn.GetComponent<Toggle>();
            tog.name = languageList[i] + "Toggle";
            tog.GetComponentInChildren<Text>().text = languageList[i];
            tog.group = toggleGroup;
            //Debug.Log(t);
            //
            //newBtn.GetComponentInChildren<Text>().text = languageList[i];
            //newBtn.GetComponentInChildren<Text>().text = languageList[i];
        }
        Debug.Log("Added buttons");
    }

    
}
