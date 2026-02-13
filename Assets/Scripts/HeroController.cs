using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float accelerationTime = 0.08f;  // Time to reach full speed (seconds)
    public float decelerationTime = 0.05f;  // Time to stop (seconds) — shorter = snappier
    public float turnBoostMultiplier = 1.5f; // Faster acceleration when changing direction

    [Header("Gravity")]
    public float gravity = 20f;
    public float groundedPullDown = 2f;

    [Header("Input Settings")]
    public KeyCode callKey = KeyCode.Space;

    [Header("Call System (Phase 3)")]
    public float callRadius = 5f;
    [HideInInspector]
    public bool isCalling = false;

    // Internal state
    private CharacterController characterController;
    private Vector3 horizontalVelocity; // XZ movement only
    private float verticalVelocity;     // Gravity only
    private Quaternion lockedRotation;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 1, 0);
        }

        // Lock initial rotation
        lockedRotation = transform.rotation;

        // Disable conflicting components
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.isKinematic = true;
        }

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.enabled = false;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleCallInput();
    }

    void LateUpdate()
    {
        // Enforce locked rotation
        if (transform.rotation != lockedRotation)
        {
            transform.rotation = lockedRotation;
        }
    }

    void HandleMovement()
    {
        // --- 1. RAW INPUT (no Unity smoothing — we handle our own) ---
        float rawH = Input.GetAxisRaw("Horizontal");
        float rawV = Input.GetAxisRaw("Vertical");

        // --- 2. ROTATE INPUT 45° for isometric camera ---
        float rotatedX = rawH + rawV;
        float rotatedZ = rawV - rawH;
        Vector3 inputDirection = new Vector3(rotatedX, 0f, rotatedZ).normalized;
        bool hasInput = inputDirection.magnitude > 0.1f;

        // --- 3. ACCELERATION / DECELERATION ---
        if (hasInput)
        {
            Vector3 targetVelocity = inputDirection * moveSpeed;

            // Detect direction change: dot < 0 means moving opposite to current velocity
            float dot = Vector3.Dot(horizontalVelocity.normalized, inputDirection);
            bool isChangingDirection = dot < 0.5f && horizontalVelocity.magnitude > 0.1f;

            // Use faster acceleration when changing direction (feels more responsive)
            float accelTime = isChangingDirection
                ? accelerationTime / turnBoostMultiplier
                : accelerationTime;

            // Smooth acceleration using SmoothDamp-style exponential approach
            float lerpFactor = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(accelTime, 0.001f));
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, lerpFactor);
        }
        else
        {
            // Smooth deceleration — exponential decay to zero
            float lerpFactor = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(decelerationTime, 0.001f));
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, lerpFactor);

            // Snap to zero when close enough to prevent micro-drift
            if (horizontalVelocity.magnitude < 0.01f)
            {
                horizontalVelocity = Vector3.zero;
            }
        }

        // --- 4. GRAVITY ---
        if (characterController.isGrounded)
        {
            verticalVelocity = -groundedPullDown;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // --- 5. APPLY MOVEMENT ---
        Vector3 finalVelocity = new Vector3(
            horizontalVelocity.x,
            verticalVelocity,
            horizontalVelocity.z
        );
        characterController.Move(finalVelocity * Time.deltaTime);
    }

    void HandleCallInput()
    {
        isCalling = Input.GetKey(callKey);
    }

    // Public accessors for other systems
    public bool IsCalling() => isCalling;
    public float GetCallRadius() => callRadius;
    public Vector3 GetHorizontalVelocity() => horizontalVelocity;
    public bool IsMoving() => horizontalVelocity.magnitude > 0.1f;
}
