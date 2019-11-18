using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class timerBox : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI timerText;

    [SerializeField] float shakeAmount;
    [SerializeField] float shakeMagnitude;

    [SerializeField] float sizeAmount;
    [SerializeField] float sizeMagnitude;

    [SerializeField] float positionAmount;
    [SerializeField] float positionMagnitude;

    [SerializeField] float colorFadeSpeed;
    [SerializeField] Color textColor;

    RectTransform timBox;
    Vector3 startingPos;
    Vector3 startingScale;


    // Start is called before the first frame update
    void Start()
    {
        timBox = GetComponent<RectTransform>();
        startingPos = transform.position;
        startingScale = transform.localScale;
        timerText = GetComponentInChildren<TextMeshProUGUI>();
        timerText.color = textColor;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //ShakeBox(shakeAmount, shakeMagnitude);
        //FluctuateSize(sizeAmount, sizeMagnitude);
        //FluctuatePosition(positionAmount, positionMagnitude);

    }

    public void FadeTextToRed()
    {
        textColor.r += colorFadeSpeed * Time.deltaTime;
        textColor.b -= colorFadeSpeed * Time.deltaTime;    
        textColor.g -= colorFadeSpeed * Time.deltaTime;
        timerText.color = textColor;
    }

    public void FluctuatePosition()
    {
        float moveX = Random.Range(-positionAmount, positionAmount);
        float moveY = Random.Range(-positionAmount, positionAmount);

        timBox.position = new Vector3(timBox.position.x + moveX, timBox.position.y + moveY, 0);

        float clampedX = Mathf.Clamp(timBox.position.x, startingPos.x - positionMagnitude, startingPos.x + positionMagnitude);
        float clampedY = Mathf.Clamp(timBox.position.y, startingPos.y - positionMagnitude, startingPos.y + positionMagnitude);

        timBox.position = new Vector3(clampedX, clampedY, 0);

        print(startingPos + "This is the starting position");
    }

    public void FluctuateSize()
    {
        float expand = Random.Range(-sizeAmount, sizeAmount);

        timBox.localScale = new Vector3(timBox.localScale.x + expand, timBox.localScale.y + expand, 1);

        if(timBox.localScale.x < startingScale.x)
        {
            timBox.localScale = startingScale;
        }
        else if(timBox.localScale.x >= startingScale.x + sizeMagnitude)
        {
            timBox.localScale = new Vector3(startingScale.x + sizeMagnitude, startingScale.y + sizeMagnitude, 1);
        }
    }


    public void ShakeBox()
    {

        float shake = Random.Range(-shakeAmount, shakeAmount);
        
        timBox.rotation  = Quaternion.Euler(0f, 0f, timBox.rotation.z + shake);

        if(timBox.rotation.z <= -shakeMagnitude)
        {
            timBox.rotation = Quaternion.Euler(0, 0, -shakeMagnitude);
        } else if(timBox.rotation.z >= shakeMagnitude)   
        {
            timBox.rotation = Quaternion.Euler(0, 0, shakeMagnitude);
        }

    }

    public void UpdateText(float currentTime)
    {
        timerText.text = currentTime.ToString();
        if (currentTime.ToString().Length >= 6) timerText.text = currentTime.ToString().Remove(5);

    }
}
