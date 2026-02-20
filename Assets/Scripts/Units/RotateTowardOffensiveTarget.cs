using UnityEngine;

/// <summary>
/// Rotates this transform around the Y axis so it points at the current offensive target.
/// Use on a turret head, ballista, or any object that should face the target horizontally.
/// </summary>
[RequireComponent(typeof(OffensiveTargeting))]
public class RotateTowardOffensiveTarget : MonoBehaviour
{
    [Header("Targeting")]
    [Tooltip("If null, uses OffensiveTargeting on this object or in children.")]
    public OffensiveTargeting offensiveTargeting;

    [Header("Rotation")]
    [Tooltip("Transform to rotate (default: this transform). Use a child for turret head only.")]
    public Transform rotateTransform;
    [Tooltip("How fast to turn in degrees per second (0 = instant).")]
    public float turnSpeed = 360f;
    [Tooltip("When no target, rotate toward default target or keep current rotation.")]
    public bool fallbackToDefaultTarget = true;

    private void Start()
    {
        if (offensiveTargeting == null)
            offensiveTargeting = GetComponent<OffensiveTargeting>();
        if (offensiveTargeting == null)
            offensiveTargeting = GetComponentInChildren<OffensiveTargeting>();
        if (rotateTransform == null)
            rotateTransform = transform;
    }

    private void Update()
    {
        if (offensiveTargeting == null || rotateTransform == null) return;

        Transform targetTransform = GetTargetTransform();
        if (targetTransform == null) return;

        Vector3 targetPos = targetTransform.position;
        Vector3 myPos = rotateTransform.position;

        // Flatten to XZ so we only rotate around Y
        Vector3 toTarget = targetPos - myPos;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f) return;

        toTarget.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(toTarget, Vector3.up);

        if (turnSpeed <= 0f)
        {
            rotateTransform.rotation = targetRotation;
        }
        else
        {
            rotateTransform.rotation = Quaternion.RotateTowards(
                rotateTransform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }
    }

    private Transform GetTargetTransform()
    {
        if (offensiveTargeting.target != null && offensiveTargeting.target.activeInHierarchy)
            return offensiveTargeting.target.transform;
        if (fallbackToDefaultTarget)
            return offensiveTargeting.GetDefaultTarget();
        return null;
    }
}
