using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDHandler : enemyHandler
{
    float dodgeTimer;
    float dodgeTime;
    bool dodging;
    float dodgeCooldownTimer;
    float dodgeCooldownTime;
    bool dodgeCooldown;
    bool attackBReady;

    protected FMOD.Studio.EventInstance soundSlide;

    public override void Start()
    {
        base.Start();

        maxHP = 12;
        currentHP = maxHP;

        walkAcc = -30;
        walkSpeed = 3.2f;

        hitstunLimit = 1;
        knockbackLimit = 3;
        bigKnockbackLimit = 8;

        currencyValue = 6;

        stopDistance = 1.1f;

        parryTime = 1.3f;

        dodgeTime = 0.2f;
        dodgeCooldownTime = (60f / mainHandler.currentBpm) * 1.1f;

        damage.Add(3);
        damage.Add(4);

        material = 0;

        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Ninja_attack");
        soundDeath = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Ninja_death");
        soundFall = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_warning");
        soundImpact = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_impact");
        soundSlide = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_swoosh");
    }

    public override void Init(bool fromAbove)
    {
        base.Init(fromAbove);

        groundY = -2.12f;

        transform.position = new Vector3(transform.position.x, groundY, Random.Range(0f, 0.1f));

    }

    public override void Update()
    {
        base.UpdateAnimations();

        animator.SetBool("dodging", dodging);

        if (dead) return;

        if (!dodging) base.Update();

        if (parried) Parried();
        if (invincible) Invincible();
        if (attacking)
        {
            if (currentAttack == 0) AttackA();
            if (currentAttack == 1) AttackB();
        }
        if (fallFromAbove) FallFromAbove();
        if (attackHitboxActive) UpdateAttackHitbox();
        if (dodging) Dodge();

        // check for attack
        if (Vector2.Distance(transform.position, player.transform.position) < 1.7f && !attacking && !fallFromAbove && !hitstun && attackRecovery <= 0 && !player.GetComponent<playerHandler>().dead && !dodging && !parried)
        {
            UpdateHitboxes();
            attackDelay = Random.Range(0.1f, 1.1f);
            attacking = true;
            busy = true;
            velX = 0;
            accX = 0;
            if (attackBReady) currentAttack = 1;
            else currentAttack = 0;
            attackBReady = false;
            attackState = 1;
            timeToMoveOn = false;
            newBeat = false;
        }
        
        if (!dodging) base.UpdateMovement();

        if (dodgeCooldown)
        {
            dodgeCooldownTimer += Time.deltaTime;
            if (dodgeCooldownTimer >= dodgeCooldownTime)
            {
                dodgeCooldownTimer = 0;
                dodgeCooldown = false;
            }
        }
        else if (player.GetComponent<playerHandler>().punchingSuccess && player.GetComponent<playerHandler>().attackType == 2 && 
            !player.GetComponent<playerHandler>().punchingActive && !dodging && Vector2.Distance(transform.position, player.transform.position) < 2f &&
            !fallFromAbove)
        {
            soundSlide.start();
            attackRecovery = 0;
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
            attacking = false;
            busy = true;
            velX = -15 * direction;
            dodging = true;
            hitboxBody.enabled = false;
        }
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
                attackRecovery = Random.Range(1, 1.7f);
            }
        }

        if (attackState > 0 && mainHandler.currentState == NOTBEAT && attackHitboxActive && !attackHitting)
        {
            parryable = false;
            attackHitting = true;
        }
    }

    void AttackB()
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
                Instantiate(pYellowWarning, warningPoints[1].position, new Quaternion(0, 0, 0, 0));
                attackState = 2;
            }
            else if (attackState == 2)
            {
                soundAttack.setParameterValue("Pre", 2);
                soundAttack.start();
                Instantiate(pRedWarning, warningPoints[1].position, new Quaternion(0, 0, 0, 0));
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
                hitboxAttacks[1].enabled = true;
                attackState = 4;
            }
            else if (attackState == 4)
            {
                attackHitting = false;
                attackActive = false;
                hitboxAttacks[1].enabled = false;
                attacking = false;
                busy = false;
                attackState = 0;
                attackRecovery = Random.Range(0.8f, 1.2f);
            }
        }

        if (attackState > 0 && mainHandler.currentState == NOTBEAT && attackHitboxActive && !attackHitting)
        {
            parryable = false;
            attackHitting = true;
        }
    }

    void Dodge()
    {
        dodgeTimer += Time.deltaTime;
        transform.Translate(new Vector3(velX * Time.deltaTime, 0));
        if ((dodgeTimer >= dodgeTime) || (transform.position.x < player.transform.position.x - 1.5f && direction == LEFT) || (transform.position.x > player.transform.position.x + 1.5f && direction == RIGHT))
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
            attackBReady = true;
            dodging = false;
            dodgeTimer = 0;
            velX = 0;
            hitboxBody.enabled = true;
            busy = false;
            dodgeCooldown = true; ;
        }
    }

    public override void Die(int dmg)
    {
        base.Die(dmg);
    }
}
