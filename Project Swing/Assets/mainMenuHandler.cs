using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class mainMenuHandler : MonoBehaviour
{
    FMOD.Studio.EventInstance soundMenuMusic;
    FMOD.Studio.EventInstance soundUIClick;
    FMOD.Studio.EventInstance soundUIStart;

    // Start is called before the first frame update
    void Start()
    {
        soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/MenuMusic");
        soundMenuMusic.start();

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickOptions()
    {

    }

    public void ClickPlay()
    {
        soundMenuMusic.setParameterValue("End", 1);
        soundUIClick.start();
        SceneManager.LoadScene("MenuScene");
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
