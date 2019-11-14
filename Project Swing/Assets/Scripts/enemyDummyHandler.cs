using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDummyHandler : enemyHandler
{
    public override void Start()
    {
        base.Start();

        transform.position = new Vector3(transform.position.x, -2.24f);

        maxHP = 10;
        currentHP = maxHP;

        walkAcc = -0.0001f;
        walkSpeed = 0.0001f;

        hitstunLimit = 1;
        knockbackLimit = 1000;
        bigKnockbackLimit = 1000;

        stopDistance = 1000;

        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_attack");
        soundDeath = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_death");
    }
    
    public override void Update()
    {
        base.UpdateAnimations();

        base.Update();

        if (invincible) Invincible();

        base.UpdateMovement();
    }

    public override int TakeDamage(int dmg, int attackID, bool specialHitstun = false, int attackType = 1)
    {
        lastAttackHitBy = attackID;

        randomHitValue = Random.Range(0f, 1f);
        if (dmg >= hitstunLimit)
        {
            hitstun = true;
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
        if (player.transform.position.x < transform.position.x) hitstunDirection = LEFT;
        else hitstunDirection = RIGHT;
        invincible = true;

        return dmg;
    }

    void Hitstun()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= hitstunTime)
        {
            actionTimer = 0;
            hitstun = false;
            invincible = false;
        }
    }
}
