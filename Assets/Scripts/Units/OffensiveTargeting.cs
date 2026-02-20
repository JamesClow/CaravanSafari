using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Offensive targeting: picks who to attack. Team-aware, configurable tag priorities, aggro, and default target.
/// Use for units, towers, anything that needs to pick a target. Behavior switcher (e.g. UnitAI) reads .target when in offensive mode.
/// </summary>
public class OffensiveTargeting : MonoBehaviour
{
    [System.Serializable]
    public struct TagPriority
    {
        public string tag;
        [Range(0f, 1f)]
        public float priority;
    }

    [Header("Tag priorities (Unity tags)")]
    [Tooltip("Targets must have a tag in this list with priority > 0. Same tags as GameObject.tag.")]
    public TagPriority[] tagPriorities = new TagPriority[] { new TagPriority { tag = "Enemy", priority = 0.8f } };

    [Header("Default target")]
    [Tooltip("When no one in range (or as fallback), use this. Leave empty for none.")]
    public Transform defaultTarget;
    [Tooltip("Or resolve default by Unity tag (e.g. HomeBase). Ignored if defaultTarget is set.")]
    public string defaultTargetTag = "";

    [Header("Scoring & Detection")]
    [Tooltip("Detection radius: targets beyond this distance are ignored. Distance-based only; no colliders required.")]
    public float detectionRadius = 10f;
    [Tooltip("Bonus multiplier for targets that recently damaged this entity.")]
    public float aggroMultiplier = 1.5f;
    [Tooltip("Evaluation interval in seconds (0 = every frame).")]
    public float evaluationInterval = 0.3f;

    [Tooltip("Filled each evaluation with hostile targets within detection radius (read-only).")]
    public List<GameObject> targetsInRange = new List<GameObject>();
    public GameObject target;

    private readonly Dictionary<GameObject, float> _aggroTable = new Dictionary<GameObject, float>();
    private const float AggroDecaySeconds = 5f;
    private float _nextEvaluationTime;
    private Transform _resolvedDefault;
    private readonly List<GameObject> _candidates = new List<GameObject>();

    /// <summary>
    /// Call when this entity is damaged so targeting can boost priority for that source.
    /// </summary>
    public void NotifyDamagedBy(GameObject source)
    {
        if (source != null)
            _aggroTable[source] = Time.time;
    }

    /// <summary>
    /// Returns the default target transform (resolved from defaultTarget or defaultTargetTag). Can be used by behavior switcher for advance movement.
    /// </summary>
    public Transform GetDefaultTarget()
    {
        if (defaultTarget != null) return defaultTarget;
        if (_resolvedDefault != null) return _resolvedDefault;
        if (!string.IsNullOrEmpty(defaultTargetTag))
        {
            if (defaultTargetTag == "HomeBase")
            {
                HomeBase hb = HomeBase.getInstance();
                _resolvedDefault = hb != null ? hb.transform : null;
            }
            else
            {
                GameObject go = GameObject.FindGameObjectWithTag(defaultTargetTag);
                _resolvedDefault = go != null ? go.transform : null;
            }
        }
        return _resolvedDefault;
    }

    void Start()
    {
        _nextEvaluationTime = Time.time + evaluationInterval;
        GetDefaultTarget();
    }

    void Update()
    {
        if (target == null || !target.activeInHierarchy)
            target = null;

        float interval = Mathf.Max(0f, evaluationInterval);
        bool shouldEval = interval <= 0f || Time.time >= _nextEvaluationTime;
        if (shouldEval)
        {
            Evaluate();
            if (interval > 0f)
                _nextEvaluationTime = Time.time + interval;
        }
    }

    private float GetPriorityForTag(string tag)
    {
        if (tagPriorities == null) return 0f;
        for (int i = 0; i < tagPriorities.Length; i++)
        {
            if (tagPriorities[i].tag == tag)
                return tagPriorities[i].priority;
        }
        return 0f;
    }

    private bool IsValidCandidate(GameObject go)
    {
        if (go == null || !go.activeInHierarchy) return false;
        float p = GetPriorityForTag(go.tag);
        if (p <= 0f) return false;
        if (!Team.AreHostile(gameObject.tag, go.tag)) return false;
        return true;
    }

    private float GetAggroBonus(GameObject candidate)
    {
        if (candidate == null) return 1f;
        if (_aggroTable.TryGetValue(candidate, out float t))
        {
            if (Time.time - t < AggroDecaySeconds)
                return aggroMultiplier;
            _aggroTable.Remove(candidate);
        }
        return 1f;
    }

    private void Evaluate()
    {
        _candidates.Clear();
        targetsInRange.Clear();

        Team.TeamType myTeam = Team.GetTeamFromTag(gameObject.tag);
        Team.TeamType hostileTeam = Team.GetHostileTeam(myTeam);
        List<GameObject> hostileObjects = Team.GetGameObjectsForTeam(hostileTeam);

        Vector3 myPos = transform.position;
        float radius = Mathf.Max(detectionRadius, 0.1f);

        foreach (GameObject go in hostileObjects)
        {
            if (go == null || !go.activeInHierarchy) continue;
            float dist = Vector3.Distance(myPos, go.transform.position);
            if (dist > radius) continue;
            if (IsValidCandidate(go))
            {
                _candidates.Add(go);
                targetsInRange.Add(go);
            }
        }

        Transform def = GetDefaultTarget();
        if (def != null && def.gameObject.activeInHierarchy)
        {
            GameObject defGo = def.gameObject;
            if (!_candidates.Contains(defGo))
            {
                float p = GetPriorityForTag(defGo.tag);
                if (p > 0f && Team.AreHostile(gameObject.tag, defGo.tag))
                {
                    _candidates.Add(defGo);
                    if (!targetsInRange.Contains(defGo))
                        targetsInRange.Add(defGo);
                }
            }
        }

        float bestScore = 0f;
        GameObject best = null;

        foreach (GameObject candidate in _candidates)
        {
            float dist = Vector3.Distance(myPos, candidate.transform.position);
            float normalizedDist = Mathf.Max(dist / radius, 0.01f);
            float priorityWeight = GetPriorityForTag(candidate.tag);
            if (priorityWeight <= 0f) continue;

            float distanceFactor = 1f / normalizedDist;
            float aggroBonus = GetAggroBonus(candidate);
            float healthBonus = 1f;
            Health h = candidate.GetComponent<Health>();
            if (h != null && h.healthMax > 0f)
            {
                float pct = h.GetHealthPercent();
                healthBonus = 1f + (1f - Mathf.Clamp01(pct)) * 0.5f;
            }

            float score = priorityWeight * distanceFactor * aggroBonus * healthBonus;
            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        target = best;
    }
}
