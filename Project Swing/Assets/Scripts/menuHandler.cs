using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public class menuHandler : MonoBehaviour {

    /*
    public Button btnLvl2;
    public Button btnLvl3;
    public Button btnPractiseLvl2;
    public Button btnPractiseLvl3;
    public Button btnEndless;

    
    public SpriteRenderer spriteLockLvl2;
    public SpriteRenderer spriteLockLvl3;
    public SpriteRenderer spriteLockEndless;
    */
    public GameObject levelListContent;

    public List<GameObject> levelContainers;
    public GameObject levelContainerMoon;
    public GameObject levelContainerSwing;
    public GameObject levelContainerPirate;
    public GameObject levelContainerHell;
    /*
    public Text txtClearLvl1;
    public Text txtClearLvl2;
    public Text txtClearLvl3;
    public Text txtStreakLvl1;
    public Text txtStreakLvl2;
    public Text txtStreakLvl3;
    public Text txtStreakLvlEndless;
    public Text txtEndlessRecord;
    public Text txtScoreLvl2;
    public Text txtRankLvl2;
    */
    bool clickedLevel;

    FMOD.Studio.EventInstance soundMenuMusic;
    FMOD.Studio.EventInstance soundUIClick;
    FMOD.Studio.EventInstance soundUIStart;

    public EventSystem eventSystem;
    public ScrollRect scrollLevelList;

    PlayerData data;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
    }
    
    // Use this for initialization
    void Start()
    {
        soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/MenuMusic");
        soundMenuMusic.start();

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");

        levelContainers = new List<GameObject>();

        levelContainers.Add(levelContainerMoon);
        levelContainers.Add(levelContainerSwing);
        levelContainers.Add(levelContainerPirate);
        levelContainers.Add(levelContainerHell);

        data.Init();
        print(data.unlockedLevel.Length);

        for (int i = 1; i < data.unlockedLevel.Length; i++)
        {
            GameObject levelListItem = Instantiate(levelContainers[i-1]) as GameObject;
            levelListItem.SetActive(true);

            levelListItem.transform.SetParent(levelListContent.transform, false);

            string tRank = "";
            if (data.rankRecord[i] == 0) tRank = "E";
            if (data.rankRecord[i] == 1) tRank = "D";
            if (data.rankRecord[i] == 2) tRank = "C";
            if (data.rankRecord[i] == 3) tRank = "B";
            if (data.rankRecord[i] == 4) tRank = "A";
            if (data.rankRecord[i] == 5) tRank = "S";
            if (!data.clearedLevel[i]) tRank = "F";

            if (data.unlockedLevel[i]) levelListItem.GetComponent<levelContainerHandler>().Init(tRank, data.scoreRecord[i], data.streakRecord[i], true);
            else levelListItem.GetComponent<levelContainerHandler>().Init(" ", 0, 0, false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // scroll with input
        if (Input.GetButton("Dodge"))
        {
            scrollLevelList.verticalNormalizedPosition -= 2.5f * Time.deltaTime;
        }
        if(Input.GetButton("Block"))
        {
            scrollLevelList.verticalNormalizedPosition += 2.5f * Time.deltaTime;
        }

        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Super"))
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

        if (num > 0 && num < 100) SceneManager.LoadScene("Level" + num + "Scene");
        
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
