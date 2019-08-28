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

    public int bpm;
    public static int currentBpm;
    public float offset;
    public float leniency = 0.1f;
    public static float currentLeniency;
    float offBeatTime;
    float preBeatTime;
    public int gameMode;
    public static int currentGameMode;

    readonly int NORMAL = 0, BIRD = 1, HARD = 2;
    
    public GameObject enemyA;
    public GameObject enemyB;
    public GameObject enemyC;
    public GameObject enemyD;
    public GameObject enemyBird;

    List<GameObject> enemies;

    public GameObject player;

    public Text txtVictory;
    public Text txtGameOver;
    float gameOverTimer;
    public bool gameOver;
    public static bool normalLevelFinished;
    public static bool birdLevelFinished;

    bool gotCool;

    public Button btnRetry;
    public Button btnGameOver;
    public SpriteMask maskProgressFill;
    public SpriteRenderer rendererProgressMarker;

    float beatTime;
    float beatTimer;
    float offsetTimer;
    
    float indicatorTimer;
    bool indicatorReady;

    public int level;
    public static int staticLevel;

    public static float levelTimer;
    int currentSpawn;
    float prevTime;
    bool waitingForDeath;
    public static int enemiesDead;
    public int totalEnemies;
    public int totalBirds;
    float[] songLengths;

    float nextSpawn;

    readonly int FAIL = 0, SUCCESS = 1, BEAT = 2, OFFBEAT = 3;
    public static int currentState;
    public static bool offBeat;
    public static bool songStarted;

    public Text SecondCounter;
    public Text SecondDisplay;
    public Text txtRecordDisplay;
    public Text txtRecordAnnouncement;
    float victoryTimer;

    EnemySpawn[] levelEndlessSpawn;
    EnemySpawn[] levelTrainingSpawn;
    EnemySpawn[] level1Spawn;
    EnemySpawn[] level2Spawn;
    EnemySpawn[] level3Spawn;
    EnemySpawn[] level4Spawn;
    EnemySpawn[] testSpawn;
    EnemySpawn[] currentLevelSpawn;
    float[] endWaitTimer;
    public static int[] currentRankLimits;
    List<int[]> levelRankLimits;

    public int endlessRecord;
    public bool[] clearedLevel;
    public bool[] clearedBirdLevel;
    public bool[] unlockedLevel;
    public bool[] unlockedBirdLevel;
    public int[] streakRecord;
    public int[] comboRecord;
    public int streakLevelEndlessRecord;
    public int[] scoreRecord;
    public int[] scoreBirdRecord;
    public int[] rankRecord;
    public int[] rankBirdRecord;
    int currentMaxStreak;
    public int currency;
    public bool[] itemBought;
    public bool[] itemActive;
    public int lastMode;

    int[] numBirds;
    public List<int[]> birdRankLimits; 

    FMOD.Studio.EventInstance soundAmbCafe;
    FMOD.Studio.EventInstance soundAmbSea;

    FMOD.Studio.EventInstance soundMusic;
    FMOD.Studio.EVENT_CALLBACK callBack;

    FMOD.Studio.EventInstance soundUIClick;

    FMOD.Studio.EventInstance soundSinus;

    public string musicPath;

    float beatTimer2;
    public static float currentBeatTimer;

    // PRACTICE ROOM
    public Text txtSongName;
    public Text txtSongBPM;

    List<FMOD.Studio.EventInstance> soundPracticeSongs;
    List<FMOD.Studio.EventInstance> soundAvailableracticeSongs;
    List<string> practiceSongNames;
    List<string> availablePracticeSongNames;
    List<int> practiceSongBPM;
    List<int> availablePracticeSongBPM;
    int currentSong;

    public Sprite[] spriteCombos;
    public Sprite comboHolder;
    public Sprite comboBasicHeavy;
    public Sprite comboBasicLight;

    bool holdingRT;
    bool holdingLT;

    void Awake()
    {
        currentBpm = bpm;
        currentLeniency = leniency;
        staticLevel = level;
        currentGameMode = gameMode;
    }
    
    void Start()
    {
        // set music
        soundMusic = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        offBeatTime = 60 / ((float)bpm * 2);
        preBeatTime = 60 / (float)bpm - leniency;
        
        callBack = new FMOD.Studio.EVENT_CALLBACK(StudioEventCallback);
        soundMusic.setCallback(callBack, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        if (level != -1) soundMusic.start();

        soundAmbCafe = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Amb_cafe");
        soundAmbSea = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Amb_sea");

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");

        soundSinus = FMODUnity.RuntimeManager.CreateInstance("event:/Sinus_test");
        
        enemies = new List<GameObject>();

        // load data
        PlayerData data = SaveSystem.LoadPlayer();
        data.Init();
        endlessRecord = data.endlessRecord;
        clearedLevel = data.clearedLevel;
        clearedBirdLevel = data.clearedBirdLevel;
        unlockedLevel = data.unlockedLevel;
        unlockedBirdLevel = data.unlockedBirdLevel;
        streakRecord = data.streakRecord;
        comboRecord = data.comboRecord;
        streakLevelEndlessRecord = data.streakLevelEndlessRecord;
        scoreRecord = data.scoreRecord;
        scoreBirdRecord = data.scoreBirdRecord;
        rankRecord = data.rankRecord;
        rankBirdRecord = data.rankBirdRecord;
        currency = data.currency;
        itemBought = data.itemBought;
        itemActive = data.itemActive;
        lastMode = data.lastMode;

        player.GetComponent<playerHandler>().Init(currency);
        
        currentSpawn = 0;
        levelTimer = 0;
        enemiesDead = 0;
        victoryTimer = 0;
        totalEnemies = 0;
        songStarted = false;

        // load practice
        if (level == -1)
        {
            soundPracticeSongs = new List<FMOD.Studio.EventInstance>();
            soundPracticeSongs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Music/SpaceBagle"));
            soundPracticeSongs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Music/Elextroswing"));
            soundPracticeSongs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Music/PirateBoy"));
            soundPracticeSongs.Add(FMODUnity.RuntimeManager.CreateInstance("event:/Music/DoomMusic"));

            for (int i = 0; i < soundPracticeSongs.Count; i++)
            {
                soundPracticeSongs[i].setCallback(callBack, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
            }

            soundAvailableracticeSongs = new List<FMOD.Studio.EventInstance>();

            practiceSongNames = new List<string> { "Moon", "Swing", "Pirate", "Hell" };

            availablePracticeSongNames = new List<string>();

            practiceSongBPM = new List<int> { 100, 128, 140, 108 };

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
            tSprite2.transform.localPosition = new Vector3(0, 0);
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
            tSprite2.transform.localPosition = new Vector3(0, 0);
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
                    tSprite2.transform.localPosition = new Vector3(0, 0);
                    tSprite2.transform.localScale = new Vector3(1, 1, 1);
                    j++;
                }
            }
        }

        levelTrainingSpawn = new EnemySpawn[] { };

        // spawn data
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

        level1Spawn = new EnemySpawn[] {
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 10 }, new bool[] { false }, 0 ),
            new EnemySpawn(0, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }),
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 10 }, new bool[] { false }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { 0 }, new bool[] { true }, 1),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }, new bool[] { false, false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyB }, new float[] { 10 }, new bool[] { false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }, new bool[] { false, false }),
            new EnemySpawn(5, new GameObject[] { enemyA, enemyA }, new float[] { -2, 2 }, new bool[] { true, true }, 1),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyA }, new float[] { -10, 7 }, new bool[] { false, true }),
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyB }, new float[] { -10, 10 }, new bool[] { false, false })
        };

        /*
        level2Spawn = new EnemySpawn[] {
            new EnemySpawn(7, new GameObject[] { enemyA, enemyB }, new float[] { -8, 12 }, new bool[] { true, false }),
            new EnemySpawn(8, new GameObject[] { enemyB, enemyA }, new float[] { -12, 7 }, new bool[] { false, true }, 0),
            new EnemySpawn(0, new GameObject[] { enemyA }, new float[] { 0 }, new bool[] { true }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { -2 }, new bool[] { true }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { 2 }, new bool[] { true }),
            new EnemySpawn(2, new GameObject[] { enemyA, enemyA }, new float[] { 10, 11 }, new bool[] { false, false }),
            new EnemySpawn(2, new GameObject[] { enemyA, enemyA }, new float[] { -4, -11 }, new bool[] { true, false }, 2),
            new EnemySpawn(0, new GameObject[] { enemyB }, new float[] { 0 }, new bool[] { true }, 0),
            new EnemySpawn(0, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -10 }, new bool[] { false }),
            new EnemySpawn(3, new GameObject[] { enemyB }, new float[] { -10 }, new bool[] { false }, 0),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyA, enemyB, enemyA }, new float[] { -6, -10,  11, 7}, new bool[] { true, false, false, true }, 2),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA }, new float[] { -11, 11}, new bool[] { false, false }),
        };
        */

        level2Spawn = new EnemySpawn[] {
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

        /*
        level3Spawn = new EnemySpawn[] {
            new EnemySpawn(10, new GameObject[] { enemyB, enemyB }, new float[] { -11, 11 }, new bool[] { false, false }),
            new EnemySpawn(12, new GameObject[] { enemyA, enemyA }, new float[] { -2, 2 }, new bool[] { true, true }),
            new EnemySpawn(6, new GameObject[] { enemyA, enemyA }, new float[] { -6, 12 }, new bool[] { true, false }, 2),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { -10, -12, -14 }, new bool[] { false, false, false }),
            new EnemySpawn(4, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { 10, 12, 14 }, new bool[] { false, false, false }, 2),
            new EnemySpawn(0, new GameObject[] { enemyB, enemyA, enemyB, enemyA, enemyA }, new float[] { -10, -11, -12, 8, 5 }, new bool[] { false, false, false, true, true }, 1),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA }, new float[] { 9, -9 }, new bool[] { true, true }),
            new EnemySpawn(3, new GameObject[] { enemyB, enemyB }, new float[] { -4, -7 }, new bool[] { true, true }),
            new EnemySpawn(8, new GameObject[] { enemyB, enemyB }, new float[] { 4, 7 }, new bool[] { true, true }, 0),
            new EnemySpawn(0, new GameObject[] { enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA }, new float[] { 10, 11, 12, 13, 14, 15, -10, -11, -12, -13, -14, -15, 0 }, new bool[] { false, false,false,false,false,false,false,false,false,false,false,false,true })
        };
        */

        level3Spawn = new EnemySpawn[] {
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

        level4Spawn = new EnemySpawn[] {
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

        testSpawn = new EnemySpawn[] {
            new EnemySpawn(5, new GameObject[] { enemyB }, new float[] { 2 }, new bool[] { true }),

        };

        endWaitTimer = new float[] { 6.3f, 5f, 7.4f, 5f, 0, 0, 0, 5, 5, 5 };

        // load spawn
        if (level == 1) currentLevelSpawn = level1Spawn;
        if (level == 2) currentLevelSpawn = level2Spawn;
        if (level == 3) currentLevelSpawn = level3Spawn;
        if (level == 4) currentLevelSpawn = level4Spawn;
        if (level == 10) currentLevelSpawn = testSpawn;
        if (level == 100)
        {
            SecondCounter.enabled = true;
            currentLevelSpawn = levelEndlessSpawn;
            txtRecordDisplay.enabled = true;
            txtRecordDisplay.text = "RECORD: " + endlessRecord;
        }
        if (level <= 0 || gameMode == BIRD) currentLevelSpawn = levelTrainingSpawn;

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
            129.6f,
            127.3f,
            129.2f,
            64.4f
        };

        // rank data
        levelRankLimits = new List<int[]>() {
            new int[] { 10, 20, 30, 40, 50 },
            new int[] { 4500, 5500, 6500, 8000, 10000 },
            new int[] { 6000, 7500, 9000, 12000, 14000 },
            new int[] { 1000, 13000, 16000, 18000, 22000 },
            new int[] { 9000, 11000, 13000, 15000, 18000 },
        };

        birdRankLimits = new List<int[]>() {
            new int[] { 10, 20, 30, 40, 50 },
            CalculateRankLimits(100),
            CalculateRankLimits(147),
            CalculateRankLimits(178),
            CalculateRankLimits(127),
        };

        // load rank data
        if (gameMode == NORMAL)
        {
            if (level < 100 && level > 0) currentRankLimits = levelRankLimits[level];
        }
        if (gameMode == BIRD)
        {
            if (level < 100 && level > 0) currentRankLimits = birdRankLimits[level];
        }
        // play ambience
        if (level == 2) soundAmbCafe.start();
        if (level == 3) soundAmbSea.start();

        // send analytics
        AnalyticsEvent.LevelStart("Level_" + level, level);
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
            if (name == "End" && gameMode == BIRD && !player.GetComponent<playerHandler>().dead)
            {
                birdLevelFinished = true;
            }
            if (name.Contains("12") || name.Contains("11") || name.Contains("10") || name.Contains("13") || name.Contains("14") || name.Contains("21"))
            {
                if (name == "128") soundMusic.setParameterValue("Intro", 1);
                bpm = int.Parse(name);
                currentBpm = bpm;
                offBeatTime = 60 / ((float)bpm * 2);
                preBeatTime = 60 / (float)bpm - leniency;
            }
            if (name.Contains("B") && name.Length == 2 && level >= 0)
            {
                totalBirds++;
                player.GetComponent<playerHandler>().birds.Add(Instantiate(enemyBird, new Vector3(0f, 0f), Quaternion.identity));
                if (name == "B1") player.GetComponent<playerHandler>().birds[player.GetComponent<playerHandler>().birds.Count - 1].GetComponent<enemyBirdHandler>().init(1);
                if (name == "B2") player.GetComponent<playerHandler>().birds[player.GetComponent<playerHandler>().birds.Count - 1].GetComponent<enemyBirdHandler>().init(2);
                if (name == "B4") player.GetComponent<playerHandler>().birds[player.GetComponent<playerHandler>().birds.Count - 1].GetComponent<enemyBirdHandler>().init(0);
            }
        }
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            print("beat");
            //FMOD.Studio.TIMELINE_BEAT_PROPERTIES beat = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
            beatTimer2 = 0;
            //soundSinus.start();
            currentBeatTimer = 0;
            if (songStarted)
            {
                offBeat = false;
                currentState = BEAT;
                //rendererIndicator.sprite = spriteIndicatorGreen;

            }
        }
        return FMOD.RESULT.OK;
    }
    
    void Update()
    {
        if (player.GetComponent<playerHandler>().dead)
        {
            if (gameOverTimer == 0)
            {
                songStarted = false;
                soundMusic.setParameterValue("Die", 1);
                if (level != 100) AnalyticsEvent.LevelFail("Level_" + level, level, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_alive", Mathf.Round(levelTimer) } });
            }
            gameOverTimer += Time.deltaTime;
        }

        if (gameOverTimer >= 1.5f)
        {
            txtGameOver.enabled = true;
            if (level == 100)
            {
                SecondDisplay.text = "You survived for " + Mathf.Round(levelTimer) + " seconds";
                SecondDisplay.enabled = true;
                SecondCounter.enabled = false;
            }
        }
        if (gameOverTimer >= 2f && !gameOver)
        {
            if (level == 100)
            {
                // send analytics
                AnalyticsEvent.LevelComplete("level_100", 100, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_survived", Mathf.Round(levelTimer) } });

                if (Mathf.Round(levelTimer) > endlessRecord)
                {
                    endlessRecord = (int)Mathf.Round(levelTimer);
                    txtRecordAnnouncement.enabled = true;
                }
                if (currentMaxStreak > streakLevelEndlessRecord) streakLevelEndlessRecord = currentMaxStreak;
                currency = player.GetComponent<playerHandler>().currentCurrency;
                SaveSystem.SavePlayer(this);
            }
            gameOver = true;
            print("buttons");
            btnRetry.gameObject.SetActive(true);
            btnGameOver.gameObject.SetActive(true);
            btnRetry.Select();
        }

        // update progress bar
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

        // update streak parameter
        if (player.GetComponent<playerHandler>().streakLevel > 2)
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

        if (player.GetComponent<playerHandler>().currentStreak > currentMaxStreak) currentMaxStreak = player.GetComponent<playerHandler>().currentStreak;
        
        if (player.GetComponent<playerHandler>().streakLevel >= 3) GetComponent<Camera>().orthographicSize = 5.35f + (beatTimer2 / 0.6f) * 0.05f;
        else GetComponent<Camera>().orthographicSize = 5.4f;

        if (Input.GetButtonDown("Cancel"))
        {
            QuitLevel();
        }
        
        beatTimer2 += Time.deltaTime;
        currentBeatTimer += Time.deltaTime;
        
        /*
        if (beatTimer2 <= leniency && currentState != BEAT && songStarted)
        {
            offBeat = false;
            currentState = BEAT;
            rendererIndicator.sprite = spriteIndicatorGreen;
            
        }
        */
        if (beatTimer2 >= preBeatTime && currentState != SUCCESS && songStarted)
        {
            currentState = SUCCESS;
            //rendererIndicator.sprite = spriteIndicatorOrange;
            
        }
        else if (beatTimer2 > 0.15f && beatTimer2 < preBeatTime && currentState != FAIL)
        {
            currentState = FAIL;
            //rendererIndicator.sprite = spriteIndicatorEmpty;
        }
        
        if (beatTimer2 >= offBeatTime)
        {
            offBeat = true;
        }
        
        ProgressLevel();

        if (level == 100 && !player.GetComponent<playerHandler>().dead)
        {
            SecondCounter.text = "TIME: " + Mathf.Round(levelTimer).ToString();
        }

        // check practice input
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
        }
        

	}

    void SetReady()
    {
        //rendererIndicator.sprite = spriteIndicatorOrange;
    }

    void SetBeat()
    {
        //rendererIndicator.sprite = spriteIndicatorGreen;

        indicatorTimer += Time.deltaTime;

        if (indicatorTimer >= 0.1f)
        {
            indicatorTimer = 0;
            //rendererIndicator.sprite = spriteIndicatorEmpty;
        }
    }

    public void StartLevel(int lvl)
    {
        level = lvl;
        currentSpawn = 0;
        levelTimer = 0;
    }

    public void QuitLevel()
    {
        songStarted = false;
        normalLevelFinished = false;
        birdLevelFinished = false;
        lastMode = currentGameMode;
        SaveSystem.SavePlayer(this);
        // send analytics
        if (victoryTimer == 0) AnalyticsEvent.LevelQuit("level_" + level, level, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_alive", Mathf.Round(levelTimer) } });
        enemiesDead = 0;
        if (level == 2) soundAmbCafe.setParameterValue("End", 1);
        if (level == 3) soundAmbSea.setParameterValue("End", 1);
        soundMusic.setCallback(null, 0);
        soundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (level == -1) soundAvailableracticeSongs[currentSong].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("MenuScene");
    }

    public static void EnemyDead()
    {
        enemiesDead++;
    }

    void ProgressLevel()
    {
        if (!player.GetComponent<playerHandler>().dead) levelTimer += Time.deltaTime;

        // normal mode
        if (level > 0 && level < 100 && gameMode == NORMAL)
        {
            // reached end of level
            if (currentLevelSpawn.Length == currentSpawn)
            {
                bool tAllDead = true;
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (!enemies[i].GetComponent<enemyHandler>().dead) tAllDead = false;
                }

                if (tAllDead && victoryTimer == 0)
                {
                    normalLevelFinished = true;
                    victoryTimer = levelTimer;
                    txtVictory.enabled = true;
                    soundMusic.setParameterValue("Win", 1);
                }

                // save data
                if (victoryTimer > 0 && levelTimer > (victoryTimer + endWaitTimer[level - 1]))
                {
                    if (currentMaxStreak > streakRecord[level]) streakRecord[level] = currentMaxStreak;
                    clearedLevel[level] = true;
                    if (player.GetComponent<playerHandler>().currentScore > scoreRecord[level]) scoreRecord[level] = player.GetComponent<playerHandler>().currentScore;
                    if (player.GetComponent<playerHandler>().currentRank > rankRecord[level]) rankRecord[level] = player.GetComponent<playerHandler>().currentRank;

                    if (clearedLevel[1]) { unlockedLevel[2] = true; unlockedLevel[3] = true; }
                    if (clearedLevel[2] && clearedLevel[3]) unlockedLevel[4] = true;
                    currency = player.GetComponent<playerHandler>().currentCurrency;
                    SaveSystem.SavePlayer(this);

                    // send analytics
                    AnalyticsEvent.LevelComplete("Level_" + level, level, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_alive", Mathf.Round(levelTimer) } } );

                    print("save, level 1 clear: " + clearedLevel[1] + ", unlocked hell: " + unlockedLevel[4] + ", streak record: " + streakRecord[1]);
                    QuitLevel();
                }
            }
            else if (levelTimer > prevTime + currentLevelSpawn[currentSpawn].spawnTime && !waitingForDeath)
            {
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
                if (enemies.Count - enemiesDead <= currentLevelSpawn[currentSpawn].nextWaveReq)
                {
                    print("dead req met");
                    prevTime = levelTimer;
                    currentSpawn++;
                    waitingForDeath = false;
                }
            }
        }

        // bird mode
        if (level > 0 && gameMode == BIRD)
        {
            if (birdLevelFinished)
            {
                victoryTimer += Time.deltaTime;
                txtVictory.enabled = true;
                txtVictory.text = "Level Cleared";
                print("Number of birds: " + totalBirds);
            }

            if (victoryTimer > 2 )
            {
                if (currentMaxStreak > comboRecord[level]) comboRecord[level] = currentMaxStreak;
                clearedBirdLevel[level] = true;
                if (player.GetComponent<playerHandler>().currentScore > scoreBirdRecord[level]) scoreBirdRecord[level] = player.GetComponent<playerHandler>().currentScore;
                if (player.GetComponent<playerHandler>().currentRank > rankBirdRecord[level]) rankBirdRecord[level] = player.GetComponent<playerHandler>().currentRank;

                if (clearedBirdLevel[1]) { unlockedBirdLevel[2] = true; unlockedBirdLevel[3] = true; }
                if (clearedBirdLevel[2] && clearedBirdLevel[3]) unlockedBirdLevel[4] = true;
                currency = player.GetComponent<playerHandler>().currentCurrency;
                SaveSystem.SavePlayer(this);

                // send analytics
                AnalyticsEvent.LevelComplete("Level_" + level, level, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_alive", Mathf.Round(levelTimer) } });

                print("save, level 1 clear: " + clearedLevel[1] + ", unlocked hell: " + unlockedLevel[4] + ", streak record: " + streakRecord[1]);
                QuitLevel();
            }
        }

        if (level == 100)
        {
            if (currentLevelSpawn.Length == currentSpawn)
            {
                if (levelTimer >= nextSpawn)
                {
                    nextSpawn = levelTimer;
                    nextSpawn += 18 - (Mathf.Clamp(14 * (levelTimer / 300), 0, 16));
                    int tDistance = 0;
                    print("levelTimer: " + levelTimer + ", nextSpawn: " + nextSpawn);
                    for (int i = 0; i < 2 + levelTimer / 75; i++)
                    {
                        bool tAbove = false;
                        print("spawn");
                        if (Random.value > 0.25f)
                        {
                            if (Random.value < 0.3f)
                            {
                                enemies.Add(Instantiate(enemyA, new Vector3(Random.Range(-5f, 5f), 0), Quaternion.identity));
                                tAbove = true;
                            }
                            else enemies.Add(Instantiate(enemyA, new Vector3((10.1f + tDistance) * RandDirection(), 0), Quaternion.identity));
                        }
                        else
                        {
                            if (Random.value < 0.3f)
                            {
                                enemies.Add(Instantiate(enemyB, new Vector3(Random.Range(-5f, 5f), 0), Quaternion.identity));
                                tAbove = true;
                            }
                            else enemies.Add(Instantiate(enemyB, new Vector3((10.1f + tDistance) * RandDirection(), 0), Quaternion.identity));
                        }
                        enemies[enemies.Count - 1].GetComponent<enemyHandler>().Init(tAbove);
                        tDistance++;
                    }
                }
            }
            else if (levelTimer > prevTime + currentLevelSpawn[currentSpawn].spawnTime && !waitingForDeath)
            {
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

    public void RestartLevel()
    {
        soundUIClick.start();

        AnalyticsEvent.LevelStart("Level_" + level, level);

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("currency"))
        {
            Destroy(g);
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("HPPickup"))
        {
            Destroy(g);
        }

        currentSpawn = 0;
        levelTimer = 0;
        if (level == 3) levelTimer = 6.9f;
        nextSpawn = 0;
        prevTime = 0;
        currentMaxStreak = 0;
        player.GetComponent<playerHandler>().Reset();
        txtGameOver.enabled = false;
        gameOver = false;
        songStarted = false;
        btnRetry.gameObject.SetActive(false);
        btnGameOver.gameObject.SetActive(false);
        waitingForDeath = false;
        enemiesDead = 0;

        if (level == 100)
        {
            txtRecordAnnouncement.enabled = false;
            SecondCounter.enabled = true;
            SecondDisplay.enabled = false;
            txtRecordDisplay.text = "RECORD: " + endlessRecord;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);
        }
        enemies.Clear();
        gameOverTimer = 0;
        soundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        soundMusic = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        soundMusic.setCallback(callBack, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        if (level == 3) soundMusic.setParameterValue("Loop", 1);
        soundMusic.start();
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

    int RandDirection()
    {
        return 1 * (int)Mathf.Pow(-1, Random.Range((int)1, (int)3));
    }

    GameObject RandEnemy()
    {
        bool eB = Random.Range((int)0, (int)2) == 0;
        if (eB) return enemyA;
        else return enemyB;
    }

    int[] CalculateRankLimits(int nBirds)
    {
        int tPScore = 0;
        tPScore += 100 * 1 * 10;
        tPScore += (int)Mathf.Round(100 * 1.1f) * 20;
        tPScore += (int)Mathf.Round(100 * 1.3f) * 20;
        tPScore += (int)Mathf.Round(100 * 1.5f) * 25;
        tPScore += (int)Mathf.Round(100 * 1.7f) * 25;
        int tBirds = nBirds;
        tBirds -= 100;
        
        tPScore += 100 * 2 * tBirds;
        tPScore += 1000;

        print("score for P: " + tPScore);
        //fick 242000, borde vara det

        return new int[] { (int)(tPScore * 0.25f), (int)(tPScore * 0.5f), (int)(tPScore * 0.7f), (int)(tPScore * 0.8f), tPScore };
    }
}
