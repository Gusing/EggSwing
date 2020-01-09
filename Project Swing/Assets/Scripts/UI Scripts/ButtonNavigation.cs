﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonNavigation : MonoBehaviour
{
    EventSystem eventSystem;

    GameObject oldSelected;
    GameObject sel;

    public Material matUIGlow;
    public Material matUIGlowGreen;
    public Material matUIGlowNormal;
    public Material matUIGlowBird;
    public Material matUIGlowHard;

    float matGlowTimer;
    float matGlowTime;
    bool glowingGreenStrong;

    FMOD.Studio.EventInstance soundUIMove;

    void Awake()
    {
        
    }

    void Start()
    {
        eventSystem = EventSystem.current;

        oldSelected = eventSystem.currentSelectedGameObject;

        soundUIMove = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_hover");

        matGlowTime = 0.2f;
        matGlowTimer = 0;
        glowingGreenStrong = false;
    }
    
    void Update()
    {
        // prevent deselection
        if (eventSystem.currentSelectedGameObject != null && eventSystem.currentSelectedGameObject != sel)
        {
            sel = eventSystem.currentSelectedGameObject;
        }
        else if (sel != null && eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(sel);
        }

        // update flicker
        matGlowTimer += Time.deltaTime;

        if (matGlowTimer >= matGlowTime)
        {
            glowingGreenStrong = !glowingGreenStrong;
            if (glowingGreenStrong)
            {
                if (Random.value > 0.4f) matGlowTime = Random.Range(0.1f, 0.8f);
                else matGlowTime = Random.Range(0.02f, 0.04f);
                matGlowTimer = 0;
                matUIGlowGreen.SetVector("_OutlineColor", new Color(0.2f, 0.8f, 0.2f, 1) * 2.7f);
            }
            else
            {
                matGlowTime = Random.Range(0.02f, 0.04f);
                matGlowTimer = 0;
                matUIGlowGreen.SetVector("_OutlineColor", new Color(0.1f, 0.1f, 0.1f, 1) * 2);
            }
        }

        // play sound and change material when selecting new object
        if (eventSystem.currentSelectedGameObject != null)
        {
            if (eventSystem.currentSelectedGameObject != oldSelected && eventSystem.currentSelectedGameObject.GetComponent<Button>() != null)
            {
                soundUIMove.start();

                eventSystem.currentSelectedGameObject.GetComponent<Image>().material = matUIGlowGreen;
                
                string tButtonName = eventSystem.currentSelectedGameObject.name;
                if (tButtonName == "btnStartLevel")
                {
                    matUIGlowGreen.SetVector("_OutlineColor", new Color(0.2f, 0.8f, 0.2f, 1) * 2.7f);
                    eventSystem.currentSelectedGameObject.GetComponentInChildren<Text>().color = new Color(0.4f, 0.9f, 0.2f);
                }
                
                if (oldSelected != null && eventSystem.currentSelectedGameObject.GetComponent<Button>() != null)
                {
                    if (oldSelected.name == "btnStartLevel") oldSelected.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f);
                    
                    oldSelected.GetComponent<Image>().material = matUIGlow;
                    if (oldSelected.name == "btnNormal") oldSelected.GetComponent<Image>().material = matUIGlowNormal;
                    if (oldSelected.name == "btnBird") oldSelected.GetComponent<Image>().material = matUIGlowBird;
                    if (oldSelected.name == "btnHard") oldSelected.GetComponent<Image>().material = matUIGlowHard;
                }
                
            }
        }

        if (eventSystem.currentSelectedGameObject.GetComponent<Button>() != null) oldSelected = eventSystem.currentSelectedGameObject;
    }
}