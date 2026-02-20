using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves toward target and damages the first valid hit. Requires a trigger Collider on this GameObject for hit detection.
/// </summary>
public class Projectile : MonoBehaviour
{
  public float damage = 10f;
  public float speed = 15f;           // Units per second (frame-rate independent)
  public float maxLifetime = 5f;      // Auto-destroy after this many seconds

  [HideInInspector]
  public GameObject target;

  [Tooltip("Source unit's tag for friendly-fire check. Set by Launcher when spawning.")]
  [HideInInspector]
  public string sourceTag;
  [HideInInspector]
  public GameObject source;

  private float spawnTime;

  void Start()
  {
    spawnTime = Time.time;
  }

  void Update()
  {
    // Auto-destroy if we've been alive too long (prevents orphaned projectiles)
    if (Time.time - spawnTime > maxLifetime)
    {
      Destroy(gameObject);
      return;
    }

    if (target != null && target.activeInHierarchy)
    {
      // Move toward target (frame-rate independent)
      transform.position = Vector3.MoveTowards(
          transform.position,
          target.transform.position,
          speed * Time.deltaTime
      );
    }
    else
    {
      Destroy(gameObject);
    }
  }

  /// <summary>
  /// True if this projectile should damage the hit object (same-team = no damage).
  /// </summary>
  public bool ShouldDamage(GameObject hitObject)
  {
    if (hitObject == null) return false;
    if (string.IsNullOrEmpty(sourceTag)) return true;
    return !Team.IsSameTeam(sourceTag, hitObject.tag);
  }

  void OnTriggerEnter(Collider other)
  {
    if (other == null || other.gameObject == null) return;
    GameObject hit = other.gameObject;
    if (!ShouldDamage(hit)) return;
    Health health = hit.GetComponent<Health>();
    if (health != null)
    {
      health.TakeDamage(damage, source);
      Destroy(gameObject);
    }
  }
}
