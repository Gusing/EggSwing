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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
