﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;
using UnityEngine.UI;

public class playerHandler : MonoBehaviour
{
    float velX;
    float accX;
    
    public Animator animator;

    public ParticleSystem pAttackHit;
    public ParticleSystem pPlayerkHit;
    public ParticleSystem pSpecialAttack;
    public ParticleSystem pBlock;
    public ParticleSystem pCounter;
    public ParticleSystem pBirdHit;
    public ParticleSystem pCurrencyPick;
    public ParticleSystem pHPPickupPick;

    public GameObject effectHitBlue;
    public GameObject effectHitRed;
    public GameObject effectHitSuper;

    public GameObject damageNumberEnemy;
    public GameObject damageNumberPlayer;

    public List<GameObject> birds;

    int comboState;
    int currentBeat;
    int lastAttackBeat;
    bool lastAttackSlow;
    bool switchBeat;

    public BoxCollider2D hitboxAttack1A;
    public BoxCollider2D hitboxAttack1B;
    public BoxCollider2D hitboxAttack1C;

    public BoxCollider2D hitboxAttack2A;
    public BoxCollider2D hitboxAttack2B;
    public BoxCollider2D hitboxAttack2C;
    public BoxCollider2D hitboxAttack2D;

    public BoxCollider2D hitboxAttack3A;

    public BoxCollider2D hitboxAttack4A;

    public BoxCollider2D hitboxAttackBird;

    public BoxCollider2D hitboxPickup;

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
    bool usingSuper;
    bool beatPassed;
    bool lastAttackHit;
    int lastHitBeat;
    bool birdHitReady;
    bool birdHitting;
    float birdPunchSuccessTime = 0.2f;
    bool birdHitstun;

    int attackID;
    int attackIDStart;

    bool dodgeSucces;
    bool dodgeFail;
    float dodgeSuccessTime = 0.5f;
    float dodgeFailTime = 0.5f;

    bool blocking;
    float blockTime;
    int blockBeat;
    bool successfulBlock;
   
    bool counterReady;
    float counterTime = 0.4f;
    bool countering;

    int currentHP;
    int maxHP;
    int specialCharges;
    int maxSpecialCharges;
    float specialChargeTimer;
    float specialChargeExtraTime;
    public int currentStreak;
    float streakDisappearDelay;
    float streakTimer;
    public int streakLevel;
    public int currentCurrency;
    int startingCurrency;

    public SpriteRenderer rendererHPFill;
    public SpriteMask maskHPFill;
    public SpriteRenderer[] renderersSpecialCharges;
    public Text textStreak;
    public Text textCurrency;

    public bool dead;
    
    public int direction;

    bool hitstun;
    int hitstunDirection;
    float hitstunTime = 0.32f;

    int beatState;
    readonly int FAIL = 0, SUCCESS = 1, BEAT = 2;

    List<Attack[]> allCombos;
    List<Attack[]> currentCombos;

    GameObject mainCamera;
    GameObject beatIndicator;

    readonly int QUICK = 0, SLOW = 1;
    
    FMOD.Studio.EventInstance soundAttackSuper;
    FMOD.Studio.EventInstance soundAttackSuperUnable;

    FMOD.Studio.EventInstance soundDodge;
    FMOD.Studio.EventInstance soundDie;
    FMOD.Studio.EventInstance soundFail;

    FMOD.Studio.EventInstance soundPickupCurrency;

    // debug
    public Text txtComboState;
    public Text txtNumberCombos;
    
    void Start()
    {
        soundAttackSuper = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_super");
        soundAttackSuperUnable = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_super_fail");
        soundDodge = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Dodge");
        soundDie = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Die");
        soundFail = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Miss_Brad");
        soundPickupCurrency = FMODUnity.RuntimeManager.CreateInstance("event:/Object/Pickup_gold_random");

        mainCamera = GameObject.Find("Main Camera");
        beatIndicator = GameObject.Find("BeatIndicator");

        allCombos = new List<Attack[]>();
        birds = new List<GameObject>();

        // populate available combos
        allCombos.Add(new Attack[] { new Attack2A(hitboxAttack2A), new Attack2B(hitboxAttack2B), new Attack2C(hitboxAttack2C) });
        allCombos.Add(new Attack[] { new Attack1A(hitboxAttack1A), new Attack1B(hitboxAttack1B) });
        allCombos.Add(new Attack[] { new Attack1A(hitboxAttack1A), new Attack2D(hitboxAttack2D), new Attack1C(hitboxAttack1C) });

        currentCombos = new List<Attack[]>();
        RestockCombos();

        maxHP = 15;
        currentHP = maxHP;
        maxSpecialCharges = 3;
        specialCharges = maxSpecialCharges;
        specialChargeExtraTime = 15;
        streakDisappearDelay = 3;
        direction = 1;
        blockTime = (60f / mainHandler.currentBpm) * 1.5f;
        blockBeat = -10;

        localRenderer = GetComponent<SpriteRenderer>();

        foreach (BoxCollider2D x in GetComponentsInChildren<BoxCollider2D>())
        {
            x.enabled = false;
        }
        hitboxPickup.enabled = true;

        hitboxBody = GetComponent<BoxCollider2D>();
        hitboxBody.enabled = true;
    }
    
    public void Init(int numCurrency)
    {
        currentCurrency = numCurrency;
        startingCurrency = currentCurrency;
    }

    void Update()
    {
        //txtComboState.text = "Current combo state: " + comboState;
        //txtNumberCombos.text = "Number of available combos: " + currentCombos.Count;

        // update animations
        UpdateAnimations();

        // update HUD
        maskHPFill.transform.localPosition = new Vector3(-4.44f + (((float)currentHP / (float)maxHP * 4.77f)), 0);
        
        for (int i = 2; i >= 0; i--)
        {
            if (specialCharges > i) renderersSpecialCharges[i].enabled = true;
            else renderersSpecialCharges[i].enabled = false;
        }

        if (currentStreak > 0)
        {
            textStreak.text = "Streak: " + currentStreak;
            textStreak.transform.localScale = new Vector3((0.5f + streakLevel * 0.15f) - (streakTimer / streakDisappearDelay) * 0.5f, (0.5f + streakLevel * 0.15f) - (streakTimer / streakDisappearDelay) * 0.5f, 1);
        }

        if (mainHandler.staticLevel > 0) textCurrency.text = "Munny: " + currentCurrency;

        // return if dead
        if (dead)
        {
            return;
        }

        // update moves
        if (punchingSuccess)
        {
            if (attackType == 1) SuccessfulQuickPunch();
            if (attackType == 2) SuccessfulPunch();
            if (attackType == 3) SuccessfulSpecialPunch();
        }
        if (punchingFail) FailedPunch();
        if (dodgeSucces) SuccessfulDodge();
        if (dodgeFail) FailedDodge();
        if (blocking) SuccessfulBlock();
        if (countering) SuccessfulCounterPunch();
        if (birdHitting) SuccessfulBirdPunch();

        if (hitstun) Hitstun();
        
        // update special charges
        if (specialCharges < 3) specialChargeTimer += Time.deltaTime;

        if (specialChargeTimer >= specialChargeExtraTime)
        {
            specialCharges++;
            specialChargeTimer = 0;
        }

        // update streak
        if (currentStreak > 0) streakTimer += Time.deltaTime;

        if (streakTimer >= streakDisappearDelay)
        {
            streakTimer = 0;
            streakLevel = 0;
            currentStreak = 0;
            textStreak.enabled = false;
        }

        // update birds
        birdHitReady = false;
        for (int i = 0; i < birds.Count; i++)
        {
            if (birds[i].GetComponent<enemyBirdHandler>().readyToBeDestroyed)
            {
                Destroy(birds[i]);
                birds.RemoveAt(i);
                i--;
            }
            else if (birds[i].GetComponent<enemyBirdHandler>().readyToBeHit) birdHitReady = true;
        }

        // update beat and combo state
        if (mainHandler.currentState == BEAT)
        {
            beatState = BEAT;
        }
        else if (mainHandler.currentState == SUCCESS)
        {
            beatState = SUCCESS;
            
            if (!switchBeat)
            {
                switchBeat = true;
                currentBeat++;
            }

            if (successfulBlock)
            {
                blockBeat = currentBeat;
                successfulBlock = false;
            }

            if ((currentBeat > lastAttackBeat + 1 && !lastAttackSlow) || 
                (currentBeat > lastAttackBeat + 2 && lastAttackSlow) || 
                (!lastAttackHit && (
                (currentBeat > lastAttackBeat && !lastAttackSlow) || 
                (currentBeat > lastAttackBeat + 1 && lastAttackSlow))))
            {
                RestockCombos();
            }

        }
        else
        {
            if ((currentBeat > lastHitBeat + 0 && !lastAttackSlow) ||
                (currentBeat > lastHitBeat + 1 && lastAttackSlow))
            {
                lastAttackHit = false;
            }

            switchBeat = false;
            beatState = FAIL;
        }

        // check if hit bird
        if (birdHitReady)
        {
            if (Input.GetButtonDown("Light Attack") || Input.GetButtonDown("Heavy Attack"))
            {
                attackID++;
                attackIDStart = attackID;
                BirdPunch();
            }
        }

        if (!busy && mainHandler.songStarted)
        {
            if (Input.GetButtonDown("Heavy Attack"))
            {
                attackID++;
                attackIDStart = attackID;
                Punch();
                beatIndicator.GetComponent<beatIndicatorHandler>().PlayerInput();
            }
            if (Input.GetButtonDown("Light Attack"))
            {
                attackID++;
                attackIDStart = attackID;
                QuickPunch();
                beatIndicator.GetComponent<beatIndicatorHandler>().PlayerInput();
            }
            if (Input.GetButtonDown("Super"))
            {
                attackID++;
                attackIDStart = attackID;
                SpecialPunch();
                beatIndicator.GetComponent<beatIndicatorHandler>().PlayerInput();
            }
            if (Input.GetButtonDown("Dodge"))
            {
                Dodge();
            }
            if (Input.GetButtonDown("Block"))
            {
                Block();
                beatIndicator.GetComponent<beatIndicatorHandler>().PlayerInput();
            }
        }
        if ((Input.GetButton("MoveRight") || Input.GetAxisRaw("Move Axis") > 0 || Input.GetAxisRaw("Move Axis 2") > 0) && !busy)
        {
            accX = 26f;
            direction = 1;
            localRenderer.flipX = false;
        }
        else if ((Input.GetButton("MoveLeft") || Input.GetAxisRaw("Move Axis") < 0 || Input.GetAxisRaw("Move Axis 2") < 0) && !busy)
        {
            accX = -26f;
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

    void UpdateAnimations()
    {
        animator.SetBool("dead", dead);
        //animator.SetBool("countering", countering);
        animator.SetBool("blocking", blocking);
        animator.SetBool("usingSuper", usingSuper);
        animator.SetBool("hit", hitstun);
        animator.SetBool("dodging", dodgeSucces);
        animator.SetFloat("speed", Mathf.Abs(velX));
        animator.SetInteger("attackType", attackType);
        if (comboState >= 0) animator.SetInteger("attackID", currentCombos[0][comboState].ID);
        animator.SetBool("attacking", punchingSuccess);
        animator.SetBool("failedAttack", punchingFail);
        animator.SetBool("attackActive", punchingActive);
        animator.SetBool("offBeat", mainHandler.offBeat);
    }

    void Punch()
    {
        accX = 0;
        velX = 0;
        attackType = 2;
        if (beatState == SUCCESS || beatState == BEAT)
        {
            if (blockBeat == currentBeat - 1)
            {
                CounterPunch();
                return;
            }

            UpdateHitboxes();
            lastAttackBeat = currentBeat;
            lastAttackSlow = true;
            punchingSuccess = true;
            busy = true;
            
            // update which combo is used
            for (int i = 0; i < currentCombos.Count; i++)
            {
                if (currentCombos[i][comboState + 1].type != SLOW)
                {
                    currentCombos.RemoveAt(i);
                    i--;
                }
            }

            if (currentCombos.Count == 0)
            {
                RestockCombos();

                for (int i = 0; i < currentCombos.Count; i++)
                {
                    if (currentCombos[i][comboState + 1].type != SLOW)
                    {
                        currentCombos.RemoveAt(i);
                        i--;
                    }
                }
            }

            comboState++;

            currentCombos[0][comboState].soundAttackHit.setParameterValue("Hit", 0);
            currentCombos[0][comboState].soundAttackHit.start();

            SuccessfulPunch();
        }
        else if (beatState == FAIL)
        {
            soundFail.start();
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
                DisableHurtboxes();
                if (currentCombos[0].Length - 1 == comboState) RestockCombos();
                
                actionTimer = 0;
                punchingSuccess = false;
                busy = false;
                beatPassed = false;
                punchingActive = false;
            }
        }
        else
        {

            if (!beatPassed)
            {
                if (beatState == FAIL) beatPassed = true;
            }
            else
            {
                if (beatState == SUCCESS)
                {
                    
                    currentCombos[0][comboState].soundAttackHit.setParameterValue("Hit", 1);
                    currentCombos[0][comboState].soundAttackHit.start();

                    currentCombos[0][comboState].hitbox.enabled = true;
                    transform.Translate(new Vector3(currentCombos[0][comboState].push * direction, 0));
                    

                    punchingActive = true;
                }
            }
        }

    }

    void FailedPunch()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= punchFailTime)
        {
            actionTimer = 0;
            punchingFail = false;
            busy = false;
        }
    }

    void QuickPunch()
    {
        accX = 0;
        velX = 0;
        attackType = 1;
        if (beatState == SUCCESS || beatState == BEAT)
        {
            if (blockBeat == currentBeat - 1)
            {
                CounterPunch();
                return;
            }

            UpdateHitboxes();
            lastAttackBeat = currentBeat;
            lastAttackSlow = false;
            punchingSuccess = true;
            busy = true;

            for (int i = 0; i < currentCombos.Count; i++)
            {
                if (currentCombos[i][comboState+1].type != QUICK)
                {
                    currentCombos.RemoveAt(i);
                    i--;
                }
            }

            if (currentCombos.Count == 0)
            {
                RestockCombos();

                for (int i = 0; i < currentCombos.Count; i++)
                {
                    if (currentCombos[i][comboState + 1].type != QUICK)
                    {
                        currentCombos.RemoveAt(i);
                        i--;
                    }
                }
            }

            comboState++;
            
            currentCombos[0][comboState].soundAttackHit.setParameterValue("Hit", 0);
            currentCombos[0][comboState].soundAttackHit.start();
            currentCombos[0][comboState].hitbox.enabled = true;
            transform.Translate(new Vector3(currentCombos[0][comboState].push * direction, 0));
            
            SuccessfulQuickPunch();
        }
        else if (beatState == FAIL)
        {
            soundFail.start();
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
            DisableHurtboxes();
            if (currentCombos[0].Length - 1 == comboState) RestockCombos();
            
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
            soundAttackSuperUnable.start();
            return;
        }
        accX = 0;
        velX = 0;
        attackType = 3;
        if (beatState == SUCCESS || beatState == BEAT)
        {
            Instantiate(effectHitSuper, transform.position + new Vector3(0, 1.5f), new Quaternion(0, 0, 0, 0));
            mainCamera.GetComponent<ScreenShake>().TriggerShake(0.8f, 0.5f, 1.2f);
            soundAttackSuper.start();
            //Instantiate(pSpecialAttack, transform.position, new Quaternion(0, 0, 0, 0));
            UpdateHitboxes();
            specialCharges--;
            lastAttackBeat = currentBeat;
            lastAttackSlow = false;
            punchingSuccess = true;
            busy = true;
            usingSuper = true;
            hitboxAttack3A.enabled = true;
            RestockCombos();
            SuccessfulSpecialPunch();
        }
        else if (beatState == FAIL)
        {
            soundFail.start();
            punchingFail = true;
            busy = true;
            FailedPunch();
        }
    }

    void SuccessfulSpecialPunch()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= specialPunchSuccessTime)
        {
            usingSuper = false;
            specialChargeTimer = 0;
            hitboxAttack3A.enabled = false;
            actionTimer = 0;
            punchingSuccess = false;
            busy = false;
            beatPassed = false;
            punchingActive = false;

        }
    }

    void CounterPunch()
    {
        mainCamera.GetComponent<ScreenShake>().TriggerShake(0.3f, 0.3f, 1.2f);
        Instantiate(pCounter, hitboxAttack4A.transform.position, new Quaternion(0, 0, 0, 0));
        UpdateHitboxes();
        punchingSuccess = true;
        busy = true;
        countering = true;
        attackType = 4;
        hitboxAttack4A.enabled = true;
        soundAttackSuper.start();
        transform.Translate(new Vector3(0.5f * direction, 0));
        SuccessfulCounterPunch();
    }

    void SuccessfulCounterPunch()
    {
        actionTimer += Time.deltaTime;

        print((actionTimer >= counterTime / 3) + " " + (attackID == attackIDStart));

        if (actionTimer >= counterTime / 3 && attackID == attackIDStart)
        {
            attackID++;
        }

        if (actionTimer >= counterTime)
        {
            hitboxAttack4A.enabled = false;
            actionTimer = 0;
            punchingSuccess = false;
            countering = false;
            busy = false;
            beatPassed = false;
            punchingActive = false;

        }
    }

    void BirdPunch()
    {
        accX = 0;
        velX = 0;
        hitboxAttackBird.enabled = false;
        birdHitReady = false;
        for (int i = 0; i < birds.Count; i++)
        {
            if (birds[i].GetComponent<enemyBirdHandler>().readyToBeHit)
            {
                birds[i].GetComponent<enemyBirdHandler>().setHit();
                break;
            }
        }
        birdHitting = true;

        hitboxBody.enabled = true;
        hitboxAttackBird.enabled = true;
        Instantiate(pBirdHit, transform);

        actionTimer = 0;
        punchingSuccess = false;
        punchingFail = false;
        usingSuper = false;
        dodgeFail = false;
        busy = true;
        beatPassed = false;
        punchingActive = false;
        DisableHurtboxes();
        RestockCombos();
        SuccessfulBirdPunch();
    }

    void SuccessfulBirdPunch()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= birdPunchSuccessTime)
        {
            hitboxAttackBird.enabled = false;
            RestockCombos();
            birdHitting = false;
            actionTimer = 0;
            punchingSuccess = false;
            busy = false;
            beatPassed = false;
            punchingActive = false;
        }
    }

    void Block()
    {
        accX = 0;
        velX = 0;
        attackType = 0;
        if (beatState == SUCCESS || beatState == BEAT)
        {
            //soundBlock.start();
            UpdateHitboxes();
            lastAttackBeat = currentBeat;
            blocking = true;
            busy = true;
            RestockCombos();
        }
        else if (beatState == FAIL)
        {
            soundFail.start();
            punchingFail = true;
            busy = true;
            FailedPunch();
        }
    }

    void SuccessfulBlock()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= blockTime)
        {
            actionTimer = 0;
            blocking = false;
            busy = false;
            beatPassed = false;
        }
    }

    void RestockCombos()
    {
        currentCombos = new List<Attack[]>(allCombos);
        comboState = -1;
    }

    void Hitstun()
    {
        hitboxBody.enabled = false;

        velX = (-3f - actionTimer * 5) * hitstunDirection;

        actionTimer += Time.deltaTime;

        if (actionTimer >= hitstunTime)
        {
            hitboxBody.enabled = true;
            actionTimer = 0;
            hitstun = false;
            busy = false;
            hitboxBody.enabled = true;
        }
    }

    void UpdateHitboxes()
    {
        foreach (BoxCollider2D x in GetComponentsInChildren<BoxCollider2D>())
        {
            x.offset = new Vector2(Mathf.Abs(x.offset.x) * direction, x.offset.y);
        }
    }

    void DisableHurtboxes()
    {
        for (int i = 0; i < currentCombos.Count; i++)
        {
            for (int j = 0; j < currentCombos[i].Length; j++)
            {
                currentCombos[i][j].hitbox.enabled = false;
            }
        }
        hitboxAttack3A.enabled = false;
        hitboxAttack4A.enabled = false;
    }

    void Dodge()
    {
        accX = 0;
        velX = 0;
        dodgeSucces = true;
        busy = true;
        if (Input.GetButton("MoveRight"))
        {
            direction = 1;
            localRenderer.flipX = false;
        }
        if (Input.GetButton("MoveLeft"))
        {
            direction = -1;
            localRenderer.flipX = true;
        }
        soundDodge.start();
        SuccessfulDodge();
    }

    void SuccessfulDodge()
    {
        hitboxBody.enabled = false;
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
            actionTimer = 0;
            dodgeSucces = false;
            busy = false;
        }
    }

    void FailedDodge()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= dodgeFailTime)
        {
            actionTimer = 0;
            dodgeFail = false;
            busy = false;
        }
    }

    public void TakeDamage(int dmg, int hitDirection, bool alwaysHit = false)
    {
        if (hitstun && !alwaysHit) return;
        
        if (blocking)
        {
            successfulBlock = true;
            
            Instantiate(pBlock, transform.position, new Quaternion(0, 0, 0, 0));
            //blockBeat = currentBeat;
            //actionTimer = 0;
        }
        else
        {
            streakTimer = 0;
            currentStreak = 0;
            streakLevel = 0;
            textStreak.enabled = false;

            hitstunDirection = hitDirection;

            currentHP -= dmg;

            GameObject tDmgNumber = Instantiate(damageNumberPlayer, transform.position + new Vector3(0, 0.7f), new Quaternion(0, 0, 0, 0));
            tDmgNumber.GetComponent<dmgNumberHandler>().Init(dmg);

            hitboxBody.enabled = false;

            velX = 0;
            accX = 0;

            actionTimer = 0;
            punchingSuccess = false;
            punchingFail = false;
            usingSuper = false;
            dodgeFail = false;
            busy = true;
            beatPassed = false;
            punchingActive = false;

            DisableHurtboxes();
            RestockCombos();

            Instantiate(pPlayerkHit, transform.position, new Quaternion(0, 0, 0, 0));

            if (currentHP <= 0) Die();
            else
            {
                hitstun = true;
            }
        }
        
    }

    public void Die()
    {
        soundDie.start();
        currentHP = 0;
        hitboxBody.enabled = false;
        dead = true;
    }

    public void Reset()
    {
        currentHP = maxHP;
        dead = false;
        specialCharges = maxSpecialCharges;
        busy = false;
        hitstun = false;
        RestockCombos();
        comboState = 0;
        currentCurrency = startingCurrency;
        hitboxBody.enabled = true;
        streakTimer = 0;
        streakLevel = 0;
        currentStreak = 0;
        textStreak.enabled = false;
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }

    public void SetBeatState(int state)
    {
        beatState = state;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        // pick up
        if (other.tag == "currency" && hitboxPickup.IsTouching(other))
        {
            soundPickupCurrency.start();
            currentCurrency++;
            Instantiate(pCurrencyPick, other.transform.position, new Quaternion(0, 0, 0, 0));
            GameObject.Destroy(other.gameObject);
        }

        if (other.tag == "HPPickup" && hitboxPickup.IsTouching(other))
        {
            soundPickupCurrency.start();
            currentHP += 5;
            if (currentHP > maxHP) currentHP = maxHP;
            Instantiate(pHPPickupPick, other.transform.position, new Quaternion(0, 0, 0, 0));
            GameObject.Destroy(other.gameObject);
        }

        // hit enemy
        if (other.tag.Contains("enemy"))
        {
            if (!other.GetComponent<enemyHandler>().CheckHit(attackID)) return;

            birdHitstun = false;
            int tDmg = 0;
            Vector3 tBox = new Vector3();

            if (comboState >= 0)
            {
                if (currentCombos[0][comboState].hitbox.IsTouching(other))
                {
                    currentCombos[0][comboState].soundAttackHit.setParameterValue("Hit", 2);
                    if (other.tag == "enemyDummy") currentCombos[0][comboState].soundAttackHit.setParameterValue("Material", 1);
                    currentCombos[0][comboState].soundAttackHit.start();
                    tDmg = currentCombos[0][comboState].damage;
                    tBox = (Vector2)currentCombos[0][comboState].hitbox.transform.position + currentCombos[0][comboState].hitbox.offset;
                }
            }

            if (hitboxAttack3A.IsTouching(other))
            {
                tDmg = 8;
                tBox = (Vector2)hitboxAttack3A.transform.position + hitboxAttack3A.offset;
            }

            if (hitboxAttack4A.IsTouching(other))
            {
                tDmg = 4;
                tBox = (Vector2)hitboxAttack4A.transform.position + hitboxAttack4A.offset;
            }

            if (hitboxAttackBird.IsTouching(other))
            {
                birdHitstun = true;
                tDmg = 1;
                tBox = (Vector2)hitboxAttackBird.transform.position + hitboxAttackBird.offset;
            }

            if (tDmg > 0)
            {
                if (other.tag == "enemy")
                {
                    other.GetComponent<enemyHandler>().TakeDamage(tDmg, attackID, birdHitstun);
                }

                // update streak
                if (mainHandler.staticLevel > 0)
                {
                    currentStreak++;
                    textStreak.enabled = true;
                    textStreak.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-4f, 4f));
                    streakTimer = 0;

                    if (currentStreak >= 100 && streakLevel != 6)
                    {
                        streakLevel = 6;
                        textStreak.color = new Color(0.3f, 1f, 0.9f);
                    }
                    if (currentStreak >= 75 && currentStreak < 100 && streakLevel != 5)
                    {
                        streakLevel = 5;
                        textStreak.color = new Color(0.74f, 0.31f, 1f);
                    }
                    if (currentStreak >= 50 && currentStreak < 75 && streakLevel != 4)
                    {
                        streakLevel = 4;
                        textStreak.color = new Color(0.1f, 1f, 0.1f);
                    }
                    else if (currentStreak >= 30 && currentStreak < 50 && streakLevel != 3)
                    {
                        streakLevel = 3;
                        textStreak.color = new Color(0.93f, 0.26f, 0.37f);
                    }
                    else if (currentStreak >= 10 && currentStreak < 30 && streakLevel != 2)
                    {
                        streakLevel = 2;
                        textStreak.color = new Color(0.85f, 0.85f, 0.4f);
                    }
                    else if (currentStreak >= 0 && currentStreak < 10 && streakLevel != 1)
                    {
                        streakLevel = 1;
                        textStreak.color = new Color(0.4f, 0.49f, 0.41f);
                    }
                }

                lastAttackHit = true;
                lastHitBeat = currentBeat;

                if (comboState == currentCombos[0].Length - 1)
                {
                    mainCamera.GetComponent<ScreenShake>().TriggerShake(0.07f + 0.027f * tDmg, 0.2f + 0.08f * tDmg, 1.2f);
                    Instantiate(effectHitRed, tBox, new Quaternion(0, 0, 0, 0));
                }
                else if (tDmg < 8 && !birdHitstun) Instantiate(effectHitBlue, tBox, new Quaternion(0, 0, 0, 0));

                //Instantiate(pAttackHit, tBox, new Quaternion(0, 0, 0, 0));
                GameObject tDmgNumber = Instantiate(damageNumberEnemy, other.transform.position + new Vector3(0, 0.7f), new Quaternion(0, 0, 0, 0));
                tDmgNumber.GetComponent<dmgNumberHandler>().Init(tDmg);
            }
        }

    }
}
