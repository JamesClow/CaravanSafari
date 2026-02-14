using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
  public string targetTag = "Enemy";

  [HideInInspector]
  public List<GameObject> targetsInRange = new List<GameObject>();
  [HideInInspector]
  public GameObject target;

  void Update()
  {
    // Clean up destroyed targets and find a new target if needed
    if (target == null || !target.activeInHierarchy)
    {
      target = null;
      FindNewTarget();
    }
  }

  private void FindNewTarget()
  {
    // Clean null/destroyed entries and pick the first valid one
    for (int i = targetsInRange.Count - 1; i >= 0; i--)
    {
      if (targetsInRange[i] == null || !targetsInRange[i].activeInHierarchy)
      {
        targetsInRange.RemoveAt(i);
      }
    }

    if (targetsInRange.Count > 0)
    {
      // Pick the closest target
      float closestDist = float.MaxValue;
      GameObject closest = null;

      foreach (GameObject candidate in targetsInRange)
      {
        float dist = Vector3.Distance(transform.position, candidate.transform.position);
        if (dist < closestDist)
        {
          closestDist = dist;
          closest = candidate;
        }
      }

      target = closest;
    }
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.CompareTag(targetTag))
    {
      if (!targetsInRange.Contains(collider.gameObject))
      {
        targetsInRange.Add(collider.gameObject);
        // Immediately try to select a target if we don't have one
        if (target == null)
        {
          FindNewTarget();
        }
      }
    }
  }

  void OnTriggerExit(Collider collider)
  {
    if (collider.gameObject.CompareTag(targetTag))
    {
      targetsInRange.Remove(collider.gameObject);
      if (collider.gameObject == target)
      {
        target = null;
      }
    }
  }
}
