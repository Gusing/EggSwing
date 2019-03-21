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
    }

}
