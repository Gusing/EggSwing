using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyParryPracticeHandler : enemyHandler {
    
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

    float fallTimer;
    bool falling = true;

    mainHandlerTutorial gameManager;

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

        gameManager = GameObject.Find("GameManager").GetComponent<mainHandlerTutorial>();

        parried = false;

        falling = true;

        player = GameObject.Find("PlayerTutorial");

        localSpriteRenderer = GetComponent<SpriteRenderer>();
        hitboxBody = GetComponent<BoxCollider2D>();

        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;

        //hitboxBody.enabled = false;

        maxHP = 10;
        currentHP = maxHP;
        
        walkAcc = 0;
        walkSpeed = 0f;

        defense = 100;

        hitstunLimit = 1;
        knockbackLimit = 999;
        bigKnockbackLimit = 999;

        currencyValue = 0;

        stopDistance = 1.4f;

        parryTime = 1f;

        damage.Add(0);

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
        animator.SetBool("offBeat", mainHandlerTutorial.offBeat);
        animator.SetBool("dead", dead);

        if (mainHandlerTutorial.currentState == BEAT) setOffbeat(false);
        
        // update invincibility
        //if (invincible) GetComponent<BoxCollider2D>().enabled = false;
        //else GetComponent<BoxCollider2D>().enabled = true;
        
        // update attack recovery
        if (attackRecovery > 0)
        {
            attackRecovery -= Time.deltaTime;
        }

        if (dead) return;

        if (mainHandlerTutorial.currentState == BEAT) setOffbeat(false);

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
        if (Vector2.Distance(transform.position, player.transform.position) < 1.8f && !attacking && !fallFromAbove && !hitstun && attackRecovery <= 0 && !parried && !falling)
        {
            gameManager.ParryIncoming(false);
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

    protected override void Parried()
    {
        actionTimer += Time.deltaTime;

        velX = 0;

        if (actionTimer >= parryTime)
        {
            actionTimer = 0;
            busy = false;
            parried = false;
            gameManager.ParryIncoming(false);
        }
    }

    public override void Die(int dmg)
    {
        mainHandler.EnemyDead();
        player.GetComponent<playerHandlerTutorial>().enemyKilled = true;
      
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
        if (mainHandlerTutorial.currentState == NOTBEAT) timeToMoveOn = true;

        // move to next attack state
        if (timeToMoveOn && mainHandlerTutorial.currentState == BEAT)
        {
            timeToMoveOn = false;
            if (attackState == 1)
            {
                gameManager.ParryIncoming(true);
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
                gameManager.ParryIncoming(false);
                attackHitting = false;
                attackActive = false;
                hitboxAttacks[0].enabled = false;
                localSpriteRenderer.sprite = spriteIdle;
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
