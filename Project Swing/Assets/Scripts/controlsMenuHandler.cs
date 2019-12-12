using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class controlsMenuHandler : MonoBehaviour
{
    FMOD.Studio.EventInstance soundUIClick;

    public SpriteRenderer rendererHideSuper;
    public SpriteRenderer rendererBirdControls;
    public SpriteRenderer rendererControlsList;

    public Sprite spriteControlsKeyboard;
    public Sprite spriteControlsXbox;
    public Sprite spriteControlsPS;

    public Sprite spriteControlsBirdKeyboard;
    public Sprite spriteControlsBirdXbox;
    public Sprite spriteControlsBirdPS;
    

    PlayerData data;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
    }

    void Start()
    {
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");

        data.Init();
        /*if (data.itemBought[3])*/ rendererHideSuper.enabled = false;
        if (data.seenBirdTutorial)
        {
            rendererBirdControls.enabled = true;
            if (sceneSelectionHandler.Instance.inputIcons == 0) rendererBirdControls.sprite = spriteControlsBirdXbox;
            if (sceneSelectionHandler.Instance.inputIcons == 1) rendererBirdControls.sprite = spriteControlsBirdPS;
            if (sceneSelectionHandler.Instance.inputIcons == 2) rendererBirdControls.sprite = spriteControlsBirdKeyboard;
        }
        
        if (sceneSelectionHandler.Instance.inputIcons == 0) rendererControlsList.sprite = spriteControlsXbox;
        if (sceneSelectionHandler.Instance.inputIcons == 1) rendererControlsList.sprite = spriteControlsPS;
        if (sceneSelectionHandler.Instance.inputIcons == 2) rendererControlsList.sprite = spriteControlsKeyboard;

        if (!data.unlockedBirdLevel[1]) rendererBirdControls.enabled = false;

        //AnalyticsEvent.CustomEvent(new Dictionary<string, object> { { "Enter_Controls", 1 } });
    }

    void Update()
    {
        // back to play menu
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Super"))
        {
            BackToMenu();
        }
    }

    public void ChangeInputIcons()
    {
        soundUIClick.start();
        sceneSelectionHandler.Instance.sceneArrivedFrom = "ControlsScene";
        SceneManager.LoadScene("InputSelectionScene");
    }

    public void BackToMenu()
    {
        soundUIClick.start();
        SceneManager.LoadScene("MenuScene");
    }
}
