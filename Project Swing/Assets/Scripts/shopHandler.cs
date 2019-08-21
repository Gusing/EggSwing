using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class shopHandler : MonoBehaviour
{
    FMOD.Studio.EventInstance soundUIClick;
    FMOD.Studio.EventInstance soundUIStart;

    PlayerData data;

    public Text textCurrency;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
    }

    void Start()
    {
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");

        textCurrency.text = "Munny: " + data.currency;
    }
    
    void Update()
    {
        
    }

    public void BackToPlayMenu()
    {
        soundUIClick.start();
        SceneManager.LoadScene("MenuScene");
    }

    public void Purchase(int ID)
    {

    }
}
