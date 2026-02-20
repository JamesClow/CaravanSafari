using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Timer-based auto-attack system. Uses a sibling Targeting component to find enemies,
/// then deals damage at a fixed rate. Works for enemies, friendly units, and towers.
///
/// For melee: set attackRange and leave launcher null.
/// For ranged: assign a Launcher component and it will fire projectiles instead.
/// </summary>
public class Attack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 5f;
    public float attacksPerSecond = 1f;
    public float attackRange = 2f;          // Max distance to deal melee damage

    [Header("Components")]
    public OffensiveTargeting offensiveTargeting;

    private float attackCooldown = 0f;

    void Start()
    {
        if (offensiveTargeting == null)
            offensiveTargeting = GetComponent<OffensiveTargeting>();
        if (offensiveTargeting == null)
            offensiveTargeting = GetComponentInChildren<OffensiveTargeting>();
    }

    void Update()
    {
        if (offensiveTargeting == null) return;

        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0f && offensiveTargeting.target != null)
        {
            GameObject currentTarget = offensiveTargeting.target;

            // Check if target is within attack range
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (dist <= attackRange)
            {
                PerformAttack(currentTarget);
                attackCooldown = 1f / Mathf.Max(attacksPerSecond, 0.01f);
            }
        }
    }

    private void PerformAttack(GameObject target)
    {
        if (Team.IsSameTeam(gameObject, target)) return;

        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage, gameObject);
        }
    }
}
