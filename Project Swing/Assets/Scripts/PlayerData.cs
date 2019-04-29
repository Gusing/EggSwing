using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public bool clearedLevel1;
    public bool clearedLevel2;
    public bool clearedLevel3;
    public bool unlockedLevelsA;
    public bool unlockedLevelsB;
    public int endlessRecord;
    public int streakLevel1Record;
    public int streakLevel2Record;
    public int streakLevel3Record;
    public int streakLevelEndlessRecord;
    public int currency;

    public PlayerData()
    {

    }

    public PlayerData(mainHandler handler)
    {
        clearedLevel1 = handler.clearedLevel1;
        clearedLevel2 = handler.clearedLevel2;
        clearedLevel3 = handler.clearedLevel3;

        unlockedLevelsA = handler.unlockedLevelsA;
        unlockedLevelsB = handler.unlockedLevelsB;

        endlessRecord = handler.endlessRecord;

        streakLevel1Record = handler.streakLevel1Record;
        streakLevel2Record = handler.streakLevel2Record;
        streakLevel3Record = handler.streakLevel3Record;
        streakLevelEndlessRecord = handler.streakLevelEndlessRecord;

        currency = handler.currency;
    }

}
