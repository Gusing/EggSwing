using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudHandler : MonoBehaviour
{
    public SpriteRenderer rendererA;
    public SpriteRenderer rendererB;
    public float speedDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rendererA.transform.Translate(new Vector3(0.5f * speedDirection * Time.deltaTime, 0));
        rendererB.transform.Translate(new Vector3(0.5f * speedDirection * Time.deltaTime, 0));

        if (rendererA.transform.localPosition.x < -20.5f) rendererA.transform.localPosition = new Vector3(20, rendererA.transform.localPosition.y);
        if (rendererB.transform.localPosition.x < -20.5f) rendererB.transform.localPosition = new Vector3(20, rendererB.transform.localPosition.y);

        if (rendererA.transform.localPosition.x > 20.5f) rendererA.transform.localPosition = new Vector3(-20, rendererA.transform.localPosition.y);
        if (rendererB.transform.localPosition.x > 20.5f) rendererB.transform.localPosition = new Vector3(-20, rendererB.transform.localPosition.y);
    }
}
