using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SynchronizerData;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainHandler : MonoBehaviour {

    int bpm;
    public float offset;

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

    //public AudioSource audioPlayer;
    //public AudioClip audioSong140;

    float beatTime;
    float beatTimer;
    float offsetTimer;

    float indicatorTimer;
    bool indicatorOn;
    bool indicatorReady;

    public int level;
    bool playingLevel;
    float levelTimer;
    int currentSpawn;

    float nextSpawn;

    readonly int FAIL = 0, SUCCESS = 1, BEAT = 2, OFFBEAT = 3;
    int currentState;

    private BeatObserver beatObserver;

    public Text SecondCounter;
    float victoryTimer;

    FMOD.Studio.EventInstance soundAmbCafe;
    FMOD.Studio.EventInstance soundAmbSea;

    // Use this for initialization
    void Start () {
        //beatTime = 500;

        soundAmbCafe = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Amb_cafe");
        soundAmbSea = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Amb_sea");

        enemies = new List<GameObject>();

        //audioPlayer = GetComponent<AudioSource>();

        beatObserver = GetComponent<BeatObserver>();

        currentSpawn = 0;
        levelTimer = 0;

        if (level == 2) soundAmbCafe.start();
        if (level == 3) soundAmbSea.start();
    }
	
	// Update is called once per frame
	void Update() {

        if (player.GetComponent<playerHandler>().dead)
        {
            gameOverTimer += Time.deltaTime;
        }

        if (gameOverTimer >= 3)
        {
            txtGameOver.enabled = true;
        }
        if (gameOverTimer >= 5)
        {
            if (level == 2) soundAmbCafe.setParameterValue("End", 1);
            if (level == 3) soundAmbSea.setParameterValue("End", 1);
            SceneManager.LoadScene("MenuScene");
        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (level == 2) soundAmbCafe.setParameterValue("End", 1);
            if (level == 3) soundAmbSea.setParameterValue("End", 1);
            SceneManager.LoadScene("MenuScene");
        }

        if ((beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat && currentState != BEAT)
        {
            currentState = BEAT;
            rendererIndicator.sprite = spriteIndicatorGreen;
            
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "enemy") enemies[i].GetComponent<enemyAHandler>().setState(BEAT);
                else enemies[i].GetComponent<enemyBHandler>().setState(BEAT);
            }
            
        }
        else if ((beatObserver.beatMask & BeatType.OffBeat) == BeatType.OffBeat && currentState != SUCCESS)
        {
            currentState = SUCCESS;

            
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "enemy") enemies[i].GetComponent<enemyAHandler>().setState(SUCCESS);
                else enemies[i].GetComponent<enemyBHandler>().setState(SUCCESS);
            }
            

            rendererIndicator.sprite = spriteIndicatorOrange;
            //player.GetComponent<playerHandler>().SetBeatState(SUCCESS);
            
        }
        else if (!((beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat) &&
                  !((beatObserver.beatMask & BeatType.OffBeat) == BeatType.OffBeat) && currentState != FAIL)
        {
            currentState = FAIL;
            rendererIndicator.sprite = spriteIndicatorEmpty;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "enemy") enemies[i].GetComponent<enemyAHandler>().setState(FAIL);
                else enemies[i].GetComponent<enemyBHandler>().setState(FAIL);

            }
        }

        if ((beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "enemy") enemies[i].GetComponent<enemyAHandler>().setState(OFFBEAT);
                else enemies[i].GetComponent<enemyBHandler>().setState(OFFBEAT);
            }

        }

        ProgressLevel();

        if (level == 100 && !player.GetComponent<playerHandler>().dead)
        {
            SecondCounter.text = Mathf.Round(levelTimer).ToString();
        }







        /*
        if (playingLevel) { ProgressLevel(); }

        if (!playingLevel) return;

        if (offsetTimer < offset)
        {
            offsetTimer += Time.deltaTime;
            return;
        }
        */
        
        


            /*
            beatTimer += Time.deltaTime * 1000;

            if (beatTimer >= beatTime)
            {
                print("beat");
                beatTimer = 0;
                //beatTimer -= beatTime;
                indicatorOn = true;
                indicatorReady = false;
            }
            else if (beatTimer >= (beatTime - 100))
            {
                indicatorReady = true;
            }
            

            if (indicatorReady)
        {
            player.GetComponent<playerHandler>().SetBeatState(SUCCESS);
            SetReady();
        }
        else if (indicatorOn)
        {
            player.GetComponent<playerHandler>().SetBeatState(BEAT);
            SetBeat();
        }
        else
        {
            player.GetComponent<playerHandler>().SetBeatState(FAIL);
        }
        */

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
            indicatorOn = false;
        }
    }

    public void StartLevel(int lvl)
    {
        level = lvl;
        //if (lvl == 1) audioPlayer.clip = audioSong140;
        
        //bpm = 140;
        //beatTime = 60000 / bpm;
        //offset = 0.11f;
        //offsetTimer = 0;
        //beatTimer = 0;
        playingLevel = true;
        currentSpawn = 0;
        levelTimer = 0;
        //audioPlayer.Play();

        print(lvl);
    }

    void ProgressLevel()
    {
        //enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
        //enemies.Add(Instantiate(enemyB, new Vector3(10.1f, 0.98f), Quaternion.identity));

        if (!player.GetComponent<playerHandler>().dead) levelTimer += Time.deltaTime;
        if (level == 10 && currentSpawn == 0)
        {
            currentSpawn++;
            enemies.Add(Instantiate(enemyA, new Vector3(11.1f, 0f), Quaternion.identity));
        }

        if (level == 1)
        {
            if (levelTimer > 2 && currentSpawn == 0)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 14 && currentSpawn == 1)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 22 && currentSpawn == 2)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 24 && currentSpawn == 3)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 29 && currentSpawn == 4)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 42 && currentSpawn == 5)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(10.1f, 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 54 && currentSpawn == 6)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 59 && currentSpawn == 7)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 64 && currentSpawn == 8)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyB, new Vector3(-10.1f, 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 67 && currentSpawn == 9)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 79 && currentSpawn == 10)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(10.1f, 0.98f), Quaternion.identity));
                enemies.Add(Instantiate(enemyB, new Vector3(-10.1f, 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 80)
            {
                bool tAllDead = true;
                for (int i = 0; i < enemies.Count; i++)
                {
                    try
                    {
                        if (!enemies[i].GetComponent<enemyAHandler>().dead) tAllDead = false;
                    }
                    catch (System.Exception)
                    {
                        if (!enemies[i].GetComponent<enemyBHandler>().dead) tAllDead = false;
                    }
                    
                }

                if (tAllDead && victoryTimer == 0)
                {
                    victoryTimer = levelTimer;
                    txtVictory.enabled = true;
                }

                
            }
            if (victoryTimer > 0 && levelTimer > (victoryTimer + 3))
            {
                if (level == 2) soundAmbCafe.setParameterValue("End", 1);
                if (level == 3) soundAmbSea.setParameterValue("End", 1);
                SceneManager.LoadScene("MenuScene");
            }
            

        }

        if (level == 2)
        {
            if (levelTimer > 2 && currentSpawn == 0)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyB, new Vector3(12.1f, 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 10 && currentSpawn == 1)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(-12.1f, 0.98f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 22 && currentSpawn == 2)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 24 && currentSpawn == 3)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 26 && currentSpawn == 4)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 28 && currentSpawn == 5)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(11.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 30 && currentSpawn == 6)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-11.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 45 && currentSpawn == 7)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(12.1f, 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 46 && currentSpawn == 8)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 47 && currentSpawn == 9)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 48 && currentSpawn == 10)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 49 && currentSpawn == 11)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 52 && currentSpawn == 12)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(-10.1f, 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 53)
            {
                bool tAllDead = true;
                for (int i = 0; i < enemies.Count; i++)
                {
                    try
                    {
                        if (!enemies[i].GetComponent<enemyAHandler>().dead) tAllDead = false;
                    }
                    catch (System.Exception)
                    {
                        if (!enemies[i].GetComponent<enemyBHandler>().dead) tAllDead = false;
                    }

                }

                if (tAllDead && victoryTimer == 0)
                {
                    victoryTimer = levelTimer;
                    txtVictory.enabled = true;
                }


            }
            if (victoryTimer > 0 && levelTimer > (victoryTimer + 3))
            {
                SceneManager.LoadScene("MenuScene");
            }


        }

        if (level == 3)
        {
            if (levelTimer > 2 && currentSpawn == 0)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(-10.1f, 0.98f), Quaternion.identity));
                enemies.Add(Instantiate(enemyB, new Vector3(12.1f, 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 14 && currentSpawn == 1)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-12.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 20 && currentSpawn == 2)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(12.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 24 && currentSpawn == 3)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-12.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-14.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 28 && currentSpawn == 4)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(12.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(14.1f, 0f), Quaternion.identity));
                currentSpawn++;

            }
            if (levelTimer > 42 && currentSpawn == 5)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(-10.1f, 0.98f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-11.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyB, new Vector3(-12.1f, 0.98f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(12.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 60 && currentSpawn == 6)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(-11.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(11.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 63 && currentSpawn == 7)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(-10.1f, 0.98f), Quaternion.identity));
                enemies.Add(Instantiate(enemyB, new Vector3(-12.1f, 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 70 && currentSpawn == 8)
            {
                enemies.Add(Instantiate(enemyB, new Vector3(10.1f, 0.98f), Quaternion.identity));
                enemies.Add(Instantiate(enemyB, new Vector3(12.1f, 0.98f), Quaternion.identity));
                currentSpawn++;
            }
            if (levelTimer > 90 && currentSpawn == 9)
            {
                enemies.Add(Instantiate(enemyA, new Vector3(10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(11.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(12.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(13.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(14.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(15.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-10.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-11.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-12.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-13.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-14.1f, 0f), Quaternion.identity));
                enemies.Add(Instantiate(enemyA, new Vector3(-15.1f, 0f), Quaternion.identity));
                currentSpawn++;
            }

            if (levelTimer > 95)
            {
                bool tAllDead = true;
                for (int i = 0; i < enemies.Count; i++)
                {
                    try
                    {
                        if (!enemies[i].GetComponent<enemyAHandler>().dead) tAllDead = false;
                    }
                    catch (System.Exception)
                    {
                        if (!enemies[i].GetComponent<enemyBHandler>().dead) tAllDead = false;
                    }

                }

                if (tAllDead && victoryTimer == 0)
                {
                    victoryTimer = levelTimer;
                    txtVictory.enabled = true;
                }


            }
            if (victoryTimer > 0 && levelTimer > (victoryTimer + 3))
            {
                SceneManager.LoadScene("MenuScene");
            }
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
