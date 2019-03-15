using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuHandler : MonoBehaviour {

    FMOD.Studio.EventInstance soundMenuMusic;

    // Use this for initialization
    void Start () {

        soundMenuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/MenuMusic");
        soundMenuMusic.start();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayLevel(int num)
    {
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
        Application.Quit();
    }
}
