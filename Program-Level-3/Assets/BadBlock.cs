using UnityEngine;
using System.Collections;
using NUnit.Framework;
public class BadBlock : MonoBehaviour
{

    public bool eliminated = false;
    public bool HasTouchedBad = false;
    void Start()
    {

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Find the DragShoot script on the ball
            DragShoot ball = collision.gameObject.GetComponent<DragShoot>();

            if (ball != null)
            {
                ball.Animate();

                ball.rb.linearVelocity = Vector2.zero; // Stop ball movement
                ball.waitingToReappear = true;
                eliminated = true;
                Invoke(nameof(ball.Reset), 2f);

            }
        }
    }
    
    
}
