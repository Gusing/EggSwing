using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class inputSelectionHandler : MonoBehaviour
{
    EventSystem eventSystem;

    GameObject oldSelected;
    public GameObject UIMarker;
    GameObject currentUIMarker;
    float UIMarkerColor;
    bool UIMarkerColorSwitch;

    FMOD.Studio.EventInstance soundUIClick;

    void Start()
    {
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");

        eventSystem = EventSystem.current;

        oldSelected = eventSystem.currentSelectedGameObject;
    }
    
    void Update()
    {
        UpdateSelection();
    }

    void UpdateSelection()
    {
        if (!UIMarkerColorSwitch)
        {
            if (UIMarkerColor < 1)
            {
                UIMarkerColor += 2f * Time.deltaTime;
            }
            else UIMarkerColorSwitch = true;
        }
        if (UIMarkerColorSwitch)
        {
            if (UIMarkerColor > 0.4)
            {
                UIMarkerColor -= 2f * Time.deltaTime;
            }
            else UIMarkerColorSwitch = false;
        }

        if (currentUIMarker != null)
        {
            currentUIMarker.GetComponent<Image>().color = new Color(UIMarkerColor * 0.5f, UIMarkerColor, UIMarkerColor * 0.5f);
        }

        if (eventSystem.currentSelectedGameObject != null)
        {
            if (eventSystem.currentSelectedGameObject != oldSelected)
            {
                Destroy(currentUIMarker);

                currentUIMarker = Instantiate(UIMarker, eventSystem.currentSelectedGameObject.transform);
                currentUIMarker.GetComponent<RectTransform>().sizeDelta = new Vector2(eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta.x, eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta.y);
            }
        }

        oldSelected = eventSystem.currentSelectedGameObject;

        oldSelected = eventSystem.currentSelectedGameObject;
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
