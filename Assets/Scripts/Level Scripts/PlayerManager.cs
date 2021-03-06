﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public float speedX = 5.0f;
    public float jumpSpeedY;

    private bool facingRight = true;
    private bool goingRight = true;
    private bool isJumping = false;
    private bool isDead;

    private const int BASIC_CAT_INDEX = 0;
    private const int JUMPING_CAT_INDEX = 1;
    private const int CHARGING_CAT_INDEX = 2;

    private Animator anim;
    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isDead = false;
    }

    void Update()
    {
        if (GameManager.getInstance().debugInvincibleMode)
        {
            isDead = false;
        }

        if (!isDead)
        {
            // move left, stop
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * speedX * 100;

            if (Input.GetKeyDown(KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.RightArrow))
            {
                x = 0;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                goingRight = false;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                goingRight = true;
            }
            rb.velocity = new Vector3(x, rb.velocity.y, 0);

            if (!isJumping && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)))
            {
                jump(false);
                //isJumping = true;
                //SfxManager.PlaySound("jump");
                //anim.SetBool("isJumping", true);
                //rb.velocity = new Vector3(rb.velocity.x, jumpSpeedY, 0);
            }
            // add fake gravity to land faster
            if (isJumping)
            {
                Vector3 vel = rb.velocity;
                vel.y -= 15 * Time.deltaTime;
                rb.velocity = vel;
            }

            if (rb.velocity.x != 0)
            {
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
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
        
    }

    public void jump(bool isRecoil)
    {
        isJumping = true;
        SfxManager.PlaySound("jump");
        anim.SetBool("isJumping", true);

        if (isRecoil)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeedY * 0.8f, 0);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeedY, 0);
        }
    }

    private bool catIsStunned(GameObject cat)
    {
        if (cat.GetComponent<ChargeCatMovement>() != null)
        {
            return cat.GetComponent<ChargeCatMovement>().getIsStunned();
        }
        else if (cat.GetComponent<JumpCatMovement>() != null)
        {
            return cat.GetComponent<JumpCatMovement>().getIsStunned();
        }
        else if (cat.GetComponent<CatMovement>() != null)
        {
            return cat.GetComponent<CatMovement>().getIsStunned();
        }
        else if (cat.transform.parent.gameObject.GetComponent<ChargeCatMovement>() != null)
        {
            return cat.transform.parent.gameObject.GetComponent<ChargeCatMovement>().getIsStunned();
        }
        else if (cat.transform.parent.gameObject.GetComponent<JumpCatMovement>() != null)
        {
            return cat.transform.parent.gameObject.GetComponent<JumpCatMovement>().getIsStunned();
        }
        else if (cat.transform.parent.gameObject.GetComponent<CatMovement>() != null)
        {
            return cat.transform.parent.gameObject.GetComponent<CatMovement>().getIsStunned();
        }

        Debug.Log("not stunned");
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // for jump
        if (isJumping && collision.gameObject.tag == "Ground" && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector3(0, 0, 0);
            isJumping = false;
            anim.SetBool("isJumping", false);
        }

        // if mouse touches bottom of cat
        if (collision.gameObject.tag == "Cat") // then kill player
        {
            if (!catIsStunned(collision.gameObject))
            {
                // game over code
                anim.SetTrigger("isDead");
                if (!isDead)
                {
                    StartCoroutine(lagGameOver());
                }
            }
            else
            {
                GameObject deadCat = collision.gameObject.GetComponent<Transform>().parent.gameObject;
                SfxManager.PlaySound("killCat");
                GameManager.getInstance().onSuccessfulKill(deadCat);
            }
        }
    }

    // on collision with other cats
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead && collision.gameObject.tag == "CatBack") // then kill cat
        {
            GameObject deadCat = collision.gameObject.GetComponent<Transform>().parent.gameObject;
            SfxManager.PlaySound("killCat");
            GameManager.getInstance().onSuccessfulKill(deadCat);
        }

        // if mouse touches front of cat
        else if (collision.gameObject.tag == "Cat") // then game over
        {
            if (!catIsStunned(collision.gameObject))
            {
                anim.SetTrigger("isDead");
                if (!isDead)
                {
                    StartCoroutine(lagGameOver());
                }
            }
            else
            {
                GameObject deadCat = collision.gameObject.GetComponent<Transform>().parent.gameObject;
                SfxManager.PlaySound("killCat");
                GameManager.getInstance().onSuccessfulKill(deadCat);
            }
        }
    }

    IEnumerator lagGameOver()
    {
        isDead = true;
        yield return new WaitForSeconds(0.4f); // delay to play explosion anim
        SfxManager.PlaySound("gameOverSound");
        GameManager.getInstance().gameOver();
    }
}
