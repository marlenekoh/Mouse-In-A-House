/*/
* Script by Devin Curry
* www.Devination.com
* www.youtube.com/user/curryboy001
* Please like and subscribe if you found my tutorials helpful :D
* Google+ Community: https://plus.google.com/communities/108584850180626452949
* Twitter: https://twitter.com/Devination3D
* Facebook Page: https://www.facebook.com/unity3Dtutorialsbydevin
/*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CatMovement : MonoBehaviour
{
    //public LayerMask enemyMask;
    public float speed = 1;
    public float leftLimit, rightLimit;

    protected Rigidbody2D rb;
    protected Transform trans;
    protected Animator anim;

    protected GameObject mouse;

    protected void Awake()
    {
        mouse = GameObject.Find("Mouse");
    }

    protected void Start()
    {
        trans = this.transform;
        rb = this.GetComponent<Rigidbody2D>();
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
            GameManager.Instance.spawnCat();
        }
    }
}
