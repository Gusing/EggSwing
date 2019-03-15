using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAHandler : enemyHandler {
    
    public Sprite spriteIdle;
    public Sprite spriteStomp1;
    public Sprite spriteStomp2;
    public Sprite spriteDead;
    public Sprite spriteDead2;
    public Sprite spriteDead3;
    public Sprite spriteDead4;
    public Sprite spriteAttackPre;
    public Sprite spriteAttackActive;
    public Sprite spriteHitstun;
    
    public override void Start ()
    {
        base.Start();
        
        maxHP = 10;
        currentHP = maxHP;
        
        walkAcc = -18;
        walkSpeed = 1.8f;

        hitstunLimit = 1;
        knockbackLimit = 3;
        bigKnockbackLimit = 8;

        stopDistance = 1;

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
        if (attacking) AttackA();
        if (attackHitboxActive) UpdateAttackHitbox();
        
        // check for attack
        if (Vector2.Distance(transform.position, player.transform.position) < 1.8f && !attacking && !hitstun && attackRecovery <= 0 && !player.GetComponent<playerHandler>().dead)
        {
            localSpriteRenderer.sprite = spriteIdle;
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

        // update dancing sprite
        if (!busy)
        {
            if (!mainHandler.offBeat)
            {
                localSpriteRenderer.sprite = spriteStomp2;
            }
            if (mainHandler.offBeat)
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

        GetComponent<SpriteRenderer>().sprite = spriteHitstun;

        if (actionTimer >= hitstunTime)
        {
            GetComponent<SpriteRenderer>().sprite = spriteIdle;
        }
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
                soundAttack.setParameterValue("Pre", 1);
                soundAttack.start();
                Instantiate(pYellowWarning, warningPoints[0].position, new Quaternion(0, 0, 0, 0));
                attackState = 2;
                localSpriteRenderer.sprite = spriteAttackPre;
            }
            else if (attackState == 2)
            {
                soundAttack.setParameterValue("Pre", 2);
                soundAttack.start();
                Instantiate(pRedWarning, warningPoints[0].position, new Quaternion(0, 0, 0, 0));
                attackState = 3;
                localSpriteRenderer.sprite = spriteAttackPre;
            }
            else if (attackState == 3)
            {
                attackActive = true;
                soundAttack.setParameterValue("Pre", 4);
                soundAttack.start();
                attackHitboxTimer = 0;
                attackHitboxActive = true;
                hitboxAttacks[0].enabled = true;
                attackState = 4;
                localSpriteRenderer.sprite = spriteAttackActive;
            }
            else if (attackState == 4)
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
}
