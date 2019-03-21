using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn
{
    public float spawnTime;
    public GameObject[] enemies;
    public float[] xPos;

    public EnemySpawn(float pSpawnTime, GameObject[] pEnemies, float[] pXPos)
    {
        spawnTime = pSpawnTime;
        enemies = pEnemies;
        xPos = pXPos;
    }
}
