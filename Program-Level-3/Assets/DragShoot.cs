using UnityEngine;
using UnityEngine.InputSystem;

public class DragShoot : MonoBehaviour
{
    
    public Rigidbody2D rb;
    private LineRenderer lr;
    private SpriteRenderer sr;
    private Vector2 dragStartPos;
    public Hit_Animation animate;

    private bool isDragging = false;
    public bool hasShot = false;
    public bool isVisible = false;
    public bool waitingToReappear = false;

    public bool HasTouchedBad = false;

    [Header("Force Settings")]
    // max distance the object can be dragged
    public float maxDragDistance = 3f;
    // multiplier for the force applied when shooting
    public float forceMultiplier = 500f;

    private InputAction pointerAction;
    private InputAction clickAction;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        // Sprite to be invisible initially
        sr.enabled = false;

        animate = GetComponent<Hit_Animation>();

        rb = GetComponent<Rigidbody2D>();
        dragStartPos = transform.position;
        rb.bodyType = RigidbodyType2D.Kinematic; // No physics till respawn
        lr = GetComponent<LineRenderer>();
        sr = GetComponent<SpriteRenderer>();

        lr.startColor = new Color(1f, 1f, 1f, 1f);
        lr.endColor = new Color(1f, 1f, 1f, 0f);

        // Set variables
        rb.gravityScale = 0;
        rb.linearDamping = 2f;

        lr.positionCount = 0;
        lr.startWidth = 0.5f;
        lr.endWidth = 0.1f;

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
        Vector2 pointerPos = Camera.main.ScreenToWorldPoint(pointerAction.ReadValue<Vector2>());

        // If ball is not visible and not waiting to reappear, allow ball to reappear on click
        if (!isVisible && !waitingToReappear && clickAction.WasPressedThisFrame())
        {
            transform.position = pointerPos;
            sr.enabled = true;
            isVisible = true;
            hasShot = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            // Disable gravity
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (isVisible && !hasShot)
        {
            if (clickAction.WasPressedThisFrame())
            {
                dragStartPos = pointerPos;
                isDragging = true;
            }
            else if (clickAction.IsPressed() && isDragging)
            {
                UpdateLine(dragStartPos, pointerPos);
            }
            else if (clickAction.WasReleasedThisFrame() && isDragging)
            {
                Shoot(dragStartPos, pointerPos);
                lr.positionCount = 0;
                isDragging = false;
                hasShot = true;
            }
        }

        if (isVisible && hasShot && rb.linearVelocity.magnitude < 0.00001f && !waitingToReappear)
        {
            // If ball not moving and has been shot, wait for 0.5s before making it invisible
            waitingToReappear = true;

            Invoke(nameof(Reset), 3f);
        }

    }

    public void Reset()
    {
        LevelManager.Instance.ResetLevel(); // Reset level when ball stops moving
    }
    void UpdateLine(Vector2 start, Vector2 end)
    {
        Vector2 direction = start - end;
        float distance = Mathf.Min(direction.magnitude, maxDragDistance);
        Vector2 clampedEnd = start + direction.normalized * distance;

        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + (Vector3)(start - clampedEnd));
    }

    void Shoot(Vector2 start, Vector2 end)
    {
        Vector2 direction = start - end;
        float distance = Mathf.Min(direction.magnitude, maxDragDistance);
        Vector2 force = direction.normalized * distance * forceMultiplier;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force);
    }
}