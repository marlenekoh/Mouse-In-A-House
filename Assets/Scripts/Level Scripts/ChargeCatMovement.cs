using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeCatMovement : CatMovement
{
    // todo: implement animation
    private new void Start()
    {
        base.Start();
        StartCoroutine(windup());
    }

    IEnumerator windup()
    {
        speed = 0;
        yield return new WaitForSeconds(2); // delay 1 second
        speed = 10;
    }
}
