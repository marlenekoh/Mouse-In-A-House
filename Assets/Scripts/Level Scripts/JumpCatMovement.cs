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
            anim.SetBool("isJumping", true);
            // add artifical gravity to make jump smoother
            if (rb != null)
            {
                Vector3 vel = rb.velocity;
                vel.y -= 15 * Time.deltaTime;
                rb.velocity = vel;
            }
        }
        else
        {
            anim.SetBool("isJumping", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "JumpPoint":
                if (!isJumping)
                {
                    if (mouse != null && mouse.GetComponent<Transform>().position.y > trans.position.y) //jump
                    {
                        rb.AddForce(new Vector2(rb.velocity.x, jumpSpeedY));
                        isJumping = true;
                        SfxManager.PlaySound("jumpingCat");
                    }
                }
                break;
            case "JumpPoint2":
                if (gameObject.transform.localScale.x > 0) // jump over platform gap
                {
                    rb.AddForce(new Vector2(rb.velocity.x * 400, jumpSpeedY * 0.8f));
                    Debug.Log("jumppoint2");
                    isJumping = true;
                    SfxManager.PlaySound("jumpingCat");
                }
                break;
            case "JumpPoint3": // jump over platform gap
                isJumping = true;
                if (gameObject.transform.localScale.x < 0)
                {
                    rb.AddForce(new Vector2(rb.velocity.x * 400, jumpSpeedY * 0.8f));
                    Debug.Log("jumppoint3");
                    isJumping = true;
                    SfxManager.PlaySound("jumpingCat");
                }
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // for jump
        if (collision.gameObject.tag == "Ground" && rb.velocity.y == 0)
        {
            rb.velocity = new Vector3(0, 0, 0);
            isJumping = false;
        }
        // if cat exits the screen
        if (collision.gameObject.tag == "StopPoint")
        {
            onCatExitScreen();
        }
    }
}
