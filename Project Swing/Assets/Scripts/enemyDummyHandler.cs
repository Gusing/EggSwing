using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDummyHandler : MonoBehaviour
{
    float accX;
    float velX;

    bool hitstun;
    bool knockBack;
    float hitstunTime = 0.32f;

    public Sprite spriteIdle;
    public Sprite spriteHitstun;

    SpriteRenderer localSpriteRenderer;

    bool invincible;

    int health;
    int maxHealth = 23;

    float actionTimer;
    bool attackingA;
    bool attackingB;
    bool busy;
    bool dead;

    int lastAttack;
    int attackStreak;

    GameObject player;

    int attackState;
    bool timeToMoveOn;
    bool newBeat;
    
    public ParticleSystem pYellowWarning;
    public ParticleSystem pRedWarning;

    float attackDelay;
    float attackRecovery;

    int direction;

    int currentState;
    readonly int NOTBEAT = 0, BEAT = 1;

    // Use this for initialization
    void Start()
    {
        health = maxHealth;

        player = GameObject.Find("Player");

        localSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead) return;

        if (hitstun) Hitstun();
        
        if (invincible) GetComponent<BoxCollider2D>().enabled = false;
        else GetComponent<BoxCollider2D>().enabled = true;

        velX = 0.0001f;

        velX += accX * Time.deltaTime;
        if (!knockBack) velX = Mathf.Clamp(velX, -1.4f, 1.4f);
        transform.Translate(new Vector3(velX * Time.deltaTime, 0));
    }

    public void TakeDamage(int dmg)
    {
        if (player.transform.position.x < transform.position.x)
        {
            direction = 1;
            localSpriteRenderer.flipX = false;
        }
        else
        {
            direction = -1;
            localSpriteRenderer.flipX = true;
        }
        invincible = true;
        hitstun = true;
        if (dmg >= 5) knockBack = true;
    }

    void Hitstun()
    {
        GetComponent<SpriteRenderer>().sprite = spriteHitstun;
        actionTimer += Time.deltaTime;

        if (actionTimer >= hitstunTime)
        {
            GetComponent<SpriteRenderer>().sprite = spriteIdle;
            actionTimer = 0;
            hitstun = false;
            knockBack = false;
            invincible = false;
        }
    }
    
    public void setState(int state)
    {
        currentState = state;
    }
}
