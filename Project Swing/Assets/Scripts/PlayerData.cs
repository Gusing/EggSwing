using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public bool[] clearedLevel;
    public bool[] unlockedLevel;
    public bool[] clearedBirdLevel;
    public bool[] unlockedBirdLevel;
    public int[] streakRecord;
    public int[] comboRecord;
    public int[] scoreRecord;
    public int[] scoreBirdRecord;
    public int[] rankRecord;
    public int[] rankBirdRecord;
    public int endlessRecord;
    public int streakLevelEndlessRecord;
    public int currency;
    public bool[] itemBought;
    public bool[] itemActive;
    public int lastMode;
    public bool seenControls;
    public bool seenBirdTutorial;

    public PlayerData()
    {
        
    }

    public void Init()
    {
        if (unlockedLevel == null)
        {
            clearedLevel = new bool[5];
            unlockedLevel = new bool[5];
            clearedBirdLevel = new bool[5];
            unlockedBirdLevel = new bool[5];
            streakRecord = new int[5];
            comboRecord = new int[5];
            scoreRecord = new int[5];
            scoreBirdRecord = new int[5];
            rankRecord = new int[5];
            rankBirdRecord = new int[5];
            itemBought = new bool[10];
            itemActive = new bool[10];
            lastMode = 0;
            seenControls = false;
            seenBirdTutorial = false;

            for (int i = 0; i < rankRecord.Length; i++)
            {
                rankRecord[i] = -1;
                rankBirdRecord[i] = -1;
            }

            unlockedLevel[1] = true;
            unlockedBirdLevel[1] = true;

            for (int i = 0; i < itemActive.Length; i++)
            {
                itemActive[i] = true;
            }
        }
    }

    public PlayerData(mainHandler handler)
    {
        clearedLevel = handler.clearedLevel;
        clearedBirdLevel = handler.clearedBirdLevel;

        unlockedLevel = handler.unlockedLevel;
        unlockedBirdLevel = handler.unlockedBirdLevel;

        endlessRecord = handler.endlessRecord;

        streakRecord = handler.streakRecord;
        comboRecord = handler.comboRecord;
        streakLevelEndlessRecord = handler.streakLevelEndlessRecord;

        scoreRecord = handler.scoreRecord;
        scoreBirdRecord = handler.scoreBirdRecord;

        rankRecord = handler.rankRecord;
        rankBirdRecord = handler.rankBirdRecord;

        currency = handler.currency;
        itemBought = handler.itemBought;
        itemActive = handler.itemActive;

        lastMode = handler.lastMode;

        seenControls = handler.seenControls;
        seenBirdTutorial = handler.seenBirdTutorial;
    }

    public PlayerData(shopHandler handler)
    {
        clearedLevel = handler.clearedLevel;
        clearedBirdLevel = handler.clearedBirdLevel;

        unlockedLevel = handler.unlockedLevel;
        unlockedBirdLevel = handler.unlockedBirdLevel;

        endlessRecord = handler.endlessRecord;

        streakRecord = handler.streakRecord;
        comboRecord = handler.comboRecord;
        streakLevelEndlessRecord = handler.streakLevelEndlessRecord;

        scoreRecord = handler.scoreRecord;
        scoreBirdRecord = handler.scoreBirdRecord;

        rankRecord = handler.rankRecord;
        rankBirdRecord = handler.rankBirdRecord;

        currency = handler.currency;
        itemBought = handler.itemBought;
        itemActive = handler.itemActive;

        lastMode = handler.lastMode;

        seenControls = handler.seenControls;
        seenBirdTutorial = handler.seenBirdTutorial;
    }

}


[System.Serializable]
public class PlayerOptions
{
    public string resolution;
    public bool fullScreen;
    public bool vSync;
    public int textureQuality;
    public float volumeMaster;
    public float volumeMusic;
    public float volumeSFX;
    public float volumeAmbience;

    public PlayerOptions()
    {

    }

    public void Init()
    {
        if (resolution == null)
        {
            Resolution[] resolutions = Screen.resolutions;
            //resolution = resolutions[resolutions.Length - 1].ToString().Remove(resolutions[resolutions.Length - 1].ToString().IndexOf('@'));
            resolution = Screen.currentResolution.ToString().Remove(Screen.currentResolution.ToString().IndexOf('@'));
            fullScreen = Screen.fullScreen;
            vSync = false;
            textureQuality = 0;
            volumeMaster = 1;
            volumeMusic = 1;
            volumeSFX = 1;
            volumeAmbience = 1;
        }

        fullScreen = Screen.fullScreen;
        resolution = Screen.currentResolution.ToString().Remove(Screen.currentResolution.ToString().IndexOf('@'));
    }

    public PlayerOptions(optionsHandler handler)
    {
        volumeSFX = handler.volumeSFX;
        volumeMusic = handler.volumeMusic;
        volumeAmbience = handler.volumeAmbience;
        volumeMaster = handler.volumeMaster;
        fullScreen = handler.fullScreen;
        vSync = handler.vSync;
        textureQuality = handler.textureQuality;
        resolution = handler.resolution;
    }
    
}