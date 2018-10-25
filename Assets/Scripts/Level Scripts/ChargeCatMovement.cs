using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeCatMovement : CatMovement
{
    public float startSpeed;
    public float chargeSpeed;

    private bool walkedToWindup = false;
    private bool hasWindup = false;

    // todo: implement animation
    private new void Start()
    {
        base.Start();
        speed = startSpeed;

        StartCoroutine(walkToWindup());
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        //TODO: add debug statements
        
        if (isStunned)
        {
            //Debug.Log("a");
            speed = 0;
        }
        else if (walkedToWindup)
        {
            //Debug.Log("b");
            speed = 10;
            //speed = 0;
            //StartCoroutine(windup());
            //walkedToWindup = false;
        }

        if (!isStunned)
        {
            //Debug.Log("I'm not stunned");
        }
    }


    IEnumerator walkToWindup()
    {
        yield return new WaitForSeconds(1f); // move this distance
        speed = 0;
        walkedToWindup = true;
    }

    IEnumerator windup()
    {
        yield return new WaitForSeconds(1); // delay 1 second
        hasWindup = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if cat exits the screen
        if (collision.gameObject.tag == "StopPoint")
        {
            onCatExitScreen();
        }
    }
}
