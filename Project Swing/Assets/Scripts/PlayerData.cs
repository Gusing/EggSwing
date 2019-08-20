using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public bool[] clearedLevel;
    public bool[] unlockedLevel;
    public int endlessRecord;
    public int[] streakRecord;
    public int streakLevelEndlessRecord;
    public int[] scoreRecord;
    public int[] rankRecord;
    public int currency;

    public PlayerData()
    {
        
    }

    public void Init()
    {
        if (unlockedLevel == null)
        {
            clearedLevel = new bool[5];
            unlockedLevel = new bool[5];
            streakRecord = new int[5];
            scoreRecord = new int[5];
            rankRecord = new int[5];

            for (int i = 0; i < rankRecord.Length; i++)
            {
                rankRecord[i] = -1;
            }

            unlockedLevel[1] = true;
        }
    }

    public PlayerData(mainHandler handler)
    {
        clearedLevel = handler.clearedLevel;

        unlockedLevel = handler.unlockedLevel;

        endlessRecord = handler.endlessRecord;

        streakRecord = handler.streakRecord;
        streakLevelEndlessRecord = handler.streakLevelEndlessRecord;

        scoreRecord = handler.scoreRecord;

        rankRecord = handler.rankRecord;

        currency = handler.currency;
    }

}
