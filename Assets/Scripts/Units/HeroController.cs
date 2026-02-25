using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float accelerationTime = 0.08f;  // Time to reach full speed (seconds)
    public float decelerationTime = 0.05f;  // Time to stop (seconds) — shorter = snappier
    public float turnBoostMultiplier = 1.5f; // Faster acceleration when changing direction

    [Header("Gravity")]
    public float gravity = 20f;
    public float groundedPullDown = 2f;

    [Header("Rotation")]
    [Tooltip("Optional: assign the visual model here; only this will rotate to face movement direction.")]
    public GameObject model;
    public float rotationSpeed = 10f;  // How quickly the model pivots to face movement direction

    [Header("Input Settings")]
    public KeyCode callKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Sprint")]
    [Tooltip("Speed multiplier when sprinting.")]
    private float sprintSpeedMultiplier = 2.5f;

    [Header("Call System (Phase 3)")]
    public float callRadius = 5f;
    [HideInInspector]
    public bool isCalling = false;

    [Header("Coin Magnet")]
    [Tooltip("Coins within this radius accelerate towards the hero and are collected on contact.")]
    public float magnetRadius = 4f;

    [Header("Animation")]
    public Animator animator;
    [Tooltip("Animator bool parameter names. Leave empty to skip updating that parameter.")]
    public string animParamIsMoving = "isMoving";
    public string animParamIsSprinting = "isSprinting";
    public string animParamIsDead = "isDead";

    // Internal state
    private CharacterController characterController;
    private Health health;
    private Vector3 horizontalVelocity; // XZ movement only
    private float verticalVelocity;     // Gravity only
    private bool isSprinting;
    private bool hasMovementInput;      // True the frame player gives input — used for instant animation switch


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        health = GetComponent<Health>();

        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 1, 0);
        }

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
        UpdateAnimator();
    }

    void HandleMovement()
    {
        bool dead = health != null && health.IsDead();
        if (dead)
        {
            isSprinting = false;
            hasMovementInput = false;
            float lerpFactor = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(decelerationTime, 0.001f));
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, lerpFactor);
            if (horizontalVelocity.magnitude < 0.01f) horizontalVelocity = Vector3.zero;
            verticalVelocity -= gravity * Time.deltaTime;
            characterController.Move(new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z) * Time.deltaTime);
            return;
        }

        // --- 1. RAW INPUT (no Unity smoothing — we handle our own) ---
        float rawH = Input.GetAxisRaw("Horizontal");
        float rawV = Input.GetAxisRaw("Vertical");

        // --- 2. ROTATE INPUT 45° for isometric camera ---
        float rotatedX = rawH + rawV;
        float rotatedZ = rawV - rawH;
        Vector3 inputDirection = new Vector3(rotatedX, 0f, rotatedZ).normalized;
        bool hasInput = inputDirection.magnitude > 0.1f;
        hasMovementInput = hasInput;

        // --- 3. SPRINT & TARGET SPEED ---
        isSprinting = hasInput && Input.GetKey(sprintKey);
        float currentMoveSpeed = isSprinting ? moveSpeed * sprintSpeedMultiplier : moveSpeed;

        // --- 4. ACCELERATION / DECELERATION ---
        if (hasInput)
        {
            Vector3 targetVelocity = inputDirection * currentMoveSpeed;

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

        // --- 5. GRAVITY ---
        if (characterController.isGrounded)
        {
            verticalVelocity = -groundedPullDown;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // --- 6. PIVOT MODEL TO FACE MOVEMENT DIRECTION ---
        if (model != null && horizontalVelocity.magnitude > 0.1f)
        {
            Vector3 lookDirection = horizontalVelocity.normalized;
            lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // --- 7. APPLY MOVEMENT ---
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

    void UpdateAnimator()
    {
        if (animator == null) return;

        bool dead = health != null && health.IsDead();

        // Use input so animation switches the frame the player presses a key, not when velocity builds up
        if (!string.IsNullOrEmpty(animParamIsMoving))
            animator.SetBool(animParamIsMoving, hasMovementInput);
        if (!string.IsNullOrEmpty(animParamIsSprinting))
            animator.SetBool(animParamIsSprinting, isSprinting);
        if (!string.IsNullOrEmpty(animParamIsDead))
            animator.SetBool(animParamIsDead, dead);
    }

    // Public accessors for other systems
    public bool IsCalling() => isCalling;
    public float GetCallRadius() => callRadius;
    public float GetMagnetRadius() => magnetRadius;
    public Vector3 GetHorizontalVelocity() => horizontalVelocity;
    public bool IsMoving() => horizontalVelocity.magnitude > 0.1f;
    public bool IsSprinting() => isSprinting;
}
