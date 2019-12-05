using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class timerBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;

    [SerializeField] float timeStartShaking;
    [SerializeField] float timeStartShakingText;
    [SerializeField] float timeStartColorFade;
    [SerializeField] float timeStartSizeJittering;
    [SerializeField] float timeStartFlashing;

    [SerializeField] float shakeOffsetStart;
    [SerializeField] float shakeOffsetEnd;

    [SerializeField] float sizeJitterStart;
    [SerializeField] float sizeJitterEnd;
    [SerializeField] float sizeMagnitude;

    [SerializeField] float positionAmount;
    [SerializeField] float positionMagnitude;

    [SerializeField] float colorFadeSpeed;
    [SerializeField] Color textColor;

    RectTransform transformTimerBox;
    RectTransform transformTimeText;
    Vector3 startingPos;
    float startingX;
    float textStartingX;
    float startingY;
    float textStartingY;
    Vector3 startingScale;

    mainHandler gameManager;

    bool stopped;
    Color currentColor;
    Color fadeColor;
    bool swappedColor;

    float timeLeft;
    
    void Start()
    {
        transformTimerBox = GetComponent<RectTransform>();
        transformTimeText = GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>();
        startingX = GetComponent<RectTransform>().anchoredPosition.x;
        startingY = GetComponent<RectTransform>().anchoredPosition.y;
        textStartingX = GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>().anchoredPosition.x;
        textStartingY = GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>().anchoredPosition.y;
        startingScale = transform.localScale;
        timerText = GetComponentInChildren<TextMeshProUGUI>();
        timerText.color = textColor;
        gameManager = GameObject.Find("GameManager").GetComponent<mainHandler>();
        fadeColor = textColor;
        currentColor = fadeColor;

        timeLeft = 200;
    }
    
    void FixedUpdate()
    {
        if ((gameManager.gameOver || mainHandler.normalLevelFinished) && !stopped)
        {
            stopped = true;
            transformTimeText.anchoredPosition = new Vector3(textStartingX, textStartingY);
            transformTimerBox.anchoredPosition = new Vector3(startingX, startingY);
            currentColor = fadeColor;
            if (timeLeft <= 0) timerText.text = "0";
        }
        
        if (!stopped)
        {
            if (timeLeft <= timeStartShaking)
            {
                FluctuatePosition();
            }
            if (timeLeft <= timeStartShakingText)
            {
                FluctuateTextPosition();
            }
            if (timeLeft <= timeStartColorFade)
            {
                FadeTextToRed();
            }
            if (timeLeft <= timeStartSizeJittering)
            {
                FluctuateSize();
            }
            if (timeLeft <= timeStartFlashing)
            {
                SwapColor();
            }
        }

        if (swappedColor) currentColor = new Color(1, 0.7f, 0.7f);
        else currentColor = fadeColor;
        timerText.color = currentColor;
        //ShakeBox(shakeAmount, shakeMagnitude);
        //FluctuateSize(sizeAmount, sizeMagnitude);
    }

    public void FadeTextToRed()
    {
        fadeColor.r += colorFadeSpeed * Time.deltaTime;
        fadeColor.b -= colorFadeSpeed * Time.deltaTime;
        fadeColor.g -= colorFadeSpeed * Time.deltaTime;
    }

    public void FluctuateTextPosition()
    {
        float currentShakeOffset = shakeOffsetStart + (shakeOffsetEnd - shakeOffsetStart) * (1 - timeLeft / timeStartShakingText);
        transformTimeText.anchoredPosition = new Vector3(Random.Range(textStartingX - currentShakeOffset, textStartingX + currentShakeOffset), Random.Range(textStartingY - currentShakeOffset, textStartingY + currentShakeOffset), 0);
    }

    public void FluctuatePosition()
    {
        float currentShakeOffset = shakeOffsetStart + (shakeOffsetEnd - shakeOffsetStart) * (1 - timeLeft / timeStartShaking);
        transformTimerBox.anchoredPosition = new Vector3(Random.Range(startingX - currentShakeOffset, startingX + currentShakeOffset), Random.Range(startingY - currentShakeOffset, startingY + currentShakeOffset), 0);
        
        //float moveX = Random.Range(-positionAmount, positionAmount);
        //float moveY = Random.Range(-positionAmount, positionAmount);
        //
        //timBox.position = new Vector3(timBox.position.x + moveX, timBox.position.y + moveY, 0);
        //
        //float clampedX = Mathf.Clamp(timBox.position.x, startingPos.x - positionMagnitude, startingPos.x + positionMagnitude);
        //float clampedY = Mathf.Clamp(timBox.position.y, startingPos.y - positionMagnitude, startingPos.y + positionMagnitude);
        //
        //timBox.position = new Vector3(clampedX, clampedY, 0);
        //
        //print(startingPos + "This is the starting position");
    }

    public void FluctuateSize()
    {
        float currentSizeJitter = sizeJitterStart + (sizeJitterEnd - sizeJitterStart) * (1 - timeLeft / timeStartSizeJittering);
        float tJitter = Random.Range(1, 1 + currentSizeJitter);
        transformTimeText.localScale = new Vector3(tJitter, tJitter);

        //float expand = Random.Range(-sizeAmount, sizeAmount);
        //
        //transformTimerBox.localScale = new Vector3(transformTimerBox.localScale.x + expand, transformTimerBox.localScale.y + expand, 1);
        //
        //if(transformTimerBox.localScale.x < startingScale.x)
        //{
        //    transformTimerBox.localScale = startingScale;
        //}
        //else if (transformTimerBox.localScale.x >= startingScale.x + sizeMagnitude)
        //{
        //    transformTimerBox.localScale = new Vector3(startingScale.x + sizeMagnitude, startingScale.y + sizeMagnitude, 1);
        //}
    }

    public void SwapColor()
    {
        swappedColor = !swappedColor;
    }

    public void ShakeBox()
    {
        //float shake = Random.Range(-shakeAmount, shakeAmount);
        //
        //timBox.rotation  = Quaternion.Euler(0f, 0f, timBox.rotation.z + shake);
        //
        //if (timBox.rotation.z <= -shakeMagnitude)
        //{
        //    timBox.rotation = Quaternion.Euler(0, 0, -shakeMagnitude);
        //} 
        //else if (timBox.rotation.z >= shakeMagnitude)   
        //{
        //    timBox.rotation = Quaternion.Euler(0, 0, shakeMagnitude);
        //}
    }

    public void UpdateText(float currentTime)
    {
        timeLeft = currentTime;

        //string tTimerText = (Mathf.Round(currentTime * 100f) / 100f).ToString();
        //
        //if (!tTimerText.Contains(".")) tTimerText += ".00";
        //else
        //{
        //    int dot = tTimerText.IndexOf('.');
        //    if (tTimerText.Substring(dot).Length == 2) tTimerText += "0";
        //}
        //
        //currentTime.ToString().Remove(currentTime.ToString().IndexOf('.') + 2);
        //
        //timerText.text = tTimerText;



        //timerText.text = currentTime.ToString();
        //if (currentTime.ToString().Length >= 6) timerText.text = currentTime.ToString().Remove(5);
        //print(currentTime.ToString("000.00"));
        timerText.text = currentTime.ToString("0.00");
    }

    public void GameOver()
    {
        timerText.text = "0";
        transformTimerBox.localScale = new Vector3(2, 2, 1);
        transformTimerBox.position = startingPos;
        transformTimerBox.rotation = new Quaternion(0, 0, 0, 0);
    }
}
