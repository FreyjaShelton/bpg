using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float GroundSpeed = 30f;
    [SerializeField]
    private float AirSpeed = 5f;
    [SerializeField]
    private float JumpHeight = 10f;
    [SerializeField]
    private float DashSpeed = 45f;
    [SerializeField]
    private float MaxSpeed = 100f;
    [SerializeField]
    private float DistToGround = .5f;
    [SerializeField]
    private float fallMultiplier = 2.5f;
    [SerializeField]
    private float lowJumpMultiplier = 2f;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private LayerMask GroundLayerMask;

    private Rigidbody body;
    private Collider sphereCollider;
    private PlayerInput playerInput;
    private Vector3 direction;

    // Store actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction brakeAction;

    // Trigger actions
    private bool dashTriggered;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        sphereCollider = GetComponent<Collider>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        dashAction = playerInput.actions["Dash"];
        dashAction = playerInput.actions["Brake"];
    }

    void Update()
    {
        // Collect Move Input
        Vector2 inputs = moveAction.ReadValue<Vector2>();
        direction = new Vector3(inputs.x, 0f, inputs.y);

        // Keep Player below max speed
        if (body.velocity.magnitude > MaxSpeed)
        {
            body.velocity = (Vector3.ClampMagnitude(body.velocity, MaxSpeed));
        }

        // Check for triggered actions
        if (jumpAction.triggered && IsGrounded())
        {
            Debug.Log("Jump");
            body.AddForce(Vector3.up * JumpHeight, ForceMode.VelocityChange);
        }

        if (dashAction.triggered)
        {
            Debug.Log("Dash");
            dashTriggered = true;
        }
    }

    // need to add air movement with gravity
    void FixedUpdate()
    {
        // Handle movement
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        if (direction != Vector3.zero)
        {
            if (dashTriggered)
            {
                body.AddForce(moveDir * DashSpeed, ForceMode.VelocityChange);
                dashTriggered = false;
            } 
            else if (IsGrounded())
            {
                body.AddForce(moveDir * GroundSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
            else
            {
                body.AddForce(moveDir * AirSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }

        // Handle falling
        if (body.velocity.y < 0 && !IsGrounded())
        {
            body.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        } else if (body.velocity.y > 0 && !IsGrounded() && jumpAction.)
        {
            body.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(sphereCollider.bounds.center, sphereCollider.bounds.extents.y, GroundLayerMask);
    }
}