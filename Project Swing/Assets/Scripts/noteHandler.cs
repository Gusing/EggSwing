﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noteHandler : MonoBehaviour
{

    public Sprite[] notes;

    public SpriteRenderer rendererA;
    public SpriteRenderer rendererB;

    public int count;
    int currentCount;

    SpriteRenderer localRenderer;

    bool currentBeat;
    bool currentSprite;

    bool songStarted;

    Vector3 angleA;
    Vector3 angleB;

    // Start is called before the first frame update
    void Start()
    {
        rendererA.enabled = false;
        rendererA.GetComponentInChildren<Light>().enabled = false;
        rendererB.enabled = false;
        rendererB.GetComponentInChildren<Light>().enabled = false;

        localRenderer = GetComponent<SpriteRenderer>();
        currentCount = 1;
        angleA = Vector3.Normalize(new Vector3(Random.Range(0.1f, 0.8f), Random.Range(0.3f, 0.9f)));
        angleB = Vector3.Normalize(new Vector3(Random.Range(0.1f, 0.8f), Random.Range(0.3f, 0.9f)));
    }

    // Update is called once per frame
    void Update()
    {
        if (!songStarted && mainHandler.songStarted)
        {
            songStarted = true;
            rendererA.enabled = true;
            rendererA.GetComponentInChildren<Light>().enabled = true;
            rendererB.enabled = true;
            rendererB.GetComponentInChildren<Light>().enabled = true;
        }
        if (songStarted && !mainHandler.songStarted)
        {
            songStarted = false;
            rendererA.enabled = false;
            rendererA.GetComponentInChildren<Light>().enabled = false;
            rendererB.enabled = false;
            rendererB.GetComponentInChildren<Light>().enabled = false;
        }

        rendererA.transform.Translate(angleA * 1.1f * Time.deltaTime, 0);
        rendererB.transform.Translate(angleB * 1.1f * Time.deltaTime, 0);
        if (rendererA.transform.localScale.x >= 0.01f) rendererA.transform.localScale = new Vector3(rendererA.transform.localScale.x - 0.06f * Time.deltaTime, rendererA.transform.localScale.y - 0.06f * Time.deltaTime, 1);
        if (rendererB.transform.localScale.x >= 0.01f) rendererB.transform.localScale = new Vector3(rendererB.transform.localScale.x - 0.06f * Time.deltaTime, rendererB.transform.localScale.y - 0.06f * Time.deltaTime, 1);

        if ((!mainHandler.offBeat && !currentBeat) || (mainHandler.offBeat && currentBeat))
        {
            currentCount++;
            currentBeat = !currentBeat;
            if (currentCount >= count)
            {
                angleA = Vector3.Normalize(new Vector3(Random.Range(0.1f, 0.8f), Random.Range(0.3f, 0.9f)));
                angleB = Vector3.Normalize(new Vector3(Random.Range(0.1f, 0.8f), Random.Range(0.3f, 0.9f)));
                rendererA.sprite = notes[Random.Range((int)0, notes.Length - 1)];
                rendererB.sprite = notes[Random.Range((int)0, notes.Length - 1)];
                rendererA.transform.localScale = new Vector3(0.15f, 0.15f, 1);
                rendererB.transform.localScale = new Vector3(0.15f, 0.15f, 1);
                rendererA.transform.localPosition = new Vector3(-3.6f, 1.5f);
                rendererB.transform.localPosition = new Vector3(-2.35f, 1.65f);
                currentCount = 0;
            }
            if (currentCount == count / 2)
            {
                rendererA.transform.localScale = new Vector3(0.15f, 0.15f, 1);
                rendererB.transform.localScale = new Vector3(0.15f, 0.15f, 1);
            }
        }

    }
}
