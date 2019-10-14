using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelContainerHandler : MonoBehaviour
{
    public Text txtLevelRank;
    public Text txtLevelScore;
    public Text txtLevelStreak;
    public Text txtLevelName;
    public Text txtBpm;
    public Image imgLocked;
    public Button btnPlayLevel;
     int levelNumber;
    GameObject mainHandler;
    public Image imgBg;

    public Sprite[] bgImages;
    public string[] levelNames;
    public string[] levelBPMs;

    // Start is called before the first frame update
    void Start()
    {
        mainHandler = GameObject.Find("Main Camera");
    }

    public void Init(string rank, int score, float streak, int level, bool unlocked = true, int gameMode = 0)
    {
        if (!unlocked)
        {
            imgLocked.enabled = true;
            btnPlayLevel.enabled = false;
        }

        imgBg.sprite = bgImages[level - 1];
        txtLevelName.text = levelNames[level - 1];
        txtBpm.text = "BPM: " + levelBPMs[level - 1];
        levelNumber = level;

        txtLevelRank.text = rank;
        if (rank == "F") txtLevelRank.color = new Color(0.35f, 0.1f, 0.08f);
        if (rank == "E") txtLevelRank.color = new Color(0.65f, 0.65f, 0.65f);
        if (rank == "D") txtLevelRank.color = new Color(0.57f, 0.6f, 0.91f);
        if (rank == "C") txtLevelRank.color = new Color(0.94f, 0.69f, 0.3f);
        if (rank == "B") txtLevelRank.color = new Color(0.2f, 0.76f, 1f);
        if (rank == "A") txtLevelRank.color = new Color(1f, 0.05f, 0.95f);
        if (rank == "S" || rank == "P") txtLevelRank.color = new Color(1f, 0.9f, 0f);
        if (score > 0) txtLevelScore.text = score.ToString();
        else txtLevelScore.text = "";
        if (streak > 0)
        {
            if (gameMode == 0) txtLevelStreak.text = "Highest Streak: " + streak.ToString();
            if (gameMode == 1) txtLevelStreak.text = "Longest Combo: " + streak.ToString();
            if (gameMode == 2) txtLevelStreak.text = "Most Time Left: " + streak.ToString();
        }
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
