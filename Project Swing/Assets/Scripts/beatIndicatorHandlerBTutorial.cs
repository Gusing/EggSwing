using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatIndicatorHandlerBTutorial : MonoBehaviour
{
    public GameObject indicatorLine;
    
    public Sprite spriteGreat;
    public Sprite spriteOk;
    public Sprite spriteBad;

    public SpriteRenderer rendererIcon;
    public SpriteRenderer rendererFeedback;

    float timeBeforeCollision = 0.5f;
    static float staticTimeBeforeCollision;
    float startingX = 2;
    static float staticStartingX;
    public float greatLimit = 0.04f;
    float bpmInSeconds;
    float timeToSpawn;
    bool recordedBeat;
    bool readyforNextRing;
    bool ringChanged;
    float hitTimer;
    float hitTime;
    int localBpm;
    int hitState;
    bool songStarted;
    bool stopped;
    
    List<GameObject> lines;

    void Awake()
    {
        staticTimeBeforeCollision = timeBeforeCollision;
        staticStartingX = startingX;
    }

    // Start is called before the first frame update
    void Start()
    {
        lines = new List<GameObject>();

        localBpm = mainHandlerTutorial.currentBpm;

        bpmInSeconds = (float)60 / (float)localBpm;

        hitTime = 0.13f;
        if (localBpm > 130) hitTime = 0.1f;

        recordedBeat = false;

        //timeToSpawn = ((bpmInSeconds - timeBeforeCollision) + bpmInSeconds) % bpmInSeconds;
        
        print("60 / bpm: " + (float)60 / (float)localBpm + ", time to spawn: " + timeToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if (mainHandlerTutorial.currentBpm != localBpm)
        {
            print("diff bpm");
            localBpm = mainHandlerTutorial.currentBpm;

            bpmInSeconds = (float)60 / (float)localBpm;

            hitTime = 0.13f;
            if (localBpm > 130) hitTime = 0.1f;

            for (int i = lines.Count - 8; i < lines.Count; i++)
            {
                if (i >= 0 && lines[i] != null) lines[i].GetComponent<beatLineHandler>().Kill();

            }
            lines.Clear();
            if (mainHandlerTutorial.songStarted)
            {
                lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
                lines[lines.Count - 1].transform.parent = gameObject.transform;
                lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(true, bpmInSeconds, 1);
                lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
                lines[lines.Count - 1].transform.parent = gameObject.transform;
                lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(true, bpmInSeconds, 2);
                lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
                lines[lines.Count - 1].transform.parent = gameObject.transform;
                lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(true, bpmInSeconds, 3);
                lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
                lines[lines.Count - 1].transform.parent = gameObject.transform;
                lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(false, bpmInSeconds, 1);
                lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
                lines[lines.Count - 1].transform.parent = gameObject.transform;
                lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(false, bpmInSeconds, 2);
                lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
                lines[lines.Count - 1].transform.parent = gameObject.transform;
                lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(false, bpmInSeconds, 3);
            }

            //timeToSpawn = ((bpmInSeconds - timeBeforeCollision) + bpmInSeconds) % bpmInSeconds;

            print("60 / bpm: " + (float)60 / (float)localBpm + ", time to spawn: " + timeToSpawn);
        }

        if (lines.Count > 0)
        {
            if (lines[lines.Count - 1] == null) lines.RemoveAt(0);
        }

        if (ringChanged)
        {
            hitTimer += Time.deltaTime;

            if (hitState > 0) rendererIcon.transform.localScale = new Vector3(1f - 0.5f * (hitTimer / hitTime), 1f - 0.5f * (hitTimer / hitTime), 1);

            if (hitTimer >= hitTime)
            {
                rendererIcon.transform.localScale = new Vector3(0.5f, 0.5f);
                hitTimer = 0;
                ringChanged = false;
            }
        }

        if (!mainHandlerTutorial.songStarted && songStarted) songStarted = false;

        if (mainHandlerTutorial.songStarted && !songStarted)
        {
            songStarted = true;
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(true, bpmInSeconds, 1);
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(true, bpmInSeconds, 2);
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(true, bpmInSeconds, 3);
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(false, bpmInSeconds, 1);
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(false, bpmInSeconds, 2);
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(false, bpmInSeconds, 3);
        }
        

        if (mainHandlerTutorial.currentState == 2 && !recordedBeat && !stopped)
        {
            recordedBeat = true;
            //readyforNextRing = true;
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(true, bpmInSeconds);
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(false, bpmInSeconds);
        }
        if (mainHandlerTutorial.currentState == 0 && recordedBeat)
        {
            recordedBeat = false;
        }

        /*
        if (mainHandler.currentBeatTimer >= timeToSpawn && readyforNextRing)
        {
            readyforNextRing = false;
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(true);
            lines.Add(Instantiate(indicatorLine, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            lines[lines.Count - 1].transform.parent = gameObject.transform;
            lines[lines.Count - 1].GetComponent<beatLineHandler>().Init(false);
        }
        */
    }

    public void PlayerInput(bool forceGood = false, bool forceBad = false)
    {
        ringChanged = true;

        if (forceGood)
        {
            rendererFeedback.gameObject.GetComponent<beatLightHandler>().Activate(2);
            rendererIcon.transform.localScale = new Vector3(1, 1, 1);
            hitState = 2;
        }
        else if (forceBad)
        {
            rendererFeedback.gameObject.GetComponent<beatLightHandler>().Activate(0);
            hitState = 0;
        }
        else
        {
            if (mainHandlerTutorial.currentBeatTimer <= greatLimit || bpmInSeconds - mainHandlerTutorial.currentBeatTimer <= greatLimit)
            {
                rendererFeedback.gameObject.GetComponent<beatLightHandler>().Activate(2);
                rendererIcon.transform.localScale = new Vector3(1, 1, 1);
                hitState = 2;
            }
            else if (mainHandlerTutorial.currentState == 1 || mainHandlerTutorial.currentState == 2)
            {
                rendererFeedback.gameObject.GetComponent<beatLightHandler>().Activate(1);
                rendererIcon.transform.localScale = new Vector3(1, 1, 1);
                hitState = 1;
            }
            else
            {
                rendererFeedback.gameObject.GetComponent<beatLightHandler>().Activate(0);
                hitState = 0;
            }
        }
    }

    public void HeavyAttackRemove()
    {
        if (lines.Count > 8)
        {
            if (mainHandlerTutorial.currentState == 2)
            {
                lines[lines.Count - 7].GetComponent<beatLineHandler>().Kill();
                lines[lines.Count - 8].GetComponent<beatLineHandler>().Kill();
            }
            else
            {
                lines[lines.Count - 5].GetComponent<beatLineHandler>().Kill();
                lines[lines.Count - 6].GetComponent<beatLineHandler>().Kill();
            }
        }
    }

    public void Clear()
    {
        for (int i = lines.Count - 8; i < lines.Count; i++)
        {
            if (i >= 0 && lines[i] != null) lines[i].GetComponent<beatLineHandler>().Kill();
        }
        lines.Clear();
        stopped = true;
    }

    public void Restart()
    {
        print("restart");
        stopped = false;
    }
}
