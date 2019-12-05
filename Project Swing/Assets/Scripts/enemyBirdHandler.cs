using System.Collections;
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

    public GameObject effectFeather;

    int[] spawns;
    float localBpm;
    float bpmInSeconds;
    float timeUntilCrash;
    float timeToCharge;
    bool charging;
    public bool startedCharge;
    int type;
    public bool dead;
    bool waitingForAnimation;
    public bool readyToBeDestroyed;
    float lifeTimer;
    public float animationTimeLeft;

    int beatPassed;
    int beatLimit;
    int localState;

    public bool readyToBeHit;
    public bool perfectTiming;

    int offsetAmount;

    GameObject player;

    FMOD.Studio.EventInstance soundWarning;
    FMOD.Studio.EventInstance soundAttack;
    FMOD.Studio.EventInstance soundPlayerHit;

    readonly int FAIL = 0, SUCCESS = 1, BEAT = 2, OFFBEAT = 3;

    readonly int GREEN = 0, RED = 1, YELLOW = 2;

    public void init(int speedType, int order)
    {
        localRenderer = GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();

        localBpm = mainHandler.currentBpm;

        player = GameObject.Find("Player");

        bpmInSeconds = (float)60 / (float)localBpm;

        localRenderer.sortingOrder = order;

        transform.Translate(new Vector3(0, 0, Random.Range(-0.1f, 0.1f)));

        type = speedType;
        if (type == RED)
        {
            beatLimit = 5;
            localRenderer.sprite = spriteBirdA3;
            timeToCharge = bpmInSeconds * 1;
            soundWarning.setParameterValue("Bird", 2);
        }
        if (type == YELLOW)
        {
            beatLimit = 6;
            localRenderer.sprite = spriteBirdA;
            timeToCharge = bpmInSeconds * 2;
            soundWarning.setParameterValue("Bird", 1);
        }
        if (type == GREEN)
        {
            beatLimit = 8;
            localRenderer.sprite = spriteBirdA2;
            timeToCharge = bpmInSeconds * 4;
            soundWarning.setParameterValue("Bird", 0);
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

        //print("spawn with offset: " + mainHandler.currentBeatTimer);

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
        Attack();
        readyToBeHit = false;
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

        if (!dead && !mainHandler.normalLevelFinished)
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
                    perfectTiming = true;
                    Attack();
                }
            }

            if (!charging) transform.Translate(new Vector3(-2.8f * Time.deltaTime, 0));

        }
        else if (!waitingForAnimation)
        {
            waitingForAnimation = true;
            Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        }

        if (beatPassed >= beatLimit && readyToBeHit)
        {
            if (mainHandler.currentBeatTimer >= timeUntilCrash + mainHandler.currentLeniency)
            {
                GameObject.Find("Player").GetComponent<playerHandler>().TakeDamage(1, 0, true);
                readyToBeHit = false;
                readyToBeDestroyed = true;
                dead = true;
            }

        }

        updateAnimations();
        
        if (dead && !readyToBeDestroyed)
        {
            readyToBeDestroyed = true;
        }

    }

    void Attack()
    {
        soundAttack.start();
        if (!dead)
        {
            Vector3 tv = player.transform.position - transform.position;
            tv = Vector3.Normalize(tv);
            transform.position = player.transform.position - ((Vector3.Distance(player.transform.position, transform.position) / 2) * tv);
            Vector3 vectorToTarget = player.transform.position - transform.position;
            float angle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) - 270;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 999);

            GameObject tgo;
            tgo = Instantiate(effectFeather, transform.position + Random.Range(-1.5f, -0.4f) * transform.up + Random.Range(-0.5f, 0.5f) * transform.right, new Quaternion(0, 0, 0, 0));
            tgo.transform.Rotate(new Vector3(0, 180 * Random.Range((int)0, (int)2), Random.Range(0, 360)));
            tgo = Instantiate(effectFeather, transform.position + Random.Range(-0.6f, 0.6f) * transform.up + Random.Range(-0.5f, 0.5f) * transform.right, new Quaternion(0, 0, 0, 0));
            tgo.transform.Rotate(new Vector3(0, 180 * Random.Range((int)0, (int)2), Random.Range(0, 360)));
            tgo = Instantiate(effectFeather, transform.position + Random.Range(0.4f, 1.5f) * transform.up + Random.Range(-0.5f, 0.5f) * transform.right, new Quaternion(0, 0, 0, 0));
            tgo.transform.Rotate(new Vector3(0, 180 * Random.Range((int)0, (int)2), Random.Range(0, 360)));
        }
        dead = true;
        updateAnimations();
    }

    void updateAnimations()
    {
        animator.SetInteger("Color", type);
        animator.SetBool("Ready", charging);
        animator.SetBool("Attacking", dead);
    }
}
