using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReverseClone : MonoBehaviour
{
    //variable stuff
    private List<Vector2> reversedPath;
    private int index = 0;
    private float speed = 5f;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    public ReverseClone SetPath(List<Vector2> originalPath)
    {
        reversedPath = new List<Vector2>(originalPath);
        reversedPath.Reverse();

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (sr != null) sr.color = Color.green;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            //kinematic is main design choice here. if we go rigid, it can collide with stuff, but affected by gravity and crap.
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        //
        StartCoroutine(MoveBackwards());
        return this;
    }

    IEnumerator MoveBackwards()
    {
        while (index < reversedPath.Count - 1)
        {
            Vector2 start = reversedPath[index];
            Vector2 end = reversedPath[index + 1];
            float duration = 0.05f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                //lerp just moves it a lil bit based off a percentage)
                transform.position = Vector2.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null; //wait til next frame
            }

            transform.position = end; //incase other code don't work force player to end position
            index++;
        }

        // Playback done → activate physics
        if (sr != null) sr.color = Color.black;

        if (rb != null)
        {
            //potentially add more stuff, here to differentiate it from normal body. there are many parameters
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = .5f; //makes it more floaty could be cool for parkour stuff
            rb.mass = 1f; //mass is for collisions
            rb.linearDamping = 2f; //how fast it slows down moving
            rb.angularDamping = 5f; //make an
            rb.freezeRotation = true; //doesn't rotate, kind of cool when it does rotate though
            rb.sharedMaterial = null; //removes any bounce/slip physics mat
        }
    }


}
