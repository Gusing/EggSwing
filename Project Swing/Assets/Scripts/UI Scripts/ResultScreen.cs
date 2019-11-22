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

    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI rankLetter;

    Text txtCurrency;

    public Text levelCompleteText;

    RectTransform thisRect;

    bool countingCurrency;
    bool visible;

    public GameObject UIMarker;
    GameObject oldSelected;
    GameObject currentUIMarker;
    float UIMarkerColor;
    bool UIMarkerColorSwitch;

    EventSystem eventSystem;

    float countingTimer;
    public float countingTime = 1;

    FMOD.Studio.EventInstance soundPointsCounter;

    void Awake()
    {
        thisRect = GetComponent<RectTransform>();
        mH = FindObjectOfType<mainHandler>();
        pH = FindObjectOfType<playerHandler>();
        txtCurrency = GameObject.Find("txtCurrency").GetComponent<Text>();

        MovePosition(new Vector3(100000, 100000, 0));
    }
    
    void Start()
    {
        soundPointsCounter = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Pointscounter");

        eventSystem = EventSystem.current;

        scoreText.text = "<mspace=0.65em>" + pH.currentScore.ToString() + "</mspace>";

        visible = false;
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
                currencyText.text = "<mspace=0.65em>" + pH.currentCurrency.ToString();
                currencyText.transform.parent.localScale = new Vector3(2, 2, 1);
                soundPointsCounter.setParameterValue("End", 1);
            }
        }
        
    }

    void CountCurrency()
    {
        countingTimer += Time.deltaTime;
        currencyText.text = "<mspace=0.65em>" + Mathf.RoundToInt((mH.initialCurrency + (((float)pH.currentCurrency - (float)mH.initialCurrency) * (float)countingTimer / countingTime))).ToString() + "</mspace>";
        currencyText.transform.parent.localScale = new Vector3(1.5f + (0.5f * (float)countingTimer / countingTime), 1.5f + (0.5f * (float)countingTimer / countingTime), 1);
    }

    public void MovePosition(Vector3 newPosition)
    {
        print("move");
        GetComponent<RectTransform>().position = newPosition;
    }

    public void ShowVictoryText(bool b)
    {
        if (b)
        {
            levelCompleteText.text = "Level Complete!";
            levelCompleteText.color = Color.green;
            rankLetter.text = "<mspace=0.65em>" + pH.textRank.text.ToString() + "</mspace>";
        }
        else if (!b)
        {
            levelCompleteText.text = "Level Failed!";
            levelCompleteText.color = Color.red;
            rankLetter.text = "<mspace=0.65em>" + "-" + "</mspace>";
        }

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
    
    public void ShowResultScreen(bool victory)
    {
        soundPointsCounter.start();
        countingCurrency = true;
        if (victory) scoreText.text = "<mspace=0.65em>" + pH.currentScore.ToString() + "</mspace>";

        MovePosition(new Vector3(0, 40, -10));
        ShowVictoryText(victory);
        visible = true;
    }
}