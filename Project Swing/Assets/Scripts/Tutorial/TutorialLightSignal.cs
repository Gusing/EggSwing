using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLightSignal : MonoBehaviour
{
    public float maxScale = 1.5f;
    public float minScale = 1;
    public float accDown = 1;

    void Start()
    {

    }
    
    void Update()
    {
        if (mainHandlerTutorial.currentState == 2) transform.localScale = new Vector3(maxScale, maxScale, 1);
        else transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x - accDown * Time.deltaTime, minScale, 3), Mathf.Clamp(transform.localScale.x - accDown * Time.deltaTime, minScale, 3), 1);
    }
}
