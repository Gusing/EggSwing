using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuMusicPlayerHandler : MonoBehaviour
{ 
    private static menuMusicPlayerHandler _instance;

    public static menuMusicPlayerHandler Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }



    public FMOD.Studio.EventInstance soundMenuMusic;

    //public static menuMusicPlayerHandler instance = null;

    public bool started = false;
    

    // Start is called before the first frame update
    void Start()
    {
        /*
        FMOD.Studio.PLAYBACK_STATE playbackState;
        soundMenuMusic.getPlaybackState(out playbackState);
        bool isPlaying = playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED;

        if (!isPlaying)
        {
            soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/MenuMusic");
            //soundMenuMusic.start();
        }

        */
    }

    public void checkStarted()
    {
        if (!started)
        {
            soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/MenuMusic");
            soundMenuMusic.start();
            started = true;
        }
       
    }

    public void stopMusic()
    {
        started = false;
        soundMenuMusic.setParameterValue("End", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
