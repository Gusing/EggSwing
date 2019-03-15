using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo
{

    public List<Sprite> sprites;
    public List<BoxCollider2D> hitboxes;
    public List<int> types;
    public List<float> pushes;
    public List<Sprite> preSprites;

    public Combo(List<Sprite> pSprites, List<BoxCollider2D> pHitboxes, List<int> pTypes, List<float> pPushes)
    {
        sprites = pSprites;
        hitboxes = pHitboxes;
        types = pTypes;
        pushes = pPushes;

        preSprites = new List<Sprite>();
        
        for (int i = 0; i < sprites.Count; i++)
        {
            if (types[i] == 1)
            {
                preSprites.Add(sprites[i]);
                sprites.RemoveAt(i);
            }
            else preSprites.Add(sprites[i]);
        }
    }
}
