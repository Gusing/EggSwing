using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBirdHandler : MonoBehaviour
{
    public SpriteRenderer[] renderers;

    public Sprite spriteBirdA;
    public Sprite spriteBirdB;

    int[] spawns;
    float localBpm;
    float bpmInSeconds;
    float timeUntilCrash;
    float timeToCharge;
    bool charging;
    public bool dead;
    float lifeTimer;

    public bool readyToBeHit;

    FMOD.Studio.EventInstance soundCharge;

    public void init(int[] spawnIntervals)
    {
        spawns = spawnIntervals;
    }

    // Start is called before the first frame update
    void Start()
    {
        soundCharge = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_short");

        spawns = new int[1] { 1 };

        for (int i = 0; i < renderers.Length; i++)
        {
            if (spawns.Length + 1 <= i) renderers[i].enabled = true;
        }

        transform.position = new Vector3(10.08f, 3.37f);

        localBpm = mainHandler.currentBpm;

        bpmInSeconds = (float)60 / (float)localBpm;

        timeUntilCrash = bpmInSeconds * 4;
        timeToCharge = bpmInSeconds * 2;

    }

    public void setHit()
    {
        readyToBeHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (!dead)
        {
            if (lifeTimer >= timeUntilCrash && !charging)
            {
                soundCharge.setParameterValue("Hit", 0);
                soundCharge.start();
                charging = true;
                renderers[0].sprite = spriteBirdB;
            }

            if (charging && !readyToBeHit && lifeTimer >= (timeUntilCrash + timeToCharge) - mainHandler.currentLeniency)
            {
                readyToBeHit = true;
            }

            if (charging && lifeTimer >= timeUntilCrash + timeToCharge)
            {
                dead = true;
                transform.Translate(new Vector3(0, -3f));
                soundCharge.setParameterValue("Hit", 1);
                soundCharge.start();
            }

            if (!charging) transform.Translate(new Vector3(-3f * Time.deltaTime, 0));

        }

        if (dead && readyToBeHit && lifeTimer >= timeUntilCrash + timeToCharge + mainHandler.currentLeniency)
        {
            readyToBeHit = false;
        }


    }
}
