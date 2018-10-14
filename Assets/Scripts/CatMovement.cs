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

//public static class ExtensionMethods
//{
//    public static Vector2 toVector2(this Vector3 vec3)
//    {
//        return new Vector2(vec3.x, vec3.y);
//    }
//}

public class CatMovement : MonoBehaviour
{
    //public LayerMask enemyMask;
    public float speed = 1;
    public float leftLimit, rightLimit;
    //public float myWidth, myHeight;

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
        //SpriteRenderer mySprite = this.GetComponent<SpriteRenderer>();
        //myWidth = mySprite.bounds.extents.x;
        //myHeight = mySprite.bounds.extents.y;
    }

    protected void FixedUpdate()
    {
        //NOTE: This script makes use of the .toVector2() extension method.
        //Be sure you have the following script in your project to avoid errors
        //http://www.devination.com/2015/07/unity-extension-method-tutorial.html

        ////Use this position to cast the isGrounded/isBlocked lines from
        //Vector2 lineCastPos = myTrans.position.toVector2() - myTrans.right.toVector2() * myWidth - Vector2.up * (myHeight / 2);

        ////Check to see if there's ground in front of us before moving forward
        ////NOTE: Unity 4.6 and below use "- Vector2.up" instead of "+ Vector2.down"
        //Debug.DrawLine(lineCastPos, lineCastPos + Vector2.down);
        //bool isGrounded = Physics2D.Linecast(lineCastPos, lineCastPos + Vector2.down, enemyMask);

        ////Check to see if there's a wall in front of us before moving forward
        //Debug.DrawLine(lineCastPos, lineCastPos - myTrans.right.toVector2() * .05f);
        //bool isBlocked = Physics2D.Linecast(lineCastPos, lineCastPos - myTrans.right.toVector2() * .05f, enemyMask);

        ////If theres no ground, turn around. Or if I hit a wall, turn around
        ///IF THERE IS NO GROUND, CAN POSSIBLY CHANGE TO DO A SMALL JUMP ACTION OFF THE ROAD
        //if (!isGrounded || isBlocked)
        //{
        //    Vector3 currRot = myTrans.eulerAngles;
        //    currRot.y += 180;
        //    myTrans.eulerAngles = currRot;
        //}

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
