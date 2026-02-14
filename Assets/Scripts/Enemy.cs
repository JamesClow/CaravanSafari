using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy AI controller. Continuously chases a target (defaults to HomeBase),
/// attacks when in range, and takes damage from projectiles.
///
/// Required components on the Enemy prefab:
/// - NavMeshAgent (for pathfinding)
/// - Health (for taking damage and dying)
/// - Collider set to IsTrigger (for projectile hits)
/// - Tag set to "Enemy"
///
/// Death tracking is handled by WaveManager subscribing to Health.OnDeath directly.
/// </summary>
public class Enemy : MonoBehaviour
{
  [Header("AI Settings")]
  public float destinationUpdateInterval = 0.25f; // How often to re-path (seconds)
  public float attackRange = 2f;                   // Distance at which enemy attacks
  public float damage = 5f;                        // Damage dealt per attack
  public float attackCooldown = 1f;                // Seconds between attacks

  // Components
  private NavMeshAgent agent;
  private Health health;
  private float nextDestinationUpdate;
  private float nextAttackTime;
  private Transform chaseTarget;

  void Start()
  {
    agent = GetComponent<NavMeshAgent>();
    health = GetComponent<Health>();

    if (agent == null)
    {
      Debug.LogError($"Enemy '{gameObject.name}' is missing a NavMeshAgent component!");
      enabled = false;
      return;
    }

    // Default chase target: HomeBase
    HomeBase homeBase = HomeBase.getInstance();
    if (homeBase != null)
    {
      chaseTarget = homeBase.transform;
    }

    // Set initial destination
    UpdateDestination();
    nextDestinationUpdate = Time.time + destinationUpdateInterval;
    nextAttackTime = Time.time + attackCooldown;
  }

  void Update()
  {
    // Continuously update pathfinding destination (for moving base)
    if (Time.time >= nextDestinationUpdate)
    {
      UpdateDestination();
      nextDestinationUpdate = Time.time + destinationUpdateInterval;
    }

    // Attack when close enough
    if (chaseTarget != null && Time.time >= nextAttackTime)
    {
      float dist = Vector3.Distance(transform.position, chaseTarget.position);
      if (dist <= attackRange)
      {
        AttackTarget();
        nextAttackTime = Time.time + attackCooldown;
      }
    }
  }

  private void UpdateDestination()
  {
    if (agent != null && agent.isActiveAndEnabled && chaseTarget != null)
    {
      agent.SetDestination(chaseTarget.position);
    }
  }

  private void AttackTarget()
  {
    if (chaseTarget == null) return;

    Health targetHealth = chaseTarget.GetComponent<Health>();
    if (targetHealth != null)
    {
      targetHealth.TakeDamage(damage);
    }
  }

  /// <summary>
  /// Handle being hit by a projectile.
  /// </summary>
  void OnTriggerEnter(Collider collider)
  {
    if (collider.CompareTag("Projectile"))
    {
      Projectile projectile = collider.GetComponent<Projectile>();
      if (projectile != null && health != null)
      {
        health.TakeDamage(projectile.damage);
        Destroy(projectile.gameObject);
      }
    }
  }

  /// <summary>
  /// Set a custom chase target (overrides HomeBase).
  /// </summary>
  public void SetChaseTarget(Transform target)
  {
    chaseTarget = target;
  }
}
