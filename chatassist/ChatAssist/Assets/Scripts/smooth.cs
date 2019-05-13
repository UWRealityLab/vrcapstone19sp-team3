using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smooth : MonoBehaviour
{
    public Camera m_Camera;

    // Update is called once per frame
    void Update()
    {
        float transitionSpeed = 0.005f;
        Vector3 newPos = new Vector3(transform.position.x - 0.1f, transform.position.y - 0.02f, transform.position.z + 0.05f);
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * transitionSpeed);
        transform.LookAt(m_Camera.transform);
        transform.RotateAround(transform.position, transform.up, 180f);
    }            

}
