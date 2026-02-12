using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Targeting : MonoBehaviour
{
  public string targetTag;
  [HideInInspector]
  public List<GameObject> targetsInRange = new List<GameObject>();
  [HideInInspector]
  public GameObject target;

  // Update is called once per frame
  void Update()
  {
    // find target
    if (target == null || target.activeSelf == false)
    {
      if (targetsInRange.Count > 0)
      {
        for (int i = targetsInRange.Count - 1; i >= 0; i--)
        {
          if (targetsInRange[i] != null && targetsInRange[i].activeSelf == true)
          {
            target = targetsInRange[i];
            break;
          }
          else
          {
            targetsInRange.Remove(targetsInRange[i]);
          }
        }
      }
    }
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.tag == targetTag)
    {
      targetsInRange.Add(collider.gameObject);
    }
  }

  void OnTriggerExit(Collider collider)
  {
    if (collider.gameObject.tag == targetTag)
    {
      targetsInRange.Remove(collider.gameObject);
      if (collider.gameObject == target)
      {
        target = null;
      }
    }
  }
}
