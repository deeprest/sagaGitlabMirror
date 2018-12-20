using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public GameObject LookTarget;
  public Vector3 Offset;

  void Update()
  {
    if( LookTarget!=null )
      transform.position = LookTarget.transform.position + Offset;
  }
}
