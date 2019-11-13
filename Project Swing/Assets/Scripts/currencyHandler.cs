using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currencyHandler : MonoBehaviour
{
    float velX;
    float velY;
    float accY;
    float spinRotation;
    public int value;
    bool landed;

    float endTimer;
    float landTimer;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        accY = -15f;
        velY = Random.Range(3.5f, 6.3f);

        player = GameObject.Find("Player");

        value = 0;
    }

    public void Init(bool direction, int size)
    {
        if (direction)
        {
            velX = Random.Range(0.1f, 2.8f);
            spinRotation = Random.Range(-900, -2000);
        }
        else
        {
            velX = Random.Range(-2.8f, -0.1f);
            spinRotation = Random.Range(2000, 900);
        }
        value = size;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (!mainHandler.normalLevelFinished && !mainHandler.birdLevelFinished)
        {
            if (!landed) velY += accY * Time.deltaTime;

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
                velY = 0;
                GetComponent<BoxCollider2D>().enabled = true;
                landed = true;
            }
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = true;
            endTimer += Time.deltaTime;
            transform.position = (Vector2.MoveTowards(transform.position, player.transform.position, (0.5f + endTimer * 4) * Time.deltaTime));
        }

        if (landed && !mainHandler.normalLevelFinished) landTimer += Time.deltaTime;

        if (landTimer >= 6)
        {
            Destroy(gameObject);
        }

    }
}
