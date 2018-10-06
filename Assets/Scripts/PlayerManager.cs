using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public float speedX;
    public float jumpSpeedY;

    bool facingRight = true;
    bool goingRight = true;
    bool isJumping = false;
    float speed;

    Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speedX;

        transform.Translate(x, 0, 0);


        // move right, stop
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            speed = -speedX;
            goingRight = false;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            speed = 0;
        }

        // move left, stop
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            speed = speedX;
            goingRight = true;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            speed = 0;
        }

        if (!isJumping && Input.GetKeyDown(KeyCode.UpArrow))
        {
            isJumping = true;
            rb.AddForce(new Vector2(rb.velocity.x, jumpSpeedY));
        }

        // add fake gravity to land faster
        if (isJumping)
        {
            Vector3 vel = rb.velocity;
            vel.y -= 15 * Time.deltaTime;
            rb.velocity = vel;
        }

        // Flip
        if (goingRight && !facingRight || !goingRight && facingRight)
        {
            facingRight = !facingRight;
            Vector3 temp = transform.localScale;
            temp.x *= -1;
            transform.localScale = temp;
        }
    }

    // for jump
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && rb.velocity.y == 0)
        {
            speed = 0;
            rb.velocity = new Vector3(0,0,0);
            isJumping = false;
        }
    }
}
