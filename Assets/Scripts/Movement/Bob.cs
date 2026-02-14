using UnityEngine;

/// <summary>
/// Smoothly bobs the object up and down using a sine wave.
/// Additive: layers its oscillation on top of any existing movement (e.g. from parent, Enemy, etc.).
/// Useful for pickups, floating items, or ambient visual effects.
/// </summary>
public class SlowBob : MonoBehaviour
{
    [Header("Bob Settings")]
    public float amplitude = 0.5f;          // Height of the bob in units
    public float frequency = 1f;            // Cycles per second
    public Vector3 direction = Vector3.up;  // Axis to bob along (default: up)

    private Vector3 _previousBobOffset;

    void LateUpdate()
    {
        // Remove our previous bob to get the "base" position (including any movement from other scripts)
        Vector3 basePosition = transform.position - _previousBobOffset;

        float offset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * amplitude;
        _previousBobOffset = direction.normalized * offset;

        // Add our bob on top
        transform.position = basePosition + _previousBobOffset;
    }
}
