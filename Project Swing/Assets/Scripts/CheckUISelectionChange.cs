using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckUISelectionChange : MonoBehaviour
{
    GameObject currentlySelected;
    EventSystem es;

    FMOD.Studio.EventInstance soundUIMove;

    // Start is called before the first frame update
    void Start()
    {
        soundUIMove = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_hover");

        es = EventSystem.current;
        currentlySelected = es.currentSelectedGameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlySelected != es.currentSelectedGameObject)
        {
            soundUIMove.start();
            currentlySelected = es.currentSelectedGameObject;
        }
    }
}
