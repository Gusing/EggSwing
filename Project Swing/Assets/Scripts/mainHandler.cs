using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SynchronizerData;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;

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

    float nextSpawn;

    readonly int FAIL = 0, SUCCESS = 1, BEAT = 2, OFFBEAT = 3;
    public static int currentState;
    public static bool offBeat;

    public Text SecondCounter;
    float victoryTimer;

    EnemySpawn[] level1Spawn;
    EnemySpawn[] level2Spawn;
    EnemySpawn[] level3Spawn;
    EnemySpawn[] testSpawn;
    EnemySpawn[] currentLevelSpawn;
    float[] endWaitTimer;
    
    FMOD.Studio.EventInstance soundAmbCafe;
    FMOD.Studio.EventInstance soundAmbSea;

    FMOD.Studio.EventInstance soundMusic;
    FMOD.Studio.EVENT_CALLBACK callBack;

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

        enemies = new List<GameObject>();

        currentSpawn = 0;
        levelTimer = 0;

        level1Spawn = new EnemySpawn[] {
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { 10 }),
            new EnemySpawn(14, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(22, new GameObject[] { enemyA }, new float[] { 10 }),
            new EnemySpawn(24, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(29, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }),
            new EnemySpawn(42, new GameObject[] { enemyB }, new float[] { 10 }),
            new EnemySpawn(54, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }),
            new EnemySpawn(59, new GameObject[] { enemyA, enemyA }, new float[] { -10, 10 }),
            new EnemySpawn(64, new GameObject[] { enemyB, enemyA }, new float[] { -10, 10 }),
            new EnemySpawn(67, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(79, new GameObject[] { enemyB, enemyB }, new float[] { -10, 10 })
        };

        level2Spawn = new EnemySpawn[] {
            new EnemySpawn(2, new GameObject[] { enemyA, enemyB }, new float[] { -10, 12 }),
            new EnemySpawn(10, new GameObject[] { enemyB, enemyA }, new float[] { -12, 10 }),
            new EnemySpawn(22, new GameObject[] { enemyA }, new float[] { 10 }),
            new EnemySpawn(24, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(26, new GameObject[] { enemyA }, new float[] { 10 }),
            new EnemySpawn(28, new GameObject[] { enemyA, enemyA }, new float[] { 10, 11 }),
            new EnemySpawn(30, new GameObject[] { enemyA, enemyA }, new float[] { -10, -11 }),
            new EnemySpawn(45, new GameObject[] { enemyB }, new float[] { 12 }),
            new EnemySpawn(46, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(47, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(48, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(49, new GameObject[] { enemyA }, new float[] { -10 }),
            new EnemySpawn(52, new GameObject[] { enemyB }, new float[] { -10 }),
        };

        level3Spawn = new EnemySpawn[] {
            new EnemySpawn(2, new GameObject[] { enemyB, enemyB }, new float[] { -11, 11 }),
            new EnemySpawn(14, new GameObject[] { enemyA, enemyA }, new float[] { -10, -12 }),
            new EnemySpawn(20, new GameObject[] { enemyA, enemyA }, new float[] { 10, 12 }),
            new EnemySpawn(24, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { -10, -12, -14 }),
            new EnemySpawn(28, new GameObject[] { enemyA, enemyA, enemyA }, new float[] { 10, 12, 14 }),
            new EnemySpawn(42, new GameObject[] { enemyB, enemyA, enemyB, enemyA, enemyA }, new float[] { -10, -11, -12, 10, 12 }),
            new EnemySpawn(60, new GameObject[] { enemyA, enemyA }, new float[] { 11, -11 }),
            new EnemySpawn(63, new GameObject[] { enemyB, enemyB }, new float[] { -10, -12 }),
            new EnemySpawn(70, new GameObject[] { enemyB, enemyB }, new float[] { 10, 12 }),
            new EnemySpawn(90, new GameObject[] { enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA, enemyA }, new float[] { 10, 11, 12, 13, 14, 15, -10, -11, -12, -13, -14, -15 })
        };

        testSpawn = new EnemySpawn[] {
            new EnemySpawn(2, new GameObject[] { enemyA }, new float[] { 10 }),

        };

        endWaitTimer = new float[] { 6,3f, 5f, 7,4f, 0, 0, 0, 0, 5, 5, 5 };

        if (level == 1) currentLevelSpawn = level1Spawn;
        if (level == 2) currentLevelSpawn = level2Spawn;
        if (level == 3) currentLevelSpawn = level3Spawn;
        if (level == 10) currentLevelSpawn = testSpawn;

        if (level == 2) soundAmbCafe.start();
        if (level == 3) soundAmbSea.start();
    }

    public FMOD.RESULT StudioEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance eventInstance, System.IntPtr parameters)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
        {
            //FMOD.Studio.TIMELINE_MARKER_PROPERTIES marker = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameters, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
           
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
        }
        if (gameOverTimer >= 2f && !gameOver)
        {
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
        
        if (beatTimer2 <= leniency && currentState != BEAT)
        {
            offBeat = false;
            currentState = BEAT;
            rendererIndicator.sprite = spriteIndicatorGreen;
            
        }
        else if (beatTimer2 >= preBeatTime && currentState != SUCCESS)
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
            SecondCounter.text = Mathf.Round(levelTimer).ToString();
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
                QuitLevel();
            }
        }
        else if (levelTimer > currentLevelSpawn[currentSpawn].spawnTime)
        {
            for (int i = 0; i < currentLevelSpawn[currentSpawn].enemies.Length; i++)
            {
                enemies.Add(Instantiate(currentLevelSpawn[currentSpawn].enemies[i], new Vector3(currentLevelSpawn[currentSpawn].xPos[i], 0), Quaternion.identity));
            }
            currentSpawn++;
        }




        if (level == 100)
        {
            
            if (levelTimer > 2 && currentSpawn == 0)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 8 && currentSpawn == 1)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 10 && currentSpawn == 2)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 11 && currentSpawn == 3)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 12 && currentSpawn == 4)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 15 && currentSpawn == 5)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(10.1f * RandDirection(), 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 18 && currentSpawn == 6)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 24 && currentSpawn == 7)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 28 && currentSpawn == 8)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(10.1f * RandDirection(), 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 34 && currentSpawn == 9)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 38 && currentSpawn == 10)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f * RandDirection(), 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyB, new Vector3(10.1f * RandDirection(), 0.98f), Quaternion.identity));
                currentSpawn++;
                nextSpawn = 46;
            }
            if (currentSpawn == 11)
            {
                if (levelTimer >= nextSpawn)
                {
                    nextSpawn += 15 - (Mathf.Clamp(14 * (levelTimer / 150), 0, 14));
                    int tDistance = 0;
                    for (int i = 0; i < 2 + levelTimer/30; i++)
                    {
                        if (Random.value > 0.5f) enemies.Add(Instantiate(enemyA, new Vector3((10.1f + tDistance) * RandDirection(), 0f), Quaternion.identity));
                        else enemies.Add(Instantiate(enemyB, new Vector3((10.1f + tDistance) * RandDirection(), 0.98f), Quaternion.identity));
                        tDistance++;
                    }
                }
            }

        }
    }

    public void RestartLevel()
    {
        currentSpawn = 0;
        levelTimer = 0;
        player.GetComponent<playerHandler>().Reset();
        txtGameOver.enabled = false;
        gameOver = false;
        btnRetry.gameObject.SetActive(false);
        btnGameOver.gameObject.SetActive(false);
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);
        }
        enemies.Clear();
        gameOverTimer = 0;
        soundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        soundMusic = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        soundMusic.setCallback(callBack, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
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
