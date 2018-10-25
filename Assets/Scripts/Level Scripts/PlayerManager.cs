using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public float speedX;
    public float jumpSpeedY;

    private bool facingRight = true;
    private bool goingRight = true;
    private bool isJumping = false;

    private const int BASIC_CAT_INDEX = 0;
    private const int JUMPING_CAT_INDEX = 1;
    private const int CHARGING_CAT_INDEX = 2;

    Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speedX * 100;
        rb.velocity = new Vector3(x, rb.velocity.y, 0);


        // move right, stop
        if (Input.GetKeyDown(KeyCode.LeftArrow) && goingRight)
        {
            goingRight = false;
        }
        // move left, stop
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !goingRight)
        {
            goingRight = true;
        }

        if (!isJumping && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeedY, 0);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // for jump
        if (collision.gameObject.tag == "Ground" && rb.velocity.y == 0)
        {
            rb.velocity = new Vector3(0,0,0);
            isJumping = false;
        }

        // if mouse touches bottom of cat
        if (collision.gameObject.tag == "Cat") // then kill player
        {
            // game over code
            GameManager.getInstance().gameOver();
        }

    }
    
    // on collision with other cats
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "CatBack") // then kill cat
        {
            GameObject deadCat = collision.gameObject.GetComponent<Transform>().parent.gameObject;
            GameManager.getInstance().onSuccessfulKill(deadCat);
        }

        // if mouse touches front of cat
        if (collision.gameObject.tag == "Cat") // then game over
        {
            Debug.Log("Game over!");
            GameManager.getInstance().gameOver();
        }
    }
}
