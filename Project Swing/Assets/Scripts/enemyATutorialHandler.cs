using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyATutorialHandler : enemyHandler {

    bool canTakeDamage;
    
    public override void Start()
    {
        Init(false);

        for (int i = 0; i < hitboxAttacks.Count; i++)
        {
            hitboxAttacks[i].enabled = false;
        }
        hitboxAttackFall.enabled = false;

        warningPointPositions = new List<Vector3>();
        for (int i = 0; i < warningPoints.Count; i++)
        {
            warningPointPositions.Add(warningPoints[i].localPosition);
        }

        animator = GetComponent<Animator>();

        damage = new List<int>();

        direction = LEFT;

        parried = false;

        player = GameObject.Find("PlayerTutorial");

        localSpriteRenderer = GetComponent<SpriteRenderer>();
        hitboxBody = GetComponent<BoxCollider2D>();

        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;

        maxHP = 10;
        currentHP = maxHP;
        
        walkAcc = -18;
        walkSpeed = 1.8f;

        hitstunLimit = 1;
        knockbackLimit = 3;
        bigKnockbackLimit = 8;

        currencyValue = 3;

        stopDistance = 1.4f;

        parryTime = 2f;

        damage.Add(3);

        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_attack");
        soundDeath = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_death");
        soundFall = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_warning");
        soundImpact = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_impact");
    }

    public override void Init(bool fromAbove)
    {
        base.Init(fromAbove);

        groundY = -2.12f;
        
        transform.position = new Vector3(transform.position.x, groundY, Random.Range(0f, 0.1f));

    }

    public override void Update()
    {
        animator.SetBool("parried", parried);
        animator.SetBool("attacking", attacking);
        animator.SetBool("isHit", hitstun);
        animator.SetBool("attackActive", attackActive);
        animator.SetInteger("currentAttack", currentAttack);
        animator.SetBool("offBeat", mainHandlerTutorial.offBeat);
        animator.SetBool("dead", dead);
        animator.SetFloat("randomHitValue", randomHitValue);
        animator.SetInteger("fallState", fallState);

        if (dead) return;

        if (mainHandlerTutorial.currentState == BEAT) setOffbeat(false);

        // update HP bar
        rendererHPFill.transform.localScale = new Vector3(((float)currentHP / (float)maxHP) * 1, 1);

        // update invincibility
        //if (invincible) GetComponent<BoxCollider2D>().enabled = false;
        //else GetComponent<BoxCollider2D>().enabled = true;

        // turn around
        if (!attacking && !hitstun && !parried && attackRecovery <= 1 && !player.GetComponent<playerHandlerTutorial>().dead)
        {
            hitboxBody.offset = new Vector2(Mathf.Abs(hitboxBody.offset.x) * direction, hitboxBody.offset.y);
            if (player.transform.position.x < transform.position.x)
            {
                print("left");
                direction = LEFT;
                localSpriteRenderer.flipX = false;
            }
            else
            {
                print("right");
                direction = RIGHT;
                localSpriteRenderer.flipX = true;
            }
        }

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
        if (Vector2.Distance(transform.position, player.transform.position) < 1.8f && !attacking && !fallFromAbove && !hitstun && attackRecovery <= 0 && !player.GetComponent<playerHandlerTutorial>().dead && !parried)
        {
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
        if (mainHandlerTutorial.currentState == NOTBEAT) timeToMoveOn = true;

        // move to next attack state
        if (timeToMoveOn && mainHandlerTutorial.currentState == BEAT)
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
        
        if (mainHandlerTutorial.currentState == NOTBEAT && attackHitboxActive && !attackHitting)
        {
            parryable = false;
            attackHitting = true;
        }
    }

    public void AllowDamage()
    {
        canTakeDamage = true;
        rendererHPBar.enabled = true;
        rendererHPFill.enabled = true;
    }

    public override int TakeDamage(int dmg, int attackID, bool specialHitstun = false, int attackType = 1)
    {
        lastAttackHitBy = attackID;

        int finalDmg = dmg;
        if (parried) finalDmg *= 3;

        randomHitValue = Random.Range(0f, 1f);
        if (!specialHitstun) finalDmg = Mathf.Clamp(finalDmg - defense, 0, 999);
        if (immuneToSlow && (attackType == 1 || attackType == 6) && !specialHitstun) finalDmg = 0;
        
        if (canTakeDamage) currentHP -= finalDmg;
        if (currentHP <= 0) Die(finalDmg);
        else
        {
            if (finalDmg >= hitstunLimit || specialHitstun)
            {
                attackActive = false;
                attackHitting = false;
                hitstun = true;
                attacking = false;
                attackState = 0;
                for (int i = 0; i < hitboxAttacks.Count; i++)
                {
                    hitboxAttacks[i].enabled = false;
                }
                busy = true;
            }
            else if (!attacking)
            {
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
            //fallFromAbove = false;
            fallState = 0;
            parryable = false;
            parried = false;
            //rendererHPBar.enabled = true;
            //rendererHPFill.enabled = true;
            if (player.transform.position.x < transform.position.x) hitstunDirection = LEFT;
            else hitstunDirection = RIGHT;
            invincible = true;
            if (finalDmg >= knockbackLimit) knockback = true;
            if (finalDmg >= bigKnockbackLimit) bigKnockback = true;
        }

        return finalDmg;
    }

    public override void Die(int dmg)
    {
        GameObject.Find("Main Camera").GetComponent<mainHandlerTutorial>().CompletedTutorialStep();

        GameObject tempObject;
        mainHandlerTutorial.EnemyDead();
        player.GetComponent<playerHandlerTutorial>().enemyKilled = true;
        int numCurrency = Random.Range(currencyValue, currencyValue + 4) + (player.GetComponent<playerHandlerTutorial>().currentStreak / 10);
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
        if (Random.Range((int)0, (int)9) == 0)
        {
            Instantiate(HPPickup, transform.position + new Vector3(Random.Range(-0.2f, 0.2f), 0), new Quaternion(0, 0, 0, 0));
        }
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

    public override void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "player" && attackHitting == true)
        {
            soundAttack.setParameterValue("Pre", 3);
            soundAttack.start();
            other.GetComponent<playerHandlerTutorial>().TakeDamage(damage[currentAttack], direction);
        }
    }
}
