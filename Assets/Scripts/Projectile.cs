using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  public float damage = 10f;
  public float speed = 0.1f;
  [HideInInspector]
  public GameObject target;
  void Update()
  {
    // move towards the target if you hit the target destroy the projectile 
    if (target != null)
    {
      transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
