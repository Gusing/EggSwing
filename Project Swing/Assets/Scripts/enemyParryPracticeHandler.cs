using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyParryPracticeHandler : enemyHandler {
    
    float fallTimer;
    bool falling = true;

    mainHandler gameManager;

    public override void Start()
    {
        for (int i = 0; i < hitboxAttacks.Count; i++)
        {
            hitboxAttacks[i].enabled = false;
        }

        warningPointPositions = new List<Vector3>();
        for (int i = 0; i < warningPoints.Count; i++)
        {
            warningPointPositions.Add(warningPoints[i].localPosition);
        }

        animator = GetComponent<Animator>();

        damage = new List<int>();

        direction = LEFT;

        gameManager = GameObject.Find("GameManager").GetComponent<mainHandler>();

        parried = false;

        falling = true;

        player = GameObject.Find("Player");

        localSpriteRenderer = GetComponent<SpriteRenderer>();
        hitboxBody = GetComponent<BoxCollider2D>();

        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;

        //hitboxBody.enabled = false;

        maxHP = 10;
        currentHP = maxHP;
        
        walkAcc = 0;
        walkSpeed = 0f;

        defense = 0;

        hitstunLimit = 1;
        knockbackLimit = 999;
        bigKnockbackLimit = 999;

        currencyValue = 1;

        stopDistance = 1.4f;

        parryTime = 1f;

        damage.Add(2);

        material = 0;

        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Object/Boxhand");
        soundDeath = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_death");
        soundFall = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_warning");
        soundImpact = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_impact");

        soundImpact.start();
    }

    public override void Init(bool fromAbove)
    {
        base.Init(fromAbove);

        groundY = -1.77f;
        
        transform.position = new Vector3(transform.position.x, groundY, Random.Range(0f, 0.1f));
    }

    public override void Update()
    {
        if (falling)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer >= 0.5f)
            {
                falling = false;
            }
        }

        animator.SetBool("falling", falling);
        animator.SetBool("parried", parried);
        animator.SetBool("attacking", attacking);
        animator.SetBool("attackActive", attackActive);
        animator.SetBool("offBeat", mainHandler.offBeat);
        animator.SetBool("isHit", hitstun);
        animator.SetBool("dead", dead);

        if (dead)
        {
            //GetComponent<SpriteRenderer>().enabled = false;
        }

        if (mainHandler.currentState == BEAT) setOffbeat(false);
        
        // update invincibility
        //if (invincible) GetComponent<BoxCollider2D>().enabled = false;
        //else GetComponent<BoxCollider2D>().enabled = true;
        
        // update attack recovery
        if (attackRecovery > 0)
        {
            attackRecovery -= Time.deltaTime;
        }

        if (dead) return;

        // turn around
        if (!attacking && !hitstun && !parried && attackRecovery <= 1 && !player.GetComponent<playerHandler>().dead)
        {
            hitboxBody.offset = new Vector2(Mathf.Abs(hitboxBody.offset.x) * direction, hitboxBody.offset.y);
            if (player.transform.position.x < transform.position.x)
            {
                direction = LEFT;
                localSpriteRenderer.flipX = false;
            }
            else
            {
                direction = RIGHT;
                localSpriteRenderer.flipX = true;
            }
        }

        if (mainHandler.currentState == BEAT) setOffbeat(false);

        // update HP bar
        rendererHPFill.transform.localScale = new Vector3(((float)currentHP / (float)maxHP) * 1, 1);

        // update invincibility
        //if (invincible) GetComponent<BoxCollider2D>().enabled = false;
        //else GetComponent<BoxCollider2D>().enabled = true;
        
        // update attack recovery
        if (attackRecovery > 0)
        {
            attackRecovery -= Time.deltaTime;
        }

        if (parried) Parried();
        if (invincible) Invincible();
        if (attacking) AttackA();
        if (fallFromAbove) FallFromAbove();
        if (attackHitboxActive) UpdateAttackHitbox();
        
        // check for attack
        if (Vector2.Distance(transform.position, player.transform.position) < 1.7f && !attacking && !fallFromAbove && !hitstun && attackRecovery <= 0 && !parried && !falling)
        {
            soundAttack.setParameterValue("Pre", 0);
            soundAttack.start();
            UpdateHitboxes();
            attackDelay = Random.Range(0.1f, 1.1f);
            attacking = true;
            busy = true;
            velX = 0;
            accX = 0;
            attackState = 1;
            timeToMoveOn = false;
            newBeat = false;
        }

        base.UpdateMovement();
    }

    protected override void Parried()
    {
        actionTimer += Time.deltaTime;

        velX = 0;

        if (actionTimer >= parryTime)
        {
            actionTimer = 0;
            busy = false;
            parried = false;

            if (attackRecovery < 0.1f) attackRecovery = Random.Range(0.7f, 1.1f);

            // turn around
            if (!player.GetComponent<playerHandler>().dead)
            {
                hitboxBody.offset = new Vector2(Mathf.Abs(hitboxBody.offset.x) * direction, hitboxBody.offset.y);
                if (player.transform.position.x < transform.position.x)
                {
                    direction = LEFT;
                    localSpriteRenderer.flipX = false;
                }
                else
                {
                    direction = RIGHT;
                    localSpriteRenderer.flipX = true;
                }
            }
        }
    }

    public void Destroyed()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public override void Die(int dmg)
    {
        GameObject tempObject;
        int numCurrency = Random.Range(currencyValue, currencyValue + 2) + (player.GetComponent<playerHandler>().currentStreak / 10);
        for (int i = 0; i < numCurrency; i++)
        {
            if (i <= numCurrency / 2)
            {
                tempObject = Instantiate(currency, transform.position + new Vector3(Random.Range(-0.2f, 0), 0), new Quaternion(0, 0, 0, 0));
                tempObject.GetComponent<currencyHandler>().Init(false, 0);
            }
            else
            {
                tempObject = Instantiate(currency, transform.position + new Vector3(Random.Range(0, 0.2f), 0), new Quaternion(0, 0, 0, 0));
                tempObject.GetComponent<currencyHandler>().Init(true, 0);
            }
        }

        player.GetComponent<playerHandler>().enemyKilled = true;
        mainHandler.EnemyDead();

        soundDeath.start();
        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;
        attacking = false;
        parried = false;
        attackActive = false;
        for (int i = 0; i < hitboxAttacks.Count; i++)
        {
            hitboxAttacks[i].enabled = false;
        }
        hitboxBody.enabled = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "EnemiesDead";
        dead = true;
    }

    void AttackA()
    {
        // stop if hitstunned
        if (hitstun)
        {
            for (int i = 0; i < hitboxAttacks.Count; i++)
            {
                hitboxAttacks[i].enabled = false;
            }
            attacking = false;
            attackState = 0;
            return;
        }

        // stop movement
        if (!knockback) velX = 0;
        accX = 0;

        // wait for attackdelay
        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
            return;
        }

        // ready for next attack state
        if (mainHandler.currentState == NOTBEAT) timeToMoveOn = true;

        // move to next attack state
        if (timeToMoveOn && mainHandler.currentState == BEAT)
        {
            timeToMoveOn = false;
            if (attackState == 1)
            {
                soundAttack.setParameterValue("Pre", 1);
                soundAttack.start();
                Instantiate(pYellowWarning, warningPoints[0].position, new Quaternion(0, 0, 0, 0));
                attackState = 2;
            }
            else if (attackState == 2)
            {
                soundAttack.setParameterValue("Pre", 2);
                soundAttack.start();
                Instantiate(pRedWarning, warningPoints[0].position, new Quaternion(0, 0, 0, 0));
                attackState = 3;
            }
            else if (attackState == 3)
            {
                attackActive = true;
                soundAttack.setParameterValue("Pre", 4);
                soundAttack.start();
                attackHitboxTimer = 0;
                attackHitboxActive = true;
                parryable = true;
                hitboxAttacks[0].enabled = true;
                attackState = 4;
            }
            else if (attackState == 4)
            {
                attackHitting = false;
                attackActive = false;
                hitboxAttacks[0].enabled = false;
                attacking = false;
                busy = false;
                attackState = 0;
                attackRecovery = Random.Range(2, 3);
            }
        }
        
        if (attackState > 0 && mainHandler.currentState == NOTBEAT && attackHitboxActive && !attackHitting)
        {
            parryable = false;
            attackHitting = true;
        }
    }

    public override void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "player" && attackHitting == true)
        {
            soundAttack.setParameterValue("Pre", 3);
            soundAttack.start();
            other.GetComponent<playerHandler>().TakeDamage(damage[currentAttack], direction);
        }
    }
}
