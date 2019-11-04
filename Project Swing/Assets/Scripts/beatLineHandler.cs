using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatLineHandler : MonoBehaviour
{
    float timeBeforeCollision;
    float collisionTimer;
    float currentX;
    float startingX;
    int xSide;
    int localBpm;
    float bpmInSeconds;
    float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        //timeBeforeCollision = beatIndicatorHandlerB.staticTimeBeforeCollision;
        //startingX = beatIndicatorHandlerB.staticStartingX;
        startingX = 5;

        localBpm = mainHandler.currentBpm;
        transform.localPosition = new Vector3(5 * xSide, 0);
    }

    public void Init(bool side, float bpmSec, int preset = 0)
    {
        bpmInSeconds = bpmSec;
        if (side) xSide = 1;
        else xSide = -1;

        lifeTime = bpmInSeconds * preset;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime >= bpmInSeconds * 4) Destroy(gameObject);
       
        //collisionTimer += Time.deltaTime;

        //currentX = startingX - (collisionTimer / timeBeforeCollision) * (startingX - 1);

        //if (currentX <= 1f) Destroy(gameObject);
        //if (transform.localPosition.x <= 0.01f && transform.localPosition.x >= -0.01f) Destroy(gameObject);

        transform.localPosition = (new Vector3((5.1f * -xSide) - (lifeTime / (bpmInSeconds * 4)) * 5f * -xSide, 0));

        //transform.localPosition = new Vector3((currentX - 1) * xSide, 0);
        //transform.localScale = new Vector3(currentX, currentX, 1);
    }
}
