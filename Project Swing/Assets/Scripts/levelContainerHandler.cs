﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelContainerHandler : MonoBehaviour
{
    public Text txtLevelRank;
    public Text txtLevelScore;
    public Text txtLevelStreak;
    public Image imgLocked;
    public Button btnPlayLevel;
    public int levelNumber;
    GameObject mainHandler;

    // Start is called before the first frame update
    void Start()
    {
        mainHandler = GameObject.Find("Main Camera");
    }

    public void Init(string rank, int score, int streak, bool unlocked = true)
    {
        if (!unlocked)
        {
            imgLocked.enabled = true;
            btnPlayLevel.enabled = false;
        }

        txtLevelRank.text = rank;
        if (rank == "F") txtLevelRank.color = new Color(0.35f, 0.1f, 0.08f);
        if (rank == "E") txtLevelRank.color = new Color(0.65f, 0.65f, 0.65f);
        if (rank == "D") txtLevelRank.color = new Color(0.57f, 0.6f, 0.91f);
        if (rank == "C") txtLevelRank.color = new Color(0.94f, 0.69f, 0.3f);
        if (rank == "B") txtLevelRank.color = new Color(0.2f, 0.76f, 1f);
        if (rank == "A") txtLevelRank.color = new Color(1f, 0.05f, 0.95f);
        if (rank == "S") txtLevelRank.color = new Color(1f, 0.9f, 0f);
        if (score > 0) txtLevelScore.text = score.ToString();
        else txtLevelScore.text = "";
        if (streak > 0) txtLevelStreak.text = "Highest Streak: " + streak.ToString();
        else txtLevelStreak.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickPlay()
    {
        mainHandler.GetComponent<menuHandler>().PlayLevel(levelNumber);
    }
}