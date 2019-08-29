using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class optionsHandler : MonoBehaviour
{
    public GameObject VideoScrollList;
    public GameObject AudioScrollList;

    int optionTypeActive;

    FMOD.Studio.EventInstance soundUIClick;

    // Start is called before the first frame update
    void Start()
    {
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");

        ChangeMode(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToMainMenu()
    {
        soundUIClick.start();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ChangeMode(int type)
    {
        if (type == 0)
        {
            VideoScrollList.SetActive(true);
            AudioScrollList.SetActive(false);
        }
        if (type == 1)
        {
            VideoScrollList.SetActive(false);
            AudioScrollList.SetActive(true);
        }
        optionTypeActive = type;
    }
}
