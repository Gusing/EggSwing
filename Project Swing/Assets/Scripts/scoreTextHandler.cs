﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreTextHandler : MonoBehaviour
{
    public Text scoreText;
    float lifeTimer;

    void Start()
    {
        
    }

    public void Init(string scoreSource, int score, float offset = 0)
    {
        scoreText.text = scoreSource + " +" + score.ToString();
        transform.position = new Vector3(-1.20f, 3.6f - offset);
    }
    
    void Update()
    {
        transform.Translate(new Vector3(0, (1.2f - lifeTimer) * Time.deltaTime));
        scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 1 - lifeTimer);

        lifeTimer += Time.deltaTime;

        if (lifeTimer >= 0.7f)
        {
            Destroy(gameObject);
        }

    }
}
