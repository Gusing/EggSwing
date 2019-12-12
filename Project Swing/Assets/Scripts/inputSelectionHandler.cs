using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class inputSelectionHandler : MonoBehaviour
{
    FMOD.Studio.EventInstance soundUIClick;

    void Start()
    {
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
    }
    
    void Update()
    {

    }

    public void selectInput(int type)
    {
        sceneSelectionHandler.Instance.inputIcons = type;
        print("selected input type " + type);
        
        soundUIClick.start();

        if (sceneSelectionHandler.Instance.sceneArrivedFrom == "MainMenuScene")
        {
            menuMusicPlayerHandler.Instance.StopMusic();
            SceneManager.LoadScene("TutorialScene");
        }
        else SceneManager.LoadScene("ControlsScene");

    }
}
