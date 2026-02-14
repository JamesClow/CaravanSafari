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
    public Targeting targeting;             // Assign in inspector or auto-found
    public Launcher launcher;               // Optional: if set, uses ranged attack via projectiles

    private float attackCooldown = 0f;

    void Start()
    {
        // Auto-find sibling components if not assigned
        if (targeting == null)
            targeting = GetComponent<Targeting>();
        if (targeting == null)
            targeting = GetComponentInChildren<Targeting>();

        if (launcher == null)
            launcher = GetComponent<Launcher>();
    }

    void Update()
    {
        if (targeting == null) return;

        // Tick down cooldown
        attackCooldown -= Time.deltaTime;

        // Try to attack
        if (attackCooldown <= 0f && targeting.target != null)
        {
            GameObject currentTarget = targeting.target;

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
        if (launcher != null)
        {
            // Ranged attack — Launcher handles projectile spawning on its own timer.
            // We don't double-fire here; the Launcher fires independently.
            // This path exists so we can still do melee OR ranged via the same component.
            return;
        }

        // Melee attack — deal direct damage
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
    }
}
