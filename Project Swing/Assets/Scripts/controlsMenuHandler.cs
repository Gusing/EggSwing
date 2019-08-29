using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class controlsMenuHandler : MonoBehaviour
{
    FMOD.Studio.EventInstance soundUIClick;

    public SpriteRenderer rendererHideSuper;

    PlayerData data;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
    }

    void Start()
    {
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");

        data.Init();
        if (data.itemBought[3]) rendererHideSuper.enabled = false;
    }

    void Update()
    {
        // back to play menu
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Super"))
        {
            BackToMenu();
        }
    }

    public void BackToMenu()
    {
        soundUIClick.start();
        SceneManager.LoadScene("MenuScene");
    }
}
