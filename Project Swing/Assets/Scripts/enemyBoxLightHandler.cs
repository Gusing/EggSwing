using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBoxLightHandler : enemyHandler
{
    float fallTimer;
    bool falling = true;
    public bool canDie;

    public override void Start()
    {
        animator = GetComponent<Animator>();

        damage = new List<int>();

        direction = LEFT;

        parried = false;

        player = GameObject.Find("Player");

        localSpriteRenderer = GetComponent<SpriteRenderer>();
        hitboxBody = GetComponent<BoxCollider2D>();

        maxHP = 5;
        currentHP = maxHP;

        walkAcc = 0;
        walkSpeed = 0;

        hitstunLimit = 1;
        knockbackLimit = 999;
        bigKnockbackLimit = 999;

        falling = true;
        fallTimer = 0;

        currencyValue = 2;

        stopDistance = 0;

        parryTime = 0;

        material = 1;

        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_attack");
        soundDeath = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/WoodDie");
        soundFall = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_warning");
        soundImpact = FMODUnity.RuntimeManager.CreateInstance("event:/Ligth_impact");

        soundImpact.start();
    }

    public override void Init(bool fromAbove)
    {
        base.Init(fromAbove);

        groundY = -1.7f;

        transform.position = new Vector3(transform.position.x, groundY, Random.Range(0f, 0.1f));
    }

    public override void Update()
    {
        UpdateAnimations();

        if (dead) return;

        if (mainHandler.currentState == BEAT) setOffbeat(false);

        // update attack recovery
        if (attackRecovery > 0)
        {
            attackRecovery -= Time.deltaTime;
        }

        if (falling)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer >= 0.5f)
            {
                falling = false;
            }
        }
        if (parried) Parried();
        if (invincible) Invincible();
        if (fallFromAbove) FallFromAbove();
        if (attackHitboxActive) UpdateAttackHitbox();

        base.UpdateMovement();
    }

    public void BoxDestroyed()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public override void UpdateAnimations()
    {
        animator.SetBool("falling", falling);
        animator.SetBool("hit", hitstun);
        animator.SetBool("dead", dead);
    }

    public override int TakeDamage(int dmg, int attackID, bool specialHitstun = false, int attackType = 1)
    {
        lastAttackHitBy = attackID;

        int finalDmg = dmg;

        currentHP -= finalDmg;
        if (currentHP <= 0) Die(finalDmg);
        else
        {
            if (finalDmg >= hitstunLimit || specialHitstun)
            {
                busy = true;
            }
            //fallFromAbove = false;
            hitstun = true;
            fallState = 0;
            if (player.transform.position.x < transform.position.x) hitstunDirection = LEFT;
            else hitstunDirection = RIGHT;
            invincible = true;
        }

        return finalDmg;
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
        
        if (Random.Range((int)0, (int)3) == 0)
        {
            Instantiate(HPPickup, transform.position + new Vector3(Random.Range(-0.2f, 0.2f), 0), new Quaternion(0, 0, 0, 0));
        }
        
        mainHandler.EnemyDead();
        soundDeath.start();
        //rendererHPBar.enabled = false;
        //rendererHPFill.enabled = false;
        hitboxBody.enabled = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "EnemiesDead";
        dead = true;
    }
}
