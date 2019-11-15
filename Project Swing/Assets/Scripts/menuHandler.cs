using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public class menuHandler : MonoBehaviour {
    
    public GameObject levelListContent;
    public GameObject LevelScrollList;
    public GameObject BirdListContent;
    public GameObject BirdScrollList;
    public GameObject hardListContent;
    public GameObject hardScrollList;
    
    public GameObject levelContainer;

    EventSystem eventSystem;
    GameObject oldSelected;
    public GameObject UIMarker;
    GameObject currentUIMarker;
    float UIMarkerColor;
    bool UIMarkerColorSwitch;
    public float UIMarkerSpeed = 2;
    public float UIMarkerDarkness = 0.5f;

    public Button btnEndless;
    public SpriteRenderer spriteLockEndless;
    public Text txtEndlessRecord;
    public Button btnHard;
    public SpriteRenderer spriteLockHard;

    public SpriteRenderer rendererBirdTutorial;
    bool birdTutorialVisible;
    float birdTutorialTimer;

    bool clickedLevel;

    FMOD.Studio.EventInstance soundMenuMusic;
    FMOD.Studio.EventInstance soundUIClick;
    FMOD.Studio.EventInstance soundUIStart;
    
    public ScrollRect scrollLevelList;
    public ScrollRect scrollBirdList;
    public ScrollRect hardLevelList;
    public ScrollRect currentScrollList;
    public SpriteRenderer rendererMarker;

    PlayerData data;

    int selectedGameMode;
    readonly int NORMAL = 0, BIRD = 1, HARD = 2;

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

        eventSystem = EventSystem.current;
        
        data.Init();

        for (int i = 1; i < data.unlockedLevel.Length; i++)
        {
            GameObject levelListItem = Instantiate(levelContainer) as GameObject;
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

            if (data.unlockedLevel[i]) levelListItem.GetComponent<levelContainerHandler>().Init(tRank, data.scoreRecord[i], data.streakRecord[i], i, true, 0);
            else levelListItem.GetComponent<levelContainerHandler>().Init(" ", 0, 0, i, false, 0);
        }

        if (!data.clearedLevel[2] || !data.clearedLevel[3])
        {
            spriteLockEndless.enabled = true;
            btnEndless.gameObject.SetActive(false);
            txtEndlessRecord.text = "";
        }
        else
        {
            txtEndlessRecord.text =  "Record: " + data.endlessRecord.ToString();
        }

        for (int i = 1; i < data.unlockedBirdLevel.Length; i++)
        {
            GameObject levelListItem = Instantiate(levelContainer) as GameObject;
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

            if (data.unlockedBirdLevel[i]) levelListItem.GetComponent<levelContainerHandler>().Init(tRank, data.scoreBirdRecord[i], data.comboRecord[i], i, true, 1);
            else levelListItem.GetComponent<levelContainerHandler>().Init(" ", 0, 0, i, false, 1);
        }

        if (data.unlockedHardLevel[1]) btnHard.interactable = true;
        else
        {
            spriteLockHard.enabled = true;
            btnHard.gameObject.SetActive(false);
        }

        for (int i = 1; i < data.unlockedHardLevel.Length; i++)
        {
            GameObject levelListItem = Instantiate(levelContainer) as GameObject;
            levelListItem.SetActive(true);

            levelListItem.transform.SetParent(hardListContent.transform, false);

            string tRank = "";
            if (data.rankHardRecord[i] == 0) tRank = "E";
            if (data.rankHardRecord[i] == 1) tRank = "D";
            if (data.rankHardRecord[i] == 2) tRank = "C";
            if (data.rankHardRecord[i] == 3) tRank = "B";
            if (data.rankHardRecord[i] == 4) tRank = "A";
            if (data.rankHardRecord[i] == 5) tRank = "S";
            if (!data.clearedHardLevel[i]) tRank = "F";

            if (data.unlockedHardLevel[i]) levelListItem.GetComponent<levelContainerHandler>().Init(tRank, data.scoreHardRecord[i], data.timeRecord[i], i, true, 2);
            else levelListItem.GetComponent<levelContainerHandler>().Init(" ", 0, 0, i, false, 0);
        }

        ChangeMode(data.lastMode);

        oldSelected = eventSystem.currentSelectedGameObject;
        
        if (sceneSelectionHandler.Instance.lastButtonName != "") eventSystem.SetSelectedGameObject(GameObject.Find(sceneSelectionHandler.Instance.lastButtonName));
        
    }
    
    void Update()
    {
        if (birdTutorialVisible)
        {
            if (birdTutorialTimer > 0)
            {
                if (Input.GetButtonDown("Light Attack") || Input.GetButtonDown("Heavy Attack") || Input.GetButtonDown("Alternate Bird") || Input.GetButtonDown("Super") || Input.GetButtonDown("Other Action") || Input.GetButtonDown("Dodge") || Input.GetMouseButtonDown(0))
                {
                    birdTutorialVisible = false;
                    rendererBirdTutorial.enabled = false;
                    eventSystem.enabled = true;
                }
            }
            birdTutorialTimer += Time.deltaTime;
        }
        else
        {
            if (Input.GetAxis("NavigateRight") > 0.5f)
            {
                currentScrollList.verticalNormalizedPosition -= 2.5f * Time.deltaTime;
            }
            if (Input.GetAxis("NavigateLeft") > 0.5f)
            {
                currentScrollList.verticalNormalizedPosition += 2.5f * Time.deltaTime;
            }
            // back to main menu
            if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Super"))
            {
                Back();
            }
            if (Input.GetButtonDown("Cycle Right")) CycleMode(true);
            if (Input.GetButtonDown("Cycle Left")) CycleMode(false);
        }

        // update marker
        if (!UIMarkerColorSwitch)
        {
            if (UIMarkerColor < 1)
            {
                UIMarkerColor += UIMarkerSpeed * Time.deltaTime;
            }
            else UIMarkerColorSwitch = true;
        }
        if (UIMarkerColorSwitch)
        {
            if (UIMarkerColor > 0.4)
            {
                UIMarkerColor -= UIMarkerSpeed * Time.deltaTime;
            }
            else UIMarkerColorSwitch = false;
        }
        
        if (currentUIMarker != null)
        {
           currentUIMarker.GetComponent<Image>().color = new Color(UIMarkerColor * UIMarkerDarkness, UIMarkerColor, UIMarkerColor * UIMarkerDarkness);
        }

        if (eventSystem.currentSelectedGameObject != null)
        {
            if (eventSystem.currentSelectedGameObject != oldSelected)
            {
                //print(eventSystem.currentSelectedGameObject.transform.position.y);

                sceneSelectionHandler.Instance.lastButtonName = eventSystem.currentSelectedGameObject.name;

                Destroy(currentUIMarker);
                currentUIMarker = Instantiate(UIMarker, eventSystem.currentSelectedGameObject.transform);
                currentUIMarker.GetComponent<RectTransform>().sizeDelta = new Vector2(eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta.x, eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta.y);
            
                if (eventSystem.currentSelectedGameObject.transform.parent.parent != null)
                {
                    
                    if (eventSystem.currentSelectedGameObject.transform.parent.parent.gameObject == levelListContent ||
                        eventSystem.currentSelectedGameObject.transform.parent.parent.gameObject == hardListContent ||
                        eventSystem.currentSelectedGameObject.transform.parent.parent.gameObject == BirdListContent)
                    {
                        if (eventSystem.currentSelectedGameObject.transform.position.y < -4f) currentScrollList.verticalNormalizedPosition = Mathf.Clamp(currentScrollList.verticalNormalizedPosition - 1f, 0, 1);
                        if (eventSystem.currentSelectedGameObject.transform.position.y > 1.8f) currentScrollList.verticalNormalizedPosition = Mathf.Clamp(currentScrollList.verticalNormalizedPosition + 1f, 0, 1);
                    }
                }
            }
        }
        
        oldSelected = eventSystem.currentSelectedGameObject;

        /*
        if (eventSystem.currentSelectedGameObject.transform.parent.parent != null)
        {
            if (eventSystem.currentSelectedGameObject.transform.parent.parent.gameObject == levelListContent ||
                eventSystem.currentSelectedGameObject.transform.parent.parent.gameObject == hardListContent ||
                eventSystem.currentSelectedGameObject.transform.parent.parent.gameObject == BirdListContent)
            {
                if (eventSystem.currentSelectedGameObject.transform.position.y < -3.42f) eventSystem.SetSelectedGameObject()
                if (eventSystem.currentSelectedGameObject.transform.position.y > 2.9f) currentScrollList.verticalNormalizedPosition = Mathf.Clamp(currentScrollList.verticalNormalizedPosition + 1f, 0, 1);
            }
        }
        */

        // update mode marker
        rendererMarker.transform.position = new Vector3(-2.6f + 4.21f * selectedGameMode, rendererMarker.transform.position.y);
    }

    public void PlayLevel(int num)
    {
        print("playing level " + num);
        if (num > 0) soundUIStart.start();
        else soundUIClick.start();

        menuMusicPlayerHandler.Instance.stopMusic();

        sceneSelectionHandler.Instance.gameModeSelected = selectedGameMode;
        
        if (num == -1) SceneManager.LoadScene("PracticeScene");

        if (num > 0 && num < 100) SceneManager.LoadScene("Level" + num + "Scene");
        
        if (num == 100) SceneManager.LoadScene("LevelEndlessScene");
    }

    public void CycleMode(bool right)
    {
        int maxmodes = 2;
        if (data.unlockedHardLevel[1]) maxmodes = 3;

        if (right) ChangeMode(mod(selectedGameMode + 1, maxmodes));
        else ChangeMode(mod(selectedGameMode - 1, maxmodes));

        eventSystem.SetSelectedGameObject(GameObject.Find("btnStartLevelMoon"));
    }

    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void ChangeMode(int num)
    {
        if (num == 0)
        {
            selectedGameMode = NORMAL;
            LevelScrollList.SetActive(true);
            BirdScrollList.SetActive(false);
            hardScrollList.SetActive(false);
            currentScrollList = scrollLevelList;
        }

        if (num == 1)
        {
            if (!data.seenBirdTutorial)
            {
                print("show bird tut");
                eventSystem.enabled = false;
                rendererBirdTutorial.enabled = true;
                birdTutorialVisible = true;
            }
            selectedGameMode = BIRD;
            LevelScrollList.SetActive(false);
            hardScrollList.SetActive(false);
            BirdScrollList.SetActive(true);
            currentScrollList = scrollBirdList;
        }

        if (num == 2)
        {
            selectedGameMode = HARD;
            LevelScrollList.SetActive(false);
            BirdScrollList.SetActive(false);
            hardScrollList.SetActive(true);
            currentScrollList = hardLevelList;
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

    public void EnterTutorial()
    {
        menuMusicPlayerHandler.Instance.stopMusic();
        soundUIClick.start();
        SceneManager.LoadScene("TutorialScene");
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
