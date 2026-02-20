using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

/// <summary>
/// Wraps a Unity SplineContainer and provides helper methods for the caravan system:
/// world-space position/direction at t, nearest t from a world position, and distance-to-t conversion.
/// </summary>
public class SplineRoute : MonoBehaviour
{
    [Tooltip("The spline path. Assign a GameObject with a SplineContainer component.")]
    public SplineContainer splineContainer;

    [Tooltip("Spline index when the container has multiple splines. Usually 0.")]
    public int splineIndex = 0;

    private float _cachedLength = -1f;
    private float _cachedLocalLength = -1f;

    void OnValidate()
    {
        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();
    }

    void Start()
    {
        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();
        CacheLengths();
    }

    private void CacheLengths()
    {
        if (splineContainer == null || splineContainer.Splines == null || splineIndex >= splineContainer.Splines.Count)
            return;
        _cachedLength = splineContainer.CalculateLength(splineIndex);
        _cachedLocalLength = splineContainer.Splines[splineIndex].GetLength();
    }

    /// <summary>World-space position at normalized t (0..1).</summary>
    public Vector3 GetPosition(float t)
    {
        if (splineContainer == null) return transform.position;
        t = Mathf.Clamp01(t);
        float3 p = splineContainer.EvaluatePosition(splineIndex, t);
        return new Vector3(p.x, p.y, p.z);
    }

    /// <summary>World-space forward tangent at t (normalized).</summary>
    public Vector3 GetDirection(float t)
    {
        if (splineContainer == null) return transform.forward;
        t = Mathf.Clamp01(t);
        float3 tan = splineContainer.EvaluateTangent(splineIndex, t);
        Vector3 d = new Vector3(tan.x, tan.y, tan.z);
        if (d.sqrMagnitude > 0.0001f)
            d.Normalize();
        return d;
    }

    /// <summary>Find the closest normalized t for a world position.</summary>
    public float GetNearestT(Vector3 worldPos)
    {
        if (splineContainer == null || splineContainer.Splines == null || splineIndex >= splineContainer.Splines.Count)
            return 0f;
        var spline = splineContainer.Splines[splineIndex];
        Vector3 localPos = splineContainer.transform.InverseTransformPoint(worldPos);
        float3 localFloat3 = new float3(localPos.x, localPos.y, localPos.z);
        SplineUtility.GetNearestPoint(spline, localFloat3, out float3 nearest, out float t);
        return Mathf.Clamp01(t);
    }

    /// <summary>Total length of the spline in world units.</summary>
    public float GetSplineLength()
    {
        if (splineContainer == null) return 0f;
        if (_cachedLength < 0f)
            CacheLengths();
        return _cachedLength > 0f ? _cachedLength : splineContainer.CalculateLength(splineIndex);
    }

    /// <summary>
    /// Convert a world-distance offset to a normalized t offset (for use as delta from current t).
    /// E.g. DistanceToT(-3) returns approximately -3/Length so that currentT + result moves ~3m backward.
    /// </summary>
    public float DistanceToT(float distance)
    {
        float length = GetSplineLength();
        if (length <= 0f) return 0f;
        return distance / length;
    }

    /// <summary>
    /// Get the normalized t that is at the given world-distance offset from the given current t.
    /// Used by CaravanFollower to compute target t for a slot (e.g. currentT + offset behind).
    /// </summary>
    public float GetTAtDistanceOffset(float currentT, float distanceOffsetInWorld)
    {
        if (splineContainer == null || splineContainer.Splines == null || splineIndex >= splineContainer.Splines.Count)
            return Mathf.Clamp01(currentT);
        currentT = Mathf.Clamp01(currentT);
        float length = GetSplineLength();
        if (length <= 0f) return currentT;
        float localLength = _cachedLocalLength > 0f ? _cachedLocalLength : splineContainer.Splines[splineIndex].GetLength();
        if (localLength <= 0f) return currentT;
        float localOffset = distanceOffsetInWorld * (localLength / length);
        var spline = splineContainer.Splines[splineIndex];
        spline.GetPointAtLinearDistance(currentT, localOffset, out float resultT);
        return Mathf.Clamp01(resultT);
    }
}
