using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class controlsMenuHandler : MonoBehaviour
{
    FMOD.Studio.EventInstance soundUIClick;

    public SpriteRenderer rendererHideSuper;
    public SpriteRenderer rendererBirdControls;

    EventSystem eventSystem;
    GameObject oldSelected;
    public GameObject UIMarker;
    GameObject currentUIMarker;
    float UIMarkerColor;
    bool UIMarkerColorSwitch;

    PlayerData data;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
    }

    void Start()
    {
        eventSystem = EventSystem.current;

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");

        data.Init();
        if (data.itemBought[3]) rendererHideSuper.enabled = false;
        if (data.seenBirdTutorial) rendererBirdControls.enabled = true;

        AnalyticsEvent.CustomEvent(new Dictionary<string, object> { { "Enter_Controls", 1 } });
    }

    void Update()
    {
        // back to play menu
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Super"))
        {
            BackToMenu();
        }

        // update marker
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
                print(eventSystem.currentSelectedGameObject.transform.position.y);

                Destroy(currentUIMarker);
                currentUIMarker = Instantiate(UIMarker, eventSystem.currentSelectedGameObject.transform);
                currentUIMarker.GetComponent<RectTransform>().sizeDelta = new Vector2(eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta.x, eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta.y);
            }
        }

        oldSelected = eventSystem.currentSelectedGameObject;
    }

    public void BackToMenu()
    {
        soundUIClick.start();
        SceneManager.LoadScene("MenuScene");
    }
}
