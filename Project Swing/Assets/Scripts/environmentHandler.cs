using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class environmentHandler : MonoBehaviour
{
    public Sprite imgA;
    public Sprite imgB;
    public int count;
    int currentCount;

    SpriteRenderer localRenderer;

    bool currentBeat;
    bool currentSprite;

    // Start is called before the first frame update
    void Start()
    {
        localRenderer = GetComponent<SpriteRenderer>();
        currentCount = -1;
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
                    currentSprite = false;
                    localRenderer.sprite = imgB;
                }
                else if (!currentSprite)
                {
                    currentSprite = true;
                    localRenderer.sprite = imgA;
                }
            }
            
        }

    }
}
