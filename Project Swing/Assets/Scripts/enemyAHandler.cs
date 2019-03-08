using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAHandler : MonoBehaviour {

    float accX;
    float velX;

    bool hitstun;
    bool knockBack;
    bool bigKnockBack;
    float hitstunTime = 0.32f;

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

    SpriteRenderer localSpriteRenderer;

    bool invincible;

    int health;
    int maxHealth = 10;

    public SpriteRenderer rendererHPBar;
    public SpriteRenderer rendererHPFill;

    float actionTimer;
    bool attacking;
    bool busy;
    public bool dead;
    bool attackHitboxActive;
    float attackHitboxTimer;
    float attackHitboxTime = 0.3f;

    GameObject player;

    int attackState;
    bool timeToMoveOn;
    bool newBeat;

    public BoxCollider2D rendererAttack;

    BoxCollider2D hitboxBody;

    public ParticleSystem pYellowWarning;
    public ParticleSystem pRedWarning;

    public Transform warningPoint;

    float attackDelay;
    float attackRecovery;

    int direction;
    bool directionRight;
    int hitstunDirection;

    int currentState;
    readonly int NOTBEAT = 0, BEAT = 1, ACTUALBEAT = 2, OFFBEAT = 3;

    FMOD.Studio.EventInstance soundAttack;
    FMOD.Studio.EventInstance soundDeath;

    // Use this for initialization
    void Start () {
        direction = 1;
        directionRight = false;

        health = maxHealth;

        soundAttack = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_attack");
        soundDeath = FMODUnity.RuntimeManager.CreateInstance("event:/Egg_death");

        player = GameObject.Find("Player");

        localSpriteRenderer = GetComponent<SpriteRenderer>();
        hitboxBody = GetComponent<BoxCollider2D>();

        rendererAttack.enabled = false;

        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        // update HP bar
        rendererHPFill.transform.localScale = new Vector3(((float)health / (float)maxHealth) * 1, 1);

        if (dead) return;

        if (hitstun) Hitstun();
        if (attacking) Attack();
        if (attackHitboxActive) AttackHitbox();

        if (attackRecovery > 0)
        {
            attackRecovery -= Time.deltaTime;
        }
        
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

        if (!busy)
        {
            if (currentState == ACTUALBEAT)
            {
                localSpriteRenderer.sprite = spriteStomp2;
            }
            if (currentState == OFFBEAT)
            {
                print("sd");
                localSpriteRenderer.sprite = spriteStomp1;
            }
        }
    
        if (invincible) GetComponent<BoxCollider2D>().enabled = false;
        else GetComponent<BoxCollider2D>().enabled = true;
        
        if (!attacking && !hitstun && attackRecovery <= 1 && !player.GetComponent<playerHandler>().dead)
        {
            hitboxBody.offset = new Vector2(Mathf.Abs(hitboxBody.offset.x) * direction, hitboxBody.offset.y);
            if (player.transform.position.x < transform.position.x)
            {
                direction = 1;
                directionRight = false;
                localSpriteRenderer.flipX = false;
            }
            else
            {
                direction = -1;
                directionRight = true;
                localSpriteRenderer.flipX = true;
            }
        }

        if (attacking) accX = -0.001f * direction;
        else if (!busy) accX = -18f * direction;
        
        velX += accX * Time.deltaTime;
        if (!knockBack) velX = Mathf.Clamp(velX, -1.8f, 1.8f);
        transform.Translate(new Vector3(velX * Time.deltaTime, 0));

    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        print("hit for " + dmg);
        if (health <= 0) Die(dmg);
        else
        {
            if (player.transform.position.x < transform.position.x)
            {
                direction = 1;
                directionRight = false;
                localSpriteRenderer.flipX = false;
            }
            else
            {
                direction = -1;
                directionRight = true;
                localSpriteRenderer.flipX = true;
            }
            rendererHPBar.enabled = true;
            rendererHPFill.enabled = true;
            if (player.transform.position.x < transform.position.x) hitstunDirection = 1;
            else hitstunDirection = -1;
            invincible = true;
            hitstun = true;
            busy = true;
            if (dmg >= 3) knockBack = true;
            if (dmg >= 8) bigKnockBack = true;
            rendererAttack.enabled = false;
            attacking = false;
            attackState = 0;
        }
    }

    public void Die(int dmg)
    {
        soundDeath.start();
        rendererHPBar.enabled = false;
        rendererHPFill.enabled = false;
        rendererAttack.enabled = false;
        if (dmg == 6) GetComponent<SpriteRenderer>().sprite = spriteDead4;
        else
        {
            int rand = Random.Range(0, 3);
            if (rand == 0) GetComponent<SpriteRenderer>().sprite = spriteDead;
            if (rand == 1) GetComponent<SpriteRenderer>().sprite = spriteDead2;
            if (rand == 2) GetComponent<SpriteRenderer>().sprite = spriteDead3;
        }
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "EnemiesDead";
        dead = true;
    }

    void Hitstun()
    {
        GetComponent<SpriteRenderer>().sprite = spriteHitstun;
        actionTimer += Time.deltaTime;

        if (bigKnockBack) velX = (7f + actionTimer * 6) * hitstunDirection;
        else if (knockBack) velX = (3f + actionTimer * 5) * hitstunDirection;
        else velX = 0;
        
        if (actionTimer >= hitstunTime)
        {
            GetComponent<SpriteRenderer>().sprite = spriteIdle;
            actionTimer = 0;
            hitstun = false;
            busy = false;
            knockBack = false;
            bigKnockBack = false;
            invincible = false;
        }
    }

    void UpdateHitboxes()
    {
        warningPoint.localPosition = new Vector3(Mathf.Abs(warningPoint.localPosition.x) * direction, warningPoint.localPosition.y);
        rendererAttack.offset = new Vector2(Mathf.Abs(rendererAttack.offset.x) * -direction, rendererAttack.offset.y);
    }

    void Attack()
    {
        if (hitstun)
        {
            rendererAttack.enabled = false;
            attacking = false;
            attackState = 0;
            return;
        }

        if (!hitstun) velX = 0;
        accX = 0;

        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
            return;
        }

        if (currentState == NOTBEAT) timeToMoveOn = true;
        
        if (timeToMoveOn && currentState == BEAT)
        {
            timeToMoveOn = false;
            if (attackState == 1)
            {
                soundAttack.setParameterValue("Pre", 1);
                soundAttack.start();
                Instantiate(pYellowWarning, warningPoint.position, new Quaternion(0, 0, 0, 0));
                attackState = 2;
                localSpriteRenderer.sprite = spriteAttackPre;
            }
            else if (attackState == 2)
            {
                soundAttack.setParameterValue("Pre", 2);
                soundAttack.start();
                Instantiate(pRedWarning, warningPoint.position, new Quaternion(0, 0, 0, 0));
                attackState = 3;
                localSpriteRenderer.sprite = spriteAttackPre;
            }
            else if (attackState == 3)
            {
                soundAttack.setParameterValue("Pre", 4);
                soundAttack.start();
                attackHitboxTimer = 0;
                attackHitboxActive = true;
                rendererAttack.enabled = true;
                attackState = 4;
                localSpriteRenderer.sprite = spriteAttackActive;
            }
            else if (attackState == 4)
            {
                rendererAttack.enabled = false;
                localSpriteRenderer.sprite = spriteIdle;
                attacking = false;
                busy = false;
                attackState = 0;
                attackRecovery = Random.Range(2, 3);
            }
        }

        if (currentState == NOTBEAT && attackState == 4) rendererAttack.enabled = false;
    }

    void AttackHitbox()
    {
        attackHitboxTimer += Time.deltaTime;

        if (attackHitboxTimer >= attackHitboxTime)
        {
            attackHitboxActive = false;
            attackHitboxTimer = 0;
            rendererAttack.enabled = false;
        }
    }

    public void setState(int state)
    {
        currentState = state;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "player")
        {
            soundAttack.setParameterValue("Pre", 3);
            soundAttack.start();
            other.GetComponent<playerHandler>().TakeDamage(3, direction);
        }
    }
}
