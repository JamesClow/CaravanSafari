using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
  public GameObject projectilePrefab;
  public Targeting targeting;
  public float launchInterval = 3f;
  [Tooltip("Distance in front of the tower to spawn the projectile (avoids spawning inside tower collider).")]
  public float spawnOffset = 0.5f;
  private float nextLaunchTime = 0f;

  void Start()
  {
    // Auto-find sibling components if not assigned
    if (targeting == null)
      targeting = GetComponent<Targeting>();
    if (targeting == null)
      targeting = GetComponentInChildren<Targeting>();

    nextLaunchTime = Time.time + launchInterval;
  }

  void Update()
  {
    if (Time.time >= nextLaunchTime)
    {
      LaunchProjectile();
      nextLaunchTime = Time.time + launchInterval;
    }
  }

  void LaunchProjectile()
  {
    if (projectilePrefab != null && targeting != null && targeting.target != null)
    {
      // Spawn slightly in front of the tower toward the target so we don't spawn inside the tower collider
      Vector3 toTarget = targeting.target.transform.position - transform.position;
      float dist = toTarget.magnitude;
      Vector3 spawnPos = dist > 0.01f
        ? transform.position + (toTarget / dist) * Mathf.Min(spawnOffset, dist * 0.5f)
        : transform.position + Vector3.forward * spawnOffset;

      GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
      Projectile proj = projectile.GetComponent<Projectile>();
      if (proj != null)
      {
        proj.target = targeting.target;
      }
    }
  }
}
