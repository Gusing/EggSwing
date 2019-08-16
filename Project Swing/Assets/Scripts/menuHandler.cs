using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class menuHandler : MonoBehaviour {

    public Button btnLvl2;
    public Button btnLvl3;
    public Button btnPractiseLvl2;
    public Button btnPractiseLvl3;
    public Button btnEndless;

    public SpriteRenderer spriteLockLvl2;
    public SpriteRenderer spriteLockLvl3;
    public SpriteRenderer spriteLockEndless;

    public Text txtClearLvl1;
    public Text txtClearLvl2;
    public Text txtClearLvl3;
    public Text txtStreakLvl1;
    public Text txtStreakLvl2;
    public Text txtStreakLvl3;
    public Text txtStreakLvlEndless;
    public Text txtEndlessRecord;
    bool clickedLevel;

    FMOD.Studio.EventInstance soundMenuMusic;
    FMOD.Studio.EventInstance soundUIClick;
    FMOD.Studio.EventInstance soundUIStart;

    PlayerData data;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
    }
    
    // Use this for initialization
    void Start () {

        soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/MenuMusic");
        soundMenuMusic.start();

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");

        
        if (!data.unlockedLevelsA)
        {
            btnLvl2.interactable = false;
            btnPractiseLvl2.interactable = false;
            spriteLockLvl2.enabled = true;
            btnLvl3.interactable = false;
            btnPractiseLvl3.interactable = false;
            spriteLockLvl3.enabled = true;
        }
        if (!data.unlockedLevelsB)
        {
            btnEndless.interactable = false;
            spriteLockEndless.enabled = true;
        }
        if (!data.clearedLevel1)
        {
            txtClearLvl1.enabled = false;
            txtStreakLvl1.enabled = false;
        }
        else txtStreakLvl1.text = "Best Streak: " + data.streakLevel1Record;
        if (!data.clearedLevel2)
        {
            txtClearLvl2.enabled = false;
            txtStreakLvl2.enabled = false;
        }
        else txtStreakLvl2.text = "Best Streak: " + data.streakLevel2Record;
        if (!data.clearedLevel3)
        {
            txtClearLvl3.enabled = false;
            txtStreakLvl3.enabled = false;
        }
        else txtStreakLvl3.text = "Best Streak: " + data.streakLevel3Record;
        if (data.endlessRecord > 0)
        {
            txtEndlessRecord.enabled = true;
            txtEndlessRecord.text = "RECORD: " + data.endlessRecord;
            txtStreakLvlEndless.enabled = true;
            txtStreakLvlEndless.text = "Best Streak: " + data.streakLevelEndlessRecord;
        }
        else
        {
            txtEndlessRecord.enabled = false;
            txtStreakLvlEndless.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Back();
        }

        if (Input.GetButtonDown("Heavy Attack"))
        {
            soundUIStart.start();
            soundMenuMusic.setParameterValue("End", 1);
            SceneManager.LoadScene("Level4Scene");
        }
    }

    public void PlayLevel(int num)
    {
        if (num > 0) soundUIStart.start();
        else soundUIClick.start();
        
        soundMenuMusic.setParameterValue("End", 1);

        if (num == -3) SceneManager.LoadScene("Practice148");
        if (num == -2) SceneManager.LoadScene("Practice128");
        if (num == -1) SceneManager.LoadScene("Practice100");

        if (num == 1) SceneManager.LoadScene("Level1Scene");
        if (num == 2) SceneManager.LoadScene("Level2Scene");
        if (num == 3) SceneManager.LoadScene("Level3Scene");

        if (num == 100) SceneManager.LoadScene("LevelEndless");
    }

    public void Back()
    {
        soundMenuMusic.setParameterValue("End", 1);
        soundUIClick.start();
        SceneManager.LoadScene("MainMenuScene");
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
}
