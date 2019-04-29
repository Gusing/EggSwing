using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBirdHandler : MonoBehaviour
{
    public SpriteRenderer[] renderers;

    public Sprite spriteBirdA;
    public Sprite spriteBirdA2;
    public Sprite spriteBirdA3;
    public Sprite spriteBirdB;
    public Sprite spriteBirdB2;
    public Sprite spriteBirdB3;

    public SpriteRenderer localRenderer;

    int[] spawns;
    float localBpm;
    float bpmInSeconds;
    float timeUntilCrash;
    float timeToCharge;
    bool charging;
    int type;
    public bool dead;
    public bool readyToBeDestroyed;
    float lifeTimer;

    public bool readyToBeHit;

    FMOD.Studio.EventInstance soundWarning;
    FMOD.Studio.EventInstance soundAttack;
    FMOD.Studio.EventInstance soundPlayerHit;

    public void init(int speedType)
    {
        localBpm = mainHandler.currentBpm;

        bpmInSeconds = (float)60 / (float)localBpm;

        type = speedType;
        if (type == 1)
        {
            localRenderer.sprite = spriteBirdA3;
            timeToCharge = bpmInSeconds * 1;
        }
        if (type == 2)
        {
            localRenderer.sprite = spriteBirdA;
            timeToCharge = bpmInSeconds * 2;
        }
        if (type == 4)
        {
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

        spawns = new int[1] { 1 };

        for (int i = 0; i < renderers.Length; i++)
        {
            if (spawns.Length + 1 <= i) renderers[i].enabled = true;
        }

        transform.position = new Vector3(10.08f, 3.37f);

        localBpm = mainHandler.currentBpm;

        bpmInSeconds = (float)60 / (float)localBpm;

        timeUntilCrash = bpmInSeconds * 4;

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
        lifeTimer += Time.deltaTime;

        if (!dead)
        {
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

            if (!charging) transform.Translate(new Vector3(-3f * Time.deltaTime, 0));

        }

        if (dead && readyToBeHit && lifeTimer >= timeUntilCrash + timeToCharge + mainHandler.currentLeniency)
        {
            GameObject.Find("Player").GetComponent<playerHandler>().TakeDamage(1, 0, true);
            readyToBeHit = false;
            readyToBeDestroyed = true;
           

        }


    }
}
