using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBHandler : MonoBehaviour
{
    float accX;
    float velX;

    bool hitstun;
    bool knockBack;
    bool bigKnockBack;
    float hitstunTime = 0.32f;

    public Sprite spriteIdle;
    public Sprite spriteStomp1;
    public Sprite spriteStomp2;
    public Sprite spriteDead;
    public Sprite spriteDead2;
    public Sprite spriteDead3;
    public Sprite spriteDead4;
    public Sprite spriteAttackAPre;
    public Sprite spriteAttackAActive;
    public Sprite spriteAttackBPre;
    public Sprite spriteAttackBActive;
    public Sprite spriteHitstun;

    SpriteRenderer localSpriteRenderer;

    bool invincible;

    int health;
    int maxHealth = 23;

    public SpriteRenderer rendererHPBar;
    public SpriteRenderer rendererHPFill;

    float actionTimer;
    bool attackingA;
    bool attackingB;
    bool busy;
    public bool dead;

    bool attackHitboxActive;
    float attackHitboxTimer;
    float attackHitboxTime = 0.3f;

    int lastAttack;
    int attackStreak;

    GameObject player;

    int attackState;
    bool timeToMoveOn;
    bool newBeat;

    public BoxCollider2D rendererAttackA;
    public BoxCollider2D rendererAttackB;

    BoxCollider2D hitboxBody;

    public ParticleSystem pYellowWarning;
    public ParticleSystem pRedWarning;

    public Transform warningPointA;
    public Transform warningPointB;

    float attackDelay;
    float attackRecovery;

    bool directionRight;
    int direction;
    int hitstunDirection;

    int currentState;
    readonly int NOTBEAT = 0, BEAT = 1, ACTUALBEAT = 2, OFFBEAT = 3;

    FMOD.Studio.EventInstance soundAttack;
    FMOD.Studio.EventInstance soundDeath;

    // Use this for initialization
    void Start()
    {
        directionRight = false;
        direction = 1;

        health = maxHealth;

        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_attack");
        soundDeath = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_death");

        player = GameObject.Find("Player");

        localSpriteRenderer = GetComponent<SpriteRenderer>();
        hitboxBody = GetComponent<BoxCollider2D>();

        rendererAttackA.enabled = false;
        rendererAttackB.enabled = false;

        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        rendererHPFill.transform.localScale = new Vector3(((float)health / (float)maxHealth) * 1, 1);

        if (dead) return;

        if (hitstun) Hitstun();
        if (attackingA) AttackA();
        if (attackingB) AttackB();
        if (attackHitboxActive) AttackHitbox();

        if (attackRecovery > 0)
        {
            attackRecovery -= Time.deltaTime;
        }

        if (Vector2.Distance(transform.position, player.transform.position) < 2.5f && !attackingA && !attackingB && !hitstun && attackRecovery <= 0 && !player.GetComponent<playerHandler>().dead)
        {
            localSpriteRenderer.sprite = spriteIdle;
            UpdateHitboxes();
            if (attackStreak >= 2)
            {
                attackStreak = 0;
                if (lastAttack == 1) attackingB = true;
                if (lastAttack == 2) attackingA = true;
            }
            else
            {
                attackDelay = Random.Range(0.1f, 1.1f);
                if (Random.Range((int)0, (int)2) == 0)
                {
                    if (lastAttack == 1) attackStreak++;
                    else attackStreak = 0;
                    lastAttack = 1;
                    attackingA = true;
                }
                else
                {
                    if (lastAttack == 2) attackStreak++;
                    else attackStreak = 0;
                    lastAttack = 2;
                    attackingB = true;
                }
            }
            busy = true;
            velX = 0;
            accX = 0;
            attackState = 1;
            timeToMoveOn = false;
            newBeat = false;
        }

        if (!busy)
        {
            if (currentState == ACTUALBEAT)
            {
                localSpriteRenderer.sprite = spriteStomp2;
            }
            if (currentState == OFFBEAT)
            {
                print("sd");
                localSpriteRenderer.sprite = spriteStomp1;
            }
        }

        if (invincible) GetComponent<BoxCollider2D>().enabled = false;
        else GetComponent<BoxCollider2D>().enabled = true;

        if (!attackingA && !attackingB && !hitstun && attackRecovery <= 1 && !player.GetComponent<playerHandler>().dead)
        {
            hitboxBody.offset = new Vector2(Mathf.Abs(hitboxBody.offset.x) * direction, hitboxBody.offset.y);
            if (player.transform.position.x < transform.position.x)
            {
                direction = 1;
                directionRight = false;
                localSpriteRenderer.flipX = false;
            }
            else
            {
                direction = -1;
                directionRight = true;
                localSpriteRenderer.flipX = true;
            }
        }

        if (attackingA || attackingB) accX = -0.001f;
        else if (!busy) accX = -18f * direction;

        velX += accX * Time.deltaTime;
        if (!knockBack) velX = Mathf.Clamp(velX, -1.4f, 1.4f);
        transform.Translate(new Vector3(velX * Time.deltaTime, 0));

    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        print("hit for " + dmg);
        if (health <= 0) Die(dmg);
        else
        {
            if (!attackingA && !attackingB)
            {
                if (player.transform.position.x < transform.position.x)
                {
                    direction = 1;
                    directionRight = false;
                    localSpriteRenderer.flipX = false;
                }
                else
                {
                    direction = -1;
                    directionRight = true;
                    localSpriteRenderer.flipX = true;
                }
            }

            rendererHPBar.enabled = true;
            rendererHPFill.enabled = true;
            if (player.transform.position.x < transform.position.x) hitstunDirection = 1;
            else hitstunDirection = -1;
            invincible = true;
            hitstun = true;
            if (dmg >= 5) knockBack = true;
            if (dmg >= 8)
            {
                bigKnockBack = true;
                attackingA = false;
                rendererAttackA.enabled = false;
                attackingB = false;
                rendererAttackB.enabled = false;
                attackState = 0;
                GetComponent<SpriteRenderer>().sprite = spriteIdle;
            }
        }
    }

    public void Die(int dmg)
    {
        soundDeath.start();
        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;
        rendererAttackA.enabled = false;
        rendererAttackA.enabled = false;
        if (dmg == 6) GetComponent<SpriteRenderer>().sprite = spriteDead4;
        else
        {
            int rand = Random.Range(0, 3);
            if (rand == 0) GetComponent<SpriteRenderer>().sprite = spriteDead;
            if (rand == 1) GetComponent<SpriteRenderer>().sprite = spriteDead2;
            if (rand == 2) GetComponent<SpriteRenderer>().sprite = spriteDead3;
        }
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "EnemiesDead";
        dead = true;
    }

    void Hitstun()
    {
        //GetComponent<SpriteRenderer>().sprite = spriteHitstun;
        actionTimer += Time.deltaTime;

        if (bigKnockBack) velX = (7f + actionTimer * 6) * hitstunDirection;
        else if (knockBack) velX = (3f + actionTimer * 5) * hitstunDirection;
        else velX = 0;

        if (actionTimer >= hitstunTime)
        {
            if (bigKnockBack) busy = false;
            print("hitstun over");
            //GetComponent<SpriteRenderer>().sprite = spriteIdle;
            actionTimer = 0;
            hitstun = false;
            knockBack = false;
            bigKnockBack = false;
            invincible = false;
        }
    }

    void UpdateHitboxes()
    {
        rendererAttackA.offset = new Vector2(Mathf.Abs(rendererAttackA.offset.x) * -direction, rendererAttackA.offset.y);
        rendererAttackB.offset = new Vector2(Mathf.Abs(rendererAttackB.offset.x) * -direction, rendererAttackB.offset.y);
        warningPointA.localPosition = new Vector3(Mathf.Abs(warningPointA.localPosition.x) * direction, warningPointA.localPosition.y);
        warningPointB.localPosition = new Vector3(Mathf.Abs(warningPointB.localPosition.x) * -direction, warningPointB.localPosition.y);
    }

    void AttackA()
    {
        /*
        if (hitstun)
        {
            rendererAttackA.enabled = false;
            attackingA = false;
            attackState = 0;
            return;
        }
        */

        if (!hitstun) velX = 0;
        accX = 0;

        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
            return;
        }

        if (currentState == NOTBEAT) timeToMoveOn = true;

        if (timeToMoveOn && currentState == BEAT)
        {
            timeToMoveOn = false;
            if (attackState == 1)
            {
                attackState = 2;
                localSpriteRenderer.sprite = spriteAttackAPre;
            }
            else if (attackState == 2)
            {
                attackState = 3;
                localSpriteRenderer.sprite = spriteAttackAPre;
            }
            else if (attackState == 3)
            {
                soundAttack.setParameterValue("Pre", 1);
                soundAttack.start();
                Instantiate(pYellowWarning, warningPointA.position, new Quaternion(0, 0, 0, 0));
                attackState = 4;
                localSpriteRenderer.sprite = spriteAttackAPre;
            }
            else if (attackState == 4)
            {
                soundAttack.setParameterValue("Pre", 2);
                soundAttack.start();
                Instantiate(pRedWarning, warningPointA.position, new Quaternion(0, 0, 0, 0));
                attackState = 5;
                localSpriteRenderer.sprite = spriteAttackAPre;
            }
            else if (attackState == 5)
            {
                soundAttack.setParameterValue("Pre", 4);
                soundAttack.start();
                attackHitboxTimer = 0;
                attackHitboxActive = true;
                rendererAttackA.enabled = true;
                attackState = 6;
                localSpriteRenderer.sprite = spriteAttackAActive;
            }
            else if (attackState == 6)
            {
                rendererAttackA.enabled = false;
                localSpriteRenderer.sprite = spriteIdle;
                attackingA = false;
                busy = false;
                attackState = 0;
                attackRecovery = Random.Range(2, 3);
            }
        }

        if (currentState == NOTBEAT && attackState == 5) rendererAttackA.enabled = false;
    }

    void AttackB()
    {
        /*
        if (hitstun)
        {
            rendererAttackB.enabled = false;
            attackingB = false;
            attackState = 0;
            return;
        }
        */

        if (!hitstun) velX = 0;
        accX = 0;

        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
            return;
        }

        if (currentState == NOTBEAT) timeToMoveOn = true;

        if (timeToMoveOn && currentState == BEAT)
        {
            timeToMoveOn = false;
            if (attackState == 1)
            {
                soundAttack.setParameterValue("Pre", 1);
                soundAttack.start();
                Instantiate(pYellowWarning, warningPointB.position, new Quaternion(0, 0, 0, 0));
                attackState = 2;
                localSpriteRenderer.sprite = spriteAttackBPre;
            }
            else if (attackState == 2)
            {
                soundAttack.setParameterValue("Pre", 2);
                soundAttack.start();
                Instantiate(pRedWarning, warningPointB.position, new Quaternion(0, 0, 0, 0));
                attackState = 3;
                localSpriteRenderer.sprite = spriteAttackBPre;
            }
            else if (attackState == 3)
            {
                soundAttack.setParameterValue("Pre", 4);
                soundAttack.start();
                attackHitboxTimer = 0;
                attackHitboxActive = true;
                rendererAttackB.enabled = true;
                attackState = 4;
                localSpriteRenderer.sprite = spriteAttackBActive;
            }
            else if (attackState == 4)
            {
                rendererAttackB.enabled = false;
                localSpriteRenderer.sprite = spriteIdle;
                attackingB = false;
                busy = false;
                attackState = 0;
                attackRecovery = Random.Range(2, 3);
            }
        }

        if (currentState == NOTBEAT && attackState == 3) rendererAttackB.enabled = false;
    }

    void AttackHitbox()
    {
        attackHitboxTimer += Time.deltaTime;

        if (attackHitboxTimer >= attackHitboxTime)
        {
            attackHitboxActive = false;
            attackHitboxTimer = 0;
            rendererAttackA.enabled = false;
            rendererAttackB.enabled = false;
        }
    }

    public void setState(int state)
    {
        currentState = state;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "player")
        {
            soundAttack.setParameterValue("Pre", 3);
            soundAttack.start();
            other.GetComponent<playerHandler>().TakeDamage(3, direction);
        }
    }
}
