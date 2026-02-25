using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attached to friendly escort units alongside UnitAI. Sets the NavMesh destination to the follow target's position.
/// Can follow any GameObject (e.g. a CaravanCar, the CaravanCore, or any other transform). Only active when UnitAI is in Advance state; UnitAI calls Pause() for Engage/Retreat and Resume() when returning to Advance.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class CaravanFollower : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform to follow (any GameObject). Can be a CaravanCar, the CaravanCore, or anything else.")]
    public Transform followTarget;

    [Header("Throttle")]
    [Tooltip("Seconds between destination updates.")]
    public float destinationUpdateInterval = 0.25f;

    private NavMeshAgent _agent;
    private Health _health;
    private bool _active = true;
    private float _nextUpdateTime;
    private Vector3 _lastSlotWorldPosition;

    public bool IsActive => _active;
    /// <summary>Current formation goal position (for leash distance etc.).</summary>
    public Vector3 GetSlotWorldPosition() => followTarget != null ? followTarget.position : _lastSlotWorldPosition;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _health = GetComponent<Health>();

        if (_health != null)
            _health.OnDeath += OnDeath;

        _nextUpdateTime = Time.time;
        _active = true;
    }

    void OnDestroy()
    {
        if (_health != null)
            _health.OnDeath -= OnDeath;
    }

    void OnDeath(GameObject _) { }

    public void Pause()
    {
        _active = false;
    }

    public void Resume()
    {
        _active = true;
        _nextUpdateTime = 0f; // allow immediate destination update on next Update so unit returns to caravan right away
    }

    void Update()
    {
        if (!_active || followTarget == null || _agent == null || !_agent.isActiveAndEnabled)
            return;

        if (Time.time < _nextUpdateTime)
            return;

        _lastSlotWorldPosition = followTarget.position;
        Vector3 destination = _lastSlotWorldPosition;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
        else
            _agent.SetDestination(destination);

        _nextUpdateTime = Time.time + destinationUpdateInterval;
    }
}
