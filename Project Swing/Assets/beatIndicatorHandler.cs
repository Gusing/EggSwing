using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatIndicatorHandler : MonoBehaviour
{
    public GameObject indicatorRing;

    public Sprite spriteEmpty;
    public Sprite spriteGreat;
    public Sprite spriteOk;
    public Sprite spriteBad;

    SpriteRenderer localRenderer;

    public float timeBeforeCollision = 0.5f;
    public static float staticTimeBeforeCollision;
    public float startingScale = 2;
    public static float staticStartingScale;
    public float greatLimit = 0.04f;
    float bpmInSeconds;
    float timeToSpawn;
    bool recordedBeat;
    bool readyforNextRing;
    bool ringChanged;
    float hitTimer;
    int localBpm;
    
    List<GameObject> rings;

    void Awake()
    {
        staticTimeBeforeCollision = timeBeforeCollision;
        staticStartingScale = startingScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        rings = new List<GameObject>();

        localRenderer = GetComponent<SpriteRenderer>();

        localBpm = mainHandler.currentBpm;

        bpmInSeconds = (float)60 / (float)localBpm;

        timeToSpawn = ((bpmInSeconds - timeBeforeCollision) + bpmInSeconds) % bpmInSeconds;

        print("60 / bpm: " + (float)60 / (float)localBpm + ", time to spawn: " + timeToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if (mainHandler.currentBpm != localBpm)
        {
            localBpm = mainHandler.currentBpm;

            bpmInSeconds = (float)60 / (float)localBpm;

            timeToSpawn = ((bpmInSeconds - timeBeforeCollision) + bpmInSeconds) % bpmInSeconds;

            print("60 / bpm: " + (float)60 / (float)localBpm + ", time to spawn: " + timeToSpawn);
        }

        if (rings.Count > 0)
        {
            if (rings[rings.Count - 1] == null) rings.RemoveAt(0);
        }

        if (ringChanged)
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= 0.13f)
            {
                localRenderer.sprite = spriteEmpty;
                hitTimer = 0;
                ringChanged = false;
            }
        }

        if (mainHandler.currentState == 2 && !recordedBeat)
        {
            recordedBeat = true;
            readyforNextRing = true;
        }
        if (mainHandler.currentState == 0 && recordedBeat)
        {
            recordedBeat = false;
        }

        if (mainHandler.currentBeatTimer >= timeToSpawn && readyforNextRing)
        {
            readyforNextRing = false;
            rings.Add(Instantiate(indicatorRing, new Vector3(0, 0), new Quaternion(0, 0, 0, 0)));
            rings[rings.Count-1].transform.parent = gameObject.transform;
        }
    }

    public void PlayerInput()
    {
        ringChanged = true;

        // funkar inte
        if (rings.Count > 0)
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
            localRenderer.sprite = spriteGreat;
        }
        else if (mainHandler.currentState == 1 || mainHandler.currentState == 2)
        {
            localRenderer.sprite = spriteOk;
        }
        else localRenderer.sprite = spriteBad;
    }
}
