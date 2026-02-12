using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  public Camera cam;
  public UnityEngine.AI.NavMeshAgent agent;
  public Health health;

  // Update is called once per frame
  void Start()
  {
    agent.SetDestination(HomeBase.getInstance().transform.position);
  }
  void OnTriggerEnter(Collider collider)
  {
    Projectile projectile = collider.gameObject.GetComponent<Projectile>();
    if (projectile != null && collider.gameObject.tag == "Projectile")
    {
      health.TakeDamage(projectile.damage);
      Destroy(projectile.gameObject);
    }
  }
}
