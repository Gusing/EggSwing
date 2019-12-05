using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResultScreen : MonoBehaviour
{
    mainHandler mH;
    playerHandler pH;

    public Color[] colorsDifficulty;

    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI txtScore;
    public TextMeshProUGUI txtScoreRecord;
    public TextMeshProUGUI rankLetter;
    public TextMeshProUGUI txtLevelTitle;
    public TextMeshProUGUI txtNewRecord;

    public TextMeshProUGUI txtStats1Title;
    public TextMeshProUGUI txtStats1;
    public TextMeshProUGUI txtStats2Title;
    public TextMeshProUGUI txtStats2;

    public Button btnBack;
    public Button btnRetry;
    public Button btnShop;

    Text txtCurrency;

    public Text levelCompleteText;

    RectTransform thisRect;

    bool countingCurrency;
    bool visible;
    bool victory;

    public GameObject UIMarker;
    GameObject oldSelected;
    GameObject currentUIMarker;
    float UIMarkerColor;
    bool UIMarkerColorSwitch;

    EventSystem eventSystem;

    float countingTimer;
    public float countingTime = 1;

    public string[] levelNames;
    public string[] gameModeNames;
    public Color[] rankColors =
    {
        new Color(0.65f, 0.65f, 0.65f),
        new Color(0.57f, 0.6f, 0.91f),
        new Color(0.94f, 0.69f, 0.3f),
        new Color(0.2f, 0.76f, 1f),
        new Color(1f, 0.05f, 0.95f),
        new Color(1f, 0.9f, 0f)
    };
    
    FMOD.Studio.EventInstance soundPointsCounter;

    void Awake()
    {
        thisRect = GetComponent<RectTransform>();
        mH = FindObjectOfType<mainHandler>();
        pH = FindObjectOfType<playerHandler>();
        txtCurrency = GameObject.Find("txtCurrency").GetComponent<Text>();

        MovePosition(new Vector3(100000, 100000, 0));

        countingTimer = 0;
        visible = false;
        print("start false");
        countingCurrency = false;

        soundPointsCounter = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Pointscounter");

        eventSystem = EventSystem.current;

        txtScore.text = "<mspace=0.65em>" + pH.currentScore.ToString() + "</mspace>";
    }
    
    void Start()
    {

    }
    
    void Update()
    {
        if (!visible) return;

        UpdateSelectedButton();

        if (countingCurrency)
        {
            CountCurrency();
            
            if (countingTimer >= countingTime)
            {
                countingCurrency = false;
                currencyText.text = "<mspace=0.55em>" + pH.currentCurrency.ToString() + "</mspace>";
                currencyText.transform.localScale = new Vector3(1, 1, 1);
                soundPointsCounter.setParameterValue("End", 1);

                btnShop.gameObject.SetActive(true);
                btnBack.gameObject.SetActive(true);
                btnRetry.gameObject.SetActive(true);
                eventSystem.SetSelectedGameObject(btnRetry.gameObject);
            }
        }
    }

    void CountCurrency()
    {
        countingTimer += Time.deltaTime;
        currencyText.text = "<mspace=0.55em>" + Mathf.RoundToInt((mH.initialCurrency + (((float)pH.currentCurrency - (float)mH.initialCurrency) * (float)countingTimer / countingTime))).ToString() + "</mspace>";
        currencyText.transform.localScale = new Vector3(0.7f + (0.3f * (float)countingTimer / countingTime), 0.7f + (0.3f * (float)countingTimer / countingTime), 1);
    }

    public void MovePosition(Vector3 newPosition)
    {
        GetComponent<RectTransform>().position = newPosition;
    }

    void UpdateSelectedButton()
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
                Destroy(currentUIMarker);
                
                currentUIMarker = Instantiate(UIMarker, eventSystem.currentSelectedGameObject.transform);
                currentUIMarker.GetComponent<RectTransform>().sizeDelta = new Vector2(eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta.x, eventSystem.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta.y);
            }
        }

        oldSelected = eventSystem.currentSelectedGameObject;
    }
    
    public void ShowResultScreen(bool victory, bool endless)
    {
        if (!victory) rankLetter.GetComponent<Animator>().SetBool("stop", true);
        if (victory) txtScore.text = "<mspace=0.65em>" + pH.currentScore.ToString() + "</mspace>";

        txtScoreRecord.text = "<mspace=0.65em>" + mH.currentRecord.ToString() + "</mspace>";
        if (!mH.gotNewRecord) txtNewRecord.enabled = false;
        
        btnBack.gameObject.SetActive(false);
        btnShop.gameObject.SetActive(false);
        btnRetry.gameObject.SetActive(false);

        if (endless)
        {
            txtStats1Title.text = "Highest Streak";
            txtStats1.text = mH.currentMaxStreak.ToString();
            txtStats2Title.text = "Survival Time";
            txtStats2.text = Mathf.Round(mainHandler.levelTimer).ToString();
            levelCompleteText.color = Color.green;
            levelCompleteText.text = "Survival Data";
            txtLevelTitle.text = "Survival";
            rankLetter.text = "-";
        }
        else
        {
            if (mH.gameMode == 0)
            {
                txtStats1Title.text = "Highest Streak";
                txtStats1.text = mH.currentMaxStreak.ToString();
                txtStats2Title.enabled = false;
                txtStats2.enabled = false;

            }
            if (mH.gameMode == 1)
            {
                txtStats1Title.text = "Highest Combo";
                txtStats1.text = mH.currentMaxStreak.ToString();
                txtStats2Title.enabled = false;
                txtStats2.enabled = false;
            }
            if (mH.gameMode == 2)
            {
                txtStats1Title.text = "Highest Streak";
                txtStats1.text = mH.currentMaxStreak.ToString();
                txtStats2Title.text = "Time Left";
                txtStats2.text = mH.currentTimeLimit.ToString("0.00");
            }
            if (!victory)
            {
                txtStats1Title.enabled = false;
                txtStats1.enabled = false;
                txtStats2Title.enabled = false;
                txtStats2.enabled = false;
                levelCompleteText.text = "Level Failed";
                levelCompleteText.color = Color.red;
                rankLetter.text = "<mspace=0.65em>" + "-" + "</mspace>";
                rankLetter.color = Color.gray;
            }
            else
            {
                levelCompleteText.color = Color.green;
                levelCompleteText.text = "Level Complete!";
                rankLetter.text = "<mspace=0.65em>" + pH.textRank.text.ToString() + "</mspace>";
                rankLetter.color = rankColors[pH.currentRank];
            }
            txtLevelTitle.text = levelNames[mH.level - 1] + "<color=#" + ColorUtility.ToHtmlStringRGB(colorsDifficulty[mH.gameMode]) + "> " + gameModeNames[mH.gameMode] + "</color>";
        }

        MovePosition(new Vector3(0, 40, -10));
        
        if (pH.currentCurrency - mH.initialCurrency > 0)
        {
            countingCurrency = true;
            soundPointsCounter.start();
            countingTime = Mathf.Clamp((((float)pH.currentCurrency - (float)mH.initialCurrency) * 0.02f), 0.1f, 4);
        }
        else
        {
            currencyText.text = "<mspace=0.55em>" + pH.currentCurrency.ToString() + "</mspace>";
            btnBack.gameObject.SetActive(true);
            btnShop.gameObject.SetActive(true);
            btnRetry.gameObject.SetActive(true);
            eventSystem.SetSelectedGameObject(btnRetry.gameObject);
        }
        
        visible = true;
    }
}