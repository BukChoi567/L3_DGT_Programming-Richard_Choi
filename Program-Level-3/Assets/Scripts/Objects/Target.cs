using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Target : MonoBehaviour
{
    // Indicate if the target is been hit
    public bool isHit { get; private set; } = false;

    public ShadowCaster2D shadowCaster;
    private Hit_Animation animate;
    void Start()
    {
        shadowCaster = GetComponent<ShadowCaster2D>();
        animate = GetComponent<Hit_Animation>();
        isHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit) return;

        if (collision.gameObject.CompareTag("Ball"))
        {
            DragShoot ball = collision.gameObject.GetComponent<DragShoot>();

            if (ball != null && ball.hasShot)
            {
                
                shadowCaster.enabled = false; // Disable shadow casting

                BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
                boxCollider.enabled = false; // Disable collider to prevent further hits
                isHit = true;
                animate.Animate(2f, 1f); // Play hit animation, x2 size and 1 sec 
                LevelManager.Instance.TargetHit(); // tell LevelManager that a target been hit
                
                

            }

        }
    }

    public void ResetTarget()
    {
        isHit = false;
        gameObject.SetActive(true);
        animate.ResetAnimation(); // Reset animation
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = true; // Enable collider
        shadowCaster.enabled = true; // enable shadow casting
    }   
    
    // Update is called once per frame
    void Update()
    {    


    }
    
}
