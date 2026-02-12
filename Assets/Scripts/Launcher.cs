using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Launcher : MonoBehaviour
{
  public GameObject projectilePrefab;
  public Targeting targeting;
  public float launchInterval = 3f; // Time between launches
  private float nextLaunchTime = 0;

  void Start()
  {
    nextLaunchTime = Time.time + launchInterval; // Set the initial launch time
  }

  void Update()
  {
    // Check if it's time to launch another projectile
    if (Time.time >= nextLaunchTime)
    {
      launchProjectile();
      nextLaunchTime = Time.time + launchInterval; // Reset the launch time
    }
  }

  void launchProjectile()
  {
    if (projectilePrefab != null && targeting.target != null)
    {
      // Instantiate the projectile at this object's position and rotation
      GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
      projectile.GetComponent<Projectile>().target = targeting.target;
    }
  }
}
