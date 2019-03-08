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
    
    protected bool hitstun;
    protected bool knockback;
    protected bool bigKnockback;
    protected int hitstunLimit;
    protected int knockbackLimit;
    protected int bigKnockbackLimit;
    protected bool invincible;

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

    protected bool busy;

    protected int attackState;
    protected bool timeToMoveOn;
    protected bool newBeat;

    // when enemy has multiple attacks
    protected int currentAttack;
    protected int attackStreak;
    protected int lastAttack;

    public SpriteRenderer rendererHPBar;
    public SpriteRenderer rendererHPFill;

    public ParticleSystem pYellowWarning;
    public ParticleSystem pRedWarning;

    protected GameObject player;

    public List<Transform> warningPoints;

    protected SpriteRenderer localSpriteRenderer;

    protected BoxCollider2D hitboxBody;
    public List<BoxCollider2D> hitboxAttacks;

    protected int currentState;
    protected readonly int NOTBEAT = 0, PREBEAT = 1, BEAT = 2, OFFBEAT = 3;

    protected FMOD.Studio.EventInstance soundAttack;
    protected FMOD.Studio.EventInstance soundDeath;

    public enemyHandler()
    {
        
    }

    public virtual void Start()
    {
        for (int i = 0; i < hitboxAttacks.Count; i++)
        {
            hitboxAttacks[i].enabled = false;
        }

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
        // update HP bar
        rendererHPFill.transform.localScale = new Vector3(((float)currentHP / (float)maxHP) * 1, 1);

        // update invincibility
        if (invincible) GetComponent<BoxCollider2D>().enabled = false;
        else GetComponent<BoxCollider2D>().enabled = true;

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

    public virtual void UpdateMovement()
    {
        // keep moving if not busy
        if (attacking) accX = -0.001f * direction;
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

    public virtual void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        print("hit for " + dmg);
        if (currentHP <= 0) Die(dmg);
        else
        {
            if (dmg >= hitstunLimit)
            {
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
            if (dmg >= knockbackLimit) knockback = true;
            if (dmg >= bigKnockbackLimit) bigKnockback = true;
        }
    }

    public virtual void Die(int dmg)
    {
        soundDeath.start();
        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;
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
            warningPoints[i].localPosition = new Vector3(Mathf.Abs(warningPoints[i].localPosition.x) * direction, warningPoints[i].localPosition.y);
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
        if (currentState == NOTBEAT) timeToMoveOn = true;
    }

    protected virtual void UpdateAttackHitbox()
    {
        attackHitboxTimer += Time.deltaTime;

        if (attackHitboxTimer >= attackHitboxTime)
        {
            print("no longer active");
            attackHitboxActive = false;
            attackHitboxTimer = 0;
            for (int i = 0; i < hitboxAttacks.Count; i++)
            {
                hitboxAttacks[i].enabled = false;
            }
        }
    }

    public virtual void setState(int state)
    {
        print("parent set state");
        currentState = state;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "player")
        {
            soundAttack.setParameterValue("Pre", 3);
            soundAttack.start();
            other.GetComponent<playerHandler>().TakeDamage(damage[currentAttack], direction);
        }
    }
}
