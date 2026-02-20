using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attached to friendly escort units alongside UnitAI. Sets the NavMesh destination to the unit's formation slot (goal);
/// the unit's own NavMeshAgent speed is used. Only active when UnitAI is in Advance state; UnitAI calls Pause() for Engage/Retreat and Resume() when returning to Advance.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class CaravanFollower : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Formation manager that assigns slots. Auto-found if null.")]
    public FormationManager formationManager;
    [Tooltip("Caravan core (HomeBase). Auto-found if null.")]
    public HomeBase caravanCore;

    [Header("Throttle")]
    [Tooltip("Seconds between destination updates.")]
    public float destinationUpdateInterval = 0.25f;

    private NavMeshAgent _agent;
    private Health _health;
    private FormationManager.Slot _slot;
    private bool _hasSlot;
    private bool _active = true;
    private float _nextUpdateTime;
    private Vector3 _lastSlotWorldPosition;

    public bool IsActive => _active;
    public Vector3 GetSlotWorldPosition() => _lastSlotWorldPosition;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _health = GetComponent<Health>();

        if (formationManager == null)
            formationManager = FindAnyObjectByType<FormationManager>();
        if (caravanCore == null)
            caravanCore = HomeBase.getInstance();

        if (formationManager != null && caravanCore != null && caravanCore.Route != null)
        {
            _slot = formationManager.Register(this);
            _hasSlot = true;
        }
        else
            _hasSlot = false;

        if (_health != null)
            _health.OnDeath += OnDeath;

        _nextUpdateTime = Time.time;
        _active = true;
    }

    void OnDestroy()
    {
        if (_health != null)
            _health.OnDeath -= OnDeath;
        if (formationManager != null)
            formationManager.Unregister(this);
    }

    void OnDeath(GameObject _)
    {
        if (formationManager != null)
            formationManager.Unregister(this);
        _hasSlot = false;
    }

    public void Pause()
    {
        _active = false;
    }

    public void Resume()
    {
        _active = true;
        _nextUpdateTime = Time.time;
    }

    void Update()
    {
        if (!_active || !_hasSlot || caravanCore == null || caravanCore.Route == null || _agent == null || !_agent.isActiveAndEnabled)
            return;

        if (Time.time < _nextUpdateTime)
            return;

        SplineRoute route = caravanCore.Route;
        float currentT = caravanCore.CurrentT;
        float targetT = route.GetTAtDistanceOffset(currentT, _slot.distanceOffset);
        Vector3 slotCenter = route.GetPosition(targetT);
        Vector3 direction = route.GetDirection(targetT);
        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
        _lastSlotWorldPosition = slotCenter + right * _slot.lateralOffset;

        if (NavMesh.SamplePosition(_lastSlotWorldPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
        else
            _agent.SetDestination(_lastSlotWorldPosition);

        _nextUpdateTime = Time.time + destinationUpdateInterval;
    }
}
