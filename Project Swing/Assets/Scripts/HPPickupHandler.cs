using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPickupHandler : MonoBehaviour
{
    float velX;
    float velY;
    float accY;
    float spinRotation;
    bool landed;
    float lifeTimer;

    // Start is called before the first frame update
    void Start()
    {
        accY = -15f;
        velY = Random.Range(7f, 9f);

        bool direction = Random.Range((int)0, (int)2) == 0;

        if (direction)
        {
            velX = Random.Range(0.1f, 1f);
            spinRotation = Random.Range(-900, -600);
        }
        else
        {
            velX = Random.Range(-1f, -0.1f);
            spinRotation = Random.Range(600, 900);
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        velY += accY * Time.deltaTime;

        if (transform.position.y > -3f)
        {
            if (transform.position.x > 9.3f)
            {
                transform.position = new Vector3(9.3f, transform.position.y);
                velX = Mathf.Abs(velX) * -1;
            }
            if (transform.position.x < -9.3f)
            {
                transform.position = new Vector3(-9.3f, transform.position.y);
                velX = Mathf.Abs(velX);
            }
            transform.Translate(new Vector3(velX * Time.deltaTime, velY * Time.deltaTime), Space.World);
            transform.Rotate(new Vector3(0, 0, spinRotation * Time.deltaTime));
        }
        else if (!landed)
        {
            GetComponent<BoxCollider2D>().enabled = true;
            landed = true;
        }

        if (landed) lifeTimer += Time.deltaTime;

        if (lifeTimer >= 8)
        {
            Destroy(gameObject);
        }

    }
}
