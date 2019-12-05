using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn
{
    public float spawnTime;
    public int nextWaveReq;
    public bool waitForDeath;
    public bool relativeToPlayer;
    public GameObject[] enemies;
    public float[] xPos;
    public bool[] fallFromAbove;

    public EnemySpawn(float pSpawnTime, GameObject[] pEnemies, float[] pXPos, bool[] pFallFromAbove, int pNextWaveReq = -1, bool pRelativeToPlayer = false )
    {
        spawnTime = pSpawnTime;
        enemies = pEnemies;
        xPos = pXPos;
        fallFromAbove = pFallFromAbove;
        if (pNextWaveReq == -1) waitForDeath = false;
        else waitForDeath = true;
        nextWaveReq = pNextWaveReq;
        relativeToPlayer = pRelativeToPlayer;
    }
}
