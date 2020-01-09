using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SynchronizerData;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public class mainHandler : MonoBehaviour {

    [Header("_____Level Data_____")]
    public int level;
    public int bpm;
    public float leniency = 0.1f;
    public int gameMode;
    public string musicPath;
    public string soundClockPath;

    [Header("______Object Prefabs_____")]
    public GameObject enemyA;
    public GameObject enemyB;
    public GameObject enemyC;
    public GameObject enemyD;
    public GameObject enemyBoxLight;
    public GameObject enemyBoxHeavy;
    public GameObject enemyParryPractice;
    public GameObject enemyBird;
    public GameObject currencyObject;
    

    [Header("______Resources_____")]
    public Sprite[] spriteCombos;
    public Sprite comboHolder;
    public Sprite comboBasicHeavy;
    public Sprite comboBasicLight;
    public Sprite spriteControlsClosed;
    public Sprite spriteControlsXbox;
    public Sprite spriteControlsPS;
    public Sprite spriteControlsKeyboard;

    //--------------------------------local objects
    Camera levelCamera;
    timerBox timerBox;
    EventSystem eventSystem;
    playerHandler player;

    // normal HUD
    Canvas levelUI;
    Canvas playerHUD;
 
    SpriteMask maskProgressFill;
    SpriteRenderer rendererProgressMarker;

    ResultScreen resultScreen;
    
    // endless HUD
    Text SecondCounter;
    Text SecondDisplay;
    Text txtRecordDisplay;
    Text txtRecordAnnouncement;

    // practice HUD
    Text txtSongName;
    Text txtSongBPM;
    Image imgControlsTraining;

    //------------------------------------level progression
    public static int staticLevel;
    public static int currentBpm;
    public static float currentLeniency;
    public static int currentGameMode;
    public static bool normalLevelFinished;
    public static bool birdLevelFinished;
    public static float levelTimer;
    public static int enemiesDead;
    public static float currentBeatTimer;
    public static int currentState;
    public static bool beatFrame;
    public static bool offBeat;
    public static bool songStarted;

    int currentSpawn;
    float nextSpawn;
    float prevTime;
    bool waitingForDeath;
    bool clockStarted;
    float victoryTimer;
    float beatTimer;
    bool songFinishedCallbackRecieved;

    [HideInInspector] public int initialCurrency;

    float offBeatTime;
    float preBeatTime;
    bool gotCool;
    bool currencyCounting;
    [HideInInspector] public bool gameOver;
    [HideInInspector] public int totalEnemies;
    [HideInInspector] public int totalBirds;
    

    //----------------------------------level data
    public static int[] currentRankLimits;

    EnemySpawn[] levelEndlessSpawn;
    EnemySpawn[] levelTrainingSpawn;
    EnemySpawn[] level1Spawn;
    EnemySpawn[] level2Spawn;
    EnemySpawn[] level3Spawn;
    EnemySpawn[] level4Spawn;
    EnemySpawn[] level5Spawn;
    EnemySpawn[] level1HardSpawn;
    EnemySpawn[] level2HardSpawn;
    EnemySpawn[] level3HardSpawn;
    EnemySpawn[] level4HardSpawn;
    EnemySpawn[] level5HardSpawn;
    EnemySpawn[] testSpawn;
    EnemySpawn[] currentLevelSpawn;
    float[] endWaitTimer;
    List<int[]> levelRankLimits;
    List<int[]> hardRankLimits;
    public List<int[]> birdRankLimits;
    float[] songLengths;
    float[] hardLevelTimeLimits;
    public float currentTimeLimit;
    int[] numBirds;
    
    List<GameObject> enemies;
    
    // practice mode data
    List<string> practiceSongNames;
    List<string> availablePracticeSongNames;
    List<int> practiceSongBPM;
    List<int> availablePracticeSongBPM;
    int currentSong;
    bool trainingControlsOpen;
    bool holdingRT;
    bool holdingLT;

    //---------------------------------HUD
    public static bool HUDTurnedOff;
    [HideInInspector] public bool moveResultScreen;

    //---------------------------------save data
    PlayerData data;

    [HideInInspector] public int endlessRecord;
    [HideInInspector] public bool[] clearedLevel;
    [HideInInspector] public bool[] clearedBirdLevel;
    [HideInInspector] public bool[] clearedHardLevel;
    [HideInInspector] public bool[] unlockedLevel;
    [HideInInspector] public bool[] unlockedBirdLevel;
    [HideInInspector] public bool[] unlockedHardLevel;
    [HideInInspector] public int[] streakRecord;
    [HideInInspector] public int[] comboRecord;
    [HideInInspector] public float[] timeRecord;
    [HideInInspector] public int streakLevelEndlessRecord;
    [HideInInspector] public int[] scoreRecord;
    [HideInInspector] public int[] scoreBirdRecord;
    [HideInInspector] public int[] scoreHardRecord;
    [HideInInspector] public int[] rankRecord;
    [HideInInspector] public int[] rankBirdRecord;
    [HideInInspector] public int[] rankHardRecord;
    [HideInInspector] public int currentMaxStreak;
    [HideInInspector] public int currency;
    [HideInInspector] public bool[] itemBought;
    [HideInInspector] public bool[] itemActive;
    [HideInInspector] public int lastMode;
    [HideInInspector] public bool seenTutorial;
    [HideInInspector] public bool seenBirdTutorial;
    [HideInInspector] public int inputSelected;
    public bool gotNewRecord;
    public int currentRecord;
    
    //---------------------------------audio
    FMOD.Studio.EventInstance soundMusic;
    FMOD.Studio.EventInstance soundAmbCafe;
    FMOD.Studio.EventInstance soundAmbSea;
    
    FMOD.Studio.EventInstance soundClock;
    FMOD.Studio.EventInstance soundUIClick;

    FMOD.Studio.EventInstance soundSinus;

    FMOD.Studio.EVENT_CALLBACK callBack;
    
    List<FMOD.Studio.EventInstance> soundPracticeSongs;
    List<FMOD.Studio.EventInstance> soundAvailableracticeSongs;
    
    // game modes
    readonly int NORMAL = 0, BIRD = 1, HARD = 2;

    // beat states
    readonly int FAIL = 0, SUCCESS = 1, BEAT = 2, OFFBEAT = 3;

    void Awake()
    {
        // set static variables
        currentBpm = bpm;
        currentLeniency = leniency;
        staticLevel = level;

        // load selected game mode (if available)
        if (sceneSelectionHandler.Instance != null)
        {
            if (level < 100 && level > 0) gameMode = sceneSelectionHandler.Instance.gameModeSelected;
            else gameMode = NORMAL;
        }
        currentGameMode = gameMode;

        // load save data
        data = SaveSystem.LoadPlayer();
        
        // scene object references
        levelCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        player = GameObject.Find("Player").GetComponent<playerHandler>();
        resultScreen = FindObjectOfType<ResultScreen>();

        // endless mode object references
        if (level == 100)
        {
            SecondCounter = GameObject.Find("SecondCounter").GetComponent<Text>();
            SecondDisplay = GameObject.Find("SecondDisplay").GetComponent<Text>();
            txtRecordDisplay = GameObject.Find("RecordDisplay").GetComponent<Text>();
            txtRecordAnnouncement = GameObject.Find("RecordAnnouncement").GetComponent<Text>();
        }

        timerBox = GameObject.Find("TimerBox").GetComponent<timerBox>();

        // practice mode object references
        if (level == -1)
        {
            txtSongName = GameObject.Find("txtSongName").GetComponent<Text>();
            txtSongBPM = GameObject.Find("txtBpm").GetComponent<Text>();
            imgControlsTraining = GameObject.Find("imgControls").GetComponent<Image>();
        }
        else
        {
            maskProgressFill = GameObject.Find("ProgressBarMask").GetComponent<SpriteMask>();
            rendererProgressMarker = GameObject.Find("ProgressBarMarker").GetComponent<SpriteRenderer>();
        }

        if (level > 0) levelUI = GameObject.Find("LevelUI").GetComponent<Canvas>();
        playerHUD = GameObject.Find("playerHUD").GetComponent<Canvas>();
    }
    
    void Start()
    {
        // set music
        if (gameMode == BIRD && level < 100) soundMusic = FMODUnity.RuntimeManager.CreateInstance(musicPath.Insert(12, "/Bird") + "_bird");
        else soundMusic = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        offBeatTime = 60 / ((float)bpm * 2);
        preBeatTime = 60 / (float)bpm - leniency;
        
        callBack = new FMOD.Studio.EVENT_CALLBACK(StudioEventCallback);
        soundMusic.setCallback(callBack, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        if (level != -1) soundMusic.start();
        
        soundClock = FMODUnity.RuntimeManager.CreateInstance(soundClockPath);

        soundAmbCafe = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Amb_cafe");
        soundAmbSea = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Amb_sea");

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");

        soundSinus = FMODUnity.RuntimeManager.CreateInstance("event:/Sinus_test");
        
        enemies = new List<GameObject>();

        eventSystem = EventSystem.current;

        // load data
        data.Init();
        endlessRecord = data.endlessRecord;
        clearedLevel = data.clearedLevel;
        clearedBirdLevel = data.clearedBirdLevel;
        clearedHardLevel = data.clearedHardLevel;
        unlockedLevel = data.unlockedLevel;
        unlockedBirdLevel = data.unlockedBirdLevel;
        unlockedHardLevel = data.unlockedHardLevel;
        streakRecord = data.streakRecord;
        comboRecord = data.comboRecord;
        timeRecord = data.timeRecord;
        streakLevelEndlessRecord = data.streakLevelEndlessRecord;
        scoreRecord = data.scoreRecord;
        scoreBirdRecord = data.scoreBirdRecord;
        scoreHardRecord = data.scoreHardRecord;
        rankRecord = data.rankRecord;
        rankBirdRecord = data.rankBirdRecord;
        rankHardRecord = data.rankHardRecord;
        currency = data.currency;
        itemBought = data.itemBought;
        itemActive = data.itemActive;
        lastMode = data.lastMode;
        seenTutorial = data.seenTutorial;
        seenBirdTutorial = data.seenBirdTutorial;
        inputSelected = data.inputSelected;

        player.Init(currency);
        
        // default values
        currentSpawn = 0;
        levelTimer = 0;
        enemiesDead = 0;
        victoryTimer = 0;
        totalEnemies = 0;
        clockStarted = false;
        prevTime = 0;
        currentMaxStreak = 0;
        gameOver = false;
        songStarted = false;
        waitingForDeath = false;
        initialCurrency = player.currentCurrency;
        birdLevelFinished = false;
        normalLevelFinished = false;
        gotNewRecord = false;
        //player.Reset();

        // load practice
        if (level == -1)
        {
            InitPracticeMode();
        }

        // spawn data
        #region level spawns
        levelTrainingSpawn = new EnemySpawn[] { };
        
        levelEndlessSpawn = new EnemySpawn[] {
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }, new bool[] { false }),
            new EnemySpawn(4, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }, new bool[] { false }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }, new bool[] { false }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }, new bool[] { false }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 0 }, new bool[] { true }),
            new EnemySpawn(3, new GameObject[] { enemyB }, new float[] { 10 * RandDirection() }, new bool[] { false }),
            new EnemySpawn(3, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }, new bool[] { false }),
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 2 }, new bool[] { true }),
            new EnemySpawn(4, new GameObject[] { enemyB }, new float[] { -2 }, new bool[] { true }),
            new EnemySpawn(8, new GameObject[] { enemyA, enemyA }, new float[] { 10 * RandDirection(), 10 * RandDirection() }, new bool[] { false, false }),
            new EnemySpawn(8, new GameObject[] { enemyA, enemyA, enemyB }, new float[] { 10 * RandDirection(), 10 * RandDirection(), 0 }, new bool[] { false, false, true }),
            new EnemySpawn(3, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }, new bool[] { false })
        };

        level1Spawn = new EnemySpawn[]
        {
            new EnemySpawn(2f, new GameObject[] { enemyBoxLight }, new float[] { 2.5f }, new bool[] { false }, -1, true ),
            new EnemySpawn(0f, new GameObject[] { enemyA }, new float[] { -13f }, new bool[] { false, false }, 0, false ),
            new EnemySpawn(0f, new GameObject[] { enemyA }, new float[] { 10 }, new bool[] { false } ),
            new EnemySpawn(3f, new GameObject[] { enemyParryPractice }, new float[] { -4 }, new bool[] { false } ),
            new EnemySpawn(3f, new GameObject[] { enemyBoxHeavy }, new float[] { 3f }, new bool[] { false }, 1, true ),
            new EnemySpawn(0f, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false } ),
            new EnemySpawn(3f, new GameObject[] { enemyBoxLight }, new float[] { 4 }, new bool[] { false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyBoxLight }, new float[] { 0 }, new bool[] { false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyBoxLight }, new float[] { -4 }, new bool[] { false } ),
            new EnemySpawn(3f, new GameObject[] { enemyA }, new float[] { 10 }, new bool[] { false }, 1 ),
            new EnemySpawn(2f, new GameObject[] { enemyBoxHeavy, enemyBoxHeavy }, new float[] { -2.5f, 2.5f }, new bool[] { false, false }, 0, true ),
            new EnemySpawn(0f, new GameObject[] { enemyA, enemyA }, new float[] { -10f, 10f }, new bool[] { false, false } ),
        };

        level2Spawn = new EnemySpawn[] {
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 10 }, new bool[] { false }, 0 ),
            new EnemySpawn(0, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }),
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 10 }, new bool[] { false }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { 0 }, new bool[] { true }, 1),
            //new EnemySpawn(0, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }, new bool[] { false, false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyB }, new float[] { 10 }, new bool[] { false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }, new bool[] { false, false }, 1),
            //new EnemySpawn(5, new GameObject[] { enemyA, enemyA }, new float[] { -2, 2 }, new bool[] { true, true }, 1),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyA }, new float[] { -10, 7 }, new bool[] { false, true }),
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyB }, new float[] { -10, 10 }, new bool[] { false, false })
        };
        
        level3Spawn = new EnemySpawn[] {
            new EnemySpawn(7, new GameObject[] { enemyA, enemyB }, new float[] { -8, 12 }, new bool[] { true, false }),
            new EnemySpawn(8, new GameObject[] { enemyB, enemyA }, new float[] { -12, 7 }, new bool[] { false, true }, 0),
            new EnemySpawn(0, new GameObject[] { enemyA }, new float[] { 0 }, new bool[] { true }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { -2 }, new bool[] { true }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { 2 }, new bool[] { true }),
            new EnemySpawn(2, new GameObject[] { enemyA, enemyA }, new float[] { 10, 11 }, new bool[] { false, false }),
            new EnemySpawn(2, new GameObject[] { enemyA, enemyA }, new float[] { -4, -11 }, new bool[] { true, false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyC }, new float[] { 0 }, new bool[] { true }, 0),
            new EnemySpawn(0, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 7 }, new bool[] { true }),
            new EnemySpawn(4, new GameObject[] { enemyC }, new float[] { 12 }, new bool[] { false }),
            new EnemySpawn(3, new GameObject[] { enemyB }, new float[] { -10 }, new bool[] { false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyA, enemyC, enemyA }, new float[] { -6, -10,  11, 7}, new bool[] { true, false, false, true }, 2),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA }, new float[] { -11, 11}, new bool[] { false, false }),
        };
        
        level4Spawn = new EnemySpawn[] {
            new EnemySpawn(10, new GameObject[] { enemyC, enemyC }, new float[] { -11, 11 }, new bool[] { false, false }, 1),
            new EnemySpawn(2, new GameObject[] { enemyA, enemyA }, new float[] { -2, 2 }, new bool[] { true, true }),
            new EnemySpawn(4, new GameObject[] { enemyA, enemyB }, new float[] { -6, 12 }, new bool[] { true, false }, 1),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { -10, -12, -14 }, new bool[] { false, false, false }),
            new EnemySpawn(3, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { 10, 12, 14 }, new bool[] { false, false, false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyD, enemyD }, new float[] { 10, -10 }, new bool[] { false, false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyA, enemyB, enemyA, enemyA }, new float[] { -10, -11, -12, 8, 5 }, new bool[] { false, false, false, true, true }, 1),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA }, new float[] { 9, -9 }, new bool[] { true, true }),
            new EnemySpawn(3, new GameObject[] { enemyC, enemyB }, new float[] { -4, -7 }, new bool[] { true, true }),
            new EnemySpawn(8, new GameObject[] { enemyD, enemyB }, new float[] { 4, 7 }, new bool[] { true, true }, 0),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA }, new float[] { 10, 11, 12, 13, 14, 15, -10, -11, -12, -13, -14, -15, 0 }, new bool[] { false, false,false,false,false,false,false,false,false,false,false,false,true })
        };

        level5Spawn = new EnemySpawn[] {
            new EnemySpawn(8, new GameObject[] { enemyA, enemyC }, new float[] { -4, 11 }, new bool[] { true, false }),
            new EnemySpawn(5, new GameObject[] { enemyA, enemyA }, new float[] { -11, -13 }, new bool[] { false, false }),
            new EnemySpawn(2, new GameObject[] { enemyA, enemyA }, new float[] { 11, 13 }, new bool[] { false, false }),
            new EnemySpawn(6, new GameObject[] { enemyB, enemyC }, new float[] { -2, 7 }, new bool[] { true, true }, 1),
            new EnemySpawn(0, new GameObject[] { enemyD, enemyA }, new float[] { -13, 2 }, new bool[] { false, true }),
            new EnemySpawn(3, new GameObject[] { enemyA, enemyB }, new float[] { 0, 15 }, new bool[] { true, false }, 1),
            new EnemySpawn(1, new GameObject[] { enemyD, enemyD }, new float[] { -2, 2 }, new bool[] { true, true }),
            new EnemySpawn(3, new GameObject[] { enemyB, enemyB }, new float[] { -11, 11 }, new bool[] { false, false }),
            new EnemySpawn(6, new GameObject[] { enemyC, enemyC }, new float[] { 13, 15 }, new bool[] { true, false }, 1),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { -6 }, new bool[] { true }),
            new EnemySpawn(2, new GameObject[] { enemyD }, new float[] { 5 }, new bool[] { true }),
            new EnemySpawn(2, new GameObject[] { enemyC }, new float[] { -2 }, new bool[] { true }),
            new EnemySpawn(2, new GameObject[] { enemyC }, new float[] { 3 }, new bool[] { true }),
            new EnemySpawn(8, new GameObject[] { enemyB, enemyB }, new float[] { -11, 11 }, new bool[] { false, false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyC, enemyD, enemyA, enemyA, enemyD, enemyC, enemyB }, new float[] { -8, -6, -4, -2, 2, 4, 6, 8 }, new bool[] { true, true, true, true, true, true, true, true }),
        };

        level1HardSpawn = new EnemySpawn[] {
            new EnemySpawn(2f, new GameObject[] { enemyBoxLight, enemyBoxLight, enemyBoxLight }, new float[] { -8, -1, 6  }, new bool[] { false, false, false } ),
            new EnemySpawn(1f, new GameObject[] { enemyBoxHeavy, enemyBoxHeavy, enemyBoxHeavy }, new float[] { -5, 3, 8  }, new bool[] { false, false, false } ),
            new EnemySpawn(1, new GameObject[] { enemyA, enemyD, enemyA, enemyD }, new float[] { -10, -14, 10, 14 }, new bool[] { false, false, false, false } ),
            new EnemySpawn(4, new GameObject[] { enemyB, enemyB }, new float[] { -1, 1 }, new bool[] { true, true }, 1 ),
            new EnemySpawn(0, new GameObject[] { enemyParryPractice }, new float[] { -6 }, new bool[] { false } ),
            new EnemySpawn(0.1f, new GameObject[] { enemyC }, new float[] { -3 }, new bool[] { true } ),
            new EnemySpawn(0.2f, new GameObject[] { enemyParryPractice }, new float[] { 2 }, new bool[] { false } ),
            new EnemySpawn(1.3f, new GameObject[] { enemyParryPractice }, new float[] { 5 }, new bool[] { false } ),
            new EnemySpawn(0.1f, new GameObject[] { enemyC }, new float[] { 0 }, new bool[] { true } ),
            new EnemySpawn(0.8f, new GameObject[] { enemyParryPractice }, new float[] { -1 }, new bool[] { false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyParryPractice }, new float[] { -8 }, new bool[] { false } ),
            new EnemySpawn(1.7f, new GameObject[] { enemyParryPractice }, new float[] { 8.5f }, new bool[] { false } ),
            new EnemySpawn(0.2f, new GameObject[] { enemyA, enemyA, enemyA, enemyA }, new float[] { -10, -14, 10, 14 }, new bool[] { false, false, false, false }, 0 ),
            new EnemySpawn(0f, new GameObject[] { enemyBoxLight }, new float[] { -8f }, new bool[] { false } ),
            new EnemySpawn(0.211f, new GameObject[] { enemyBoxLight }, new float[] { -6f }, new bool[] { false } ),
            new EnemySpawn(0.211f, new GameObject[] { enemyBoxLight }, new float[] { -4f }, new bool[] { false } ),
            new EnemySpawn(0.211f, new GameObject[] { enemyBoxLight }, new float[] { -2f }, new bool[] { false } ),
            new EnemySpawn(0.211f, new GameObject[] { enemyBoxLight }, new float[] { 0f }, new bool[] { false } ),
            new EnemySpawn(0.211f, new GameObject[] { enemyBoxLight }, new float[] { 2f }, new bool[] { false } ),
            new EnemySpawn(0.211f, new GameObject[] { enemyBoxLight }, new float[] { 4f }, new bool[] { false } ),
            new EnemySpawn(0.211f, new GameObject[] { enemyBoxLight }, new float[] { 6f }, new bool[] { false } ),
            new EnemySpawn(0.211f, new GameObject[] { enemyBoxLight }, new float[] { 8f }, new bool[] { false } ),

        };

        level2HardSpawn = new EnemySpawn[] {
            new EnemySpawn(5, new GameObject[] { enemyC, enemyC, enemyB }, new float[] { -7, 7, 0.2f }, new bool[] { true, true, true } ),
            new EnemySpawn(3, new GameObject[] { enemyD, enemyD }, new float[] { -12, 12 }, new bool[] { false, false } ),
            new EnemySpawn(3, new GameObject[] { enemyB }, new float[] { -4 }, new bool[] { true } ),
            new EnemySpawn(2, new GameObject[] { enemyC }, new float[] { 2 }, new bool[] { true }, 2),
            new EnemySpawn(1, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { -8, -5, -2 }, new bool[] { true, true, true } ),
            new EnemySpawn(4, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { 8, 5, 2 }, new bool[] { true, true, true } ),
            new EnemySpawn(5, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { -4, 0, 4 }, new bool[] { true, true, true } ),
            new EnemySpawn(3, new GameObject[] { enemyD, enemyD }, new float[] { -11, 11 }, new bool[] { false, false }, 2),
            new EnemySpawn(1, new GameObject[] { enemyB, enemyB }, new float[] { -8, -5 }, new bool[] { true, true } ),
            new EnemySpawn(3, new GameObject[] { enemyA }, new float[] { 11 }, new bool[] { false } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 12 }, new bool[] { false } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { 13 }, new bool[] { false } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 11 }, new bool[] { false } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { 12 }, new bool[] { false } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 11 }, new bool[] { false }, 2 ),
            new EnemySpawn(0, new GameObject[] { enemyC, enemyC,  }, new float[] { -1, 1 }, new bool[] { true, true } ),
        };

        level3HardSpawn = new EnemySpawn[] {
            new EnemySpawn(7, new GameObject[] { enemyD, enemyD }, new float[] { -12, 6 }, new bool[] { false, true } ),
            new EnemySpawn(3, new GameObject[] { enemyC }, new float[] { 12 }, new bool[] { false } ),
            new EnemySpawn(2, new GameObject[] { enemyD }, new float[] { -2 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { 12 }, new bool[] { false } ),
            new EnemySpawn(1, new GameObject[] { enemyC, enemyD }, new float[] { -5, -13 }, new bool[] { true, false } ),
            new EnemySpawn(3, new GameObject[] { enemyD, enemyC }, new float[] { 2, 15 }, new bool[] { true, false } ),
            new EnemySpawn(2, new GameObject[] { enemyD }, new float[] { -11 }, new bool[] { false }, 2 ),
            new EnemySpawn(1, new GameObject[] { enemyB, enemyD, enemyD, enemyD, enemyD }, new float[] { 0, 11, 12, -11, -12 }, new bool[] { true, false, false, false, false }, 1 ),
            new EnemySpawn(0, new GameObject[] { enemyA }, new float[] { -8 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -6 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -4 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -2 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -0 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 2 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 4 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 6 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 8 }, new bool[] { true }, 3 ),
            new EnemySpawn(0, new GameObject[] { enemyC, enemyD }, new float[] { -9, 9 }, new bool[] { true, true } ),
            new EnemySpawn(3, new GameObject[] { enemyC, enemyD }, new float[] { -1, 1 }, new bool[] { true, true } ),
            new EnemySpawn(1, new GameObject[] { enemyB, enemyB }, new float[] { -12, 12 }, new bool[] { false, false }, 1 ),
            new EnemySpawn(0, new GameObject[] { enemyD }, new float[] { 8 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { 6 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { 4 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { 2 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { 0 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { -2 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { -4 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { -6 }, new bool[] { true } ),
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { -8 }, new bool[] { true } ),
            new EnemySpawn(4, new GameObject[] { enemyB, enemyD, enemyB, enemyD }, new float[] { -7, -3, 7, 3 }, new bool[] { true, true, true, true } ),

        };

        level4HardSpawn = new EnemySpawn[] {
            new EnemySpawn(10, new GameObject[] { enemyB, enemyB }, new float[] { -4, 4 }, new bool[] { true, true } ),
            new EnemySpawn(4, new GameObject[] { enemyB, enemyB }, new float[] { -7, 7 }, new bool[] { true, true } ),
            new EnemySpawn(4, new GameObject[] { enemyB, enemyB }, new float[] { -10, 10 }, new bool[] { true, true }, 1 ),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyC }, new float[] { -12, 0 }, new bool[] { false, true } ),
            new EnemySpawn(4, new GameObject[] { enemyB, enemyC }, new float[] { 12, 0 }, new bool[] { false, true }, 1 ),
            new EnemySpawn(0, new GameObject[] { enemyD, }, new float[] { -12, 0 }, new bool[] { false } ),
            new EnemySpawn(1, new GameObject[] { enemyD, enemyB }, new float[] { -12, -9 }, new bool[] { false, true } ),
            new EnemySpawn(1, new GameObject[] { enemyD, }, new float[] { -12 }, new bool[] { false } ),
            new EnemySpawn(1, new GameObject[] { enemyD, enemyD }, new float[] { -12, -8 }, new bool[] { false, true } ),
            new EnemySpawn(1, new GameObject[] { enemyD, }, new float[] { -12 }, new bool[] { false } ),
            new EnemySpawn(1, new GameObject[] { enemyD, enemyD }, new float[] { -12, -9 }, new bool[] { false, true } ),
            new EnemySpawn(1, new GameObject[] { enemyD, }, new float[] { -12, 0 }, new bool[] { false, true }, 1 ),
            new EnemySpawn(0, new GameObject[] { enemyD, enemyC, enemyD, enemyC }, new float[] { -5, -5, 5, 5 }, new bool[] { true, true, true, true } ),
            new EnemySpawn(2, new GameObject[] { enemyB, enemyB, }, new float[] { -11, -3 }, new bool[] { false, true } ),
            new EnemySpawn(1, new GameObject[] { enemyB, enemyB, }, new float[] { 11, 3 }, new bool[] { false, true} ),
            new EnemySpawn(1, new GameObject[] { enemyB }, new float[] { 0 }, new bool[] { true } ),
        };

        level5HardSpawn = new EnemySpawn[] {
            new EnemySpawn(8, new GameObject[] { enemyC, enemyB, enemyA }, new float[] { 8, -3, 12 }, new bool[] { true, true, false } ),
            new EnemySpawn(3, new GameObject[] { enemyA, enemyD, enemyA }, new float[] { 3, -6, -12 }, new bool[] { true, true, false } ),
            new EnemySpawn(3, new GameObject[] { enemyD, enemyD, enemyA }, new float[] { 1, -9, 12 }, new bool[] { true, true, false } ),
            new EnemySpawn(3, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { 5, -2, -12 }, new bool[] { true, true, false } ),
            new EnemySpawn(3, new GameObject[] { enemyB, enemyA, enemyA }, new float[] { 9, -1, 12 }, new bool[] { true, true, false } ),
            new EnemySpawn(3, new GameObject[] { enemyD, enemyB, enemyA }, new float[] { 4, -5, -12 }, new bool[] { true, true, false } ),
            new EnemySpawn(3, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { 6, -3, 12 }, new bool[] { true, true, false }, 0 ),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA, enemyA, enemyA }, new float[] { 8, -8, -13, 13 }, new bool[] { true, true, false, false } ),
            new EnemySpawn(1, new GameObject[] { enemyA, enemyA }, new float[] { 6, -6, }, new bool[] { true, true } ),
            new EnemySpawn(1, new GameObject[] { enemyA, enemyA }, new float[] { 4, -4, }, new bool[] { true, true }, 0 ),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyB, }, new float[] { 8, -8 }, new bool[] { true, true } ),
            new EnemySpawn(1, new GameObject[] { enemyB, enemyB }, new float[] { 6, -6 }, new bool[] { true, true } ),
            new EnemySpawn(1, new GameObject[] { enemyB, enemyB }, new float[] { 4, -4 }, new bool[] { true, true }, 0 ),
            new EnemySpawn(0, new GameObject[] { enemyC, enemyC }, new float[] { 8, -8 }, new bool[] { true, true } ),
            new EnemySpawn(1, new GameObject[] { enemyC, enemyC }, new float[] { 6, -6 }, new bool[] { true, true } ),
            new EnemySpawn(1, new GameObject[] { enemyC, enemyC }, new float[] { 4, -4 }, new bool[] { true, true }, 0 ),
            new EnemySpawn(0, new GameObject[] { enemyD, enemyD, enemyD, enemyD }, new float[] { 8, -8, -13, 13 }, new bool[] { true, true, false, false } ),
            new EnemySpawn(1, new GameObject[] { enemyD, enemyD }, new float[] { 6, -6, }, new bool[] { true, true } ),
            new EnemySpawn(1, new GameObject[] { enemyD, enemyD }, new float[] { 4, -4, }, new bool[] { true, true }, 0 ),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
            new EnemySpawn(0.5f, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11, }, new bool[] { false, false } ),
        };
        #endregion

        hardLevelTimeLimits = new float[] {
            10,
            85,
            80,
            105,
            93,
            120
        };

        if (gameMode == HARD) currentTimeLimit = hardLevelTimeLimits[level];

        testSpawn = new EnemySpawn[] {
            new EnemySpawn(1, new GameObject[] { enemyD }, new float[] { 0 }, new bool[] { true }),

        };

        endWaitTimer = new float[] { 6.5f, 5f, 7.4f, 5f, 0, 0, 0, 5, 5, 5 };

        // load spawn
        if (gameMode == NORMAL)
        {
            if (level == 1) currentLevelSpawn = level1Spawn;
            if (level == 2) currentLevelSpawn = level2Spawn;
            if (level == 3) currentLevelSpawn = level3Spawn;
            if (level == 4) currentLevelSpawn = level4Spawn;
            if (level == 5) currentLevelSpawn = level5Spawn;
            if (level == 10) currentLevelSpawn = testSpawn;
            if (level == 100)
            {
                SecondCounter.enabled = true;
                currentLevelSpawn = levelEndlessSpawn;
                txtRecordDisplay.enabled = true;
                txtRecordDisplay.text = "RECORD: " + endlessRecord;
            }
        }
        if (level <= 0 || gameMode == BIRD) currentLevelSpawn = levelTrainingSpawn;

        if (gameMode == HARD)
        {
            if (level == 1) currentLevelSpawn = level1HardSpawn;
            if (level == 2) currentLevelSpawn = level2HardSpawn;
            if (level == 3) currentLevelSpawn = level3HardSpawn;
            if (level == 4) currentLevelSpawn = level4HardSpawn;
            if (level == 5) currentLevelSpawn = level5HardSpawn;
        }

        for (int i = 0; i < currentLevelSpawn.Length; i++)
        {
            for (int j = 0; j < currentLevelSpawn[i].enemies.Length; j++)
            {
                totalEnemies++;
            }
        }

        print("total enemies: " + totalEnemies);

        // song lengths
        songLengths = new float[] {
            1f,
            55.7f,
            91.2f,
            127.3f,
            129.2f,
            64.4f
        };

        // rank data
        levelRankLimits = new List<int[]>() {
            new int[] { 10, 20, 30, 40, 50 },
            new int[] { 2000, 3200, 3700, 4100, 5500 },
            new int[] { 3000, 4200, 5500, 7000, 10000 },
            new int[] { 5000, 7000, 9000, 11500, 14000 },
            new int[] { 8000, 11000, 14000, 17000, 22000 },
            new int[] { 8000, 10000, 11000, 15000, 18000 },
        };

        hardRankLimits = new List<int[]>() {
            new int[] { 10, 20, 30, 40, 50 },
            new int[] { 8000, 10000, 1100, 13000, 15000 },
            new int[] { 5000, 6500, 7000, 10000, 15000 },
            new int[] { 9000, 14000, 17000, 20000, 25000 },
            new int[] { 8000, 11000, 14500, 18000, 23500 },
            new int[] { 20000, 26000, 33000, 38000, 50000 },
        };

        birdRankLimits = new List<int[]>() {
            new int[] { 10, 20, 30, 40, 50 },
            CalculateRankLimits(45),
            CalculateRankLimits(58),
            CalculateRankLimits(146),
            CalculateRankLimits(178),
            CalculateRankLimits(127),
        };

        if (gameMode != HARD)
        {
            timerBox.gameObject.SetActive(false);
        }

        // load rank data
        if (gameMode == NORMAL)
        {
            if (level < 100 && level > 0) currentRankLimits = levelRankLimits[level];
        }
        if (gameMode == BIRD)
        {
            if (level < 100 && level > 0) currentRankLimits = birdRankLimits[level];
        }
        if (gameMode == HARD)
        {
            if (level < 100 && level > 0) currentRankLimits = hardRankLimits[level];
        }

        // play ambience
        if (level == 3) soundAmbCafe.start();
        if (level == 4) soundAmbSea.start();

        resultScreen.gameObject.SetActive(false);

        // send analytics
        AnalyticsEvent.LevelStart("Level_" + level + "_Mode_" + gameMode, level);
    }

    void InitPracticeMode()
    {
        soundPracticeSongs = new List<FMOD.Studio.EventInstance>();
        soundPracticeSongs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Music/FirstHappy"));
        soundPracticeSongs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Music/SpaceBagle"));
        soundPracticeSongs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Music/Elextroswing"));
        soundPracticeSongs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Music/PirateBoy"));
        soundPracticeSongs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Music/DoomMusic"));

        for (int i = 0; i < soundPracticeSongs.Count; i++)
        {
            soundPracticeSongs[i].setCallback(callBack, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
            soundPracticeSongs[i].setParameterValue("Practice", 1);
        }

        soundAvailableracticeSongs = new List<FMOD.Studio.EventInstance>();

        practiceSongNames = new List<string> { "Sunshine", "Moon", "Swing", "Pirate", "Hell" };

        availablePracticeSongNames = new List<string>();

        practiceSongBPM = new List<int> { 142, 100, 128, 148, 108 };

        availablePracticeSongBPM = new List<int>();

        for (int i = 1; i < data.unlockedLevel.Length; i++)
        {
            if (data.unlockedLevel[i] || data.unlockedBirdLevel[i])
            {
                soundAvailableracticeSongs.Add(soundPracticeSongs[i - 1]);
                availablePracticeSongNames.Add(practiceSongNames[i - 1]);
                availablePracticeSongBPM.Add(practiceSongBPM[i - 1]);
            }
        }

        currentSong = 0;
        soundAvailableracticeSongs[currentSong].start();
        txtSongName.text = availablePracticeSongNames[currentSong];
        bpm = availablePracticeSongBPM[currentSong];
        txtSongBPM.text = "BPM: " + bpm;
        currentBpm = bpm;
        offBeatTime = 60 / ((float)bpm * 2);
        preBeatTime = 60 / (float)bpm - leniency;

        GameObject tSprite;
        GameObject tSprite2;
        tSprite = new GameObject("ComboSprite" + -1);
        SpriteRenderer tRenderer = tSprite.AddComponent<SpriteRenderer>();
        tRenderer.sprite = comboBasicHeavy;
        tSprite.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        tSprite.transform.position = new Vector3(-6.4f, 3.1f - 0.7f * 0);
        tRenderer.sortingLayerName = "HUD";
        tRenderer.sortingOrder = 1;
        tSprite2 = new GameObject("ComboSprite" + -1 + "bg");
        tRenderer = tSprite2.AddComponent<SpriteRenderer>();
        tRenderer.sprite = comboHolder;
        tRenderer.sortingLayerName = "HUD";
        tSprite2.transform.parent = tSprite.transform;
        tSprite2.transform.localPosition = new Vector3(0, -0.08f);
        tSprite2.transform.localScale = new Vector3(1, 1, 1);
        tSprite = new GameObject("ComboSprite" + -2);
        tRenderer = tSprite.AddComponent<SpriteRenderer>();
        tRenderer.sprite = comboBasicLight;
        tSprite.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        tSprite.transform.position = new Vector3(-6.4f, 3.1f - 0.7f * 1);
        tRenderer.sortingLayerName = "HUD";
        tRenderer.sortingOrder = 1;
        tSprite2 = new GameObject("ComboSprite" + -2 + "bg");
        tRenderer = tSprite2.AddComponent<SpriteRenderer>();
        tRenderer.sprite = comboHolder;
        tRenderer.sortingLayerName = "HUD";
        tSprite2.transform.parent = tSprite.transform;
        tSprite2.transform.localPosition = new Vector3(0, -0.08f);
        tSprite2.transform.localScale = new Vector3(1, 1, 1);

        int j = 2;
        for (int i = 0; i < data.itemBought.Length; i++)
        {
            if (data.itemActive[i] && data.itemBought[i])
            {
                tSprite = new GameObject("ComboSprite" + i);
                tRenderer = tSprite.AddComponent<SpriteRenderer>();
                tRenderer.sprite = spriteCombos[i];
                tSprite.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                tSprite.transform.position = new Vector3(-6.4f, 3.1f - 0.7f * (j));
                tRenderer.sortingLayerName = "HUD";
                tRenderer.sortingOrder = 1;
                tSprite2 = new GameObject("ComboSprite" + i + "bg");
                tRenderer = tSprite2.AddComponent<SpriteRenderer>();
                tRenderer.sprite = comboHolder;
                tRenderer.sortingLayerName = "HUD";
                tSprite2.transform.parent = tSprite.transform;
                tSprite2.transform.localPosition = new Vector3(0, -0.08f);
                tSprite2.transform.localScale = new Vector3(1, 1, 1);
                j++;
            }
        }
    }

    public FMOD.RESULT StudioEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance eventInstance, System.IntPtr parameters)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
        {
            FMOD.Studio.TIMELINE_MARKER_PROPERTIES marker = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
            string name = marker.name;
            print("callback " + name);
            if (name == "Play")
            {
                songStarted = true;
            }
            if (name == "Stop")
            {
                songStarted = false;
            }
            if (name == "End" && gameMode == BIRD && !player.dead && !birdLevelFinished)
            {
                songFinishedCallbackRecieved = true;
            }
            if (name.Contains("12") || name.Contains("11") || name.Contains("10") || name.Contains("13") || name.Contains("14") || name.Contains("21"))
            {
                if (name == "128" && gameMode != HARD) soundMusic.setParameterValue("Intro", 1);
                bpm = int.Parse(name);
                currentBpm = bpm;
                offBeatTime = 60 / ((float)bpm * 2);
                preBeatTime = 60 / (float)bpm - leniency;
            }
            if ((name.Contains("B") && name.Length == 2 && level >= 0) && gameMode != NORMAL)
            {
                totalBirds++;
                player.birds.Add(Instantiate(enemyBird, new Vector3(0f, 0f), Quaternion.identity));
                if (name == "B1") player.birds[player.birds.Count - 1].GetComponent<enemyBirdHandler>().init(1, -totalBirds);
                if (name == "B2") player.birds[player.birds.Count - 1].GetComponent<enemyBirdHandler>().init(2, -totalBirds);
                if (name == "B4") player.birds[player.birds.Count - 1].GetComponent<enemyBirdHandler>().init(0, -totalBirds);
            }
        }
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            //FMOD.Studio.TIMELINE_BEAT_PROPERTIES beat = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
            beatTimer = 0;
            //soundSinus.start();
            currentBeatTimer = 0;
            if (songStarted)
            {
                offBeat = false;
                currentState = BEAT;
            }
        }
        return FMOD.RESULT.OK;
    }
    
    void Update()
    {
        // update timers
        beatTimer += Time.deltaTime;
        currentBeatTimer += Time.deltaTime;
        
        // update beat state
        if (beatTimer >= preBeatTime && currentState != SUCCESS && songStarted)
        {
            currentState = SUCCESS;
        }
        else if (beatTimer > 0.15f && beatTimer < preBeatTime && currentState != FAIL)
        {
            currentState = FAIL;
        }
        if (beatTimer >= offBeatTime)
        {
            offBeat = true;
        }

        if (currentState == BEAT && !beatFrame) beatFrame = true;
        else beatFrame = false;

        // update game over
        #region player dies
        if (player.dead && !gameOver)
        {
            if (level == 100)
            {
                // send endless analytics
                AnalyticsEvent.LevelComplete("level_100" + "_Mode_" + gameMode, 100, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_survived", Mathf.Round(levelTimer) } });
            }
            
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].GetComponent<enemyHandler>().StopFall();
            }

            songStarted = false;
            soundMusic.setParameterValue("Clock", 0);
            soundMusic.setParameterValue("Die", 1);
            //soundClock.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            // send analytics
            if (level != 100) AnalyticsEvent.LevelFail("Level_" + level + "_Mode_" + gameMode, level, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_alive", Mathf.Round(levelTimer) } });

            gameOver = true;

            print("res");
            // show results screen
            StartCoroutine(ShowResultScreen(1.5f, false));
        }
        #endregion

        // update HUD
        #region HUD

        // progress bar
        if (level > 0 && level < 100 && currentGameMode != BIRD)
        {
            maskProgressFill.transform.localPosition = new Vector3(-4.66f * (1 - ((float)enemiesDead / (float)totalEnemies)), 0);
            rendererProgressMarker.transform.localPosition = new Vector3(-2.22f + 4.44f * ((float)enemiesDead / (float)totalEnemies), 0);
        }
        else if (currentGameMode == BIRD)
        {
            maskProgressFill.transform.localPosition = new Vector3(-4.66f * (1 - Mathf.Clamp((levelTimer / songLengths[level]), 0, 1)), 0);
            rendererProgressMarker.transform.localPosition = new Vector3(-2.22f + 4.44f * Mathf.Clamp((levelTimer / songLengths[level]), 0, 1), 0);
        }

        if (level == 100 && !player.dead)
        {
            SecondCounter.text = "TIME: " + Mathf.Round(levelTimer).ToString();
        }
        #endregion

        // check inputs
        #region inputs

        // hide HUD
        if (Input.GetKeyDown(KeyCode.F1))
        {
            levelUI.renderMode = RenderMode.ScreenSpaceOverlay;
            playerHUD.renderMode = RenderMode.ScreenSpaceOverlay;
            foreach (Text t in levelUI.GetComponentsInChildren<Text>())
            {
                t.transform.position = new Vector3(-10000, 0);
            }
            foreach (Image t in levelUI.GetComponentsInChildren<Image>())
            {
                t.transform.position = new Vector3(-10000, 0);
            }
            foreach (Text t in playerHUD.GetComponentsInChildren<Text>())
            {
                t.transform.position = new Vector3(-10000, 0);
            }
            foreach (Image t in playerHUD.GetComponentsInChildren<Image>())
            {
                t.transform.position = new Vector3(-10000, 0);
            }
            HUDTurnedOff = true;
        }

        if (Input.GetButtonDown("Cancel") && !normalLevelFinished && !birdLevelFinished)
        {
            QuitLevel("MenuScene");
        }

        // practice input
        if (level == -1)
        {
            if (Input.GetAxis("NavigateRight") > 0.5f && !holdingRT)
            {
                holdingRT = true;
                ChangeSong(true);
            }
            else if (Input.GetAxis("NavigateRight") < 0.2f && holdingRT)
            {
                holdingRT = false;
            }
            if (Input.GetAxis("NavigateLeft") > 0.5f && !holdingLT)
            {
                holdingLT = true;
                ChangeSong(false);
            }
            else if (Input.GetAxis("NavigateLeft") < 0.2f && holdingLT)
            {
                holdingLT = false;
            }
            if (Input.GetButtonDown("Other Action"))
            {
                ToggleTrainingControls();
            }
        }
        #endregion

        // update streak parameter in song
        if (player.streakLevel > 2)
        {
            soundMusic.setParameterValue("Cool", 1);
            soundMusic.setParameterValue("CoolFail", 0);
            gotCool = true;
        }
        else
        {
            if (gotCool)
            {
                soundMusic.setParameterValue("CoolFail", 1);
                gotCool = false;
            }
            soundMusic.setParameterValue("Cool", 0);
        }

        // update max streak
        if (player.currentStreak > currentMaxStreak) currentMaxStreak = player.currentStreak;
        
        // update camera zoom
        if (player.streakLevel >= 3 && !normalLevelFinished && !birdLevelFinished) levelCamera.orthographicSize = 5.35f + (beatTimer / 0.6f) * 0.05f;
        else levelCamera.orthographicSize = 5.4f;
        
        // update time limit
        if (gameMode == HARD && !normalLevelFinished && !player.dead && level != -1)
        {
            currentTimeLimit -= Time.deltaTime;

            timerBox.UpdateText(currentTimeLimit);
            
            if (currentTimeLimit <= 30 && !clockStarted && beatFrame)
            {
                clockStarted = true;
                //soundClock.start();
                soundMusic.setParameterValue("Clock", 1);
            }

            if (currentTimeLimit <= 0)
            {
                player.TakeDamage(999, 0, true);
                currentTimeLimit = 0;
            }
        }
        
        // progress level
        ProgressLevel();
	}

    void ProgressLevel()
    {
        // update timer
        if (!player.dead) levelTimer += Time.deltaTime;

        // normal/hard mode
        if (level > 0 && level < 100 && (gameMode == NORMAL || gameMode == HARD) && !normalLevelFinished)
        {
            // reached end of level
            if (currentLevelSpawn.Length == currentSpawn)
            {
                bool tAllDead = true;

                for (int i = 0; i < enemies.Count; i++)
                {
                    if (!enemies[i].GetComponent<enemyHandler>().dead) tAllDead = false;
                }

                if (tAllDead)
                {
                    // bonus currency
                    int currAmount = 20 + player.currentRank * 5;
                    currAmount = Random.Range(currAmount - 10, currAmount + 10);
                    for (int i = 0; i < currAmount / 2; i++)
                    {
                        GameObject tempObject = Instantiate(currencyObject, new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(0.3f, 1.4f), 0), new Quaternion(0, 0, 0, 0));
                        tempObject.GetComponent<currencyHandler>().Init(true, 0);
                    }
                    for (int i = 0; i < currAmount / 2; i++)
                    {
                        GameObject tempObject = Instantiate(currencyObject, transform.position + new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(0.3f, 1.4f), 0), new Quaternion(0, 0, 0, 0));
                        tempObject.GetComponent<currencyHandler>().Init(false, 0);
                    }

                    print("finished level " + level + " on gamemode " + gameMode);
                    normalLevelFinished = true;
                    soundMusic.setParameterValue("Win", 1);
                    soundClock.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

                    // send analytics
                    AnalyticsEvent.LevelComplete("Level_" + level + "_Mode_" + gameMode, level, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_alive", Mathf.Round(levelTimer) }, { "rank", player.currentRank } });

                    // show results screen
                    StartCoroutine(ShowResultScreen(2f, true));
                }
                
            }
            else if (levelTimer > prevTime + currentLevelSpawn[currentSpawn].spawnTime && !waitingForDeath)
            {
                // next spawn after time
                for (int i = 0; i < currentLevelSpawn[currentSpawn].enemies.Length; i++)
                {
                    if (currentLevelSpawn[currentSpawn].relativeToPlayer)
                    {
                        float tXPost = player.transform.position.x + currentLevelSpawn[currentSpawn].xPos[i];
                        if (tXPost > 9 || tXPost < -9) enemies.Add(Instantiate(currentLevelSpawn[currentSpawn].enemies[i], new Vector3(0, 0), Quaternion.identity));
                        else enemies.Add(Instantiate(currentLevelSpawn[currentSpawn].enemies[i], new Vector3(player.transform.position.x + currentLevelSpawn[currentSpawn].xPos[i], 0), Quaternion.identity));
                    }
                    else enemies.Add(Instantiate(currentLevelSpawn[currentSpawn].enemies[i], new Vector3(currentLevelSpawn[currentSpawn].xPos[i], 0), Quaternion.identity));
                    enemies[enemies.Count - 1].GetComponent<enemyHandler>().Init(currentLevelSpawn[currentSpawn].fallFromAbove[i]);
                }
                if (currentLevelSpawn[currentSpawn].waitForDeath) waitingForDeath = true;
                else
                {
                    prevTime = levelTimer;
                    currentSpawn++;
                }
            }
            else if (waitingForDeath)
            {
                // next spawn with kill requirement
                if (enemies.Count - enemiesDead <= currentLevelSpawn[currentSpawn].nextWaveReq)
                {
                    prevTime = levelTimer;
                    currentSpawn++;
                    waitingForDeath = false;
                }
            }
        }

        // bird mode end check
        if (level > 0 && gameMode == BIRD && songFinishedCallbackRecieved && !birdLevelFinished)
        {
            // give currency
            int currAmount = 75 + player.currentRank * 15;
            currAmount = Random.Range(currAmount - 20, currAmount + 20);
            for (int i = 0; i < currAmount / 2; i++)
            {
                GameObject tempObject = Instantiate(currencyObject, new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(0.3f, 1.4f), 0), new Quaternion(0, 0, 0, 0));
                tempObject.GetComponent<currencyHandler>().Init(true, 0);
            }
            for (int i = 0; i < currAmount / 2; i++)
            {
                GameObject tempObject = Instantiate(currencyObject, transform.position + new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(0.3f, 1.4f), 0), new Quaternion(0, 0, 0, 0));
                tempObject.GetComponent<currencyHandler>().Init(false, 0);
            }
            
            // send analytics
            AnalyticsEvent.LevelComplete("Level_" + level + "_Mode_" + gameMode, level, new Dictionary<string, object> { { "max_streak", comboRecord[level] }, { "rank", player.currentRank } });

            print("finished level " + level + " on gamemode " + gameMode);
            birdLevelFinished = true;

            print("Number of birds: " + totalBirds);

            // show results screen
            StartCoroutine(ShowResultScreen(2f, true));
        }

        // endless mode
        if (level == 100)
        {
            if (currentLevelSpawn.Length == currentSpawn)
            {
                if (levelTimer >= nextSpawn)
                {
                    // generate new spawn
                    nextSpawn = levelTimer;
                    nextSpawn += 11 - (Mathf.Clamp(14 * (levelTimer / 300), 0, 16));
                    int tDistance = 0;
                    print("levelTimer: " + levelTimer + ", nextSpawn: " + nextSpawn);
                    for (int i = 0; i < 2 + levelTimer / 75; i++)
                    {
                        bool tAbove = false;
                        if (Random.value > 0.20f)
                        {
                            if (Random.value < 0.3f)
                            {
                                enemies.Add(Instantiate(RandEnemy(), new Vector3(Random.Range(-5f, 5f), 0), Quaternion.identity));
                                tAbove = true;
                            }
                            else enemies.Add(Instantiate(RandEnemy(), new Vector3((10.1f + tDistance) * RandDirection(), 0), Quaternion.identity));

                            enemies[enemies.Count - 1].GetComponent<enemyHandler>().Init(tAbove);
                            tDistance++;
                        }

                    }
                }
            }
            else if (levelTimer > prevTime + currentLevelSpawn[currentSpawn].spawnTime && !waitingForDeath)
            {
                // next spawn after time
                for (int i = 0; i < currentLevelSpawn[currentSpawn].enemies.Length; i++)
                {
                    enemies.Add(Instantiate(currentLevelSpawn[currentSpawn].enemies[i], new Vector3(currentLevelSpawn[currentSpawn].xPos[i], 0), Quaternion.identity));
                    enemies[enemies.Count - 1].GetComponent<enemyHandler>().Init(currentLevelSpawn[currentSpawn].fallFromAbove[i]);
                }
                if (currentLevelSpawn[currentSpawn].waitForDeath) waitingForDeath = true;
                else
                {
                    prevTime = levelTimer;
                    currentSpawn++;
                }
            }
            else if (waitingForDeath)
            {
                // next spawn after kill requirement
                if (enemies.Count - enemiesDead <= currentLevelSpawn[currentSpawn].nextWaveReq)
                {
                    print("dead req met");
                    prevTime = levelTimer;
                    currentSpawn++;
                    waitingForDeath = false;
                }
            }
        }
    }

    public void ToggleTrainingControls()
    {
        if (trainingControlsOpen) imgControlsTraining.sprite = spriteControlsClosed;
        else
        {
            if (sceneSelectionHandler.Instance.inputIcons == 0) imgControlsTraining.sprite = spriteControlsXbox;
            if (sceneSelectionHandler.Instance.inputIcons == 1) imgControlsTraining.sprite = spriteControlsPS;
            if (sceneSelectionHandler.Instance.inputIcons == 2) imgControlsTraining.sprite = spriteControlsKeyboard;
        }
        trainingControlsOpen = !trainingControlsOpen;
    }

    public void QuitLevel(string scene)
    {
        songStarted = false;
        normalLevelFinished = false;
        birdLevelFinished = false;
        lastMode = currentGameMode;
        if (gameMode == BIRD) seenBirdTutorial = true;
        seenTutorial = true;
        SaveSystem.SavePlayer(this);
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].GetComponent<enemyHandler>().Stop();
        }
        // send analytics
        if (victoryTimer == 0) AnalyticsEvent.LevelQuit("level_" + level + "_Mode_" + gameMode, level, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_alive", Mathf.Round(levelTimer) } });
        enemiesDead = 0;
        soundClock.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (level == 3) soundAmbCafe.setParameterValue("End", 1);
        if (level == 4) soundAmbSea.setParameterValue("End", 1);
        soundMusic.setCallback(null, 0);
        soundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (level == -1) soundAvailableracticeSongs[currentSong].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene(scene);

    }
    
    public void RestartLevel()
    {
        soundUIClick.start();

        soundMusic.setCallback(null, 0);
        soundClock.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (level == 3) soundAmbCafe.setParameterValue("End", 1);
        if (level == 4) soundAmbSea.setParameterValue("End", 1);
        soundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        print("restart level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        //AnalyticsEvent.LevelStart("Level_" + level + "_Mode_" + gameMode, level);

        //foreach (GameObject g in GameObject.FindGameObjectsWithTag("currency"))
        //{
        //    Destroy(g);
        //}
        //foreach (GameObject g in GameObject.FindGameObjectsWithTag("HPPickup"))
        //{
        //    Destroy(g);
        //}

        //clockStarted = false;
        //currentSpawn = 0;
        //levelTimer = 0;
        //if (level == 3 && gameMode == NORMAL) levelTimer = 6.9f;
        //nextSpawn = 0;
        //prevTime = 0;
        //currentMaxStreak = 0;
        //if (gameMode == HARD) currentTimeLimit = hardLevelTimeLimits[level];
        //player.Reset();
        //txtGameOver.enabled = false;
        //if (gameMode == HARD) txtTimeLeft.transform.localScale = new Vector3(0.5f, 0.5f);
        //gameOver = false;
        //songStarted = false;
        //btnRetry.gameObject.SetActive(false);
        //btnGameOver.gameObject.SetActive(false);
        //waitingForDeath = false;
        //enemiesDead = 0;

        //if (level == 100)
        //{
        //    txtRecordAnnouncement.enabled = false;
        //    SecondCounter.enabled = true;
        //    SecondDisplay.enabled = false;
        //    txtRecordDisplay.text = "RECORD: " + endlessRecord;
        //}

        //for (int i = 0; i < enemies.Count; i++)
        //{
        //    enemies[i].GetComponent<enemyHandler>().Stop();
        //    Destroy(enemies[i]);
        //}
        //enemies.Clear();
        //gameOverTimer = 0;
        //soundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //if (gameMode == BIRD) soundMusic = FMODUnity.RuntimeManager.CreateInstance(musicPath.Insert(12, "/Bird") + "_bird");
        //else soundMusic = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        //soundMusic.setCallback(callBack, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        //if (level == 3 && gameMode == NORMAL) soundMusic.setParameterValue("Loop", 1);
        //soundMusic.start();
    }

    void OnApplicationQuit()
    {
        print("stopped running");
        soundMusic.setCallback(null, 0);
        soundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (level == -1) soundAvailableracticeSongs[currentSong].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public static void EnemyDead()
    {
        enemiesDead++;
    }

    public void ChangeSong(bool next)
    {
        soundAvailableracticeSongs[currentSong].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (next)
        {
            currentSong = (currentSong + 1) % availablePracticeSongNames.Count;
        }
        else
        {
            currentSong -= 1;
            if (currentSong == -1) currentSong = availablePracticeSongNames.Count - 1;
        }
        soundAvailableracticeSongs[currentSong].start();
        txtSongName.text = availablePracticeSongNames[currentSong];
        bpm = availablePracticeSongBPM[currentSong];
        currentBpm = bpm;
        txtSongBPM.text = "BPM: " + bpm;
        songStarted = false;
        offBeatTime = 60 / ((float)bpm * 2);
        preBeatTime = 60 / (float)bpm - leniency;
    }

    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    int RandDirection()
    {
        return 1 * (int)Mathf.Pow(-1, Random.Range((int)1, (int)3));
    }

    GameObject RandEnemy()
    {
        float eB = Random.Range(0f, 1f);
        if (eB < 0.4f) return enemyA;
        else if (eB < 0.6f) return enemyB;
        else if (eB < 0.8f) return enemyC;
        else return enemyD;
    }

    public void ChangeSceneMenu()
    {
        soundUIClick.start();

        QuitLevel("MenuScene");
    }

    public void ChangeSceneShop()
    {
        soundUIClick.start();

        QuitLevel("ShopScene");
    }
    
    int[] CalculateRankLimits(int nBirds)
    {
        int origBirds = nBirds;
        int tPScore = 0;
        int tCount = 0;
        while (nBirds > 0)
        {
            nBirds--;
            tCount++;
            if (tCount <= 10) tPScore += 100 * 1;
            else if (tCount <= 30) tPScore += (int)Mathf.Round(100 * 1.1f);
            else if (tCount <= 50) tPScore += (int)Mathf.Round(100 * 1.3f);
            else if (tCount <= 75) tPScore += (int)Mathf.Round(100 * 1.5f);
            else if (tCount <= 100) tPScore += (int)Mathf.Round(100 * 1.7f);
            else tPScore += 100 * 2;
        }
        /*
        tPScore += 100 * 1 * 10;
        tPScore += (int)Mathf.Round(100 * 1.1f) * 20;
        tPScore += (int)Mathf.Round(100 * 1.3f) * 20;
        tPScore += (int)Mathf.Round(100 * 1.5f) * 25;
        tPScore += (int)Mathf.Round(100 * 1.7f) * 25;
        int tBirds = nBirds;
        tBirds -= 100;
        
        tPScore += 100 * 2 * tBirds;
        */
        tPScore += 1000;

        print("score for P with " + origBirds + " birds: " + tPScore);

        return new int[] { (int)(tPScore * 0.25f), (int)(tPScore * 0.5f), (int)(tPScore * 0.7f), (int)(tPScore * 0.8f), tPScore };
    }

    public IEnumerator ShowResultScreen(float waitTime, bool victory)
    {
        yield return new WaitForSeconds(waitTime);

        // update save data
        if (level == 100)
        {
            if (Mathf.Round(levelTimer) > endlessRecord)
            {
                endlessRecord = (int)Mathf.Round(levelTimer);
            }
            if (currentMaxStreak > streakLevelEndlessRecord) streakLevelEndlessRecord = currentMaxStreak;
        }
        else if (gameMode == NORMAL && level != 100)
        {
            if (normalLevelFinished)
            {
                if (currentMaxStreak > streakRecord[level]) streakRecord[level] = currentMaxStreak;
                clearedLevel[level] = true;
                if (player.currentScore > scoreRecord[level])
                {
                    scoreRecord[level] = player.currentScore;
                    gotNewRecord = true;
                }

                if (player.currentRank > rankRecord[level]) rankRecord[level] = player.currentRank;

                if (clearedLevel[1]) unlockedLevel[2] = true;
                if (clearedLevel[2]) { unlockedLevel[3] = true; unlockedLevel[4] = true; }
                if (clearedLevel[3] && clearedLevel[4]) unlockedLevel[5] = true;

                if (clearedLevel[1]) unlockedBirdLevel[1] = true;
                if ((clearedLevel[3] || clearedLevel[4]) && (clearedBirdLevel[3] || clearedBirdLevel[4])) unlockedHardLevel[1] = true;
            }
            currentRecord = scoreRecord[level];
        }
        if (gameMode == HARD)
        {
            if (normalLevelFinished)
            {
                if (currentTimeLimit > timeRecord[level]) timeRecord[level] = currentTimeLimit;
                clearedHardLevel[level] = true;
                if (player.currentScore > scoreHardRecord[level])
                {
                    scoreHardRecord[level] = player.currentScore;
                    gotNewRecord = true;
                }

                if (player.currentRank > rankHardRecord[level]) rankHardRecord[level] = player.currentRank;

                if (clearedHardLevel[1]) unlockedHardLevel[2] = true;
                if (clearedHardLevel[2]) unlockedHardLevel[3] = true;
                if (clearedHardLevel[3]) unlockedHardLevel[4] = true;
                if (clearedHardLevel[4]) unlockedHardLevel[5] = true;
            }
            currentRecord = scoreHardRecord[level];
        }
        if (gameMode == BIRD)
        {
            if (birdLevelFinished)
            {
                if (currentMaxStreak > comboRecord[level]) comboRecord[level] = currentMaxStreak;
                clearedBirdLevel[level] = true;
                if (player.currentScore > scoreBirdRecord[level])
                {
                    scoreBirdRecord[level] = player.currentScore;
                    gotNewRecord = true;
                }

                if (player.currentRank > rankBirdRecord[level]) rankBirdRecord[level] = player.currentRank;

                if (clearedBirdLevel[1]) unlockedBirdLevel[2] = true;
                if (clearedBirdLevel[2]) { unlockedBirdLevel[3] = true; unlockedBirdLevel[4] = true; }
                if (clearedBirdLevel[3] && clearedBirdLevel[4]) unlockedBirdLevel[5] = true;

                if ((clearedLevel[3] || clearedLevel[4]) && (clearedBirdLevel[3] || clearedBirdLevel[4])) unlockedHardLevel[1] = true;
            }
            currentRecord = scoreBirdRecord[level];
        }

        currency = player.currentCurrency;
        inputSelected = sceneSelectionHandler.Instance.inputIcons;
        SaveSystem.SavePlayer(this);

        resultScreen.gameObject.SetActive(true);
        print("object active");
        resultScreen.ShowResultScreen(victory, level == 100);
        print("method called");
    }

}
