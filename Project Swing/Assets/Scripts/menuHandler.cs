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
    public GameObject BirdListContent;
    public GameObject LevelScrollList;
    public GameObject BirdScrollList;

    public List<GameObject> birdContainers;
    public List<GameObject> levelContainers;
    public GameObject levelContainerMoon;
    public GameObject levelContainerSwing;
    public GameObject levelContainerPirate;
    public GameObject levelContainerHell;

    public Button btnEndless;
    public SpriteRenderer spriteLockEndless;
    public Text txtEndlessRecord;
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
    public ScrollRect scrollBirdList;
    public SpriteRenderer rendererMarker;

    PlayerData data;

    int selectedGameMode;
    readonly int NORMAL = 0, BIRD = 1;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
    }
    
    void Start()
    {
        soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/MenuMusic");

        menuMusicPlayerHandler.Instance.checkStarted();

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");

        levelContainers = new List<GameObject>();

        levelContainers.Add(levelContainerMoon);
        levelContainers.Add(levelContainerSwing);
        levelContainers.Add(levelContainerPirate);
        levelContainers.Add(levelContainerHell);

        data.Init();

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

            if (data.unlockedLevel[i]) levelListItem.GetComponent<levelContainerHandler>().Init(tRank, data.scoreRecord[i], data.streakRecord[i], true, 0);
            else levelListItem.GetComponent<levelContainerHandler>().Init(" ", 0, 0, false, 0);
        }

        if (!data.clearedLevel[2] || !data.clearedLevel[3])
        {
            spriteLockEndless.enabled = true;
            btnEndless.interactable = false;
            txtEndlessRecord.text = "";
        }
        else
        {
            txtEndlessRecord.text =  "Record: " + data.endlessRecord.ToString();
        }

        birdContainers = new List<GameObject>();

        birdContainers.Add(levelContainerMoon);
        birdContainers.Add(levelContainerSwing);
        birdContainers.Add(levelContainerPirate);
        birdContainers.Add(levelContainerHell);

        for (int i = 1; i < data.unlockedBirdLevel.Length; i++)
        {
            GameObject levelListItem = Instantiate(levelContainers[i - 1]) as GameObject;
            levelListItem.SetActive(true);

            levelListItem.transform.SetParent(BirdListContent.transform, false);

            string tRank = "";
            if (data.rankBirdRecord[i] == 0) tRank = "E";
            if (data.rankBirdRecord[i] == 1) tRank = "D";
            if (data.rankBirdRecord[i] == 2) tRank = "C";
            if (data.rankBirdRecord[i] == 3) tRank = "B";
            if (data.rankBirdRecord[i] == 4) tRank = "A";
            if (data.rankBirdRecord[i] == 5) tRank = "P";
            if (!data.clearedBirdLevel[i]) tRank = "F";

            if (data.unlockedBirdLevel[i]) levelListItem.GetComponent<levelContainerHandler>().Init(tRank, data.scoreBirdRecord[i], data.comboRecord[i], true, 1);
            else levelListItem.GetComponent<levelContainerHandler>().Init(" ", 0, 0, false, 1);
        }

        ChangeMode(data.lastMode);
    }
    
    void Update()
    {
        // scroll with input
        if (selectedGameMode == NORMAL)
        {
            if (Input.GetAxis("NavigateRight") > 0.5f)
            {
                scrollLevelList.verticalNormalizedPosition -= 2.5f * Time.deltaTime;
            }
            if (Input.GetAxis("NavigateLeft") > 0.5f)
            {
                scrollLevelList.verticalNormalizedPosition += 2.5f * Time.deltaTime;
            }
        }
        if (selectedGameMode == BIRD)
        {
            if (Input.GetAxis("NavigateRight") > 0.5f)
            {
                scrollBirdList.verticalNormalizedPosition -= 2.5f * Time.deltaTime;
            }
            if (Input.GetAxis("NavigateLeft") > 0.5f)
            {
                scrollBirdList.verticalNormalizedPosition += 2.5f * Time.deltaTime;
            }
        }
            // back to main menu
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Super"))
        {
            Back();
        }

        // update marker
        rendererMarker.transform.position = new Vector3(rendererMarker.transform.position.x, 4.18f - 0.95f * selectedGameMode);
    }

    public void PlayLevel(int num)
    {
        if (num > 0) soundUIStart.start();
        else soundUIClick.start();

        menuMusicPlayerHandler.Instance.stopMusic();
        
        if (num == -1) SceneManager.LoadScene("PracticeScene");

        if (selectedGameMode == NORMAL) if (num > 0 && num < 100) SceneManager.LoadScene("Level" + num + "Scene");
        if (selectedGameMode == BIRD) if (num > 0 && num < 100) SceneManager.LoadScene("Level" + num + "BirdScene");


        if (num == 100) SceneManager.LoadScene("LevelEndless");
    }

    public void ChangeMode(int num)
    {
        if (num == 0)
        {
            selectedGameMode = NORMAL;
            LevelScrollList.SetActive(true);
            BirdScrollList.SetActive(false);
        }

        if (num == 1)
        {
            selectedGameMode = BIRD;
            LevelScrollList.SetActive(false);
            BirdScrollList.SetActive(true);
        }
    }

    public void EnterShop()
    {
        soundUIClick.start();
        menuMusicPlayerHandler.Instance.swapShop(true);
        SceneManager.LoadScene("ShopScene");
    }

    public void EnterControls()
    {
        soundUIClick.start();
        SceneManager.LoadScene("ControlsScene");
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
