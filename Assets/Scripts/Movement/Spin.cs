using UnityEngine;

/// <summary>
/// Continuously rotates the object at a configurable speed.
/// Useful for pickups, decorations, or ambient visual effects.
/// </summary>
public class SlowRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 axis = Vector3.up;       // Axis to rotate around (default: Y)
    public float degreesPerSecond = 45f;    // Rotation speed
    public bool useWorldSpace = true;       // Use world axis vs local axis

    void Update()
    {
        float angle = degreesPerSecond * Time.deltaTime;
        Vector3 rotAxis = useWorldSpace ? axis.normalized : transform.TransformDirection(axis.normalized);
        transform.Rotate(rotAxis, angle, useWorldSpace ? Space.World : Space.Self);
    }
}
