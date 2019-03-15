using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBHandler : enemyHandler
{
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

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        maxHP = 23;
        currentHP = maxHP;

        walkAcc = -18;
        walkSpeed = 1.4f;

        hitstunLimit = 8;
        knockbackLimit = 5;
        bigKnockbackLimit = 8;

        stopDistance = 1.8f;

        damage.Add(4);
        damage.Add(3);
        
        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_attack");
        soundDeath = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_death");
    }
    
    public override void Update()
    {
        base.UpdateAnimations();

        if (dead) return;

        base.Update();

        if (invincible) Invincible();
        if (attacking)
        {
            if (currentAttack == 0) AttackA();
            if (currentAttack == 1) AttackB();
        }
        if (attackHitboxActive) UpdateAttackHitbox();
        
        // check for attack
        if (Vector2.Distance(transform.position, player.transform.position) < 2.5f && !attacking && !hitstun && attackRecovery <= 0 && !player.GetComponent<playerHandler>().dead)
        {
            localSpriteRenderer.sprite = spriteIdle;
            UpdateHitboxes();
            if (attackStreak >= 2)
            {
                attackStreak = 0;
                if (lastAttack == 0) currentAttack = 1;
                if (lastAttack == 1) currentAttack = 0;
                attacking = true;
            }
            else
            {
                attackDelay = Random.Range(0.1f, 1.1f);
                if (Random.Range((int)0, (int)2) == 0)
                {
                    if (lastAttack == 0) attackStreak++;
                    else attackStreak = 0;
                    lastAttack = 0;
                    attacking = true;
                    currentAttack = 0;
                }
                else
                {
                    if (lastAttack == 1) attackStreak++;
                    else attackStreak = 0;
                    lastAttack = 1;
                    attacking = true;
                    currentAttack = 1;
                }
            }
            busy = true;
            velX = 0;
            accX = 0;
            attackState = 1;
            timeToMoveOn = false;
            newBeat = false;
        }

        // update dancing sprite
        if (!busy)
        {
            if (!offBeat)
            {
                localSpriteRenderer.sprite = spriteStomp2;
            }
            if (offBeat)
            {
                localSpriteRenderer.sprite = spriteStomp1;
            }
        }

        base.UpdateMovement();
    }
    
    public override void Die(int dmg)
    {
        base.Die(dmg);

        if (dmg == 6) GetComponent<SpriteRenderer>().sprite = spriteDead4;
        else
        {
            int rand = Random.Range(0, 3);
            if (rand == 0) GetComponent<SpriteRenderer>().sprite = spriteDead;
            if (rand == 1) GetComponent<SpriteRenderer>().sprite = spriteDead2;
            if (rand == 2) GetComponent<SpriteRenderer>().sprite = spriteDead3;
        }
    }

    protected override void Invincible()
    {
        base.Invincible();

        if (hitstun) GetComponent<SpriteRenderer>().sprite = spriteHitstun;
    }
    
    void AttackA()
    {
        base.Attack();
        
        // move to next attack state
        if (timeToMoveOn && mainHandler.currentState == PREBEAT)
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
                Instantiate(pYellowWarning, warningPoints[0].position, new Quaternion(0, 0, 0, 0));
                attackState = 4;
                localSpriteRenderer.sprite = spriteAttackAPre;
            }
            else if (attackState == 4)
            {
                soundAttack.setParameterValue("Pre", 2);
                soundAttack.start();
                Instantiate(pRedWarning, warningPoints[0].position, new Quaternion(0, 0, 0, 0));
                attackState = 5;
                localSpriteRenderer.sprite = spriteAttackAPre;
            }
            else if (attackState == 5)
            {
                attackActive = true;
                soundAttack.setParameterValue("Pre", 4);
                soundAttack.start();
                attackHitboxTimer = 0;
                attackHitboxActive = true;
                hitboxAttacks[0].enabled = true;
                attackState = 6;
                localSpriteRenderer.sprite = spriteAttackAActive;
            }
            else if (attackState == 6)
            {
                attackActive = false;
                hitboxAttacks[0].enabled = false;
                localSpriteRenderer.sprite = spriteIdle;
                attacking = false;
                busy = false;
                attackState = 0;
                attackRecovery = Random.Range(2, 3);
            }
        }
    }

    void AttackB()
    {
        base.Attack();

        // move to next attack state
        if (timeToMoveOn && mainHandler.currentState == PREBEAT)
        {
            timeToMoveOn = false;
            if (attackState == 1)
            {
                soundAttack.setParameterValue("Pre", 1);
                soundAttack.start();
                Instantiate(pYellowWarning, warningPoints[1].position, new Quaternion(0, 0, 0, 0));
                attackState = 2;
                localSpriteRenderer.sprite = spriteAttackBPre;
            }
            else if (attackState == 2)
            {
                soundAttack.setParameterValue("Pre", 2);
                soundAttack.start();
                Instantiate(pRedWarning, warningPoints[1].position, new Quaternion(0, 0, 0, 0));
                attackState = 3;
                localSpriteRenderer.sprite = spriteAttackBPre;
            }
            else if (attackState == 3)
            {
                attackActive = true;
                soundAttack.setParameterValue("Pre", 4);
                soundAttack.start();
                attackHitboxTimer = 0;
                attackHitboxActive = true;
                hitboxAttacks[1].enabled = true;
                attackState = 4;
                localSpriteRenderer.sprite = spriteAttackBActive;
            }
            else if (attackState == 4)
            {
                attackActive = false;
                hitboxAttacks[1].enabled = false;
                localSpriteRenderer.sprite = spriteIdle;
                attacking = false;
                busy = false;
                attackState = 0;
                attackRecovery = Random.Range(2, 3);
            }
        }
    }
}
