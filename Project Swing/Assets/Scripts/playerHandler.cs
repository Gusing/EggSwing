using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class playerHandler : MonoBehaviour
{
    float velX;
    float accX;

    public Sprite spriteIdle;
    public Sprite spriteStomp1;
    public Sprite spriteStomp2;
    Sprite spriteCurrentIdle;

    public Sprite spriteAttackFail;
    public Sprite spriteHit;
    public Sprite spriteDead;

    public Sprite spriteAttack;
    public Sprite spriteAttackHit;
    public Sprite spriteAttackB;
    public Sprite spriteAttackBHit;
    public Sprite spriteAttackC;
    public Sprite spriteAttackCHit;

    public Sprite spriteAttack2Hit;
    public Sprite spriteAttack2HitB;
    public Sprite spriteAttack2HitC;
    public Sprite spriteAttack2HitD;
    public Sprite spriteAttack3;

    public Sprite spriteDodge;
    public Sprite spriteDodgeFail;

    public ParticleSystem pAttackHit;
    public ParticleSystem pPlayerkHit;
    public ParticleSystem pSpecialAttack;

    public GameObject damageNumber;

    int comboState;
    int currentBeat;
    int lastAttackBeat;
    bool lastAttackSlow;
    bool switchBeat;

    public BoxCollider2D hitboxAttack;
    public BoxCollider2D hitboxAttack2;
    public BoxCollider2D hitboxAttack2B;
    public BoxCollider2D hitboxAttack2C;
    public BoxCollider2D hitboxAttackB;
    public BoxCollider2D hitboxAttackC;
    public BoxCollider2D hitboxAttack2D;
    public BoxCollider2D hitboxAttack3;
    BoxCollider2D hitboxBody;
    SpriteRenderer localRenderer;

    float actionTimer;
    bool busy;

    bool punchingSuccess;
    bool punchingFail;
    bool punchingActive;
    float punchSuccessTime = 0.32f;
    float quickPunchSuccessTime = 0.3f;
    float specialPunchSuccessTime = 0.7f;
    float punchFailTime = 0.5f;
    int attackType;
    bool beatPassed;
    bool dodgeSucces;
    bool dodgeFail;
    float dodgeSuccessTime = 0.5f;
    float dodgeFailTime = 0.5f;

    int currentHP;
    int maxHP;
    int specialCharges;
    int maxSpecialCharges;
    float specialChargeTimer;
    float specialChargeExtraTime;

    public SpriteRenderer rendererHPFill;
    public SpriteRenderer[] renderersSpecialCharges;

    public bool dead;

    bool directionRight;
    public int direction;

    bool hitstun;
    int hitstunDirection;
    float hitstunTime = 0.32f;

    int beatState;
    readonly int FAIL = 0, SUCCESS = 1, BEAT = 2;

    private BeatObserver beatObserver;

    FMOD.Studio.EventInstance soundAttackQuick;
    FMOD.Studio.EventInstance soundAttackSlow;
    FMOD.Studio.EventInstance soundAttackSuper;
    FMOD.Studio.EventInstance soundFinisherTackle;
    FMOD.Studio.EventInstance soundFinisherDonkey;
    FMOD.Studio.EventInstance soundFinisherDownkick;

    // Use this for initialization
    void Start()
    {
        soundAttackQuick = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_short");
        soundAttackSlow = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_long");
        soundAttackSuper = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_super");
        soundFinisherTackle = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Combo_tackle");
        soundFinisherDonkey = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Combo_donkey_kick");
        soundFinisherDownkick = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Combo_drop_kick");

        spriteCurrentIdle = spriteStomp1;

        maxHP = 10;
        currentHP = maxHP;
        maxSpecialCharges = 3;
        specialCharges = maxSpecialCharges;
        specialChargeExtraTime = 15;
        directionRight = true;
        direction = 1;

        beatObserver = GetComponent<BeatObserver>();
        localRenderer = GetComponent<SpriteRenderer>();

        hitboxAttack.enabled = false;
        hitboxAttackB.enabled = false;
        hitboxAttackC.enabled = false;

        hitboxAttack2.enabled = false;
        hitboxAttack2B.enabled = false;
        hitboxAttack2C.enabled = false;
        hitboxAttack2D.enabled = false;

        hitboxAttack3.enabled = false;

        hitboxBody = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // update HP bar
        rendererHPFill.transform.localScale = new Vector3(((float)currentHP / (float)maxHP) * 1, 1);

        // update special charges
        for (int i = 2; i >= 0; i--)
        {
            if (specialCharges > i) renderersSpecialCharges[i].enabled = true;
            else renderersSpecialCharges[i].enabled = false;
        }

        if (dead)
        {
            return;
        }

        if (punchingSuccess)
        {
            if (attackType == 1) SuccessfulPunch();
            if (attackType == 2) SuccessfulQuickPunch();
            if (attackType == 3) SuccessfulSpecialPunch();
        }
        if (punchingFail) FailedPunch();
        if (dodgeSucces) SuccessfulDodge();
        if (dodgeFail) FailedDodge();

        if (hitstun) Hitstun();

        if (specialCharges < 3) specialChargeTimer += Time.deltaTime;

        if (specialChargeTimer >= specialChargeExtraTime)
        {
            specialCharges++;
            specialChargeTimer = 0;
        }

        if ((beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat)
        {
            beatState = BEAT;

            spriteCurrentIdle = spriteStomp2;
            
        }
        else if ((beatObserver.beatMask & BeatType.OffBeat) == BeatType.OffBeat)
        {
            
            beatState = SUCCESS;
            
            if (!switchBeat)
            {
                switchBeat = true;
                currentBeat++;
                //currentBeat = currentBeat % 8;
                //print(currentBeat);
            }
            if ((currentBeat > lastAttackBeat + 1 && !lastAttackSlow) || (currentBeat > lastAttackBeat + 2 && lastAttackSlow))
            {
                comboState = 0;
            }

        }
        else
        {
            
            switchBeat = false;
            beatState = FAIL;
        }

        if ((beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat)
        {
            spriteCurrentIdle = spriteStomp1;
            
        }

        if (!busy) localRenderer.sprite = spriteCurrentIdle;

        if (Input.GetButtonDown("Attack") && !busy)
        {
            Punch();
        }
        if (Input.GetButtonDown("Quick Attack") && !busy)
        {
            QuickPunch();
        }
        if (Input.GetButtonDown("Special Attack") && !busy)
        {
            SpecialPunch();
        }
        if (Input.GetButtonDown("Dodge") && !busy)
        {
            Dodge();
        }
        if ((Input.GetButton("MoveRight") || Input.GetAxisRaw("Move Axis") > 0 || Input.GetAxisRaw("Move Axis 2") > 0) && !busy)
        {
            accX = 26f;
            directionRight = true;
            direction = 1;
            localRenderer.flipX = false;
        }
        else if ((Input.GetButton("MoveLeft") || Input.GetAxisRaw("Move Axis") < 0 || Input.GetAxisRaw("Move Axis 2") < 0) && !busy)
        {
            accX = -26f;
            directionRight = false;
            direction = -1;
            localRenderer.flipX = true;
        }
        else if (!busy)
        {
            accX = 0;
            velX = 0;
        }

        if ((accX > 0 && velX < 0) || (accX < 0 && velX > 0)) velX = 0;
        velX += accX * Time.deltaTime;

        if (!busy) velX = Mathf.Clamp(velX, -3.1f, 3.1f);
        transform.Translate(new Vector3(velX * Time.deltaTime, 0));

        if (transform.position.x < -9.2f) transform.position = new Vector3(-9.2f, transform.position.y);
        if (transform.position.x > 9.3f) transform.position = new Vector3(9.3f, transform.position.y);
    }

    void Punch()
    {
        accX = 0;
        velX = 0;
        attackType = 1;
        if (beatState == SUCCESS || beatState == BEAT)
        {
            soundAttackSlow.setParameterValue("Hit", 0);
            soundAttackSlow.start();
            UpdateHitboxes();
            lastAttackBeat = currentBeat;
            lastAttackSlow = true;
            punchingSuccess = true;
            busy = true;
            if (beatState == SUCCESS) print("pre");
            if (beatState == BEAT) print("post");
            SuccessfulPunch();
        }
        else if (beatState == FAIL)
        {
            punchingFail = true;
            busy = true;
            FailedPunch();
        }

    }

    void SuccessfulPunch()
    {
        if (punchingActive)
        {
            actionTimer += Time.deltaTime;

            if (actionTimer >= punchSuccessTime)
            {
                if (comboState == 0) comboState = 3;
                else comboState = 0;
                GetComponent<SpriteRenderer>().sprite = spriteIdle;
                actionTimer = 0;
                punchingSuccess = false;
                busy = false;
                beatPassed = false;
                punchingActive = false;
                hitboxAttack.enabled = false;
                hitboxAttackB.enabled = false;
                hitboxAttackC.enabled = false;
            }
        }
        else
        {
            if (comboState == 3) GetComponent<SpriteRenderer>().sprite = spriteAttackB;
            else if (comboState == 4) GetComponent<SpriteRenderer>().sprite = spriteAttackC;
            else
            {
                comboState = 0;
                GetComponent<SpriteRenderer>().sprite = spriteAttack;
            }

            if (!beatPassed)
            {
                if (beatState == FAIL) beatPassed = true;
            }
            else
            {
                if (beatState == SUCCESS)
                {
                    // tackle
                    if (comboState == 3)
                    {
                        soundFinisherTackle.setParameterValue("Hit", 0);
                        soundFinisherTackle.start();
                        GetComponent<SpriteRenderer>().sprite = spriteAttackBHit;
                        hitboxAttackB.enabled = true;
                        transform.Translate(new Vector3(0.8f * direction, 0));
                    }

                    // down kick
                    else if (comboState == 4)
                    {
                        soundFinisherDownkick.setParameterValue("Hit", 0);
                        soundFinisherDownkick.start();
                        GetComponent<SpriteRenderer>().sprite = spriteAttackCHit;
                        hitboxAttackC.enabled = true;
                        transform.Translate(new Vector3(0.5f * direction, 0));
                    }

                    // punch
                    else
                    {
                        soundAttackSlow.setParameterValue("Hit", 1);
                        soundAttackSlow.start();
                        GetComponent<SpriteRenderer>().sprite = spriteAttackHit;
                        hitboxAttack.enabled = true;
                        transform.Translate(new Vector3(0.5f * direction, 0));
                    }

                    punchingActive = true;
                }
            }
        }

    }

    void FailedPunch()
    {
        GetComponent<SpriteRenderer>().sprite = spriteAttackFail;
        actionTimer += Time.deltaTime;

        if (actionTimer >= punchFailTime)
        {
            GetComponent<SpriteRenderer>().sprite = spriteIdle;
            actionTimer = 0;
            punchingFail = false;
            busy = false;
        }
    }

    void QuickPunch()
    {
        accX = 0;
        velX = 0;
        attackType = 2;
        if (beatState == SUCCESS || beatState == BEAT)
        {
            UpdateHitboxes();
            lastAttackBeat = currentBeat;
            lastAttackSlow = false;
            punchingSuccess = true;
            busy = true;
            if (beatState == SUCCESS) print("pre");
            if (beatState == BEAT) print("post");

            // small kick
            if (comboState == 0 || comboState == 4)
            {
                soundAttackQuick.setParameterValue("Hit", 0);
                soundAttackQuick.start();
                GetComponent<SpriteRenderer>().sprite = spriteAttack2Hit;
                hitboxAttack2.enabled = true;
            }

            // small uppercut
            if (comboState == 1)
            {
                soundAttackQuick.setParameterValue("Hit", 0);
                soundAttackQuick.start();
                GetComponent<SpriteRenderer>().sprite = spriteAttack2HitB;
                hitboxAttack2B.enabled = true;
            }

            // donkey kick
            if (comboState == 2)
            {
                soundFinisherDonkey.setParameterValue("Hit", 0);
                soundFinisherDonkey.start();
                GetComponent<SpriteRenderer>().sprite = spriteAttack2HitC;
                hitboxAttack2C.enabled = true;
                transform.Translate(new Vector3(0.5f * direction, 0));
            }

            // headbutt
            if (comboState == 3)
            {
                soundAttackQuick.setParameterValue("Hit", 0);
                soundAttackQuick.start();
                GetComponent<SpriteRenderer>().sprite = spriteAttack2HitD;
                hitboxAttack2D.enabled = true;
                transform.Translate(new Vector3(0.8f * direction, 0));
            }
            SuccessfulQuickPunch();
        }
        else if (beatState == FAIL)
        {
            punchingFail = true;
            busy = true;
            FailedPunch();
        }
    }

    void SuccessfulQuickPunch()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= quickPunchSuccessTime)
        {
            if (comboState == 0 || comboState == 4) hitboxAttack2.enabled = false;
            if (comboState == 1) hitboxAttack2B.enabled = false;
            if (comboState == 2) hitboxAttack2C.enabled = false;
            if (comboState == 3) hitboxAttack2D.enabled = false;
            comboState += 1;
            if (comboState == 3) comboState = 0;
            if (comboState >= 5) comboState = 1;
            GetComponent<SpriteRenderer>().sprite = spriteIdle;
            actionTimer = 0;
            punchingSuccess = false;
            busy = false;
            beatPassed = false;
            punchingActive = false;
            
        }
        
    }

    void SpecialPunch()
    {
        if (specialCharges == 0)
        {
            // play sound
            return;
        }
        accX = 0;
        velX = 0;
        attackType = 3;
        if (beatState == SUCCESS || beatState == BEAT)
        {
            soundAttackSuper.start();
            Instantiate(pSpecialAttack, transform.position, new Quaternion(0, 0, 0, 0));
            GetComponent<SpriteRenderer>().sprite = spriteAttack3;
            UpdateHitboxes();
            specialCharges--;
            lastAttackBeat = currentBeat;
            lastAttackSlow = false;
            punchingSuccess = true;
            busy = true;
            hitboxAttack3.enabled = true;
            comboState = 0;
            if (beatState == SUCCESS) print("pre");
            if (beatState == BEAT) print("post");
            SuccessfulSpecialPunch();
        }
        else if (beatState == FAIL)
        {
            punchingFail = true;
            busy = true;
            FailedPunch();
        }
    }

    void SuccessfulSpecialPunch()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= quickPunchSuccessTime)
        {
            specialChargeTimer = 0;
            hitboxAttack3.enabled = false;
            GetComponent<SpriteRenderer>().sprite = spriteIdle;
            actionTimer = 0;
            punchingSuccess = false;
            busy = false;
            beatPassed = false;
            punchingActive = false;

        }
    }

    void Hitstun()
    {
        hitboxBody.enabled = false;

        velX = (-3f - actionTimer * 5) * hitstunDirection;

        actionTimer += Time.deltaTime;

        if (actionTimer >= hitstunTime)
        {
            hitboxBody.enabled = true;
            GetComponent<SpriteRenderer>().sprite = spriteIdle;
            actionTimer = 0;
            hitstun = false;
            busy = false;
            hitboxBody.enabled = true;
        }
    }

    void UpdateHitboxes()
    {
        hitboxAttack.offset = new Vector2(Mathf.Abs(hitboxAttack.offset.x) * direction, hitboxAttack.offset.y);
        hitboxAttackB.offset = new Vector2(Mathf.Abs(hitboxAttackB.offset.x) * direction, hitboxAttackB.offset.y);
        hitboxAttackC.offset = new Vector2(Mathf.Abs(hitboxAttackC.offset.x) * direction, hitboxAttackC.offset.y);
        hitboxAttack2.offset = new Vector2(Mathf.Abs(hitboxAttack2.offset.x) * direction, hitboxAttack2.offset.y);
        hitboxAttack2B.offset = new Vector2(Mathf.Abs(hitboxAttack2B.offset.x) * direction, hitboxAttack2B.offset.y);
        hitboxAttack2C.offset = new Vector2(Mathf.Abs(hitboxAttack2C.offset.x) * direction, hitboxAttack2C.offset.y);
        hitboxAttack2D.offset = new Vector2(Mathf.Abs(hitboxAttack2D.offset.x) * direction, hitboxAttack2D.offset.y);
    }

    void Dodge()
    {
        accX = 0;
        velX = 0;
        dodgeSucces = true;
        busy = true;
        if (Input.GetButton("MoveRight"))
        {
            directionRight = true;
            direction = 1;
            localRenderer.flipX = false;
        }
        if (Input.GetButton("MoveLeft"))
        {
            directionRight = false;
            direction = -1;
            localRenderer.flipX = true;
        }
        SuccessfulDodge();
        /*
        if (beatState == SUCCESS || beatState == BEAT)
        {
            dodgeSucces = true;
            busy = true;
            SuccessfulDodge();
        }
        else if (beatState == FAIL)
        {
            dodgeFail = true;
            busy = true;
            FailedDodge();
        }
        */
    }

    void SuccessfulDodge()
    {
        hitboxBody.enabled = false;
        GetComponent<SpriteRenderer>().sprite = spriteDodge;
        actionTimer += Time.deltaTime;

        if (velX == 0) velX = -(7f - actionTimer * 8) * direction;
        else velX = (7f - actionTimer * 8) * direction;
        
        if (actionTimer >= dodgeSuccessTime - 0.15f)
        {
            hitboxBody.enabled = true;
        }

        if (actionTimer >= dodgeSuccessTime)
        {
            hitboxBody.enabled = true;
            GetComponent<SpriteRenderer>().sprite = spriteIdle;
            actionTimer = 0;
            dodgeSucces = false;
            busy = false;
        }
    }

    void FailedDodge()
    {
        GetComponent<SpriteRenderer>().sprite = spriteDodgeFail;
        actionTimer += Time.deltaTime;

        if (actionTimer >= dodgeFailTime)
        {
            GetComponent<SpriteRenderer>().sprite = spriteIdle;
            actionTimer = 0;
            dodgeFail = false;
            busy = false;
        }
    }

    public void TakeDamage(int dmg, int hitDirection)
    {
        hitstunDirection = hitDirection;

        currentHP -= dmg;

        hitboxBody.enabled = false;

        velX = 0;
        accX = 0;

        actionTimer = 0;
        punchingSuccess = false;
        punchingFail = false;
        dodgeFail = false;
        busy = true;
        beatPassed = false;
        punchingActive = false;
        comboState = 0;
        hitboxAttack2.enabled = false;
        hitboxAttack2B.enabled = false;
        hitboxAttack2C.enabled = false;
        hitboxAttack2D.enabled = false;
        hitboxAttack.enabled = false;
        hitboxAttackB.enabled = false;
        hitboxAttackC.enabled = false;
        hitboxAttack3.enabled = false;

        Instantiate(pPlayerkHit, transform.position, new Quaternion(0, 0, 0, 0));

        if (currentHP <= 0) Die();
        else
        {
            GetComponent<SpriteRenderer>().sprite = spriteHit;
            hitstun = true;
        }
        
    }

    public void Die()
    {
        currentHP = 0;
        GetComponent<SpriteRenderer>().sprite = spriteDead;
        hitboxBody.enabled = false;
        dead = true;
    }

    public void SetBeatState(int state)
    {
        beatState = state;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        print("collision enter");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        int tDmg = 0;
        Vector3 tBox = new Vector3();

        // forward punch
        if (hitboxAttack.IsTouching(other))
        {
            soundAttackSlow.setParameterValue("Hit", 2);
            soundAttackSlow.start();
            tDmg = 3;
            tBox = (Vector2)hitboxAttack.transform.position + hitboxAttack.offset;
        }

        // tackle
        if (hitboxAttackB.IsTouching(other))
        {
            soundFinisherTackle.setParameterValue("Hit", 1);
            soundFinisherTackle.start();
            tDmg = 4;
            tBox = (Vector2)hitboxAttackB.transform.position + hitboxAttackB.offset;
        }

        // down kick
        if (hitboxAttackC.IsTouching(other))
        {
            soundFinisherDownkick.setParameterValue("Hit", 1);
            soundFinisherDownkick.start();
            tDmg = 5;
            tBox = (Vector2)hitboxAttackC.transform.position + hitboxAttackC.offset;
        }


        // small kick
        if (hitboxAttack2.IsTouching(other))
        {
            soundAttackQuick.setParameterValue("Hit", 1);
            soundAttackQuick.start();
            tDmg = 1;
            tBox = (Vector2)hitboxAttack2.transform.position + hitboxAttack2.offset;
        }

        // small uppercut
        if (hitboxAttack2B.IsTouching(other))
        {
            soundAttackQuick.setParameterValue("Hit", 1);
            soundAttackQuick.start();
            tDmg = 1;
            tBox = (Vector2)hitboxAttack2B.transform.position + hitboxAttack2B.offset;
        }

        // donkey kick
        if (hitboxAttack2C.IsTouching(other))
        {
            soundFinisherDonkey.setParameterValue("Hit", 1);
            soundFinisherDonkey.start();
            tDmg = 3;
            tBox = (Vector2)hitboxAttack2C.transform.position + hitboxAttack2C.offset;
        }

        // headbutt
        if (hitboxAttack2D.IsTouching(other))
        {
            soundAttackQuick.setParameterValue("Hit", 1);
            soundAttackQuick.start();
            tDmg = 1;
            tBox = (Vector2)hitboxAttack2D.transform.position + hitboxAttack2D.offset;
        }

        // super
        if (hitboxAttack3.IsTouching(other))
        {
            soundAttackQuick.start();
            tDmg = 8;
            tBox = (Vector2)hitboxAttack3.transform.position + hitboxAttack3.offset;
        }

        if (tDmg > 0)
        {
            print("dmg");
            if (other.tag == "enemy") other.GetComponent<enemyAHandler>().TakeDamage(tDmg);
            if (other.tag == "enemyB") other.GetComponent<enemyBHandler>().TakeDamage(tDmg);
            if (other.tag == "enemyDummy") other.GetComponent<enemyDummyHandler>().TakeDamage(tDmg);

            print(tBox);
            Instantiate(pAttackHit, tBox, new Quaternion(0, 0, 0, 0));
            GameObject tDmgNumber = Instantiate(damageNumber, other.transform.position + new Vector3(0, 0.4f), new Quaternion(0, 0, 0, 0));
            tDmgNumber.GetComponent<dmgNumberHandler>().Init(tDmg);
        }
    }
}
