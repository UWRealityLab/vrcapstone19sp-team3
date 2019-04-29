using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGen : MonoBehaviour
{
    public GameObject speechToTextPanel;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(original: speechToTextPanel, new Vector3(100, 100, 100), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
