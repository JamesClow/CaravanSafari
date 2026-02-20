using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Behavior switcher: decides which pattern is active (Advance, Engage, Retreat). Uses OffensiveTargeting when in offensive/engage mode.
/// Death tracking: WaveManager etc. can still subscribe to Health.OnDeath.
/// </summary>
public class UnitAI : MonoBehaviour
{
    public OffensiveTargeting offensiveTargeting;

    [Header("Team")]
    public Team.TeamType team = Team.TeamType.Enemy;

    [Header("Behavior Tuning")]
    [Tooltip("Distance to target at which we switch to Engage state.")]
    public float engageRadius = 10f;
    [Range(0f, 1f)]
    [Tooltip("Health % at which unit retreats. 0 = never retreat.")]
    public float retreatHealthThreshold = 0f;
    public float retreatDuration = 2f;
    [Tooltip("Max distance to chase a target before refocusing on advance objective.")]
    public float leashDistance = 20f;

    [Header("Throttle")]
    [Tooltip("Seconds between NavMesh repath.")]
    public float destinationUpdateInterval = 0.25f;

    [Header("Knockback")]
    [Tooltip("Impulse force applied away from the damage source when hit (physical push). 0 = no knockback.")]
    public float knockbackForce = 3f;
    [Tooltip("Time in seconds the agent stops steering so the knockback impulse is visible.")]
    public float knockbackDuration = 0.2f;

    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private Health _health;
    private CaravanFollower _caravanFollower;
    private float _knockbackEndTime = -1f;

    private enum State { Advance, Engage, Retreat }
    private State _state = State.Advance;
    private float _retreatEndTime;
    private float _nextDestinationUpdateTime;
    private Transform _advanceTarget;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _health = GetComponent<Health>();
        if (offensiveTargeting == null) offensiveTargeting = GetComponent<OffensiveTargeting>();

        if (_agent == null)
        {
            Debug.LogError($"UnitAI '{gameObject.name}' is missing a NavMeshAgent!");
            enabled = false;
            return;
        }

        _advanceTarget = GetAdvanceTargetTransform();
        _caravanFollower = GetComponent<CaravanFollower>();
        _nextDestinationUpdateTime = Time.time + destinationUpdateInterval;
        _state = State.Advance;
        if (_caravanFollower != null)
            _caravanFollower.Resume();

        if (_health != null)
        {
            _health.OnDeath += OnDeath;
            _health.OnDamaged += OnDamaged;
        }

        if (UnitAIManager.Instance != null)
            UnitAIManager.Instance.Register(this);
    }

    void OnDeath(GameObject _)
    {
        if (UnitAIManager.Instance != null)
            UnitAIManager.Instance.Unregister(this);
    }

    void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDeath -= OnDeath;
            _health.OnDamaged -= OnDamaged;
        }
        if (UnitAIManager.Instance != null)
            UnitAIManager.Instance.Unregister(this);
    }

    void OnDamaged(float amount, GameObject damageSource)
    {
        if (knockbackForce <= 0f || damageSource == null || _agent == null || !_agent.isActiveAndEnabled) return;
        if (_rb == null || _rb.isKinematic) return;

        Vector3 away = (transform.position - damageSource.transform.position);
        away.y = 0f; // keep knockback horizontal so it works with typical Rigidbody Y constraint
        float dist = away.magnitude;
        if (dist < 0.01f) away = -transform.forward;
        else away /= dist;

        _rb.AddForce(away * knockbackForce, ForceMode.Impulse);
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _knockbackEndTime = Time.time + knockbackDuration;
    }

    void Update()
    {
        if (_health != null && _health.IsDead()) return;

        // Re-enable agent steering after knockback impulse
        if (_knockbackEndTime >= 0f && Time.time >= _knockbackEndTime)
        {
            _knockbackEndTime = -1f;
            if (_agent != null && _agent.isActiveAndEnabled)
            {
                _agent.updatePosition = true;
                _agent.updateRotation = true;
                if (_rb != null && _agent.isOnNavMesh && NavMesh.SamplePosition(_rb.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                    _agent.Warp(hit.position);
            }
        }

        bool doRepath = UnitAIManager.Instance == null || UnitAIManager.Instance.ShouldUpdateThisFrame(this);

        switch (_state)
        {
            case State.Advance:
                UpdateAdvance(doRepath);
                break;
            case State.Engage:
                UpdateEngage(doRepath);
                break;
            case State.Retreat:
                UpdateRetreat();
                break;
        }
    }

    private Transform GetAdvanceTargetTransform()
    {
        if (offensiveTargeting != null)
        {
            Transform def = offensiveTargeting.GetDefaultTarget();
            if (def != null) return def;
        }
        HomeBase hb = HomeBase.getInstance();
        return hb != null ? hb.transform : null;
    }

    private void UpdateAdvance(bool doRepath)
    {
        if (offensiveTargeting != null && offensiveTargeting.target != null)
        {
            float dist = Vector3.Distance(transform.position, offensiveTargeting.target.transform.position);
            if (dist <= engageRadius)
            {
                if (_caravanFollower != null) _caravanFollower.Pause();
                _state = State.Engage;
                _nextDestinationUpdateTime = Time.time;
                return;
            }
        }

        if (_caravanFollower != null && _caravanFollower.IsActive)
            return;

        if (doRepath && _agent.isActiveAndEnabled)
        {
            Transform moveTarget = (offensiveTargeting != null && offensiveTargeting.target != null && offensiveTargeting.target.activeInHierarchy)
                ? offensiveTargeting.target.transform
                : null;
            if (moveTarget == null)
            {
                if (_advanceTarget == null) _advanceTarget = GetAdvanceTargetTransform();
                moveTarget = _advanceTarget;
            }
            if (moveTarget != null)
            {
                _agent.SetDestination(moveTarget.position);
            }
            _nextDestinationUpdateTime = Time.time + destinationUpdateInterval;
        }
    }

    private void UpdateEngage(bool doRepath)
    {
        if (offensiveTargeting == null || offensiveTargeting.target == null || !offensiveTargeting.target.activeInHierarchy)
        {
            if (offensiveTargeting != null) offensiveTargeting.target = null;
            if (_caravanFollower != null) _caravanFollower.Resume();
            _state = State.Advance;
            return;
        }

        if (_health != null && retreatHealthThreshold > 0f && _health.GetHealthPercent() <= retreatHealthThreshold)
        {
            if (_caravanFollower != null) _caravanFollower.Pause();
            _state = State.Retreat;
            _retreatEndTime = Time.time + retreatDuration;
            return;
        }

        if (_caravanFollower != null)
        {
            float distFromSlot = Vector3.Distance(transform.position, _caravanFollower.GetSlotWorldPosition());
            if (distFromSlot > leashDistance)
            {
                offensiveTargeting.target = null;
                _caravanFollower.Resume();
                _state = State.Advance;
                return;
            }
        }

        float distToTarget = Vector3.Distance(transform.position, offensiveTargeting.target.transform.position);
        if (distToTarget > leashDistance)
        {
            offensiveTargeting.target = null;
            if (_caravanFollower != null) _caravanFollower.Resume();
            _state = State.Advance;
            return;
        }

        if (doRepath && _agent.isActiveAndEnabled && offensiveTargeting.target != null)
        {
            _agent.SetDestination(offensiveTargeting.target.transform.position);
            _nextDestinationUpdateTime = Time.time + destinationUpdateInterval;
        }
    }

    private void UpdateRetreat()
    {
        if (Time.time >= _retreatEndTime)
        {
            if (_caravanFollower != null) _caravanFollower.Resume();
            _state = State.Advance;
            _advanceTarget = GetAdvanceTargetTransform();
            return;
        }

        if (_agent.isActiveAndEnabled && offensiveTargeting != null && offensiveTargeting.target != null && offensiveTargeting.target.activeInHierarchy)
        {
            Vector3 away = (transform.position - offensiveTargeting.target.transform.position).normalized;
            Vector3 retreatPos = transform.position + away * 5f;
            if (NavMesh.SamplePosition(retreatPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
            else
            {
                _state = State.Engage;
            }
        }
    }

    /// <summary>
    /// Override advance target (e.g. for wave scripting). Does not change FSM state.
    /// </summary>
    public void SetChaseTarget(Transform target)
    {
        _advanceTarget = target;
    }
}
