using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    mainHandler mH;
    playerHandler pH;

    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI rankLetter;

    public Text gameOverText;
    public Text levelCompleteText;

    RectTransform thisRect;

    bool countingCurrency;


    float countingTimer;
    // Start is called before the first frame update
    void Awake()
    {
        thisRect = GetComponent<RectTransform>();
        mH = GameObject.FindObjectOfType<mainHandler>();
        pH = GameObject.FindObjectOfType<playerHandler>();

    }

    // Update is called once per frame
    void Update()
    {
        if (mH.moveResultScreen)
        {
            thisRect.localPosition = new Vector3(0, 40, -10);
            countingCurrency = true;
        }
        else
        {
            thisRect.localPosition = new Vector3(0, 10000, -10);
            countingCurrency = false;
        }

        if(mH.gameOver)
        {
            levelCompleteText.enabled = false;

        } else if(mH.GetFinishedNormalLevel() || mH.GetFinishedBirdLevel())
        {
            gameOverText.enabled = false;
        }

        

        currencyText.text = "<mspace=0.65em>" + mH.txtCurrency.text.ToString() + "</mspace>";
        scoreText.text = "<mspace=0.65em>" + pH.currentScore.ToString() + "</mspace>";


        if(mH.GetFinishedNormalLevel())
        {
        rankLetter.text = "<mspace=0.65em>" + pH.textRank.text.ToString() + "</mspace>";
        }
        else
        {
        rankLetter.text = "<mspace=0.65em>" + "-" + "</mspace>";
        }


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
        currencyText.text = Mathf.RoundToInt((mH.initialCurrency + (((float)pH.currentCurrency - (float)mH.initialCurrency) * (((float)countingTimer - 1.5f) / 1f)))).ToString();
        currencyText.transform.parent.localScale = new Vector3(1.5f + (0.5f * (((float)countingTimer - 1.5f) / 1f)), 1.5f + (0.5f * (((float)countingTimer - 1.5f) / 1f)), 1);
    }

}