using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(LineRenderer))]
public class DragShoot : MonoBehaviour
{
    private Rigidbody2D rb;
    private LineRenderer lr;
    private Vector2 dragStartPos;
    private bool isDragging = false;

    [Header("Force Settings")]
    public float maxDragDistance = 3f;
    public float forceMultiplier = 500f;

    private InputAction pointerAction;
    private InputAction clickAction;

    void Start()
    {



        lr.startColor = new Color(1f, 1f, 1f, 1f);
        lr.endColor = new Color(1f, 1f, 1f, 0f);
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        
        rb.gravityScale = 0;
        lr.positionCount = 0;

        lr.startWidth = 0.5f;
        lr.endWidth = 0f;

        // Create input actions in code
        pointerAction = new InputAction(type: InputActionType.Value, binding: "<Pointer>/position");
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");

        pointerAction.Enable();
        clickAction.Enable();
    }

    void OnDestroy()
    {
        pointerAction.Disable();
        clickAction.Disable();
    }

    void Update()
    {
        Vector2 pointerPos = Camera.main.ScreenToWorldPoint(pointerAction.ReadValue<Vector2>());

        

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
        }
    }

    void UpdateLine(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
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