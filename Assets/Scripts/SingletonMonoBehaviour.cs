using UnityEngine;

abstract public class SingletonMonoBehaviour<Type> : MonoBehaviour where Type : MonoBehaviour
{
  protected static Type gInstance = null;
  static public Type getInstance()
  {
    if (gInstance == null)
    {
      gInstance = FindObjectOfType<Type>();
    }
    return gInstance;
  }


  public static Type Instance
  {
    get
    {
      if (gInstance == null)
      {
        gInstance = FindObjectOfType<Type>();
      }
      return gInstance;
    }
  }


}