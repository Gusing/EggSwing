using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudHandler : MonoBehaviour
{
    public SpriteRenderer rendererA;
    public SpriteRenderer rendererB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rendererA.transform.Translate(new Vector3(-0.5f * Time.deltaTime, 0));
        rendererB.transform.Translate(new Vector3(-0.5f * Time.deltaTime, 0));

        if (rendererA.transform.localPosition.x <= -40f) rendererA.transform.localPosition = new Vector3(40, rendererA.transform.localPosition.y);
        if (rendererB.transform.localPosition.x <= -40f) rendererB.transform.localPosition = new Vector3(40, rendererB.transform.localPosition.y);
    }
}
