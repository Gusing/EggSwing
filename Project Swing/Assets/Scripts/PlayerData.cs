using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[System.Serializable]
public class PlayerData
{
    public bool[] clearedLevel;
    public bool[] unlockedLevel;
    public bool[] clearedBirdLevel;
    public bool[] unlockedBirdLevel;
    public bool[] clearedHardLevel;
    public bool[] unlockedHardLevel;
    public int[] streakRecord;
    public int[] comboRecord;
    public int[] scoreRecord;
    public float[] timeRecord;
    public int[] scoreBirdRecord;
    public int[] scoreHardRecord;
    public int[] rankRecord;
    public int[] rankBirdRecord;
    public int[] rankHardRecord;
    public int endlessRecord;
    public int streakLevelEndlessRecord;
    public int currency;
    public bool[] itemBought;
    public bool[] itemActive;
    public int lastMode;
    public bool seenTutorial;
    public bool seenBirdTutorial;
    public int inputSelected;

    public PlayerData()
    {
        
    }

    public void Init()
    {
        if (unlockedLevel == null || clearedBirdLevel.Length < 7)
        {
            MonoBehaviour.print("new save");

            clearedLevel = new bool[10];
            unlockedLevel = new bool[10];
            clearedBirdLevel = new bool[10];
            unlockedBirdLevel = new bool[10];
            clearedHardLevel = new bool[10];
            unlockedHardLevel = new bool[10];
            streakRecord = new int[10];
            comboRecord = new int[10];
            timeRecord = new float[10];
            scoreRecord = new int[10];
            scoreBirdRecord = new int[10];
            scoreHardRecord = new int[10];
            rankRecord = new int[10];
            rankBirdRecord = new int[10];
            rankHardRecord = new int[10];
            itemBought = new bool[10];
            itemActive = new bool[10];
            lastMode = 0;
            seenTutorial = false;
            seenBirdTutorial = false;

            for (int i = 0; i < rankRecord.Length; i++)
            {
                rankRecord[i] = -1;
                rankBirdRecord[i] = -1;
            }

            unlockedLevel[1] = true;
            
            
            unlockedHardLevel[1] = true;
            unlockedHardLevel[2] = true;
            unlockedHardLevel[3] = true;
            unlockedHardLevel[4] = true;
            unlockedHardLevel[5] = true;
            unlockedLevel[2] = true;
            unlockedLevel[3] = true;
            unlockedLevel[4] = true;
            unlockedLevel[5] = true;
            unlockedBirdLevel[1] = true;
            unlockedBirdLevel[2] = true;
            unlockedBirdLevel[3] = true;
            unlockedBirdLevel[4] = true;
            unlockedBirdLevel[5] = true;
            

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
        clearedHardLevel = handler.clearedHardLevel;

        unlockedLevel = handler.unlockedLevel;
        unlockedBirdLevel = handler.unlockedBirdLevel;
        unlockedHardLevel = handler.unlockedHardLevel;

        endlessRecord = handler.endlessRecord;

        streakRecord = handler.streakRecord;
        comboRecord = handler.comboRecord;
        timeRecord = handler.timeRecord;
        streakLevelEndlessRecord = handler.streakLevelEndlessRecord;

        scoreRecord = handler.scoreRecord;
        scoreBirdRecord = handler.scoreBirdRecord;
        scoreHardRecord = handler.scoreHardRecord;

        rankRecord = handler.rankRecord;
        rankBirdRecord = handler.rankBirdRecord;
        rankHardRecord = handler.rankHardRecord;

        currency = handler.currency;
        itemBought = handler.itemBought;
        itemActive = handler.itemActive;

        lastMode = handler.lastMode;

        seenTutorial = handler.seenTutorial;
        seenBirdTutorial = handler.seenBirdTutorial;

        inputSelected = handler.inputSelected;
    }

    public PlayerData(shopHandler handler)
    {
        clearedLevel = handler.clearedLevel;
        clearedBirdLevel = handler.clearedBirdLevel;
        clearedHardLevel = handler.clearedHardLevel;

        unlockedLevel = handler.unlockedLevel;
        unlockedBirdLevel = handler.unlockedBirdLevel;
        unlockedHardLevel = handler.unlockedHardLevel;

        endlessRecord = handler.endlessRecord;

        streakRecord = handler.streakRecord;
        comboRecord = handler.comboRecord;
        timeRecord = handler.timeRecord;
        streakLevelEndlessRecord = handler.streakLevelEndlessRecord;

        scoreRecord = handler.scoreRecord;
        scoreBirdRecord = handler.scoreBirdRecord;
        scoreHardRecord = handler.scoreHardRecord;

        rankRecord = handler.rankRecord;
        rankBirdRecord = handler.rankBirdRecord;
        rankHardRecord = handler.rankHardRecord;

        currency = handler.currency;
        itemBought = handler.itemBought;
        itemActive = handler.itemActive;

        lastMode = handler.lastMode;

        seenTutorial = handler.seenTutorial;
        seenBirdTutorial = handler.seenBirdTutorial;

        inputSelected = handler.inputSelected;
    }

    public PlayerData(mainHandlerTutorial handler)
    {
        clearedLevel = handler.clearedLevel;
        clearedBirdLevel = handler.clearedBirdLevel;
        clearedHardLevel = handler.clearedHardLevel;

        unlockedLevel = handler.unlockedLevel;
        unlockedBirdLevel = handler.unlockedBirdLevel;
        unlockedHardLevel = handler.unlockedHardLevel;

        endlessRecord = handler.endlessRecord;

        streakRecord = handler.streakRecord;
        comboRecord = handler.comboRecord;
        timeRecord = handler.timeRecord;
        streakLevelEndlessRecord = handler.streakLevelEndlessRecord;

        scoreRecord = handler.scoreRecord;
        scoreBirdRecord = handler.scoreBirdRecord;
        scoreHardRecord = handler.scoreHardRecord;

        rankRecord = handler.rankRecord;
        rankBirdRecord = handler.rankBirdRecord;
        rankHardRecord = handler.rankHardRecord;

        currency = handler.currency;
        itemBought = handler.itemBought;
        itemActive = handler.itemActive;

        lastMode = handler.lastMode;

        seenTutorial = handler.seenTutorial;
        seenBirdTutorial = handler.seenBirdTutorial;

        inputSelected = handler.inputSelected;
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
            MonoBehaviour.print("no settings data found, loading res: " + Screen.width + " x " + Screen.height);
            Resolution[] resolutions = Screen.resolutions;
            //resolution = resolutions[resolutions.Length - 1].ToString().Remove(resolutions[resolutions.Length - 1].ToString().IndexOf('@'));
            resolution = Screen.width + " x " + Screen.height;
            fullScreen = Screen.fullScreen;
            vSync = false;
            textureQuality = 0;
            volumeMaster = 1;
            volumeMusic = 1;
            volumeSFX = 1;
            volumeAmbience = 1;
        }

        
    }

    public void SetStartValues()
    {
        fullScreen = Screen.fullScreen;
        MonoBehaviour.print("setting start res: " + Screen.width + " x " + Screen.height);
        resolution = Screen.width + " x " + Screen.height;
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