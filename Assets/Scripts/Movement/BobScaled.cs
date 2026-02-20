using UnityEngine;

/// <summary>
/// Same as Bob, but modulates bob frequency based on how fast the object is moving.
/// Additive: layers its oscillation on top of any existing movement.
/// </summary>
public class BobScaled : MonoBehaviour
{
    [Header("Bob Settings")]
    public float amplitude = 0.5f;          // Height of the bob in units
    public float baseFrequency = 1f;        // Frequency when stationary
    public Vector3 direction = Vector3.up;  // Axis to bob along (default: up)

    [Header("Velocity Modulation")]
    [Tooltip("Extra frequency per unit of speed. Positive = faster when moving, negative = slower when moving.")]
    public float frequencyPerSpeed = 0.5f;
    public float minFrequency = 0.1f;       // Floor to avoid zero or negative
    public float maxFrequency = 10f;        // Cap to avoid excessive speed

    private Vector3 _previousBobOffset;
    private Vector3 _lastBasePosition;
    private float _phase;
    private bool _initialized;

    void LateUpdate()
    {
        // Remove our previous bob to get the "base" position (real movement from other scripts)
        Vector3 basePosition = transform.position - _previousBobOffset;

        // Compute speed from base position delta (ignores our own bob so we don't feedback)
        float speed = 0f;
        if (_initialized)
            speed = Vector3.Distance(basePosition, _lastBasePosition) / Mathf.Max(Time.deltaTime, 0.0001f);
        _lastBasePosition = basePosition;
        _initialized = true;

        // Modulate frequency by speed
        float frequency = Mathf.Clamp(baseFrequency + speed * frequencyPerSpeed, minFrequency, maxFrequency);

        // Accumulate phase so frequency changes are smooth (no phase jumps)
        _phase += frequency * Time.deltaTime;

        float offset = Mathf.Sin(_phase * Mathf.PI * 2f) * amplitude;
        _previousBobOffset = direction.normalized * offset;

        transform.position = basePosition + _previousBobOffset;
    }
}
