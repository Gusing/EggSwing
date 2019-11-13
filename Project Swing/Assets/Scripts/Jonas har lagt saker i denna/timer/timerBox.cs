using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timerBox : MonoBehaviour
{

    [SerializeField] Text timerText;

    [SerializeField] float shakeAmount;
    [SerializeField] float shakeMagnitude;

    [SerializeField] float sizeAmount;
    [SerializeField] float sizeMagnitude;

    [SerializeField] float positionAmount;
    [SerializeField] float positionMagnitude;

    [SerializeField] float startShakeTime;
    [SerializeField] float startExpandTime;


    RectTransform timBox;
    Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        timBox = GetComponent<RectTransform>();
        startingPos = transform.position;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ShakeBox(shakeAmount, shakeMagnitude);
        FluctuateSize(sizeAmount, sizeMagnitude);
        FluctuatePosition(positionAmount, positionMagnitude);

    }

    void FluctuatePosition(float positionAmount, float positionLimit)
    {
        float moveX = Random.Range(-positionAmount, positionAmount);
        float moveY = Random.Range(-positionAmount, positionAmount);

        timBox.position = new Vector3(timBox.position.x + moveX, timBox.position.y + moveY, 0);


        


        print(startingPos + "This is the starting position");
    }

    void FluctuateSize(float sizeAmount, float sizeLimit)
    {
        float expand = Random.Range(-sizeAmount, sizeAmount);

        timBox.localScale = new Vector3(timBox.localScale.x + expand, timBox.localScale.y + expand, 1);

        if(timBox.localScale.x < 1)
        {
            timBox.localScale = new Vector3(1, 1, 1);
        }
        else if(timBox.localScale.x >= sizeLimit)
        {
            timBox.localScale = new Vector3(sizeLimit, sizeLimit, 1);
        }
    }


    void ShakeBox(float shakeAmount, float shakeMagnitude)
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
}
