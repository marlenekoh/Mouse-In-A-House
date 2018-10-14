using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCatMovement : CatMovement
{
    public float jumpSpeedY;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "JumpPoint")
        {
            Debug.Log("I should jump here");
            // TODO: Implement logic to not jump depending on mouse position
            rb.AddForce(new Vector2(rb.velocity.x, jumpSpeedY));

            Vector3 vel = rb.velocity;
            vel.y -= 15 * Time.deltaTime;
            rb.velocity = vel;
        }
    }
}
