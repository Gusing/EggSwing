using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class optionsHandler : MonoBehaviour
{
    public GameObject VideoScrollList;
    public GameObject AudioScrollList;

    public Dropdown ddResolution;
    public Toggle toggleFullscreen;
    public Toggle toggleVsync;
    public Dropdown ddAA;
    public Dropdown ddTextureQualiy;
    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderSoundEffects;
    public Slider sliderAmbience;

    public float volumeMaster;
    public int textureQuality;
    public bool fullScreen;
    public string resolution;
    public bool vSync;
    public float volumeAmbience;
    public float volumeMusic;
    public float volumeSFX;

    int optionTypeActive;

    public Resolution[] resolutions;

    PlayerOptions data;

    FMOD.Studio.EventInstance soundUIClick;

    void Awake()
    {
        data = SaveSystem.LoadOptions();
    }
    
    void Start()
    {
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");

        resolutions = Screen.resolutions;

        foreach (Resolution res in resolutions)
        {
            ddResolution.options.Add(new Dropdown.OptionData(res.ToString().Remove(res.ToString().IndexOf('@'))));
        }

        data.Init();
        volumeAmbience = data.volumeAmbience;
        volumeMusic = data.volumeMusic;
        volumeSFX = data.volumeSFX;
        volumeMaster = data.volumeMaster;
        textureQuality = data.textureQuality;
        vSync = data.vSync;
        resolution = data.resolution;
        fullScreen = data.fullScreen;

        print("loaded res: " + resolution);


        for (int i = 0; i < ddResolution.options.Count; i++)
        {
            //print(ddResolution.options[i].text + " " + resolution);
            if (ddResolution.options[i].text.Contains(resolution)) ddResolution.value = i;
        }
        toggleFullscreen.isOn = fullScreen;
        ddTextureQualiy.value = textureQuality;
        toggleVsync.isOn = vSync;
        sliderMaster.value = volumeMaster;
        sliderMusic.value = volumeMusic;
        sliderSoundEffects.value = volumeSFX;
        sliderAmbience.value = volumeAmbience;
        
        ChangeMode(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToMainMenu()
    {
        SaveSystem.SaveOptions(this);


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

    public void ChangeResolution()
    {
        print(resolutions[ddResolution.value]);
        Screen.SetResolution(resolutions[ddResolution.value].width, resolutions[ddResolution.value].height, Screen.fullScreen);
        resolution = resolutions[ddResolution.value].ToString().Remove(resolutions[ddResolution.value].ToString().IndexOf('@'));
    }

    public void ChangeFullscreenMode()
    {
        Screen.fullScreen = fullScreen = toggleFullscreen.isOn;
    }

    public void ChangeVsync()
    {
        if (!toggleVsync.isOn) QualitySettings.vSyncCount = 0;
        else QualitySettings.vSyncCount = 1;
        vSync = toggleVsync.isOn;
    }

    public void ChangeAA()
    {
        QualitySettings.antiAliasing = (int)Mathf.Pow(2f, ddAA.value);
    }

    public void ChangeTextureQuality()
    {
        QualitySettings.masterTextureLimit = textureQuality = ddTextureQualiy.value;
    }

    public void ChangeMaster()
    {
        volumeMaster = sliderMaster.value;
        FMODUnity.RuntimeManager.GetVCA("vca:/Music VCA").setVolume(sliderMusic.value * volumeMaster);
        FMODUnity.RuntimeManager.GetVCA("vca:/SFX VCA").setVolume(sliderSoundEffects.value * volumeMaster);
        FMODUnity.RuntimeManager.GetVCA("vca:/Ambience VCA").setVolume(sliderAmbience.value * volumeMaster);
    }

    public void ChangeMusic()
    {
        string vcaPath = "vca:/Music VCA";
        FMOD.Studio.VCA vca = FMODUnity.RuntimeManager.GetVCA(vcaPath);
        vca.setVolume(sliderMusic.value * volumeMaster);
        volumeMusic = sliderMusic.value;
    }

    public void ChangeSoundEffects()
    {
        string vcaPath = "vca:/SFX VCA";
        FMOD.Studio.VCA vca = FMODUnity.RuntimeManager.GetVCA(vcaPath);
        vca.setVolume(sliderSoundEffects.value * volumeMaster);
        volumeSFX = sliderSoundEffects.value;
    }

    public void ChangeAmbience()
    {
        string vcaPath = "vca:/Ambience VCA";
        FMOD.Studio.VCA vca = FMODUnity.RuntimeManager.GetVCA(vcaPath);
        vca.setVolume(sliderAmbience.value * volumeMaster);
        volumeAmbience = sliderAmbience.value;
    }




}
