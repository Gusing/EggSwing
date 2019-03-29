using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class environmentHandler : MonoBehaviour
{
    public Sprite imgA;
    public Sprite imgB;
    public int count;
    int currentCount;

    public Light[] lightsA;
    public Light[] lightsB;

    SpriteRenderer localRenderer;

    bool currentBeat;
    bool currentSprite;

    // Start is called before the first frame update
    void Start()
    {
        localRenderer = GetComponent<SpriteRenderer>();
        currentCount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if ((!mainHandler.offBeat && !currentBeat) || (mainHandler.offBeat && currentBeat))
        {
            currentCount++;
            currentBeat = !currentBeat;
            if (currentCount >= count)
            {
                
                currentCount = 0;
                if (currentSprite)
                {
                    for (int i = 0; i < lightsA.Length; i++)
                    {
                        lightsA[i].intensity = 0f;
                    }
                    for (int i = 0; i < lightsB.Length; i++)
                    {
                        lightsB[i].intensity = 0.5f;
                    }
                    currentSprite = false;
                    localRenderer.sprite = imgB;
                }
                else if (!currentSprite)
                {
                    for (int i = 0; i < lightsA.Length; i++)
                    {
                        lightsA[i].intensity = 0.5f;
                    }
                    for (int i = 0; i < lightsB.Length; i++)
                    {
                        lightsB[i].intensity = 0f;
                    }
                    currentSprite = true;
                    localRenderer.sprite = imgA;
                }
            }
            
        }

    }
}
