using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatIndicatorHandlerB : MonoBehaviour
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

        localBpm = mainHandler.currentBpm;

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
        if (mainHandler.currentBpm != localBpm)
        {
           
            localBpm = mainHandler.currentBpm;

            bpmInSeconds = (float)60 / (float)localBpm;

            hitTime = 0.13f;
            if (localBpm > 130) hitTime = 0.1f;

            for (int i = lines.Count - 8; i < lines.Count; i++)
            {
                if (i >= 0 && lines[i] != null) lines[i].GetComponent<beatLineHandler>().Kill();

            }
            lines.Clear();
            if (mainHandler.songStarted)
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

            if (hitState > 0) rendererIcon.transform.localScale = new Vector3(1.5f - 0.5f * (hitTimer / hitTime), 1.5f - 0.5f * (hitTimer / hitTime), 1);

            if (hitTimer >= hitTime)
            {
                hitTimer = 0;
                ringChanged = false;
            }
        }

        if (!mainHandler.songStarted && songStarted) songStarted = false;

        if (mainHandler.songStarted && !songStarted)
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
        

        if (mainHandler.currentState == 2 && !recordedBeat)
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
        if (mainHandler.currentState == 0 && recordedBeat)
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

    public void PlayerInput()
    {
        ringChanged = true;

        // funkar inte
        if (lines.Count > 0)
        {
            /*
            if (rings[rings.Count - 1].transform.localScale.x < startingScale / 2)
            {
                Destroy(rings[rings.Count - 1]);
                rings.RemoveAt(rings.Count - 1);
            }
            */
        }

        if (mainHandler.currentBeatTimer <= greatLimit || bpmInSeconds - mainHandler.currentBeatTimer <= greatLimit)
        {
            rendererFeedback.gameObject.GetComponent<beatLightHandler>().Activate(2);
            rendererIcon.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            hitState = 2;
        }
        else if (mainHandler.currentState == 1 || mainHandler.currentState == 2)
        {
            rendererFeedback.gameObject.GetComponent<beatLightHandler>().Activate(1);
            rendererIcon.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            hitState = 1;
        }
        else
        {
            rendererFeedback.gameObject.GetComponent<beatLightHandler>().Activate(0);
            hitState = 0;
        }
    }
}
