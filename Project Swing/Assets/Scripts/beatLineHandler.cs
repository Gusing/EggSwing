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

    // Start is called before the first frame update
    void Start()
    {
        timeBeforeCollision = beatIndicatorHandlerB.staticTimeBeforeCollision;
        startingX = beatIndicatorHandlerB.staticStartingX;

        transform.localPosition = new Vector3((startingX - 1) * xSide, 0);
    }

    public void Init(bool side)
    {
        if (side) xSide = 1;
        else xSide = -1;
    }


    // Update is called once per frame
    void Update()
    {
        collisionTimer += Time.deltaTime;

        currentX = startingX - (collisionTimer / timeBeforeCollision) * (startingX - 1);

        if (currentX <= 1f) Destroy(gameObject);

        transform.localPosition = new Vector3((currentX - 1) * xSide, 0);
        //transform.localScale = new Vector3(currentX, currentX, 1);
    }
}
