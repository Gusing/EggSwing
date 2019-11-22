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

    // singleton
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
    
    public bool started = false;
    
    public void checkStarted()
    {
        if (!started)
        {
            soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/MenuMusic");
            soundMenuMusic.start();
            started = true;
        }
       
    }

    public void stopMusic()
    {
        started = false;
        soundMenuMusic.setParameterValue("End", 1);
    }

    public void swapShop(bool shopMusic)
    {
        if (shopMusic) soundMenuMusic.setParameterValue("Shop", 1);
        else soundMenuMusic.setParameterValue("Shop", 0);
    }
}
