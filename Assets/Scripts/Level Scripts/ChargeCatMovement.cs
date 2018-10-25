using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeCatMovement : CatMovement
{
    private Vector3 stopLocation;
    private bool walkedToWindup = false;

    // todo: implement animation
    private new void Start()
    {
        base.Start();
        speed = 1;

        Vector3 stopLocation = gameObject.transform.position;
        if (stopLocation.x < 0)
        {
            stopLocation.x += 1;
        }
        else
        {
            stopLocation.x -= 1;
        }
        StartCoroutine(walkToWindup());
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        if (walkedToWindup)
        {
            StartCoroutine(windup());
            walkedToWindup = false;
        }
    }


    IEnumerator walkToWindup()
    {
        yield return new WaitForSeconds(1); // delay 1 second
        speed = 0;
        walkedToWindup = true;
    }

    IEnumerator windup()
    {
        yield return new WaitForSeconds(1); // delay 1 second
        speed = 10;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if cat exits the screen
        if (collision.gameObject.tag == "StopPoint")
        {
            GameManager.getInstance().destroyCat(gameObject);
            GameManager.getInstance().spawnCat();
        }
    }
}
