using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHandler : MonoBehaviour
{
    public bool dead;
    public int currentHP;
    public int maxHP;
    protected float walkAcc;
    protected float walkSpeed;
    protected float stopDistance;

    protected float accX;
    protected float velX;
    protected int direction;
    protected readonly int RIGHT = -1, LEFT = 1;
    protected float groundY;
    
    protected bool hitstun;
    protected bool knockback;
    protected bool bigKnockback;
    protected int hitstunLimit;
    protected int knockbackLimit;
    protected int bigKnockbackLimit;
    protected bool invincible;
    protected float randomHitValue;

    protected int hitstunDirection;

    protected float actionTimer;
    protected float hitstunTime = 0.32f;

    protected bool attackHitboxActive;
    protected float attackHitboxTimer;
    protected float attackHitboxTime = 0.1f;
    protected List<int> damage;
    protected bool attacking;
    protected float attackDelay;
    protected float attackRecovery;
    protected float attackDistance;
    protected bool attackActive;
    protected bool fallFromAbove;
    protected bool inTheSky;
    protected int fallState;

    protected bool immuneToSlow;
    protected int defense;

    protected int currencyValue;

    protected bool busy;

    protected bool playedFallSound;

    protected int attackState;
    protected bool timeToMoveOn;
    protected bool newBeat;
    protected int lastAttackHitBy;

    // when enemy has multiple attacks
    protected int currentAttack;
    protected int attackStreak;
    protected int lastAttack;

    public SpriteRenderer rendererHPBar;
    public SpriteRenderer rendererHPFill;

    public ParticleSystem pYellowWarning;
    public ParticleSystem pRedWarning;
    public GameObject currency;
    public GameObject HPPickup;

    protected GameObject player;

    public List<Transform> warningPoints;

    protected SpriteRenderer localSpriteRenderer;

    protected BoxCollider2D hitboxBody;
    public List<BoxCollider2D> hitboxAttacks;
    public BoxCollider2D hitboxAttackFall;

    protected Animator animator;

    protected int currentState;
    protected readonly int NOTBEAT = 0, PREBEAT = 1, BEAT = 2, OFFBEAT = 3;
    protected bool offBeat;

    protected FMOD.Studio.EventInstance soundAttack;
    protected FMOD.Studio.EventInstance soundDeath;
    protected FMOD.Studio.EventInstance soundFall;
    protected FMOD.Studio.EventInstance soundImpact;

    public enemyHandler()
    {
        
    }

    public virtual void Init(bool fromAbove)
    {
        fallFromAbove = fromAbove;
        inTheSky = fromAbove;
        if (fallFromAbove)
        {
            hitboxBody = GetComponent<BoxCollider2D>();
            hitboxBody.enabled = false;
            fallState = 1;
            busy = true;
            attackState = 1;
            timeToMoveOn = false;
            newBeat = false;
        }
        else fallState = -1;
    }

    public virtual void Start()
    {
        for (int i = 0; i < hitboxAttacks.Count; i++)
        {
            hitboxAttacks[i].enabled = false;
        }
        hitboxAttackFall.enabled = false;

        animator = GetComponent<Animator>();

        damage = new List<int>();

        direction = LEFT;

        player = GameObject.Find("Player");

        localSpriteRenderer = GetComponent<SpriteRenderer>();
        hitboxBody = GetComponent<BoxCollider2D>();
        
        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;
    }

    public virtual void Update()
    {
        if (mainHandler.currentState == BEAT) setOffbeat(false);

        // update HP bar
        rendererHPFill.transform.localScale = new Vector3(((float)currentHP / (float)maxHP) * 1, 1);

        // update invincibility
        //if (invincible) GetComponent<BoxCollider2D>().enabled = false;
        //else GetComponent<BoxCollider2D>().enabled = true;

        // turn around
        if (!attacking && !hitstun && attackRecovery <= 1 && !player.GetComponent<playerHandler>().dead)
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

        // update attack recovery
        if (attackRecovery > 0)
        {
            attackRecovery -= Time.deltaTime;
        }
    }

    public virtual void UpdateAnimations()
    {
        animator.SetBool("attacking", attacking);
        animator.SetBool("isHit", hitstun);
        animator.SetBool("attackActive", attackActive);
        animator.SetInteger("currentAttack", currentAttack);
        animator.SetBool("offBeat", mainHandler.offBeat);
        animator.SetBool("dead", dead);
        animator.SetFloat("randomHitValue", randomHitValue);
        animator.SetInteger("fallState", fallState);
    }

    public virtual void UpdateMovement()
    {
        // keep moving if not busy
        if (attacking || fallFromAbove) accX = -0.001f * direction;
        else if (!busy) accX = walkAcc * direction;

        // update velocity
        velX += accX * Time.deltaTime;

        // clamp velocity, stop moving when very close to player
        if (!knockback)
        {
            if (Vector2.Distance(transform.position, player.transform.position) > stopDistance)
            {
                velX = Mathf.Clamp(velX, -walkSpeed, walkSpeed);
            }
            else
            {
                if (player.transform.position.x < transform.position.x) velX = Mathf.Clamp(velX, -0.001f, walkSpeed);
                else velX = Mathf.Clamp(velX, -walkSpeed, 0.001f);
            }
        }

        // update position
        transform.Translate(new Vector3(velX * Time.deltaTime, 0));
    }

    public virtual int TakeDamage(int dmg, int attackID, bool specialHitstun = false, int attackType = 1)
    {
        lastAttackHitBy = attackID;

        int finalDmg = dmg;

        randomHitValue = Random.Range(0f, 1f);
        finalDmg -= defense;
        if (immuneToSlow && (attackType == 1 || attackType == 6)) finalDmg = 0;
        currentHP -= finalDmg;
        if (currentHP <= 0) Die(finalDmg);
        else
        {
            if (finalDmg >= hitstunLimit || specialHitstun)
            {
                attackActive = false;
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
            rendererHPBar.enabled = true;
            rendererHPFill.enabled = true;
            if (player.transform.position.x < transform.position.x) hitstunDirection = LEFT;
            else hitstunDirection = RIGHT;
            invincible = true;
            if (finalDmg >= knockbackLimit) knockback = true;
            if (finalDmg >= bigKnockbackLimit) bigKnockback = true;
        }

        return finalDmg;
    }

    public virtual bool CheckHit(int attackID)
    {
        if (lastAttackHitBy == attackID)
        {
            return false;
        }
        else return true;
    }

    public virtual void Die(int dmg)
    {
        GameObject tempObject;
        mainHandler.EnemyDead();
        int numCurrency = Random.Range(currencyValue, currencyValue + 4) + (player.GetComponent<playerHandler>().currentStreak / 10);
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
        attackActive = false;
        for (int i = 0; i < hitboxAttacks.Count; i++)
        {
            hitboxAttacks[i].enabled = false;
        }
        hitboxBody.enabled = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "EnemiesDead";
        dead = true;
    }

    protected virtual void Invincible()
    {
        actionTimer += Time.deltaTime;

        if (bigKnockback) velX = (7f + actionTimer * 6) * hitstunDirection;
        else if (knockback) velX = (3f + actionTimer * 5) * hitstunDirection;
        else velX = 0;

        if (actionTimer >= hitstunTime)
        {
            actionTimer = 0;
            if (hitstun) busy = false;
            hitstun = false;
            knockback = false;
            bigKnockback = false;
            invincible = false;
        }
    }

    protected virtual void UpdateHitboxes()
    {
        for (int i = 0; i < warningPoints.Count; i++)
        {
            warningPoints[i].localPosition = new Vector3(Mathf.Abs(warningPoints[i].localPosition.x) * -direction, warningPoints[i].localPosition.y);
        }
        for (int i = 0; i < hitboxAttacks.Count; i++)
        {
            hitboxAttacks[i].offset = new Vector2(Mathf.Abs(hitboxAttacks[i].offset.x) * -direction, hitboxAttacks[i].offset.y);
        }
    }

    protected virtual void Attack()
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
    }

    protected virtual void FallFromAbove()
    {
        if (!playedFallSound)
        {
            playedFallSound = true;
            soundFall.start();
        }

        velX = 0;
        accX = 0;

        if (mainHandler.currentState == NOTBEAT) timeToMoveOn = true;

        if (timeToMoveOn && mainHandler.currentState == BEAT)
        {
            timeToMoveOn = false;
            if (attackState == 1)
            {
                soundAttack.setParameterValue("Pre", 1);
                soundAttack.start();
                Instantiate(pYellowWarning, transform.position + new Vector3(0, 0), new Quaternion(0, 0, 0, 0));
                attackState = 2;
            }
            else if (attackState == 2)
            {
                soundAttack.setParameterValue("Pre", 2);
                soundAttack.start();
                Instantiate(pRedWarning, transform.position + new Vector3(0, 0), new Quaternion(0, 0, 0, 0));
                attackState = 3;
            }
            else if (attackState == 3)
            {
                hitboxBody.enabled = true;
                fallState = 2;
                attackActive = true;
                soundFall.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                soundImpact.start();
                attackHitboxTimer = 0;
                attackHitboxActive = true;
                inTheSky = false;
                UpdateAnimations();
                hitboxAttackFall.enabled = true;
                attackState = 4;
            }
            else if (attackState == 4)
            {
                soundFall.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                attackActive = false;
                attacking = false;
                hitboxAttackFall.enabled = false;
                busy = false;
                fallFromAbove = false;
                attackState = 0;
                attackRecovery = Random.Range(2, 3);
            }
        }
    }

    protected virtual void UpdateAttackHitbox()
    {
        attackHitboxTimer += Time.deltaTime;

        if (attackHitboxTimer >= attackHitboxTime)
        {
            attackHitboxActive = false;
            attackHitboxTimer = 0;
            for (int i = 0; i < hitboxAttacks.Count; i++)
            {
                hitboxAttacks[i].enabled = false;
            }
            hitboxAttackFall.enabled = false;
        }
    }

    public virtual void setState(int state)
    {
        currentState = state;
    }

    public virtual void setOffbeat(bool ob)
    {
        offBeat = ob;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "player")
        {
            soundAttack.setParameterValue("Pre", 3);
            soundAttack.start();
            other.GetComponent<playerHandler>().TakeDamage(damage[currentAttack], direction);
        }
    }

    //protected virtual void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.tag == "player")
    //    {
    //        soundAttack.setParameterValue("Pre", 3);
    //        soundAttack.start();
    //        other.GetComponent<playerHandler>().TakeDamage(damage[currentAttack], direction);
    //    }
    //}
}
