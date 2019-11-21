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

    public GameObject UIMarker;
    GameObject oldSelected;
    GameObject currentUIMarker;
    float UIMarkerColor;
    bool UIMarkerColorSwitch;

    EventSystem eventSystem;

    float countingTimer;
    // Start is called before the first frame update
    void Awake()
    {
        thisRect = GetComponent<RectTransform>();
        mH = GameObject.FindObjectOfType<mainHandler>();
        pH = GameObject.FindObjectOfType<playerHandler>();
        txtCurrency = GameObject.Find("txtCurrency").GetComponent<Text>();

        MovePosition(new Vector3(100000, 100000, 0));

    }


    void Start()
    {
        eventSystem = EventSystem.current;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateSelectedButton();


        //currencyText.text =  "<mspace=0.65em>"+ txtCurrency.text.ToString() + "</mspace>";
        scoreText.text = "<mspace=0.65em>" + pH.currentScore.ToString() + "</mspace>";

        


        if (countingCurrency)
        {
            CountCurrency();
        }

        if(countingTimer > 1f)
        {
            countingCurrency = false;
        }

    }

    void CountCurrency()
    {
        countingTimer += Time.deltaTime;
        currencyText.text = "<mspace=0.65em>" + Mathf.RoundToInt((mH.initialCurrency + (((float)pH.currentCurrency - (float)mH.initialCurrency) * (((float)countingTimer - 1.5f) / 1f)))).ToString() + "</mspace>";
        currencyText.transform.parent.localScale = new Vector3(1.5f + (0.5f * (((float)countingTimer - 1.5f) / 1f)), 1.5f + (0.5f * (((float)countingTimer - 1.5f) / 1f)), 1);
    }

    public void MovePosition(Vector3 newPosition)
    {
        this.transform.localPosition = newPosition;
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
        MovePosition(new Vector3(0, 40, -10));
        countingCurrency = true;
        ShowVictoryText(victory);

    }
}