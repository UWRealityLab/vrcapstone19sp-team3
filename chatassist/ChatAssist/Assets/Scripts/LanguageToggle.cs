using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageToggle : MonoBehaviour
{
    public Toggle toggle;
    public Text languageName;
    
    private LanguageScrollList scrollList;

    // Start is called before the first frame update
    void Start()
    {
        //button.on.AddListener(HandleClick);
    }

    public void SetUp(string l, LanguageScrollList currentScrollList)
    {
        languageName.text = l;
        this.scrollList = currentScrollList;
    }

    public void HandleClick()
    {

    }
}
