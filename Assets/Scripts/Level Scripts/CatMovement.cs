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
    protected int maxSpeed; //to vary according to adaptive difficulty

    protected void Start()
    {
        mouse = GameObject.Find("Mouse");
        trans = this.transform;
        rb = this.GetComponent<Rigidbody2D>();
        Random.InitState(System.DateTime.Now.Millisecond);
        speed = Random.Range(3, GameManager.getInstance().getMaxSpeed());
        //myAnim = this.GetComponent<Animator>();
    }

    protected void FixedUpdate()
    {
        //Always move forward
        Vector2 vel = rb.velocity;
        vel.x = trans.right.x * speed;
        if (trans.localScale.x < 0)
        {
            vel.x *= -1;
        }
        rb.velocity = vel;

        // to destroy cat once it goes out of screen
        if (trans.position.x < leftLimit || trans.position.x > rightLimit) // kill cat if cat goes out of screen
        {
            Destroy(gameObject);
            GameManager.getInstance().spawnCat();
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
}
