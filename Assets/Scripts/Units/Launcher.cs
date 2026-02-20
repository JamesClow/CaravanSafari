using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
  public GameObject projectilePrefab;
  public OffensiveTargeting offensiveTargeting;

  [Tooltip("Interval between launches (seconds).")]
  public float launchInterval = 3f;
  [Tooltip("Distance in front of the tower to spawn the projectile (avoids spawning inside tower collider).")]
  public float spawnOffset = 0.5f;
  private float nextLaunchTime = 0f;

  void Start()
  {
    if (offensiveTargeting == null)
      offensiveTargeting = GetComponent<OffensiveTargeting>();
    if (offensiveTargeting == null)
      offensiveTargeting = GetComponentInChildren<OffensiveTargeting>();

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
    if (projectilePrefab != null && offensiveTargeting != null && offensiveTargeting.target != null)
    {
      Vector3 toTarget = offensiveTargeting.target.transform.position - transform.position;
      float dist = toTarget.magnitude;
      Vector3 spawnPos = dist > 0.01f
        ? transform.position + (toTarget / dist) * Mathf.Min(spawnOffset, dist * 0.5f)
        : transform.position + Vector3.forward * spawnOffset;

      GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
      Projectile proj = projectile.GetComponent<Projectile>();
      if (proj != null)
      {
        proj.target = offensiveTargeting.target;
        proj.sourceTag = gameObject.tag;
        proj.source = gameObject;
      }
    }
  }
}
