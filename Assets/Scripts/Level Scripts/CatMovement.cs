using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CatMovement : MonoBehaviour
{
    //public LayerMask enemyMask;
    public float leftLimit, rightLimit;

    protected Rigidbody2D rb;
    protected Transform trans;
    protected Animator anim;
    protected GameObject mouse;
    protected int speed;
    protected int maxSpeed; // TODO: to vary according to adaptive difficulty
    protected bool isStunned;

    protected void Start()
    {
        mouse = GameObject.Find("Mouse");
        trans = this.transform;
        rb = this.GetComponent<Rigidbody2D>();
        Random.InitState(System.DateTime.Now.Millisecond);
        speed = Random.Range(3, GameManager.getInstance().getMaxSpeed());
        isStunned = false;
        //myAnim = this.GetComponent<Animator>();
    }

    protected void FixedUpdate()
    {
        //Always move forward
        if (!isStunned)
        {
            Vector2 vel = rb.velocity;
            vel.x = trans.right.x * speed;
            if (trans.localScale.x < 0)
            {
                vel.x *= -1;
            }
            rb.velocity = vel;
        }
    }

    public void setSpeed(int newSpeed)
    {
        speed = newSpeed;
    }

    public int getSpeed()
    {
        return speed;
    }

    public void setIsStunned(bool stun)
    {
        isStunned = stun;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if cat exits the screen
        if (collision.gameObject.tag == "StopPoint")
        {
            GameManager.getInstance().destroyCat(gameObject);
            GameManager.getInstance().spawnCat();
        }
    }
}
