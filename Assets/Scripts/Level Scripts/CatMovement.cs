using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CatMovement : MonoBehaviour
{
    //public LayerMask enemyMask;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Transform trans;
    protected GameObject mouse;
    protected float speed;
    protected int maxSpeed; // TODO: to vary according to adaptive difficulty
    protected bool isStunned;

    protected void Start()
    {
        mouse = GameObject.Find("Mouse");
        trans = this.transform;
        rb = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponentInChildren<Animator>();
        Random.InitState(System.DateTime.Now.Millisecond);
        //speed = Random.Range(3, GameManager.getInstance().getMaxSpeed());
        speed = 3;
        isStunned = false;
        //myAnim = this.GetComponent<Animator>();
    }

    protected void FixedUpdate()
    {
        //Always move forward
        if (isStunned)
        {
            if (rb != null)
            {
                rb.velocity = new Vector3(0, 0, 0);
            }
            anim.SetBool("isStunned", true);
        }
        else if (rb != null)
        {
            anim.SetBool("isStunned", false);
            Vector2 vel = rb.velocity;
            vel.x = trans.right.x * speed;
            if (trans.localScale.x < 0)
            {
                vel.x *= -1;
            }
            rb.velocity = vel;
        }
    }

    protected void onCatExitScreen()
    {
        GameManager.getInstance().destroyCat(gameObject);
        GameManager.getInstance().updateSpawnDelay();
        GameManager.getInstance().numCatsEscaped++;
        Debug.Log("numCatsEscaped " + GameManager.getInstance().numCatsEscaped);

    }

    public void setSpeed(int newSpeed)
    {
        speed = newSpeed;
    }

    public float getSpeed()
    {
        return speed;
    }

    public void setIsStunned(bool stun)
    {
        isStunned = stun;
    }

    public bool getIsStunned()
    {
        return isStunned;
    }

    public void onCatDeath()
    {
        gameObject.tag = "Untagged";
        //gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //Destroy(rb);
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.tag != "CatBack" && child.gameObject.GetComponent<BoxCollider2D>() != null)
            {
                //child.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                child.gameObject.tag = "Untagged";
                
            }
        }
        anim.SetTrigger("isDead");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if cat exits the screen
        if (collision.gameObject.tag == "StopPoint")
        {
            onCatExitScreen();
        }
    }
}
