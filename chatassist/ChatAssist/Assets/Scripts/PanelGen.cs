using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGen : MonoBehaviour
{
    // Generates the Speech bubbles for the user
    // TODO: anchor speechToTextPanel to visual indicator i.e. marker or Magic Leap
    // For now, we currently only generate 4 panels with same text. Will need to be adjusted
    // for multiple users pending on how Google API works for multiple outputs

    public GameObject speechToTextPanel;
    public Camera m_Camera;
    //public Transform transform;
    private GameObject[] speechToTextInstantiations;
    public int count;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Started");
        speechToTextInstantiations = new GameObject[4];
        speechToTextInstantiations[0] = Instantiate(original: speechToTextPanel, new Vector3(100, 0, 0), Quaternion.identity);
        speechToTextInstantiations[1] = Instantiate(original: speechToTextPanel, new Vector3(-100, 0, 0), Quaternion.identity);
        speechToTextInstantiations[2] = Instantiate(original: speechToTextPanel, new Vector3(0, 0, 100), Quaternion.identity);
        speechToTextInstantiations[3] = Instantiate(original: speechToTextPanel, new Vector3(0, 0, -100), Quaternion.identity);

        
        for (int i = 0; i < 4; i++)
        {
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (count == 10000)
        {
            //speechToTextInstantiation.SetActive(false);
            Destroy(speechToTextInstantiations[0]);
        } else
        {
            for (int i = 0; i < 4; i++)
            {
                speechToTextInstantiations[i].transform.LookAt(m_Camera.transform);
                speechToTextInstantiations[i].transform.RotateAround(speechToTextInstantiations[i].transform.position, speechToTextInstantiations[i].transform.up, 180f);
            }
            count++;
        }
    }

    void LateUpdate()
    {
        /*
        for (int i = 0; i < 4; i++)
        {
              speechToTextInstantiations[i].transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.back,
                    m_Camera.transform.rotation * Vector3.up);
        }
        */
    }
}
