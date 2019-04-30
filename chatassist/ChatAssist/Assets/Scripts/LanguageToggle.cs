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
//        toggle.onValueChanged.AddListener(delegate {
//            ToggleValueChanged();
//        });

    }

    public void SetUp(string l, ToggleGroup tg, LanguageScrollList currentScrollList)
    {
        languageName.text = l;
        toggle.group = tg;
        toggle.name = l + "Toggle";
        this.scrollList = currentScrollList;

        if ("English".Equals(l))
        {
            toggle.isOn = true;
        }
    }

//    public void ToggleValueChanged()
//    {
//        if (toggle.isOn)
//        {
//            scrollList.SetLanguage(languageName.text);
//        }
//    }
}
