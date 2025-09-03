using UnityEngine;
using UnityEngine.InputSystem;

public class DragShoot : MonoBehaviour
{
    
    public Rigidbody2D rb;
    private LineRenderer lr; // lineRenderer to show drag direction
    private SpriteRenderer sr;
    private Vector2 dragStartPos;
    public Hit_Animation animate;
    private LevelManager levelManager;
    private AudioManager audioManager;
    private bool isDragging = false;
    private float stillTime = 0f; // Time the ball has been still
    public bool hasShot = false; // Has the ball been shot
    public bool isVisible = false;
    public bool waitingToReappear = false;  
    public bool HasTouchedBad = false;
    public bool HasReset = false;
    



    [Header("Force Settings")]
    // max distance the object can be dragged
    private float maxDragDistance = 2.5f;
    // multiplier for the force applied when shooting
    private float forceMultiplier = 600f;

    private InputAction pointerAction; // Track position of the pointer
    private InputAction clickAction; // Tracks click state

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        // Sprite to be invisible initially
        sr.enabled = false;

        // Define components
        animate = GetComponent<Hit_Animation>();
        rb = GetComponent<Rigidbody2D>();
        dragStartPos = transform.position;
        rb.bodyType = RigidbodyType2D.Kinematic; // No physics till respawn
        lr = GetComponent<LineRenderer>();
        sr = GetComponent<SpriteRenderer>();

        


        // Set variables for ball rigidbody
        rb.gravityScale = 0;
        rb.linearDamping = 1.3f;
        rb.mass = 1f;
        // Set variables for line renderer
        lr.positionCount = 0;
        lr.startWidth = 0.5f;
        lr.endWidth = 0.1f;
        // Make line renderer fade out, white to transparent
        lr.startColor = new Color(1f, 1f, 1f, 1f);
        lr.endColor = new Color(1f, 1f, 1f, 0f);

        // Create input actions in code
        pointerAction = new InputAction(type: InputActionType.Value, binding: "<Pointer>/position");
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");

        pointerAction.Enable();
        clickAction.Enable();
        
    }

    public void Animate()
    {
        animate.Animate(1f, 0.5f); // Play hit animation, x2 size and 1 sec 
    }
    public void ResetBall()
    {
        // Reset ball position and state
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic; // No physics till respawn
        sr.enabled = false; // Hide sprite
        isVisible = false;
        hasShot = false;
        waitingToReappear = false;
        HasTouchedBad = false;
        HasReset = false;
        transform.position = new Vector2(100f, 100f);
        animate.ResetAnimation(); // Reset animation
    }
    void OnDestroy()
    {
        pointerAction.Disable();
        clickAction.Disable();
    }

    void Update()
    {
        // Get pointer position
        Vector2 pointerPos = Camera.main.ScreenToWorldPoint(pointerAction.ReadValue<Vector2>());

        // If ball is not visible and not waiting to reappear, allow ball to reappear on click
        if (!isVisible && !waitingToReappear && clickAction.WasPressedThisFrame())
        {
            // Respawn ball at pointer position
            transform.position = pointerPos;
            sr.enabled = true;
            isVisible = true;
            hasShot = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0;
            // Enable physics
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (isVisible && !hasShot) // Allow drag and shoot only if ball is visible and hasn't been shot yet, so dragging stage
        {
            // Start dragging at pointer position
            if (clickAction.WasPressedThisFrame())
            {
                dragStartPos = pointerPos;
                isDragging = true;
            }
            // Continue dragging and update line
            else if (clickAction.IsPressed() && isDragging)
            {
                UpdateLine(dragStartPos, pointerPos);
            }
            // Release and shoot
            else if (clickAction.WasReleasedThisFrame() && isDragging)
            {
                Shoot(dragStartPos, pointerPos);
                lr.positionCount = 0;
                isDragging = false;
                hasShot = true;
            }
        }
        // If ball is visible and has been shot, check if it is out of bounds or has stopped moving
        levelManager = FindAnyObjectByType<LevelManager>();
        if (isVisible && hasShot && !HasTouchedBad && !waitingToReappear && !levelManager.AllTargetsCleared())
        {
            Vector3 currentPosition = transform.position;
            // Check if out of bounds
            if (currentPosition.y < -6f || currentPosition.y > 6f || currentPosition.x < -4f || currentPosition.x > 4f)
            {
                // If out of bounds, wait for 1s before resetting
                waitingToReappear = true;
                Invoke(nameof(Reset), 1f);
            }
            else
            {
                if (rb.linearVelocity.magnitude < 0.02f)
                {
                    // If ball is almost still, start counting still time
                    stillTime += Time.deltaTime;
                }
                else
                {
                    stillTime = 0f; // Reset still time if ball is moving
                }

                if (stillTime > 0.3)
                {
                    // If ball has been still for more than 0.3s, reset after 1 second. 0.3 to prevent from running immediately after shooting
                    waitingToReappear = true;
                    stillTime = 0f;
                    // Play animation fade out
                    Animate();
                    Invoke(nameof(Reset), 1f);
                }
            }
        }
    }
    
    

    public void Reset() 
    {
        LevelManager.Instance.ResetLevel(); // Reset level through LevelManager
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // if ball hits a bad block, play bad hit sound and reset ball after animation and 2 secs
        if (collision.gameObject.CompareTag("Bad"))
        {
            if (isVisible == true && hasShot == true)
            {
                audioManager = FindAnyObjectByType<AudioManager>();
                audioManager.PlayBadHitAudio();
                HasTouchedBad = true;
                rb.linearVelocity = Vector2.zero; // Stop ball movement
                Animate();
                waitingToReappear = true;
                Invoke(nameof(Reset), 2f);

            }
        }
        // if ball hits a solid block, play solid hit sound
        if (collision.gameObject.CompareTag("Solid"))
        {
            if (isVisible == true && hasShot == true)
            {
                audioManager = FindAnyObjectByType<AudioManager>();
                audioManager.PlaySolidHitAudio();
            }
        }
    }
    // Update the line renderer to show drag direction
    void UpdateLine(Vector2 start, Vector2 end)
    {
        Vector2 direction = start - end;

        // Find minimum between actual distance and max drag distance, so drag distance is capped
        float distance = Mathf.Min(direction.magnitude, maxDragDistance);
        Vector2 clampedEnd = start + direction.normalized * distance;

        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + (Vector3)(start - clampedEnd));
    }

    // Calculate and apply force to shoot the object
    void Shoot(Vector2 start, Vector2 end)
    {
        Vector2 direction = start - end;
        float distance = Mathf.Min(direction.magnitude, maxDragDistance);
        Vector2 force = direction.normalized * distance * forceMultiplier;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force);
    }
}