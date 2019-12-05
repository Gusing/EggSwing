using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyCHandler : enemyHandler
{
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

    public override void Start()
    {
        base.Start();

        maxHP = 12;
        currentHP = maxHP;

        walkAcc = -12;
        walkSpeed = 2.4f;

        hitstunLimit = 1;
        knockbackLimit = 3;
        bigKnockbackLimit = 8;

        currencyValue = 5;

        stopDistance = 1.1f;

        parryTime = 2.5f;

        defense = 1;
        immuneToSlow = true;

        damage.Add(4);

        material = 0;

        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Knight_attack");
        soundDeath = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Knight_death");
        soundFall = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_warning");
        soundImpact = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_impact");
    }

    public override void Init(bool fromAbove)
    {
        base.Init(fromAbove);

        groundY = -1.91f;

        transform.position = new Vector3(transform.position.x, groundY, Random.Range(0f, 0.1f));

    }

    public override void Update()
    {
        base.UpdateAnimations();

        if (dead) return;

        base.Update();

        if (parried) Parried();
        if (invincible) Invincible();
        if (attacking) AttackA();
        if (fallFromAbove) FallFromAbove();
        if (attackHitboxActive) UpdateAttackHitbox();

        // check for attack
        if (Vector2.Distance(transform.position, player.transform.position) < 1.8f && !attacking && !fallFromAbove && !hitstun && attackRecovery <= 0 && !player.GetComponent<playerHandler>().dead && !parried)
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

        base.UpdateMovement();
    }

    void AttackA()
    {
        base.Attack();

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
                parryable = true;
                hitboxAttacks[0].enabled = true;
                attackState = 4;
                localSpriteRenderer.sprite = spriteAttackActive;
            }
            else if (attackState == 4)
            {
                attackHitting = false;
                attackActive = false;
                hitboxAttacks[0].enabled = false;
                localSpriteRenderer.sprite = spriteIdle;
                attacking = false;
                busy = false;
                attackState = 0;
                attackRecovery = Random.Range(2.5f, 3);
            }
        }

        if (attackState > 0 && mainHandler.currentState == NOTBEAT && attackHitboxActive && !attackHitting)
        {
            parryable = false;
            attackHitting = true;
        }
    }
}
