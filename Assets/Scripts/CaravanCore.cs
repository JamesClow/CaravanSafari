using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The caravan's center of mass. Moves along a spline path at a controlled pace.
/// Keeps the HomeBase class name and singleton so UnitAI and OffensiveTargeting resolve it unchanged.
/// Tag this GameObject "HomeBase" so enemies advance toward it.
/// </summary>
public class HomeBase : SingletonMonoBehaviour<HomeBase>
{
    [Header("Spline")]
    [Tooltip("Route definition. Assign a GameObject with SplineRoute component.")]
    public SplineRoute splineRoute;

    [Header("Movement")]
    [Tooltip("World-space distance ahead on the spline to set as NavMesh destination (keeps the wagon moving).")]
    public float lookaheadDistance = 5f;

    [Header("State")]
    [Tooltip("When false, the caravan stops moving (e.g. under heavy attack or at a waypoint).")]
    public bool isMoving = true;

    [SerializeField]
    [Range(0f, 1f)]
    private float currentT = 0f;

    private NavMeshAgent _agent;
    private float _nextTUpdateTime;

    public float CurrentT => currentT;
    /// <summary>Current movement speed (from NavMeshAgent). Followers match this pace.</summary>
    public float Speed => _agent != null && _agent.isActiveAndEnabled ? _agent.speed : 0f;
    public SplineRoute Route => splineRoute;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent != null)
            _agent.updateRotation = true;
        if (splineRoute != null)
            currentT = splineRoute.GetNearestT(transform.position);
        _nextTUpdateTime = Time.time + 0.1f;
    }

    void Update()
    {
        if (splineRoute == null) return;
        if (_agent == null || !_agent.isActiveAndEnabled) return;

        float length = splineRoute.GetSplineLength();
        if (length <= 0f) return;

        if (isMoving)
        {
            float tOffset = splineRoute.DistanceToT(lookaheadDistance);
            float lookaheadT = Mathf.Clamp01(currentT + tOffset);
            Vector3 destination = splineRoute.GetPosition(lookaheadT);
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                _agent.SetDestination(hit.position);
            else
                _agent.SetDestination(destination);
        }

        if (Time.time >= _nextTUpdateTime)
        {
            currentT = splineRoute.GetNearestT(transform.position);
            _nextTUpdateTime = Time.time + 0.15f;
        }
    }

    public void PauseMovement() => isMoving = false;
    public void ResumeMovement() => isMoving = true;
}
