using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public GameObject LookTarget;
  public float zOffset;
  public float yHalfWidth = 2;
  public Vector3 pos;
  public LerpToTarget lerp;

  public void LerpTo( GameObject go )
  {
    LookTarget = go;
    lerp.targetTransform = go.transform;
    lerp.duration = 3;
    lerp.Continuous = false;
    lerp.enabled = true;
  }

  public void LockOn( GameObject go )
  {
    lerp.targetTransform = go.transform;
    lerp.duration = 1;
    lerp.Continuous = true;
    lerp.enabled = true;
    lerp.OnLerpEnd = delegate
    {
      //LookTarget = go;
    };
  }

  void Update()
  {
    if( !lerp.enabled && LookTarget != null )
    {
      if( LookTarget.transform.position.y > transform.position.y + yHalfWidth )
        pos.y = LookTarget.transform.position.y - yHalfWidth;
      if( LookTarget.transform.position.y < transform.position.y - yHalfWidth )
        pos.y = LookTarget.transform.position.y + yHalfWidth;
      pos.x = LookTarget.transform.position.x;
      pos.z = zOffset;
      transform.position = pos;
    }
  }
}
