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
    
    public void CheckStarted(bool shop)
    {
        if (!started)
        {
            soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/MenuMusic");
            soundMenuMusic.start();
            started = true;
            if (shop)
            {
                soundMenuMusic.setParameterValue("ShopMenu", 2);
                soundMenuMusic.setParameterValue("Shop", 1);
            }
            else
            {
                soundMenuMusic.setParameterValue("ShopMenu", 1);
                soundMenuMusic.setParameterValue("Shop", 0);
            }
        }
       
    }

    public void StopMusic()
    {
        started = false;
        soundMenuMusic.setParameterValue("End", 1);
    }

    public void SwapShop(bool shopMusic)
    {
        if (shopMusic) soundMenuMusic.setParameterValue("Shop", 1);
        else soundMenuMusic.setParameterValue("Shop", 0);
    }
}
