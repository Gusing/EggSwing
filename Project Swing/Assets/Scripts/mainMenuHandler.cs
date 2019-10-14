using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class mainMenuHandler : MonoBehaviour
{
    FMOD.Studio.EventInstance soundMenuMusic;
    FMOD.Studio.EventInstance soundUIClick;
    FMOD.Studio.EventInstance soundUIStart;

    PlayerData data;
    PlayerOptions optionsData;

    EventSystem eventSystem;

    GameObject oldSelected;
    public GameObject UIMarker;
    GameObject currentUIMarker;
    float UIMarkerColor;
    bool UIMarkerColorSwitch;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
        optionsData = SaveSystem.LoadOptions();
        eventSystem = EventSystem.current;
    }

    void Start()
    {
        soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/MenuMusic");

        menuMusicPlayerHandler.Instance.checkStarted();

        oldSelected = eventSystem.currentSelectedGameObject;

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");

        optionsData.Init();
        optionsData.SetStartValues();
        if (optionsData.vSync) QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
        FMODUnity.RuntimeManager.GetVCA("vca:/Music VCA").setVolume(optionsData.volumeMusic * optionsData.volumeMaster);
        FMODUnity.RuntimeManager.GetVCA("vca:/SFX VCA").setVolume(optionsData.volumeSFX * optionsData.volumeMaster);
        FMODUnity.RuntimeManager.GetVCA("vca:/Ambience VCA").setVolume(optionsData.volumeAmbience * optionsData.volumeMaster);
    }

    void Update()
    {
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

    public void ClickOptions()
    {
        soundUIClick.start();
        SceneManager.LoadScene("OptionsScene");
    }

    public void ClickPlay()
    {
        soundMenuMusic.setParameterValue("End", 1);
        soundUIClick.start();
        if (data.seenControls) SceneManager.LoadScene("MenuScene");
        else SceneManager.LoadScene("ControlsScene");

    }

    public void VisitDiscord()
    {
        AnalyticsEvent.Custom("clicked_discord");
        Application.OpenURL("https://discord.gg/3bAe6GC");
    }

    public void VisitSunscale()
    {
        AnalyticsEvent.Custom("clicked_sunscale");
        Application.OpenURL("http://www.sunscalestudios.com/");
    }

    public void QuitGame()
    {
        soundMenuMusic.setParameterValue("End", 1);
        soundUIClick.start();
        Application.Quit();
    }
}
