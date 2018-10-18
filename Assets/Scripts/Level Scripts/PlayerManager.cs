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
            goingRight = false;
        }

        // move left, stop
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            goingRight = true;
        }

        if (!isJumping && Input.GetKeyDown(KeyCode.Space))
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
            Debug.Log("Game over!");
            GameManager.getInstance().gameOver();
        }

    }
    
    // on collision with other cats
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "CatBack") // then kill cat
        {
            Debug.Log("You killed a cat!");
            // TODO: increment cat kill count
            // TODO: implement stun all cats when one cat dies
            GameManager.getInstance().destroyCat(collision.gameObject.GetComponent<Transform>().parent.gameObject);
            GameManager.getInstance().stunAllCats();
            GameManager.getInstance().spawnCat();
        }

        // if mouse touches front of cat
        if (collision.gameObject.tag == "Cat") // then game over
        {
            // game over code
            Debug.Log("Game over!");
            GameManager.getInstance().gameOver();
        }
    }
}
