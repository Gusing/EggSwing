using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatRingHandler : MonoBehaviour
{
    float timeBeforeCollision;
    float collisionTimer;
    float currentScale;
    float startingScale;

    // Start is called before the first frame update
    void Start()
    {
        timeBeforeCollision = beatIndicatorHandler.staticTimeBeforeCollision;
        startingScale = beatIndicatorHandler.staticStartingScale;

        transform.localPosition = new Vector3(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        collisionTimer += Time.deltaTime;

        currentScale = startingScale - (collisionTimer / timeBeforeCollision) * (startingScale - 1);

        if (currentScale <= 0.7f) Destroy(gameObject);

        transform.localScale = new Vector3(currentScale, currentScale, 1);
    }
}
