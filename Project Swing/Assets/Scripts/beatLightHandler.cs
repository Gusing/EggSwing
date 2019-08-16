using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatLightHandler : MonoBehaviour
{
    SpriteRenderer localRenderer;

    public Sprite spriteGreat;
    public Sprite spriteGood;
    public Sprite spriteFail;

    float timer;
    bool activated;

    // Start is called before the first frame update
    void Start()
    {
        localRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            timer += Time.deltaTime;

            localRenderer.color = new Color(localRenderer.color.r, localRenderer.color.g, localRenderer.color.b, 1 - (timer / 0.4f));

            if (timer >= 0.4f)
            {
                activated = false;
                timer = 0;
            }
        }
    }

    public void Activate(int level)
    {
        if (level == 0) localRenderer.sprite = spriteFail;
        if (level == 1) localRenderer.sprite = spriteGood;
        if (level == 2) localRenderer.sprite = spriteGreat;
        activated = true;
        timer = 0;
    }
}
