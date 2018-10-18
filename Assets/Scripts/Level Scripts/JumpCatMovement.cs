using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCatMovement : CatMovement
{
    public float jumpSpeedY;
    private bool isJumping;

    private void Update()
    {
        if (isJumping)
        {
            // add artifical gravity to make jump smoother
            Vector3 vel = rb.velocity;
            vel.y -= 15 * Time.deltaTime;
            rb.velocity = vel;
        }

    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "JumpPoint" && !isJumping)
        {
            //Debug.Log("I should jump here");
            // TODO: Implement logic to not jump depending on mouse position
            if (mouse != null && mouse.GetComponent<Transform>().position.y > trans.position.y) //jump
            {
                isJumping = true;
                rb.AddForce(new Vector2(rb.velocity.x, jumpSpeedY));
            }
        }
    }

    // for jump
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && rb.velocity.y == 0)
        {
            rb.velocity = new Vector3(0, 0, 0);
            isJumping = false;
        }
    }
}
