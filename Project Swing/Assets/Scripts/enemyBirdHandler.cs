﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBirdHandler : MonoBehaviour
{
    public Sprite spriteBirdA;
    public Sprite spriteBirdA2;
    public Sprite spriteBirdA3;
    public Sprite spriteBirdB;
    public Sprite spriteBirdB2;
    public Sprite spriteBirdB3;

    Animator animator;

    SpriteRenderer localRenderer;

    int[] spawns;
    float localBpm;
    float bpmInSeconds;
    float timeUntilCrash;
    float timeToCharge;
    bool charging;
    public bool startedCharge;
    int type;
    public bool dead;
    public bool readyToBeDestroyed;
    float lifeTimer;

    int beatPassed;
    int beatLimit;
    int localState;

    public bool readyToBeHit;

    int offsetAmount;

    GameObject player;

    FMOD.Studio.EventInstance soundWarning;
    FMOD.Studio.EventInstance soundAttack;
    FMOD.Studio.EventInstance soundPlayerHit;

    readonly int FAIL = 0, SUCCESS = 1, BEAT = 2, OFFBEAT = 3;

    readonly int GREEN = 0, RED = 1, YELLOW = 2;

    public void init(int speedType)
    {
        localRenderer = GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();

        localBpm = mainHandler.currentBpm;

        player = GameObject.Find("Player");

        bpmInSeconds = (float)60 / (float)localBpm;

        type = speedType;
        if (type == RED)
        {
            beatLimit = 5;
            localRenderer.sprite = spriteBirdA3;
            timeToCharge = bpmInSeconds * 1;
        }
        if (type == YELLOW)
        {
            beatLimit = 6;
            localRenderer.sprite = spriteBirdA;
            timeToCharge = bpmInSeconds * 2;
        }
        if (type == GREEN)
        {
            beatLimit = 8;
            localRenderer.sprite = spriteBirdA2;
            timeToCharge = bpmInSeconds * 4;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        soundWarning = FMODUnity.RuntimeManager.CreateInstance("event:/Birds/Birds_warning");
        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Birds/Birds_attack");
        soundPlayerHit = FMODUnity.RuntimeManager.CreateInstance("event:/Birds/Birds_player_attack");
        
        transform.position = new Vector3(10.08f, 3.37f);

        localBpm = mainHandler.currentBpm;

        bpmInSeconds = (float)60 / (float)localBpm;

        print("spawn with offset: " + mainHandler.currentBeatTimer);

        timeUntilCrash = mainHandler.currentBeatTimer;

        localState = mainHandler.currentState;

        //timeUntilCrash = bpmInSeconds * 4;

    }

    public void setOffset(int amount)
    {
        startedCharge = false;
        offsetAmount = amount;
        transform.Translate(new Vector3(0, 0.3f * offsetAmount));
    }

    public void setHit()
    {
        soundPlayerHit.start();
        readyToBeHit = false;
        readyToBeDestroyed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (localState != mainHandler.currentState)
        {
            localState = mainHandler.currentState;
            if (mainHandler.currentState == BEAT)
            {
                beatPassed++;
            }
        }

        if (!dead)
        {
            if (beatPassed >= 4)
            {
                if (mainHandler.currentBeatTimer >= timeUntilCrash && !charging)
                {
                    soundWarning.start();
                    charging = true;
                    startedCharge = true;
                }
            }

            if (timeUntilCrash == 0 && beatPassed == beatLimit - 1)
            {
                if (mainHandler.currentBeatTimer >= (bpmInSeconds - mainHandler.currentLeniency))
                {
                    readyToBeHit = true;
                }
            }

            if (beatPassed >= beatLimit)
            {
                if (mainHandler.currentBeatTimer >= timeUntilCrash - mainHandler.currentLeniency)
                {
                    readyToBeHit = true;
                }

                if (mainHandler.currentBeatTimer >= timeUntilCrash)
                {
                    soundAttack.start();
                    dead = true;
                    Vector3 tv = player.transform.position - transform.position;
                    tv = Vector3.Normalize(tv);
                    transform.position = player.transform.position - ((Vector3.Distance(player.transform.position, transform.position) / 2) * tv);
                }
            }

            /*
            if (lifeTimer >= timeUntilCrash && !charging)
            {
                soundWarning.start();
                charging = true;
                if (type == 1)
                {
                    renderers[0].sprite = spriteBirdB3;
                }
                if (type == 2)
                {
                    renderers[0].sprite = spriteBirdB;
                }
                if (type == 4)
                {
                    renderers[0].sprite = spriteBirdB2;
                }
                
            }

            if (charging && !readyToBeHit && lifeTimer >= (timeUntilCrash + timeToCharge) - mainHandler.currentLeniency)
            {
                readyToBeHit = true;
            }

            if (charging && lifeTimer >= timeUntilCrash + timeToCharge)
            {
                soundAttack.start();
                dead = true;
                transform.Translate(new Vector3(0, -3f));
               
            }
            */

            if (!charging) transform.Translate(new Vector3(-3f * Time.deltaTime, 0));

        }

        if (beatPassed >= beatLimit && readyToBeHit)
        {
            if (mainHandler.currentBeatTimer >= timeUntilCrash + mainHandler.currentLeniency)
            {
                GameObject.Find("Player").GetComponent<playerHandler>().TakeDamage(1, 0, true);
                readyToBeHit = false;
                readyToBeDestroyed = true;
            }

        }

        updateAnimations();

        /*
        if (dead && readyToBeHit && lifeTimer >= timeUntilCrash + timeToCharge + mainHandler.currentLeniency)
        {
            GameObject.Find("Player").GetComponent<playerHandler>().TakeDamage(1, 0, true);
            readyToBeHit = false;
            readyToBeDestroyed = true;
        }
        */


    }

    void updateAnimations()
    {
        animator.SetInteger("Color", type);
        animator.SetBool("Ready", charging);
    }
}