using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Target : MonoBehaviour
{
    // Has this target been hit
    public bool isHit = false;
    // Reference to the ShadowCaster2D component
    public ShadowCaster2D shadowCaster;
    // Reference to Animation
    private Hit_Animation animate;
    private AudioManager audioManager;
    void Start()
    {
        // Get components
        shadowCaster = GetComponent<ShadowCaster2D>();
        animate = GetComponent<Hit_Animation>();
        isHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit) return;
        // If the ball collides with target
        if (collision.gameObject.CompareTag("Ball"))
        {
            DragShoot ball = collision.gameObject.GetComponent<DragShoot>();
            // Only register hit if ball has been shot
            if (ball != null && ball.hasShot)
            {
                // Play target hit sound
                audioManager = FindAnyObjectByType<AudioManager>();
                audioManager.PlayRandomHitAudio();

                shadowCaster.enabled = false; // Disable shadow casting
                BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
                boxCollider.enabled = false; // Disable collider to prevent further hits
                isHit = true;
                animate.Animate(2f, 1f); // Play hit animation, x2 size and 1 sec 
                LevelManager.Instance.TargetHit(); // tell LevelManager that a target been hit
                
            }

        }
    }

    public void ResetTarget() // Reset target to initial state
    {
        isHit = false;
        gameObject.SetActive(true);
        animate.ResetAnimation(); // Reset animation
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = true; // Enable collider
        shadowCaster.enabled = true; // enable shadow casting
    }   
    
    
}
