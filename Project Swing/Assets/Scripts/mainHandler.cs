using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SynchronizerData;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Text;


public class mainHandler : MonoBehaviour {

    public int bpm;
    public static int currentBpm;
    public float offset;
    public float leniency = 0.1f;
    float offBeatTime;
    float preBeatTime;

    public SpriteRenderer rendererIndicator;
    public Sprite spriteIndicatorEmpty;
    public Sprite spriteIndicatorOrange;
    public Sprite spriteIndicatorGreen;

    public GameObject enemyA;
    public GameObject enemyB;

    List<GameObject> enemies;

    public GameObject player;

    public Text txtVictory;
    public Text txtGameOver;
    float gameOverTimer;
    bool gameOver;

    public Button btnRetry;
    public Button btnGameOver;

    float beatTime;
    float beatTimer;
    float offsetTimer;

    float indicatorTimer;
    bool indicatorReady;

    public int level;

    float levelTimer;
    int currentSpawn;
    float prevTime;

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
    EnemySpawn[] testSpawn;
    EnemySpawn[] currentLevelSpawn;
    float[] endWaitTimer;

    public int endlessRecord;
    public bool clearedLevel1;
    public bool clearedLevel2;
    public bool clearedLevel3;
    public bool unlockedLevelsA;
    public bool unlockedLevelsB;

    FMOD.Studio.EventInstance soundAmbCafe;
    FMOD.Studio.EventInstance soundAmbSea;

    FMOD.Studio.EventInstance soundMusic;
    FMOD.Studio.EVENT_CALLBACK callBack;

    FMOD.Studio.EventInstance soundUIClick;
    
    public string musicPath;

    float beatTimer2;

    void Awake()
    {
        currentBpm = bpm;
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

        enemies = new List<GameObject>();

        // load data
        PlayerData data = SaveSystem.LoadPlayer();

        endlessRecord = data.endlessRecord;
        clearedLevel1 = data.clearedLevel1;
        clearedLevel2 = data.clearedLevel2;
        clearedLevel3 = data.clearedLevel3;
        unlockedLevelsA = data.unlockedLevelsA;
        unlockedLevelsB = data.unlockedLevelsB;

        currentSpawn = 0;
        levelTimer = 0;

        levelTrainingSpawn = new EnemySpawn[] { };

        levelEndlessSpawn = new EnemySpawn[] {
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }),
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }),
            new EnemySpawn(3, new GameObject[] { enemyB }, new float[] { 10 * RandDirection() }),
            new EnemySpawn(3, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }),
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() }),
            new EnemySpawn(4, new GameObject[] { enemyB }, new float[] { 10 * RandDirection() }),
            new EnemySpawn(8, new GameObject[] { enemyA, enemyA }, new float[] { 10 * RandDirection(), 10 * RandDirection() }),
            new EnemySpawn(8, new GameObject[] { enemyA, enemyA, enemyB }, new float[] { 10 * RandDirection(), 10 * RandDirection(), 10 * RandDirection() }),
            new EnemySpawn(10, new GameObject[] { enemyA }, new float[] { 10 * RandDirection() })
        };

        level1Spawn = new EnemySpawn[] {
            new EnemySpawn(6, new GameObject[] { enemyA }, new float[] { 10 }),
            new EnemySpawn(12, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(8, new GameObject[] { enemyA }, new float[] { 10 }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(5, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }),
            new EnemySpawn(13, new GameObject[] { enemyB }, new float[] { 10 }),
            new EnemySpawn(12, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }),
            new EnemySpawn(5, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }),
            new EnemySpawn(5, new GameObject[] { enemyB, enemyA }, new float[] { -10, 10 }),
            new EnemySpawn(3, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(12, new GameObject[] { enemyB, enemyB }, new float[] { -10, 10 })
        };

        level2Spawn = new EnemySpawn[] {
            new EnemySpawn(7, new GameObject[] { enemyA, enemyB }, new float[] { -10, 12 }),
            new EnemySpawn(8, new GameObject[] { enemyB, enemyA }, new float[] { -12, 10 }),
            new EnemySpawn(12, new GameObject[] { enemyA }, new float[] { 10 }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { 10 }),
            new EnemySpawn(2, new GameObject[] { enemyA, enemyA }, new float[] { 10, 11 }),
            new EnemySpawn(2, new GameObject[] { enemyA, enemyA }, new float[] { -10, -11 }),
            new EnemySpawn(11, new GameObject[] { enemyB }, new float[] { 12 }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(1, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(3, new GameObject[] { enemyB }, new float[] { -10 }),
        };

        level3Spawn = new EnemySpawn[] {
            new EnemySpawn(10, new GameObject[] { enemyB, enemyB }, new float[] { -11, 11 }),
            new EnemySpawn(12, new GameObject[] { enemyA, enemyA }, new float[] { -10, -12 }),
            new EnemySpawn(6, new GameObject[] { enemyA, enemyA }, new float[] { 10, 12 }),
            new EnemySpawn(4, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { -10, -12, -14 }),
            new EnemySpawn(4, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { 10, 12, 14 }),
            new EnemySpawn(14, new GameObject[] { enemyB, enemyA, enemyB, enemyA, enemyA }, new float[] { -10, -11, -12, 10, 12 }),
            new EnemySpawn(18, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11 }),
            new EnemySpawn(3, new GameObject[] { enemyB, enemyB }, new float[] { -10, -12 }),
            new EnemySpawn(8, new GameObject[] { enemyB, enemyB }, new float[] { 10, 12 }),
            new EnemySpawn(20, new GameObject[] { enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA }, new float[] { 10, 11, 12, 13, 14, 15, -10, -11, -12, -13, -14, -15 })
        };

        testSpawn = new EnemySpawn[] {
            new EnemySpawn(2, new GameObject[] { enemyB }, new float[] { 10 }),

        };

        endWaitTimer = new float[] { 6.3f, 5f, 7.4f, 0, 0, 0, 0, 5, 5, 5 };

        if (level == 0) currentLevelSpawn = levelTrainingSpawn;
        if (level == 1) currentLevelSpawn = level1Spawn;
        if (level == 2) currentLevelSpawn = level2Spawn;
        if (level == 3) currentLevelSpawn = level3Spawn;
        if (level == 10) currentLevelSpawn = testSpawn;
        if (level == 100)
        {
            SecondCounter.enabled = true;
            currentLevelSpawn = levelEndlessSpawn;
            txtRecordDisplay.enabled = true;
            txtRecordDisplay.text = "RECORD: " + endlessRecord;
        }

        if (level == 2) soundAmbCafe.start();
        if (level == 3) soundAmbSea.start();
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
            if (name.Contains("1"))
            {
                if (name == "128") soundMusic.setParameterValue("Intro", 1);
                bpm = int.Parse(name);
                offBeatTime = 60 / ((float)bpm * 2);
                preBeatTime = 60 / (float)bpm - leniency;
            }
        }
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            //FMOD.Studio.TIMELINE_BEAT_PROPERTIES beat = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
            beatTimer2 = 0;
        }
        return FMOD.RESULT.OK;
    }
    
    void Update() {

        if (player.GetComponent<playerHandler>().dead)
        {
            if (gameOverTimer == 0) soundMusic.setParameterValue("Die", 1);
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
                if (Mathf.Round(levelTimer) > endlessRecord)
                {
                    endlessRecord = (int)Mathf.Round(levelTimer);
                    txtRecordAnnouncement.enabled = true;
                }
                SaveSystem.SavePlayer(this);

            }
            gameOver = true;
            print("buttons");
            btnRetry.gameObject.SetActive(true);
            btnGameOver.gameObject.SetActive(true);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            QuitLevel();
        }
        
        beatTimer2 += Time.deltaTime;
        
        if (beatTimer2 <= leniency && currentState != BEAT && songStarted)
        {
            offBeat = false;
            currentState = BEAT;
            rendererIndicator.sprite = spriteIndicatorGreen;
            
        }
        else if (beatTimer2 >= preBeatTime && currentState != SUCCESS && songStarted)
        {
            currentState = SUCCESS;
            rendererIndicator.sprite = spriteIndicatorOrange;
            
        }
        else if (beatTimer2 > 0.15f && beatTimer2 < preBeatTime && currentState != FAIL)
        {
            currentState = FAIL;
            rendererIndicator.sprite = spriteIndicatorEmpty;
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
        rendererIndicator.sprite = spriteIndicatorOrange;
    }

    void SetBeat()
    {
        rendererIndicator.sprite = spriteIndicatorGreen;

        indicatorTimer += Time.deltaTime;

        if (indicatorTimer >= 0.1f)
        {
            indicatorTimer = 0;
            rendererIndicator.sprite = spriteIndicatorEmpty;
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
        if (level == 2) soundAmbCafe.setParameterValue("End", 1);
        if (level == 3) soundAmbSea.setParameterValue("End", 1);
        soundMusic.setCallback(null, 0);
        soundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("MenuScene");
    }

    void ProgressLevel()
    {
        if (!player.GetComponent<playerHandler>().dead) levelTimer += Time.deltaTime;

        if (level > 0 && level < 100)
        {
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
                        clearedLevel1 = true;
                        unlockedLevelsA = true;
                    }
                    if (level == 2) clearedLevel2 = true;
                    if (level == 3) clearedLevel3 = true;
                    if (clearedLevel2 && clearedLevel3) unlockedLevelsB = true;
                    SaveSystem.SavePlayer(this);
                    print("save, clear: " + clearedLevel1 + ", " + clearedLevel2 + ", " + clearedLevel3 + ", unlocked: " + unlockedLevelsA + ", " + unlockedLevelsB + ", record: " + endlessRecord);
                    QuitLevel();
                }
            }
            else if (levelTimer > prevTime + currentLevelSpawn[currentSpawn].spawnTime)
            {
                for (int i = 0; i < currentLevelSpawn[currentSpawn].enemies.Length; i++)
                {
                    enemies.Add(Instantiate(currentLevelSpawn[currentSpawn].enemies[i], new Vector3(currentLevelSpawn[currentSpawn].xPos[i], 0), Quaternion.identity));
                }
                prevTime = levelTimer;
                currentSpawn++;
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
                        print("spawn");
                        if (Random.value > 0.25f) enemies.Add(Instantiate(enemyA, new Vector3((10.1f + tDistance) * RandDirection(), 0f), Quaternion.identity));
                        else enemies.Add(Instantiate(enemyB, new Vector3((10.1f + tDistance) * RandDirection(), 0.98f), Quaternion.identity));
                        tDistance++;
                    }
                }
            }
            else if (levelTimer > prevTime + currentLevelSpawn[currentSpawn].spawnTime)
            {
                for (int i = 0; i < currentLevelSpawn[currentSpawn].enemies.Length; i++)
                {
                    enemies.Add(Instantiate(currentLevelSpawn[currentSpawn].enemies[i], new Vector3(currentLevelSpawn[currentSpawn].xPos[i], 0), Quaternion.identity));
                }
                prevTime = levelTimer;
                currentSpawn++;
            }
        }
    }

    public void RestartLevel()
    {
        soundUIClick.start();
        
        currentSpawn = 0;
        levelTimer = 0;
        if (level == 3) levelTimer = 6.9f;
        nextSpawn = 0;
        prevTime = 0;
        player.GetComponent<playerHandler>().Reset();
        txtGameOver.enabled = false;
        gameOver = false;
        songStarted = false;
        btnRetry.gameObject.SetActive(false);
        btnGameOver.gameObject.SetActive(false);

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
