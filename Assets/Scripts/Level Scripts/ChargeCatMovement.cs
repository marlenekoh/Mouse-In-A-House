using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeCatMovement : CatMovement
{
    public float startSpeed;
    public float chargeSpeed;

    private bool walkedToWindup = false;

    // todo: implement animation
    protected new void Start()
    {
        base.Start();
        speed = startSpeed;

        StartCoroutine(walkToWindup());
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if (walkedToWindup && !isStunned)
        {
            speed = chargeSpeed;
        }
    }

    IEnumerator walkToWindup()
    {
        yield return new WaitForSeconds(1.5f); // move this distance
        speed = 0;
        walkedToWindup = true;
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
