﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LanguageScrollList : MonoBehaviour
{
    public List<LanguageToggle> toggles = new List<LanguageToggle>();
    private List<System.String> languageList = new List<System.String>();
    private List<System.String> languageEncode = new List<System.String>();
    public Transform contentPanel;
    public GameObject prefab;
    public ToggleGroup toggleGroup;
    public Text languageDisplay;
    private string language;
    private string languageCode;

    // Awake is called before Start
    void Awake()
    {
        populateList();
        AddButtons();
        language = "English";
        languageDisplay.text = language;
    }



    private void populateList()
    {
        //StreamReader stream = new StreamReader("./shortLanguageList.txt");
        //while (!stream.EndOfStream)
        //{
        //    languageList.Add(stream.ReadLine());
        //}
        //stream.Close();


        languageList.Add("English");
        languageEncode.Add("tl en");
        languageList.Add("Japanese");
        languageEncode.Add("tl ja");
        languageList.Add("Chinese");
        languageEncode.Add("tl zh-TW");
        languageList.Add("French");
        languageEncode.Add("tl fr");
        languageList.Add("German");
        languageEncode.Add("tl de");
        languageList.Add("Italian");
        languageEncode.Add("tl it");
        languageList.Add("Korean");
        languageEncode.Add("tl ko");
        languageList.Add("Spanish");
        languageEncode.Add("tl es");
        //Debug.Log(languageList.Count);
    }
    
    private void AddButtons()
    {
        //Debug.Log("Adding buttons");
        for (int i = 0; i < languageList.Count; i++)
        {
            // Make object
            GameObject newBtn = (GameObject)GameObject.Instantiate(prefab, contentPanel);
            //newBtn.SetActive(true);
            LanguageToggle lt = newBtn.GetComponent<LanguageToggle>();
            lt.SetUp(languageList[i], toggleGroup, this);
            toggles.Add(lt);
        }
        //Debug.Log("Added buttons");
    }

    public void SetLanguage(int index)
    {
        language = languageList[index];
        languageCode = languageEncode[index];
        languageDisplay.text = languageList[index];
        toggles[index].toggle.isOn = true;

    }
    
    public string GetLanguage()
    {
        return language;
    }

    public string GetLanguageCode()
    {
        return languageCode;
    }
}
