using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHeavySignal : MonoBehaviour
{
    beatIndicatorHandlerBTutorial beatIndicator;

    void Start()
    {
        beatIndicator = GameObject.Find("BeatIndicator").GetComponent<beatIndicatorHandlerBTutorial>();
    }
    
    void Update()
    {
        if (mainHandlerTutorial.currentState == 2 && beatIndicator.otherBeat) transform.localScale = new Vector3(1.5f, 1.5f, 1);
        else transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x - 1f * Time.deltaTime, 1, 3), Mathf.Clamp(transform.localScale.x - 1f * Time.deltaTime, 1, 3), 1);
    }
}
