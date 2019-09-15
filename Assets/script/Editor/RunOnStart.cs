using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunOnStart : MonoBehaviour
{
  [RuntimeInitializeOnLoadMethod]
  static void OnStart()
  {
    Global global = FindObjectOfType<Global>();
    if( global == null )
    {
      GameObject go = Instantiate( Resources.Load<GameObject>( "GLOBAL" ) );
    }
  }
}
