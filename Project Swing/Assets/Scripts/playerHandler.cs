﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;
using UnityEngine.UI;

public class playerHandler : MonoBehaviour
{
    [Header("_____Particle Systems_____")]
    public ParticleSystem pAttackHit;
    public ParticleSystem pPlayerkHit;
    public ParticleSystem pSpecialAttack;
    public ParticleSystem pHoldAttack;
    public ParticleSystem pBlock;
    public ParticleSystem pCounter;
    public ParticleSystem pBirdHit;
    public ParticleSystem pCurrencyPick;
    public ParticleSystem pHPPickupPick;
    public ParticleSystem pSuperCharged;

    [Header("_____Animated Effects_____")]
    public GameObject effectHitBlue;
    public GameObject effectHitRed;
    public GameObject effectHitSuper;
    public GameObject effectHitBird;
    public GameObject effectHitParry;

    [Header("_____Object Prefabs_____")]
    public GameObject damageNumberEnemy;
    public GameObject damageNumberPlayer;
    public GameObject scoreBonusText;

    [Header("_____Resources_____")]
    public Sprite spriteSPBar1;
    public Sprite spriteSPBar2;
    public Sprite spriteSuperFull;
    public Sprite spriteSuperCharging;

    //-----------------------------------hitboxes
    BoxCollider2D hitboxAttack1A;
    BoxCollider2D hitboxAttack1B;
    BoxCollider2D hitboxAttack1C;

    BoxCollider2D hitboxAttack2A;
    BoxCollider2D hitboxAttack2B;
    BoxCollider2D hitboxAttack2C;
    BoxCollider2D hitboxAttack2D;

    BoxCollider2D hitboxAttack3A;

    BoxCollider2D hitboxAttack4A;

    BoxCollider2D hitboxAttack5A;

    BoxCollider2D hitboxAttack6A;
    BoxCollider2D hitboxAttack6B;

    BoxCollider2D hitboxAttackBird;

    BoxCollider2D hitboxPickup;

    BoxCollider2D hitboxParry;

    BoxCollider2D hitboxBody;

    //--------------------------------local objects
    GameObject mainCamera;
    ScreenShake screenShake;
    GameObject beatIndicator;

    SpriteRenderer localRenderer;

    Animator animator;

    SpriteRenderer rendererHPFill;
    SpriteMask maskHPFill;
    SpriteRenderer[] renderersSpecialCharges;
    Image spriteScoreBg;
    Text textStreak;
    Text textCombo;
    Text textScore;
    Text textMultiplier;
    public Text textRank;
    SpriteMask maskSPFill;
    SpriteRenderer rendererSPFill;
    GameObject SPBar;
    
    //----------------------------------combat handling
    float punchSuccessTime = 0.32f;
    float quickPunchSuccessTime = 0.3f;
    float specialPunchSuccessTime = 0.7f;
    float rapidPunchSuccessTime = 0.2f;
    float punchFailTime = 0.5f;

    [HideInInspector] public bool punchingSuccess;
    [HideInInspector] public bool punchingActive;
    [HideInInspector] public int attackType;
    [HideInInspector] public bool enemyKilled;

    [HideInInspector] public List<GameObject> birds;

    bool punchingFail;
    bool usingSuper;
    
    bool holdBeatPassed;
    bool lastAttackHit;
    int lastHitBeat;
    bool heldButton;
    bool secondRapid;
    float bonusDisplayTimer;

    bool birdHitReady;
    bool birdHitPerfectReady;
    bool birdHitting;
    float birdPunchSuccessTime = 0.2f;
    bool birdHitstun;
    int birdInSequence;
    bool dpadV1;
    bool dpadV2;
    bool dpadH1;
    bool dpadH2;

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

    bool parrying;
    bool parryHitting;
    float parryTime;
    float parryHitTime;
   
    bool counterReady;
    float counterTime = 0.4f;
    bool countering;

    int comboState;
    int currentBeat;
    int lastAttackBeat;
    bool lastAttackSlow;
    bool switchBeat;

    List<Attack[]> allCombos;
    List<Attack[]> currentCombos;
    List<int> lastCombo;

    //----------------------------------player status
    int currentHP;
    int maxHP;
    int specialCharges;
    int maxSpecialCharges;
    float currentSP;
    float actionTimer;
    bool busy;

    float velX;
    float accX;

    [HideInInspector] public float maxSP = 20;
    [HideInInspector] public bool dead;

    [HideInInspector] public int direction;

    float specialChargeTimer;
    float specialChargeExtraTime;
    [HideInInspector] public int currentStreak;
    float streakDisappearDelay;
    float streakTimer;
    [HideInInspector] public int streakLevel;
    [HideInInspector] public int currentCurrency;
    int startingCurrency;
    bool chargingSP;
    [HideInInspector] public int currentScore;
    float currentMultiplier;
    [HideInInspector] public int currentRank;
    List<Vector2> previousAttacks;
    int lastDmg;
    int birdComboHeal;
    bool tookDamage;
    bool resetted;

    bool hitstun;
    int hitstunDirection;
    float hitstunTime = 0.32f;

    //--------------------------------level progression
    int localBPM;
    bool beatPassed;
    bool birdLevelFinished;
    bool normalLevelFinished;
    int beatState;

    //------------------------------------HUD
    bool SPSprite;
    float SPBarFlickerTimer;
    float SPBarFlickerTime = 0.07f;
    bool SPBarAvailable;
    
    //-----------------------------------save data
    PlayerData data;
    
    //-----------------------------------audio
    FMOD.Studio.EventInstance soundHitArmor;
    FMOD.Studio.EventInstance soundAttackSuper;
    FMOD.Studio.EventInstance soundAttackSuperUnable;

    FMOD.Studio.EventInstance soundHitWood;
    FMOD.Studio.EventInstance soundHitMetal;

    FMOD.Studio.EventInstance soundDodge;
    FMOD.Studio.EventInstance soundDie;
    FMOD.Studio.EventInstance soundFail;
    FMOD.Studio.EventInstance soundParry;
    FMOD.Studio.EventInstance soundEnemyBlock;
    FMOD.Studio.EventInstance soundSPBarFull;
    FMOD.Studio.EventInstance soundSuperGet;

    FMOD.Studio.EventInstance soundRankAnnouncer;

    FMOD.Studio.EventInstance soundPickupCurrency;
    FMOD.Studio.EventInstance soundPickupHP;

    // beat state
    readonly int FAIL = 0, SUCCESS = 1, BEAT = 2;

    // attack type (combos)
    readonly int QUICK = 0, SLOW = 1, HOLD = 2, RAPID = 3;

    // attack type (update selection)
    readonly int BLOCK = 0, QUICKATTACK = 1, SLOWATTACK = 2, SUPERATTACK = 3, COUNTERATTACK = 4, HOLDATTACK = 5, RAPIDATTACK = 6;

    // combos
    readonly int COMBOFLATTEN = 0, COMBOCHARGEPUNCH = 1, COMBORAPIDKICKS = 2, COMBOSUPER = 3, COMBOCOUNTERHIT = 4;
    
    void Awake()
    {
        data = SaveSystem.LoadPlayer();

        //txtVictory = GameObject.Find("txtVictory").GetComponent<Text>();

        animator = GetComponent<Animator>();

        rendererHPFill = GameObject.Find("HPFill").GetComponent<SpriteRenderer>();
        maskHPFill = GameObject.Find("HPFillMask").GetComponent<SpriteMask>();
        renderersSpecialCharges = new SpriteRenderer[3]
        {
            GameObject.Find("Charge1").GetComponent<SpriteRenderer>(),
            GameObject.Find("Charge2").GetComponent<SpriteRenderer>(),
            GameObject.Find("Charge3").GetComponent<SpriteRenderer>()
        };
        spriteScoreBg = GameObject.Find("ScoreBackground").GetComponent<Image>();
        textStreak = GameObject.Find("StreakCounter").GetComponent<Text>();
        textCombo = GameObject.Find("ComboCounter").GetComponent<Text>();
        textScore = GameObject.Find("ScoreCounter").GetComponent<Text>();
        textMultiplier = GameObject.Find("MultiplierCounter").GetComponent<Text>();
        textRank = GameObject.Find("RankDisplay").GetComponent<Text>();

        maskSPFill = GameObject.Find("SPBarMask").GetComponent<SpriteMask>();
        rendererSPFill = GameObject.Find("SPBarFill").GetComponent<SpriteRenderer>();
        SPBar = GameObject.Find("SPBar");

        hitboxAttack1A = transform.Find("AttackHitBox1A").GetComponent<BoxCollider2D>();
        hitboxAttack2A = transform.Find("AttackHitBox2A").GetComponent<BoxCollider2D>();
        hitboxAttack2B = transform.Find("AttackHitBox2B").GetComponent<BoxCollider2D>();
        hitboxAttack2C = transform.Find("AttackHitBox2C").GetComponent<BoxCollider2D>();
        hitboxAttack1B = transform.Find("AttackHitBox1B").GetComponent<BoxCollider2D>();
        hitboxAttack1C = transform.Find("AttackHitBox1C").GetComponent<BoxCollider2D>();
        hitboxAttack2D = transform.Find("AttackHitBox2D").GetComponent<BoxCollider2D>();
        hitboxAttack3A = transform.Find("AttackHitBox3A").GetComponent<BoxCollider2D>();
        hitboxAttack4A = transform.Find("AttackHitBox4A").GetComponent<BoxCollider2D>();
        hitboxAttack5A = transform.Find("AttackHitBox5A").GetComponent<BoxCollider2D>();
        hitboxAttack6A = transform.Find("AttackHitBox6A").GetComponent<BoxCollider2D>();
        hitboxAttack6B = transform.Find("AttackHitBox6B").GetComponent<BoxCollider2D>();

        hitboxAttackBird = transform.Find("AttackHitBoxBird").GetComponent<BoxCollider2D>();
        hitboxPickup = transform.Find("PickupHitbox").GetComponent<BoxCollider2D>();
        hitboxParry = transform.Find("ParryHitBox").GetComponent<BoxCollider2D>();

}

    void Start()
    {
        // load audio
        soundHitArmor = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/HitArmorDamage");
        soundAttackSuper = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_super");
        soundAttackSuperUnable = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_super_fail");
        soundDodge = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Dodge");
        soundDie = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Die");
        soundFail = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Miss_Brad");
        soundPickupCurrency = FMODUnity.RuntimeManager.CreateInstance("event:/Object/Pickup_gold_random");
        soundPickupHP = FMODUnity.RuntimeManager.CreateInstance("event:/Object/Pickup_hp");
        soundParry = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Block");
        soundEnemyBlock = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/HitArmor");
        soundSPBarFull = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/BarFull");
        soundSuperGet = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/SuperRe");

        soundRankAnnouncer = FMODUnity.RuntimeManager.CreateInstance("event:/Announcer/Voice");

        soundHitWood = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Boxingbag");
        soundHitMetal = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/MetalHit");

        mainCamera = GameObject.Find("Main Camera");
        beatIndicator = GameObject.Find("BeatIndicator");
        screenShake = GameObject.Find("Main Camera").GetComponent<ScreenShake>();
        if (mainHandler.currentGameMode == 1) beatIndicator.SetActive(false);

        allCombos = new List<Attack[]>();
        birds = new List<GameObject>();

        // populate available combos
        allCombos.Add(new Attack[] { new Attack2A(hitboxAttack2A), new Attack2B(hitboxAttack2B), new Attack2C(hitboxAttack2C) });
        allCombos.Add(new Attack[] { new Attack1A(hitboxAttack1A), new Attack1B(hitboxAttack1B) });
        
        data.Init();
        if (data.itemBought[COMBOFLATTEN] && data.itemActive[COMBOFLATTEN]) allCombos.Add(new Attack[] { new Attack1A(hitboxAttack1A), new Attack2D(hitboxAttack2D), new Attack1C(hitboxAttack1C) });
        if (data.itemBought[COMBOCHARGEPUNCH] && data.itemActive[COMBOCHARGEPUNCH]) allCombos.Add(new Attack[] { new Attack1A(hitboxAttack1A), new Attack5A(hitboxAttack5A) });
        if (data.itemBought[COMBORAPIDKICKS] && data.itemActive[COMBORAPIDKICKS]) allCombos.Add(new Attack[] { new Attack2A(hitboxAttack2A), new Attack2B(hitboxAttack2B), new Attack2C(hitboxAttack2C), new Attack6A(hitboxAttack6A), new Attack6B(hitboxAttack6B) });
        
        currentCombos = new List<Attack[]>();
        RestockCombos();

        // activate SP bar if relevant
        SPBarAvailable = true;
        /*
        if (data.itemBought[COMBOCHARGEPUNCH] || data.itemBought[COMBORAPIDKICKS]) SPBarAvailable = true;
        else
        {
            SPBarAvailable = false;
            SPBar.SetActive(false);
        }
        */

        lastCombo = new List<int>();
        lastCombo.Add(-1);
        lastCombo.Add(-1);

        // set init values
        maxHP = 15;
        currentHP = maxHP;
        maxSpecialCharges = 3;
        if (data.itemBought[COMBOSUPER] && data.itemActive[COMBOSUPER]) specialCharges = maxSpecialCharges;
        specialChargeExtraTime = 50;
        streakDisappearDelay = 3;
        direction = 1;
        blockTime = (60f / mainHandler.currentBpm) * 1.5f;
        parryTime = 1.5f;
        parryHitTime = 0.2f;
        blockBeat = -10;
        currentMultiplier = 1;
        currentSP = maxSP;
        lastDmg = 0;
        punchSuccessTime = (60f / mainHandler.currentBpm) * 0.75f;
        quickPunchSuccessTime = (60f / mainHandler.currentBpm) * 0.6f;
        specialPunchSuccessTime = (60f / mainHandler.currentBpm) * 1f;
        rapidPunchSuccessTime = (60f / mainHandler.currentBpm) * 0.5f;
        localBPM = mainHandler.currentBpm;
        normalLevelFinished = false;
        birdLevelFinished = false;
        tookDamage = false;

        textRank.text = "E";
        textMultiplier.enabled = false;
        if (mainHandler.staticLevel < 1 || mainHandler.staticLevel > 99) textRank.enabled = false;
        if (mainHandler.staticLevel < 1)
        {
            textScore.enabled = false;
            spriteScoreBg.enabled = false;
        }

        localRenderer = GetComponent<SpriteRenderer>();

        if (mainHandler.currentGameMode == 1) textCombo.enabled = true;

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
        
        // update special visuals
        for (int i = 2; i >= 0; i--)
        {
            if (specialCharges > i)
            {
                if (renderersSpecialCharges[i].sprite != spriteSuperFull)
                {
                    renderersSpecialCharges[i].sprite = spriteSuperFull;
                    renderersSpecialCharges[i].enabled = true;
                    renderersSpecialCharges[i].GetComponentInChildren<RectTransform>().localPosition = new Vector3(0, 0);
                    Instantiate(pSuperCharged, renderersSpecialCharges[i].transform.position + new Vector3(0, 0.4f), new Quaternion(0, 0, 0, 0));
                }
            }
            else if (specialCharges == i)
            {
                renderersSpecialCharges[i].sprite = spriteSuperCharging;
                renderersSpecialCharges[i].enabled = true;
                renderersSpecialCharges[i].gameObject.GetComponentInChildren<RectTransform>().localPosition = new Vector3(0, -0.82f + 0.82f * (specialChargeTimer / specialChargeExtraTime));
            }
            else renderersSpecialCharges[i].enabled = false;

        }

        if (currentStreak > 0 && mainHandler.currentGameMode != 1)
        {
            textStreak.text = "Streak: " + currentStreak;
            textStreak.transform.localScale = new Vector3((0.5f + streakLevel * 0.15f) - (streakTimer / streakDisappearDelay) * 0.5f, (0.5f + streakLevel * 0.15f) - (streakTimer / streakDisappearDelay) * 0.5f, 1);
        }
        if (mainHandler.currentGameMode == 1)
        {
            textCombo.text = "Combo: " + currentStreak;
        }

        // return if dead
        if (dead)
        {
            return;
        }

        // update SP and SP bar
        if (SPBarAvailable)
        {
            if (currentSP <= 0 && !chargingSP)
            {
                chargingSP = true;
            }
            if (chargingSP)
            {
                currentSP += 1.3f * Time.deltaTime;
                SPBarFlickerTimer += Time.deltaTime;
                if (SPBarFlickerTimer >= SPBarFlickerTime)
                {
                    SPBarFlickerTimer = 0;
                    if (SPSprite) rendererSPFill.sprite = spriteSPBar1;
                    else rendererSPFill.sprite = spriteSPBar2;
                    SPSprite = !SPSprite;
                }

            }
            if (currentSP < maxSP) currentSP += 1.7f * Time.deltaTime;
            if (currentSP > maxSP)
            {
                if (!normalLevelFinished && !birdLevelFinished) soundSPBarFull.start();
                currentSP = maxSP;
                chargingSP = false;
                rendererSPFill.sprite = spriteSPBar1;
            }
            maskSPFill.transform.localPosition = new Vector3(-2.5f + (currentSP / maxSP) * 2.5f, 0);
        }

        // update bonus score display
        if (bonusDisplayTimer > 0)
        {
            bonusDisplayTimer -= Time.deltaTime * 2.5f;
        }

        // update moves
        if (punchingSuccess)
        {
            if (attackType == QUICKATTACK) SuccessfulQuickPunch();
            else if (attackType == SLOWATTACK) SuccessfulPunch();
            else if (attackType == SUPERATTACK) SuccessfulSpecialPunch();
            else if (attackType == HOLDATTACK) SuccessfulHoldPunch();
            else if (attackType == RAPIDATTACK) SuccessfulRapidPunch();
        }
        if (punchingFail) FailedPunch();
        if (dodgeSucces) SuccessfulDodge();
        if (dodgeFail) FailedDodge();
        if (blocking) SuccessfulBlock();
        if (parryHitting) ParryHitting();
        else if (parrying) SuccessfulParry();
        if (countering) SuccessfulCounterPunch();
        if (birdHitting) SuccessfulBirdPunch();

        if (hitstun) Hitstun();
        
        // update special charges
        //if (specialCharges < 3 && data.itemBought[COMBOSUPER] && data.itemActive[COMBOSUPER]) specialChargeTimer += Time.deltaTime;

        if (specialChargeTimer >= specialChargeExtraTime)
        {
            soundSuperGet.setParameterValue("SuperPara", specialCharges - 1);
            soundSuperGet.start();
            specialCharges++;
            specialChargeTimer = 0;
        }

        // update streak
        if (mainHandler.currentGameMode != 1)
        {
            if (currentStreak > 0) streakTimer += Time.deltaTime;

            if (streakTimer >= streakDisappearDelay)
            {
                streakTimer = 0;
                streakLevel = 0;
                currentStreak = 0;
                birdComboHeal = 0;
                textStreak.enabled = false;
                textMultiplier.enabled = false;
            }
        }

        // update score
        textScore.text = currentScore.ToString();

        // update multiplier
        textMultiplier.text = "x " + currentMultiplier;

        // update rank
        if (currentRank < 5 && mainHandler.staticLevel < 100 && mainHandler.staticLevel > 0)
        {
            if (currentScore >= mainHandler.currentRankLimits[currentRank])
            {
                currentRank++;
                if (currentRank == 1)
                {
                    soundRankAnnouncer.setParameterValue("Announcer", 1);
                    if (mainHandler.currentGameMode != 1) soundRankAnnouncer.start();
                    textRank.text = "D";
                    textRank.color = new Color(0.57f, 0.6f, 0.91f);
                }
                if (currentRank == 2)
                {
                    soundRankAnnouncer.setParameterValue("Announcer", 2);
                    if (mainHandler.currentGameMode != 1) soundRankAnnouncer.start();
                    textRank.text = "C";
                    textRank.color = new Color(0.94f, 0.69f, 0.3f);
                }
                if (currentRank == 3)
                {
                    soundRankAnnouncer.setParameterValue("Announcer", 3);
                    if (mainHandler.currentGameMode != 1) soundRankAnnouncer.start();
                    textRank.text = "B";
                    textRank.color = new Color(0.2f, 0.76f, 1f);
                }
                if (currentRank == 4)
                {
                    soundRankAnnouncer.setParameterValue("Announcer", 4);
                    if (mainHandler.currentGameMode != 1) soundRankAnnouncer.start();
                    textRank.text = "A";
                    textRank.color = new Color(1f, 0.05f, 0.95f);
                }
                if (currentRank == 5)
                {
                    if (mainHandler.currentGameMode != 1) soundRankAnnouncer.setParameterValue("Announcer", 5);
                    else soundRankAnnouncer.setParameterValue("Announcer", 6);
                    soundRankAnnouncer.start();
                    textRank.text = "S";
                    if (mainHandler.currentGameMode == 1) textRank.text = "P";
                    textRank.color = new Color(1f, 0.9f, 0f);
                }
            }
        }
        
        // update bpm
        if (localBPM != mainHandler.currentBpm)
        {
            localBPM = mainHandler.currentBpm;
            punchSuccessTime = (60f / mainHandler.currentBpm) * 0.75f;
            quickPunchSuccessTime = (60f / mainHandler.currentBpm) * 0.6f;
            specialPunchSuccessTime = (60f / mainHandler.currentBpm) * 1f;
            rapidPunchSuccessTime = (60f / mainHandler.currentBpm) * 0.5f;
        }

        // update birds
        birdHitReady = false;
        for (int i = 0; i < birds.Count; i++)
        {
            bool removed = false;
            if (birds[i] == null)
            {
                //Destroy(birds[i]);
                birds.RemoveAt(i);
                i--;
                removed = true;
            }
            else if (birds[i].GetComponent<enemyBirdHandler>().readyToBeHit)
            {
                birdInSequence = 0;
                birdHitReady = true;
            }
            else if (birds[i].GetComponent<enemyBirdHandler>().startedCharge)
            {
                birds[i].GetComponent<enemyBirdHandler>().setOffset(birdInSequence);
                birdInSequence++;
            }

            if (!removed && birds[i].GetComponent<enemyBirdHandler>().perfectTiming)
            {
                birdHitPerfectReady = true;
            }
        }

        if (birdComboHeal >= 5)
        {
            birdComboHeal = 0;
            currentHP++;
            if (currentHP > maxHP) currentHP = maxHP;
        }
        
        if (mainHandler.normalLevelFinished && !normalLevelFinished)
        {
            beatIndicator.GetComponent<beatIndicatorHandlerB>().Clear();
            normalLevelFinished = true;
            if (currentHP == maxHP) AddBonusScore("Full HP", 2000, false, true);
            else AddBonusScore("HP Bonus", currentHP * 50, false, true);
            if (specialCharges > 0) AddBonusScore("Supers Left", specialCharges * 150, false, true);
            if (mainHandler.levelTimer <= 180) AddBonusScore("Time Bonus", 1000 - (int)((mainHandler.levelTimer / 180f) * 1000f), false, true);
        }

        if (mainHandler.birdLevelFinished && !birdLevelFinished)
        {
            GameObject tScoreBonus;
            birdLevelFinished = true;
            if (currentHP == maxHP && !tookDamage)
            {
                tScoreBonus = Instantiate(scoreBonusText, new Vector3(0, 0f), new Quaternion(0, 0, 0, 0));
                tScoreBonus.GetComponent<scoreTextHandler>().Init("PERFECT", 1000, 1);
                currentScore += 1000;
            }
            else
            {
                tScoreBonus = Instantiate(scoreBonusText, new Vector3(0, 0f), new Quaternion(0, 0, 0, 0));
                tScoreBonus.GetComponent<scoreTextHandler>().Init("Health Bonus", currentHP * 50);
                currentScore += 50 * currentHP;
            }
            if (currentRank != 5 && currentRank > 0)
            {
                if (currentRank < 5 && mainHandler.staticLevel < 100 && mainHandler.staticLevel > 0)
                {
                    if (currentScore >= mainHandler.currentRankLimits[currentRank])
                    {
                        currentRank++;
                        if (currentRank == 1)
                        {
                            textRank.text = "D";
                            textRank.color = new Color(0.57f, 0.6f, 0.91f);
                        }
                        if (currentRank == 2)
                        {
                            textRank.text = "C";
                            textRank.color = new Color(0.94f, 0.69f, 0.3f);
                        }
                        if (currentRank == 3)
                        {
                            textRank.text = "B";
                            textRank.color = new Color(0.2f, 0.76f, 1f);
                        }
                        if (currentRank == 4)
                        {
                            textRank.text = "A";
                            textRank.color = new Color(1f, 0.05f, 0.95f);
                        }
                        if (currentRank == 5)
                        {
                            textRank.text = "P";
                            textRank.color = new Color(1f, 0.9f, 0f);
                        }
                    }
                }
                if (currentRank < 5 && currentRank > 0)
                {
                    soundRankAnnouncer.setParameterValue("Announcer", currentRank);
                    soundRankAnnouncer.start();
                }
                else if (currentRank == 5)
                {
                    soundRankAnnouncer.setParameterValue("Announcer", 6);
                    soundRankAnnouncer.start();
                }
            }
            
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
                //blockBeat = currentBeat;
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
        if (Input.GetAxisRaw("Alternate Bird Axis") != 0)
        {
            if (dpadV1 == false)
            {
                dpadV2 = true;
                dpadV1 = true;
            }
            else dpadV2 = false;
        }
        if (Input.GetAxisRaw("Alternate Bird Axis") == 0)
        {
            dpadV2 = false;
            dpadV1 = false;
        }
        if (Input.GetAxisRaw("Move Axis") != 0)
        {
            if (dpadH1 == false)
            {
                dpadH2 = true;
                dpadH1 = true;
            }
            else dpadH2 = false;
        }
        if (Input.GetAxisRaw("Move Axis") == 0)
        {
            dpadH2 = false;
            dpadH1 = false;
        }
        if (birdHitReady)
        {

            // AUTOPLAY
            /*
            if (birdHitReady)
            {
                attackID++;
                attackIDStart = attackID;
                BirdPunch();
            }
            */
            /////////////

            if ((Input.GetButtonDown("Light Attack") || Input.GetButtonDown("Heavy Attack") || Input.GetButtonDown("Alternate Bird") || dpadV2 || dpadH2 || Input.GetButtonDown("Super") || Input.GetButtonDown("Other Action")))
            {
                attackID++;
                attackIDStart = attackID;
                BirdPunch();
            }
        }
        else if (mainHandler.currentGameMode == 1 && !birdLevelFinished)
        {
            if ((Input.GetButtonDown("Heavy Attack") || Input.GetButtonDown("Light Attack") || Input.GetButtonDown("Alternate Bird") || dpadV2 || dpadH2 || Input.GetButtonDown("Super") || Input.GetButtonDown("Other Action")) && !punchingFail && !resetted)
            {
                TakeDamage(1, 0, true);
                soundFail.start();
                punchingFail = true;
                busy = true;
                FailedPunch();
            }
        }
        if (resetted) resetted = false;

        if ((punchingActive || (lastAttackSlow == true && punchingSuccess == true)) && (Input.GetButtonDown("Heavy Attack") || Input.GetButtonDown("Light Attack")))
        {
            DisableHurtboxes();
            RestockCombos();

            soundAttackSuperUnable.start();
            //soundFail.start();
            holdBeatPassed = false;
            punchingFail = true;
            busy = true;
            actionTimer = 0;
            punchingSuccess = false;
            beatPassed = false;
            punchingActive = false;
            heldButton = false;
            FailedPunch();
        }

        if (!busy && mainHandler.songStarted && mainHandler.currentGameMode != 1 && !normalLevelFinished)
        {
            if (Input.GetButtonDown("Heavy Attack"))
            {
                attackID++;
                attackIDStart = attackID;
                Punch();
                beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput();
            }
            if (Input.GetButtonDown("Light Attack"))
            {
                attackID++;
                attackIDStart = attackID;
                QuickPunch();
                beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput();
            }
            if (Input.GetButtonDown("Super") && data.itemBought[COMBOSUPER] && data.itemActive[COMBOSUPER])
            {
                attackID++;
                attackIDStart = attackID;
                SpecialPunch();
                //beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput();
            }
            if (Input.GetButtonDown("Dodge"))
            {
                Dodge();
            }
            if (Input.GetButtonDown("Block"))
            {
                Parry();
                //Block();
                //beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput();
            }
        }

        if (((Input.GetButton("MoveRight") || Input.GetAxisRaw("Move Axis") > 0) && !busy) && mainHandler.currentGameMode != 1 && !normalLevelFinished)
        {
            accX = 26f;
            direction = 1;
            localRenderer.flipX = false;
        }
        else if (((Input.GetButton("MoveLeft") || Input.GetAxisRaw("Move Axis") < 0) && !busy) && mainHandler.currentGameMode != 1 && !normalLevelFinished)
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
        animator.SetBool("parrying", parrying);
        animator.SetBool("parryHit", parryHitting);
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
        animator.SetBool("victory", normalLevelFinished || birdLevelFinished);
    }

    void Punch()
    {
        accX = 0;
        velX = 0;
        attackType = SLOWATTACK;
        if (beatState == SUCCESS || beatState == BEAT)
        {
            if (blockBeat == currentBeat - 1 && data.itemBought[COMBOCOUNTERHIT] && data.itemActive[COMBOCOUNTERHIT])
            {
                CounterPunch();
                return;
            }

            UpdateHitboxes();
            lastAttackBeat = currentBeat;
            lastAttackSlow = true;
            punchingSuccess = true;
            busy = true;
            heldButton = true;

            // update which combo is used
            for (int i = 0; i < currentCombos.Count; i++)
            {
                if (currentCombos[i].Length > comboState + 1)
                {
                    if (currentCombos[i][comboState + 1].type != SLOW)
                    {
                        currentCombos.RemoveAt(i);
                        i--;

                    }
                }
                else
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

            beatIndicator.GetComponent<beatIndicatorHandlerB>().HeavyAttackRemove();
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
                CheckCombosEndOfCombo();
                
                actionTimer = 0;
                punchingSuccess = false;
                busy = false;
                beatPassed = false;
                punchingActive = false;
            }
        }
        else
        {
            if (!Input.GetButton("Heavy Attack")) heldButton = false;

            if (!beatPassed)
            {
                if (beatState == FAIL) beatPassed = true;
            }
            else
            {
                if (beatState == BEAT)
                {
                    bool holdAvailable = false;

                    for (int i = 0; i < currentCombos.Count; i++)
                    {
                        if (currentCombos[i].Length > comboState + 1)
                        {
                            if (currentCombos[i][comboState + 1].type == HOLD)
                            {
                                holdAvailable = true;
                                break;
                            }
                        }
                    }
                    
                    if (heldButton && holdAvailable && !chargingSP)
                    {
                        currentSP -= 10;
                        actionTimer = 0;
                        beatPassed = false;
                        holdBeatPassed = false;
                        HoldPunch();
                    }
                    else
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
        attackType = QUICKATTACK;
        if (beatState == SUCCESS || beatState == BEAT)
        {
            if (blockBeat == currentBeat - 1 && data.itemBought[COMBOCOUNTERHIT] && data.itemActive[COMBOCOUNTERHIT])
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
                if (currentCombos[i].Length > comboState + 1)
                {
                    if (currentCombos[i][comboState + 1].type != QUICK)
                    {
                        currentCombos.RemoveAt(i);
                        i--;

                    }
                }
                else
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
        // check for rapid attack
        if (Input.GetButtonDown("Light Attack") && actionTimer > 0 && beatState == FAIL)
        {
            for (int i = 0; i < currentCombos.Count; i++)
            {
                if (currentCombos[i].Length > comboState + 1)
                {
                    if (currentCombos[i][comboState + 1].type == RAPID && !chargingSP)
                    {
                        currentSP -= 5;
                        actionTimer = 0;
                        beatPassed = false;
                        holdBeatPassed = false;
                        DisableHurtboxes();
                        attackID++;
                        attackIDStart = attackID;
                        beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput(true);
                        RapidPunch();
                        break;
                    }
                }
            }
        }

        actionTimer += Time.deltaTime;

        // end attack
        if (actionTimer >= quickPunchSuccessTime)
        {
            DisableHurtboxes();
            CheckCombosEndOfCombo();
            
            actionTimer = 0;
            punchingSuccess = false;
            busy = false;
            beatPassed = false;
            punchingActive = false;
        }
    }

    void RapidPunch()
    {
        accX = 0;
        velX = 0;
        attackType = RAPIDATTACK;
        if (beatState == SUCCESS || beatState == BEAT || (beatState == FAIL && !secondRapid))
        {
            UpdateHitboxes();
            lastAttackBeat = currentBeat;
            lastAttackSlow = false;
            punchingSuccess = true;
            busy = true;

            for (int i = 0; i < currentCombos.Count; i++)
            {
                if (currentCombos[i].Length > comboState + 1)
                {
                    if (currentCombos[i][comboState + 1].type != RAPID)
                    {
                        currentCombos.RemoveAt(i);
                        i--;

                    }
                }
                else
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
                    if (currentCombos[i][comboState + 1].type != RAPID)
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

            SuccessfulRapidPunch();
        }
        else if (beatState == FAIL)
        {
            soundFail.start();
            punchingFail = true;
            busy = true;
            secondRapid = false;
            FailedPunch();
        }
    }

    void SuccessfulRapidPunch()
    {
        if (Input.GetButtonDown("Light Attack") && actionTimer > 0.01)
        {
            for (int i = 0; i < currentCombos.Count; i++)
            {
                if (currentCombos[i].Length > comboState + 1)
                {
                    if (currentCombos[i][comboState + 1].type == RAPID && !chargingSP)
                    {
                        currentSP -= 5;
                        actionTimer = 0;
                        beatPassed = false;
                        holdBeatPassed = false;
                        secondRapid = true;
                        DisableHurtboxes();
                        attackID++;
                        attackIDStart = attackID;
                        beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput();
                        RapidPunch();
                        break;
                    }
                }
            }
        }

        actionTimer += Time.deltaTime;

        if ((actionTimer >= quickPunchSuccessTime && !secondRapid) || (actionTimer >= rapidPunchSuccessTime && secondRapid))
        {
            DisableHurtboxes();
            CheckCombosEndOfCombo();

            actionTimer = 0;
            punchingSuccess = false;
            busy = false;
            beatPassed = false;
            punchingActive = false;
            secondRapid = false;
        }
    }

    void SpecialPunch()
    {
        if (specialCharges == 0)
        {
            soundAttackSuperUnable.start();
            beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput(false, true);
            return;
        }
        beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput();
        accX = 0;
        velX = 0;
        attackType = SUPERATTACK;
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
            //specialChargeTimer = 0;
            hitboxAttack3A.enabled = false;
            actionTimer = 0;
            punchingSuccess = false;
            busy = false;
            beatPassed = false;
            punchingActive = false;

        }
    }

    void HoldPunch()
    {
        accX = 0;
        velX = 0;
        attackType = HOLDATTACK;
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
                if (currentCombos[i].Length > comboState + 1)
                {
                    if (currentCombos[i][comboState + 1].type != HOLD)
                    {
                        currentCombos.RemoveAt(i);
                        i--;

                    }
                }
                else
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
                    if (currentCombos[i][comboState + 1].type != HOLD)
                    {
                        currentCombos.RemoveAt(i);
                        i--;
                    }
                }
            }

            comboState++;

            currentCombos[0][comboState].soundAttackHit.setParameterValue("Hit", 0);
            currentCombos[0][comboState].soundAttackHit.start();

            SuccessfulHoldPunch();
        }
        else if (beatState == FAIL)
        {
            soundFail.start();
            punchingFail = true;
            busy = true;
            FailedPunch();
        }

    }

    void SuccessfulHoldPunch()
    {
        if (punchingActive)
        {
            actionTimer += Time.deltaTime;

            if (actionTimer >= punchSuccessTime)
            {
                DisableHurtboxes();
                CheckCombosEndOfCombo();

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
                if (beatState == BEAT || beatState == SUCCESS)
                {
                    holdBeatPassed = true;
                    if (heldButton && Input.GetButtonUp("Heavy Attack"))
                    {
                        currentCombos[0][comboState].soundAttackHit.setParameterValue("Hit", 1);
                        currentCombos[0][comboState].soundAttackHit.start();

                        currentCombos[0][comboState].hitbox.enabled = true;
                        transform.Translate(new Vector3(currentCombos[0][comboState].push * direction, 0));

                        Instantiate(pHoldAttack, currentCombos[0][comboState].hitbox.transform.position + new Vector3(0.5f * direction, -0.4f), new Quaternion(0, Mathf.Clamp(direction, -1, 0) * 180, 0, 0));

                        beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput();

                        punchingActive = true;
                    }
                }

                if ((!Input.GetButton("Heavy Attack") || (beatState == FAIL && holdBeatPassed)) && !punchingActive)
                {
                    DisableHurtboxes();
                    RestockCombos();

                    soundFail.start();
                    holdBeatPassed = false;
                    punchingFail = true;
                    busy = true;
                    actionTimer = 0;
                    punchingSuccess = false;
                    beatPassed = false;
                    punchingActive = false;
                    heldButton = false;
                    FailedPunch();

                }
            }
            
        }

    }

    void CounterPunch()
    {
        mainCamera.GetComponent<ScreenShake>().TriggerShake(0.3f, 0.3f, 1.2f);
        //Instantiate(pCounter, hitboxAttack4A.transform.position, new Quaternion(0, 0, 0, 0));
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

        hitboxBody.enabled = false;

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
            hitboxBody.enabled = true;
        }
    }

    void BirdPunch()
    {
        accX = 0;
        velX = 0;
        hitboxAttackBird.enabled = false;
        birdHitReady = false;
        birdHitPerfectReady = false;
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
        DisableHurtboxes();
        hitboxAttackBird.enabled = true;
        Instantiate(effectHitBird, transform.position + new Vector3(0, 0.3f), new Quaternion(0, 0, 0, 0));
        currentScore += (int)Mathf.Round(100 * currentMultiplier);
        //print("100 * " + currentMultiplier + " = " + (int)Mathf.Round(100 * currentMultiplier));
        UpdateStreak(1);

        actionTimer = 0;
        punchingSuccess = false;
        punchingFail = false;
        usingSuper = false;
        dodgeFail = false;
        busy = true;
        beatPassed = false;
        punchingActive = false;
        hitboxBody.enabled = true;
        hitstun = false;
        dodgeSucces = false;
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
            soundParry.start();
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

    void Parry()
    {
        accX = 0;
        velX = 0;
        attackType = 0;
        parrying = true;
        soundParry.setParameterValue("Hit", 0);
        soundParry.start();
        hitboxParry.enabled = true;
        UpdateHitboxes();
        lastAttackBeat = currentBeat;
        busy = true;
        RestockCombos();
    }
    
    void SuccessfulParry()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= 0.2f)
        {
            hitboxParry.enabled = false;
        }

        if (actionTimer >= parryTime)
        {
            actionTimer = 0;
            parrying = false;
            hitboxParry.enabled = false;
            busy = false;
            beatPassed = false;
        }
    }

    void ParryHitting()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer >= parryHitTime)
        {
            actionTimer = 0;
            parrying = false;
            parryHitting = false;
            hitboxParry.enabled = false;
            busy = false;
            beatPassed = false;
        }
    }

    void RestockCombos()
    {
        currentCombos = new List<Attack[]>(allCombos);
        comboState = -1;
    }

    void CheckCombosEndOfCombo()
    {
        bool restock = true;
        for (int i = 0; i < currentCombos.Count; i++)
        {
            if (!(currentCombos[i].Length - 1 == comboState)) restock = false;
        }
        if (restock)
        {
            for (int i = 0; i < allCombos.Count; i++)
            {
                if (allCombos[i] == currentCombos[0])
                {
                    lastCombo.Add(i);
                    print("combo used: " + i);
                }
            }
            if (lastCombo[lastCombo.Count-1] != lastCombo[lastCombo.Count-2] && lastCombo[lastCombo.Count-1] != lastCombo[lastCombo.Count-3] && lastCombo[lastCombo.Count - 3] != -1 && lastDmg > 0)
            {
                AddBonusScore("combo variety", 100);
            }
            RestockCombos();
        }
    }

    void Hitstun()
    {
        hitboxBody.enabled = false;

        velX = (-3f - actionTimer * 5) * hitstunDirection;

        actionTimer += Time.deltaTime;

        if (actionTimer >= hitstunTime)
        {
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
        hitboxAttackBird.enabled = false;
        hitboxParry.enabled = false;
    }

    void Dodge()
    {
        if (!chargingSP)
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
            currentSP -= 5;
            soundDodge.start();
            SuccessfulDodge();
        }
        else soundAttackSuperUnable.start();
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
        if ((hitstun && !alwaysHit) || dead) return;
        
        if (blocking)
        {
            successfulBlock = true;

            blockBeat = currentBeat;
            
            Instantiate(pBlock, transform.position, new Quaternion(0, 0, 0, 0));
            //blockBeat = currentBeat;
            //actionTimer = 0;
        }
        else
        {
            tookDamage = true;
            streakTimer = 0;
            currentStreak = 0;
            textCombo.color = new Color(0.4f, 0.49f, 0.41f);
            birdComboHeal = 0;
            streakLevel = 0;
            textStreak.enabled = false;
            textMultiplier.enabled = false;
            
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
            birdHitting = false;
            secondRapid = false;
            holdBeatPassed = false;
            dodgeSucces = false;
            parrying = false;

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
        if (mainHandler.currentGameMode != 1) beatIndicator.GetComponent<beatIndicatorHandlerB>().Clear();
        hitboxBody.enabled = false;
        dead = true;
    }

    public void Reset()
    {
        normalLevelFinished = false;
        birdLevelFinished = false;
        if (mainHandler.currentGameMode != 1) beatIndicator.GetComponent<beatIndicatorHandlerB>().Restart();
        currentHP = maxHP;
        dead = false;
        if (data.itemBought[COMBOSUPER] && data.itemActive[COMBOSUPER]) specialCharges = maxSpecialCharges;
        busy = false;
        hitstun = false;
        RestockCombos();
        comboState = 0;
        birdInSequence = 0;
        currentCurrency = startingCurrency;
        hitboxBody.enabled = true;
        streakTimer = 0;
        tookDamage = false;
        streakLevel = 0;
        specialChargeTimer = 0;
        currentStreak = 0;
        birdComboHeal = 0;
        currentScore = 0;
        currentRank = 0;
        currentSP = maxSP;
        chargingSP = false;
        rendererSPFill.sprite = spriteSPBar1;
        currentMultiplier = 1;
        textRank.text = "E";
        textRank.color = new Color(0.65f, 0.65f, 0.65f);
        textMultiplier.enabled = false;
        textStreak.enabled = false;
        resetted = true;
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
        for (int i = 0; i < birds.Count; i++)
        {
            Destroy(birds[i]);
            birds.RemoveAt(i);
            i--;
        }
    }

    public void SetBeatState(int state)
    {
        beatState = state;
    }
    
    void UpdateStreak(int dmg)
    {
        // update streak
        if (mainHandler.staticLevel > 0)
        {
            if (dmg > 0) currentStreak++;
            if (mainHandler.currentGameMode == 1) birdComboHeal++;
            if (mainHandler.currentGameMode != 1)
            {
                textStreak.enabled = true;
                textStreak.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-4f, 4f));
            }
            streakTimer = 0;

            if (currentStreak >= 100 && streakLevel != 6)
            {
                currentMultiplier = 2;
                streakLevel = 6;
                textStreak.color = new Color(0.3f, 1f, 0.9f);
                textCombo.color = new Color(0.3f, 1f, 0.9f);
                textMultiplier.color = new Color(0.3f, 1f, 0.9f);
            }
            if (currentStreak >= 75 && currentStreak < 100 && streakLevel != 5)
            {
                currentMultiplier = 1.7f;
                streakLevel = 5;
                textStreak.color = new Color(0.74f, 0.31f, 1f);
                textCombo.color = new Color(0.74f, 0.31f, 1f);
                textMultiplier.color = new Color(0.74f, 0.31f, 1f);
            }
            if (currentStreak >= 50 && currentStreak < 75 && streakLevel != 4)
            {
                currentMultiplier = 1.5f;
                streakLevel = 4;
                textStreak.color = new Color(0.1f, 1f, 0.1f);
                textCombo.color = new Color(0.1f, 1f, 0.1f);
                textMultiplier.color = new Color(0.1f, 1f, 0.1f);
            }
            else if (currentStreak >= 30 && currentStreak < 50 && streakLevel != 3)
            {
                currentMultiplier = 1.3f;
                streakLevel = 3;
                textStreak.color = new Color(0.93f, 0.26f, 0.37f);
                textCombo.color = new Color(0.93f, 0.26f, 0.37f);
                textMultiplier.color = new Color(0.93f, 0.26f, 0.37f);
            }
            else if (currentStreak >= 10 && currentStreak < 30 && streakLevel != 2)
            {
                currentMultiplier = 1.1f;
                streakLevel = 2;
                textStreak.color = new Color(0.85f, 0.85f, 0.4f);
                textCombo.color = new Color(0.85f, 0.85f, 0.4f);
                textMultiplier.color = new Color(0.85f, 0.85f, 0.4f);
                textMultiplier.enabled = true;
            }
            else if (currentStreak >= 0 && currentStreak < 10 && streakLevel != 1)
            {
                currentMultiplier = 1;
                streakLevel = 1;
                textStreak.color = new Color(0.4f, 0.49f, 0.41f);
                textCombo.color = new Color(0.4f, 0.49f, 0.41f);
                textMultiplier.color = new Color(0.4f, 0.49f, 0.41f);
                textMultiplier.enabled = false;
            }
        }
    }

    void AddBonusScore(string text, int amount, bool notBonus = false, bool noMultiplier = false)
    {
        if (mainHandler.HUDTurnedOff) return;
        if (mainHandler.staticLevel > 0)
        {
            if (!notBonus)
            {
                GameObject tScoreBonus = Instantiate(scoreBonusText, new Vector3(0, 0f), new Quaternion(0, 0, 0, 0));
                if (!noMultiplier) tScoreBonus.GetComponent<scoreTextHandler>().Init(text, (int)(amount * currentMultiplier), bonusDisplayTimer);
                else tScoreBonus.GetComponent<scoreTextHandler>().Init(text, amount, bonusDisplayTimer);
                bonusDisplayTimer += 0.4f;
            }
            if (!noMultiplier) currentScore += (int)(amount * currentMultiplier);
            else currentScore += amount;

        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (dead) return;

        // pick up
        if (other.tag == "currency" && hitboxPickup.IsTouching(other))
        {
            //currentScore += (int)(5 * currentMultiplier);
            soundPickupCurrency.start();
            if (currentCurrency <= 99999) currentCurrency++;
            Instantiate(pCurrencyPick, other.transform.position, new Quaternion(0, 0, 0, 0));
            Destroy(other.gameObject);
        }

        if (other.tag == "HPPickup" && hitboxPickup.IsTouching(other))
        {
            soundPickupHP.start();
            currentHP += 5;
            if (currentHP > maxHP) currentHP = maxHP;
            Instantiate(pHPPickupPick, other.transform.position, new Quaternion(0, 0, 0, 0));
            Destroy(other.gameObject);
        }

        // hit enemy
        if (other.tag.Contains("enemy"))
        {
            // parry enemy
            if (hitboxParry.IsTouching(other))
            {
                if (other.GetComponent<enemyHandler>().parryable && other.GetComponent<enemyHandler>().direction == direction)
                {
                    GameObject tg = Instantiate(effectHitParry, (transform.position + other.transform.position)/2, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
                    mainCamera.GetComponent<ScreenShake>().TriggerShake(0.05f, 0.9f, 1.2f);

                    beatIndicator.GetComponent<beatIndicatorHandlerB>().PlayerInput();
                    blockBeat = currentBeat;
                    actionTimer = 0;
                    parryHitting = true;
                    other.GetComponent<enemyHandler>().GetParried();
                    soundParry.setParameterValue("Hit", 1);
                    //other.GetComponent<enemyHandler>().TakeDamage(4, 100);
                }
            }

            if (!other.GetComponent<enemyHandler>().CheckHit(attackID)) return;
            
            birdHitstun = false;
            int tDmg = 0;
            Vector3 tBox = new Vector3();

            if (comboState >= 0)
            {
                if (currentCombos[0][comboState].hitbox.IsTouching(other))
                {
                    currentCombos[0][comboState].soundAttackHit.setParameterValue("Hit", 2);
                    //if (other.tag == "enemyDummy") currentCombos[0][comboState].soundAttackHit.setParameterValue("Material", 1);
                    
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
                tDmg = 3;
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
                if (other.tag.Contains("enemy"))
                {
                    tDmg = other.GetComponent<enemyHandler>().TakeDamage(tDmg, attackID, birdHitstun, attackType);
                    lastDmg = tDmg;
                    if (chargingSP) currentSP += (Mathf.Clamp((float)tDmg / 2f, 0, 4));
                    if (usingSuper) AddBonusScore("Super Hit", 30);

                    // gain super charge
                    if (!usingSuper && data.itemBought[COMBOSUPER] && data.itemActive[COMBOSUPER] && specialCharges < 3) specialChargeTimer += tDmg;

                    if (comboState >= 0)
                    {
                        if (tDmg > 0)
                        {
                            if (other.GetComponent<enemyHandler>().material == 1) soundHitWood.start();
                            if (other.GetComponent<enemyHandler>().material == 2) soundHitMetal.start();
                            currentCombos[0][comboState].soundAttackHit.start();
                        }
                        else soundEnemyBlock.start();
                        if (other.GetComponent<enemyHandler>().GetDefense() > 0 && tDmg > 0) soundHitArmor.start();
                    }
                }

                if (!(tDmg == 0 && currentStreak == 0)) UpdateStreak(tDmg);

                lastAttackHit = true;
                lastHitBeat = currentBeat;

                if (comboState == currentCombos[0].Length - 1 && tDmg > 0)
                {
                    mainCamera.GetComponent<ScreenShake>().TriggerShake(0.07f + 0.027f * tDmg, 0.2f + 0.08f * tDmg, 1.2f);
                    Instantiate(effectHitRed, tBox, new Quaternion(0, 0, 0, 0));
                }
                else if (tDmg < 8 && tDmg > 0 && !birdHitstun)
                {
                    Instantiate(effectHitBlue, tBox, new Quaternion(0, 0, 0, 0));
                }

                AddBonusScore("", tDmg * 10, true);

                if (enemyKilled)
                {
                    enemyKilled = false;
                    if (comboState == currentCombos[0].Length - 1 && currentCombos.Count == 1) AddBonusScore("Finisher Kill", 100);
                    else AddBonusScore("Kill", 50);
                }

                //Instantiate(pAttackHit, tBox, new Quaternion(0, 0, 0, 0));
                GameObject tDmgNumber = Instantiate(damageNumberEnemy, other.transform.position + new Vector3(0, 0.7f), new Quaternion(0, 0, 0, 0));
                tDmgNumber.GetComponent<dmgNumberHandler>().Init(tDmg);
            }
        }

    }
}
