using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn
{
    public float spawnTime;
    public int nextWaveReq;
    public bool waitForDeath;
    public GameObject[] enemies;
    public float[] xPos;

    public EnemySpawn(float pSpawnTime, GameObject[] pEnemies, float[] pXPos, int pNextWaveReq = -1)
    {
        spawnTime = pSpawnTime;
        enemies = pEnemies;
        xPos = pXPos;
        if (pNextWaveReq == -1) waitForDeath = false;
        else waitForDeath = true;
        nextWaveReq = pNextWaveReq;
    }
}
