using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  public float damage = 10f;
  public float speed = 15f;           // Units per second (frame-rate independent)
  public float maxLifetime = 5f;      // Auto-destroy after this many seconds

  [HideInInspector]
  public GameObject target;

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
      // Target is gone — destroy projectile
      Destroy(gameObject);
    }
  }
}
