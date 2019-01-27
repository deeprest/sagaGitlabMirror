using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  [SerializeField] Camera cam;
  public GameObject LookTarget;

  public bool UseVerticalRange = true;
  public bool CursorInfluence = false;
  [SerializeField] RectTransform cursor;
  public float cursorAlpha = 0;
  public float lerpAlpha = 50;
  Vector3 WithinRectPos;
  public float zOffset;
  public float yHalfWidth = 2;
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

  public void CameraLateUpdate()
  {
    if( !lerp.enabled && LookTarget != null )
    {
      if( UseVerticalRange )
      {
        if( LookTarget.transform.position.y > WithinRectPos.y + yHalfWidth )
          WithinRectPos.y = LookTarget.transform.position.y - yHalfWidth;
        if( LookTarget.transform.position.y < WithinRectPos.y - yHalfWidth )
          WithinRectPos.y = LookTarget.transform.position.y + yHalfWidth;
      }
      else
        WithinRectPos.y = LookTarget.transform.position.y;

      WithinRectPos.x = LookTarget.transform.position.x;

      Vector3 final = WithinRectPos;
      if( CursorInfluence )
        final = Vector3.Lerp( WithinRectPos, cam.ScreenToWorldPoint( cursor.anchoredPosition ), cursorAlpha );

      final.z = zOffset;

      if( lerpAlpha > 0 )
        transform.localPosition = Vector3.Lerp( transform.position, final, Mathf.Clamp01( lerpAlpha * Time.deltaTime) );
        //transform.position = Vector3.MoveTowards( transform.position, final, SmoothSpeed * Time.deltaTime );
      else
        transform.position = final;
    }
  }

}
