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


public class mainHandler : MonoBehaviour {

    public int bpm;
    public static int currentBpm;
    public float offset;
    public float leniency = 0.1f;
    public static float currentLeniency;
    float offBeatTime;
    float preBeatTime;
    
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

    float levelTimer;
    int currentSpawn;
    float prevTime;
    bool waitingForDeath;
    public static int enemiesDead;
    public int totalEnemies;

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

    public int endlessRecord;
    public bool clearedLevel1;
    public bool clearedLevel2;
    public bool clearedLevel3;
    public bool unlockedLevelsA;
    public bool unlockedLevelsB;
    public int streakLevel1Record;
    public int streakLevel2Record;
    public int streakLevel3Record;
    public int streakLevelEndlessRecord;
    int currentMaxStreak;
    public int currency;

    FMOD.Studio.EventInstance soundAmbCafe;
    FMOD.Studio.EventInstance soundAmbSea;

    FMOD.Studio.EventInstance soundMusic;
    FMOD.Studio.EVENT_CALLBACK callBack;

    FMOD.Studio.EventInstance soundUIClick;

    FMOD.Studio.EventInstance soundSinus;

    public string musicPath;

    float beatTimer2;
    public static float currentBeatTimer;

    void Awake()
    {
        currentBpm = bpm;
        currentLeniency = leniency;
    }
    
    void Start()
    {
        // set music
        soundMusic = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        offBeatTime = 60 / ((float)bpm * 2);
        preBeatTime = 60 / (float)bpm - leniency;
        
        callBack = new FMOD.Studio.EVENT_CALLBACK(StudioEventCallback);
        soundMusic.setCallback(callBack, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        soundMusic.start();

        soundAmbCafe = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Amb_cafe");
        soundAmbSea = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Amb_sea");

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");

        soundSinus = FMODUnity.RuntimeManager.CreateInstance("event:/Sinus_test");

        enemies = new List<GameObject>();

        // load data
        PlayerData data = SaveSystem.LoadPlayer();
        endlessRecord = data.endlessRecord;
        clearedLevel1 = data.clearedLevel1;
        clearedLevel2 = data.clearedLevel2;
        clearedLevel3 = data.clearedLevel3;
        unlockedLevelsA = data.unlockedLevelsA;
        unlockedLevelsB = data.unlockedLevelsB;
        streakLevel1Record = data.streakLevel1Record;
        streakLevel2Record = data.streakLevel2Record;
        streakLevel3Record = data.streakLevel3Record;
        streakLevelEndlessRecord = data.streakLevelEndlessRecord;
        currency = data.currency;

        player.GetComponent<playerHandler>().Init(currency);

        currentSpawn = 0;
        levelTimer = 0;
        enemiesDead = 0;
        victoryTimer = 0;
        totalEnemies = 0;
        songStarted = false;

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

        level1Spawn = new EnemySpawn[] {
            new EnemySpawn(6, new GameObject[] { enemyD }, new float[] { 10 }, new bool[] { false }, 0 ),
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
            new EnemySpawn(2, new GameObject[] { enemyD }, new float[] { -6 }, new bool[] { true }),
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

        if (level <= 0) currentLevelSpawn = levelTrainingSpawn;
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

        for (int i = 0; i < currentLevelSpawn.Length; i++)
        {
            for (int j = 0; j < currentLevelSpawn[i].enemies.Length; j++)
            {
                totalEnemies++;
            }
        }

        print("total enemies: " + totalEnemies);

        if (level == 2) soundAmbCafe.start();
        if (level == 3) soundAmbSea.start();

        staticLevel = level;

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
                player.GetComponent<playerHandler>().birds.Add(Instantiate(enemyBird, new Vector3(0f, 0f), Quaternion.identity));
                if (name == "B1") player.GetComponent<playerHandler>().birds[player.GetComponent<playerHandler>().birds.Count - 1].GetComponent<enemyBirdHandler>().init(1);
                if (name == "B2") player.GetComponent<playerHandler>().birds[player.GetComponent<playerHandler>().birds.Count - 1].GetComponent<enemyBirdHandler>().init(2);
                if (name == "B4") player.GetComponent<playerHandler>().birds[player.GetComponent<playerHandler>().birds.Count - 1].GetComponent<enemyBirdHandler>().init(0);
            }
        }
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
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
        }

        // update progress bar
        if (level > 0 && level < 100)
        {
            maskProgressFill.transform.localPosition = new Vector3(-4.66f * (1 - ((float)enemiesDead / (float)totalEnemies)), 0);
            rendererProgressMarker.transform.localPosition = new Vector3(-2.22f + 4.44f * ((float)enemiesDead / (float)totalEnemies), 0);
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
        if (victoryTimer == 0) AnalyticsEvent.LevelQuit("level_" + level, level, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_alive", Mathf.Round(levelTimer) } });
        enemiesDead = 0;
        if (level == 2) soundAmbCafe.setParameterValue("End", 1);
        if (level == 3) soundAmbSea.setParameterValue("End", 1);
        soundMusic.setCallback(null, 0);
        soundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("MenuScene");
    }

    public static void EnemyDead()
    {
        enemiesDead++;
    }

    void ProgressLevel()
    {
        if (!player.GetComponent<playerHandler>().dead) levelTimer += Time.deltaTime;

        if (level > 0 && level < 100)
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
                    victoryTimer = levelTimer;
                    txtVictory.enabled = true;
                    soundMusic.setParameterValue("Win", 1);
                }

                if (victoryTimer > 0 && levelTimer > (victoryTimer + endWaitTimer[level - 1]))
                {
                    if (level == 1)
                    {
                        if (currentMaxStreak > streakLevel1Record) streakLevel1Record = currentMaxStreak;
                        clearedLevel1 = true;
                        unlockedLevelsA = true;
                    }
                    if (level == 2)
                    {
                        if (currentMaxStreak > streakLevel2Record) streakLevel2Record = currentMaxStreak;
                        clearedLevel2 = true;
                    }
                    if (level == 3)
                    {
                        if (currentMaxStreak > streakLevel3Record) streakLevel3Record = currentMaxStreak;
                        clearedLevel3 = true;
                    }
                    if (clearedLevel2 && clearedLevel3) unlockedLevelsB = true;
                    currency = player.GetComponent<playerHandler>().currentCurrency;
                    SaveSystem.SavePlayer(this);

                    // send analytics
                    AnalyticsEvent.LevelComplete("Level_" + level, level, new Dictionary<string, object> { { "max_streak", currentMaxStreak }, { "time_alive", Mathf.Round(levelTimer) } } );

                    print("save, clear: " + clearedLevel1 + ", " + clearedLevel2 + ", " + clearedLevel3 + ", unlocked: " + unlockedLevelsA + ", " + unlockedLevelsB + ", record: " + endlessRecord + ", streak records: " + streakLevel1Record + " " + streakLevel2Record + " " + streakLevel3Record);
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
}
