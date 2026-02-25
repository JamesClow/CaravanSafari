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

  [Tooltip("Peak arc height as a fraction of distance to target (0 = no arc). e.g. 0.2 = 20% of distance.")]
  [Range(0f, 1f)]
  public float arcHeightFactor = 0f;

  [HideInInspector]
  public GameObject target;

  [Tooltip("Source unit's tag for friendly-fire check. Set by Launcher when spawning.")]
  [HideInInspector]
  public string sourceTag;
  [HideInInspector]
  public GameObject source;

  private float spawnTime;
  private Vector3 origin;
  private float totalDistance;
  private float progress;

  void Start()
  {
    spawnTime = Time.time;
    origin = transform.position;
    if (target != null && target.activeInHierarchy)
      totalDistance = Vector3.Distance(origin, target.transform.position);
    else
      totalDistance = 1f;
    progress = 0f;
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
      Vector3 targetPos = target.transform.position;

      if (arcHeightFactor > 0f && totalDistance > 0.001f)
      {
        // Move along straight line from origin to target so we land exactly at target height
        float distToTarget = Vector3.Distance(origin, targetPos);
        if (distToTarget > 0.001f)
          progress += (speed * Time.deltaTime) / distToTarget;

        if (progress >= 1f)
        {
          transform.position = targetPos;
        }
        else
        {
          float t = Mathf.Clamp01(progress);
          Vector3 linearPos = Vector3.Lerp(origin, targetPos, t);
          float arcOffset = (arcHeightFactor * totalDistance) * 4f * t * (1f - t);
          transform.position = linearPos + Vector3.up * arcOffset;
        }
      }
      else
      {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
      }
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
