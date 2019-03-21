using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuHandler : MonoBehaviour {

    public Button btnLvl2;
    public Button btnLvl3;
    public Button btnPractiseLvl2;
    public Button btnPractiseLvl3;
    public Button btnEndless;

    public SpriteRenderer spriteLockLvl2;
    public SpriteRenderer spriteLockLvl3;
    public SpriteRenderer spriteLockEndless;

    public Text txtClearLvl1;
    public Text txtClearLvl2;
    public Text txtClearLvl3;
    public Text txtEndlessRecord;
    bool clickedLevel;

    FMOD.Studio.EventInstance soundMenuMusic;
    FMOD.Studio.EventInstance soundUIClick;
    FMOD.Studio.EventInstance soundUIStart;

    // Use this for initialization
    void Start () {

        soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/MenuMusic");
        soundMenuMusic.start();

        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");

        PlayerData data = SaveSystem.LoadPlayer();

        if (!data.unlockedLevelsA)
        {
            btnLvl2.interactable = false;
            btnPractiseLvl2.interactable = false;
            spriteLockLvl2.enabled = true;
            btnLvl3.interactable = false;
            btnPractiseLvl3.interactable = false;
            spriteLockLvl3.enabled = true;
        }
        if (!data.unlockedLevelsB)
        {
            btnEndless.interactable = false;
            spriteLockEndless.enabled = true;
        }
        if (!data.clearedLevel1) txtClearLvl1.enabled = false;
        if (!data.clearedLevel2) txtClearLvl2.enabled = false;
        if (!data.clearedLevel3) txtClearLvl3.enabled = false;
        if (data.endlessRecord > 0)
        {
            txtEndlessRecord.enabled = true;
            txtEndlessRecord.text = "RECORD: " + data.endlessRecord;
        }
        else txtEndlessRecord.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayLevel(int num)
    {
        if (num > 0) soundUIStart.start();
        else soundUIClick.start();
        
        soundMenuMusic.setParameterValue("End", 1);

        if (num == -3) SceneManager.LoadScene("Practice148");
        if (num == -2) SceneManager.LoadScene("Practice128");
        if (num == -1) SceneManager.LoadScene("Practice100");

        if (num == 1) SceneManager.LoadScene("Level1Scene");
        if (num == 2) SceneManager.LoadScene("Level2Scene");
        if (num == 3) SceneManager.LoadScene("Level3Scene");

        if (num == 100) SceneManager.LoadScene("LevelEndless");
    }

    public void QuitGame()
    {
        soundUIClick.start();
        Application.Quit();
    }
}
