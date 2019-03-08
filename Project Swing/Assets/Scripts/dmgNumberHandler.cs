using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dmgNumberHandler : MonoBehaviour {

    public Text dmgText;
    int damage;

    float lifeTimer;

	// Use this for initialization
	void Start () {
		
	}

    public void Init(int dmg)
    {
        dmgText.text = dmg.ToString();
        transform.Translate(new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f)));
    }
	
	// Update is called once per frame
	void Update () {

        transform.Translate(new Vector3(0, (1.2f - lifeTimer) * Time.deltaTime));

        lifeTimer += Time.deltaTime;

        if (lifeTimer >= 0.7f)
        {
            Destroy(gameObject);
        }

	}
}
