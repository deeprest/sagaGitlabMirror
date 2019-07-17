using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  [SerializeField] Camera cam;
  public GameObject LookTarget;
  public LerpToTarget lerp;
  public bool UseVerticalRange = true;
  public bool CursorInfluence = false;
  public float cursorAlpha = 0.5f;
  public float lerpAlpha = 50;
  public float SmoothSpeed = 1;
  public float zOffset;
  public float yHalfWidth = 2;

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

  Vector3 pos;

  public void CameraLateUpdate()
  {
    if( !lerp.enabled && LookTarget != null )
    {
      pos.x = LookTarget.transform.position.x;
      pos.z = zOffset;

      if( UseVerticalRange )
      {
        if( LookTarget.transform.position.y > pos.y + yHalfWidth )
          pos.y = LookTarget.transform.position.y - yHalfWidth;
        if( LookTarget.transform.position.y < pos.y - yHalfWidth )
          pos.y = LookTarget.transform.position.y + yHalfWidth;
      }
      else
      {
        pos.y = LookTarget.transform.position.y;
      }

      if( CursorInfluence )
      {
        /*Vector3 stwp = Global.instance.cursor.anchoredPosition;
        stwp.z = -cam.transform.position.z;
        pos = Vector3.Lerp( pos, cam.ScreenToWorldPoint( stwp ), cursorAlpha );*/
        pos = Vector3.Lerp( pos, Global.instance.CursorWorldPos, cursorAlpha );
      }

      // camera polygon bounds
      // todo Decide whether to adjust the camera zOffset to fit into polygon 
      // bounds, or allow the offset to change freely and ignore one of the 
      // clip points (current), which allows the camera to see outside the polygon.

      // todo fix the jump-up-from-bottom snappy camera movement
      if( Global.instance.CameraBounds != null )
      {
        // draw gray lines showing camera rectangle on z=0
        float xangle = (Mathf.Deg2Rad * Camera.main.fieldOfView * 0.5f) * ((float)Camera.main.pixelWidth / (float)Camera.main.pixelHeight);
        float hh = Mathf.Tan( Mathf.Deg2Rad * Camera.main.fieldOfView * 0.5f ) * -transform.position.z;
        float hw = Mathf.Tan( xangle ) * -transform.position.z;

        Vector3 cp3 = transform.position;
        Debug.DrawLine( new Vector3( -hw, -hh, 0 ) + cp3, new Vector3( -hw, hh, 0 ) + cp3, Color.blue );
        Debug.DrawLine( new Vector3( -hw, hh, 0 ) + cp3, new Vector3( hw, hh, 0 ) + cp3, Color.blue );
        Debug.DrawLine( new Vector3( hw, hh, 0 ) + cp3, new Vector3( hw, -hh, 0 ) + cp3, Color.blue );
        Debug.DrawLine( new Vector3( hw, -hh, 0 ) + cp3, new Vector3( -hw, -hh, 0 ) + cp3, Color.blue );

        Vector2 UL = (Vector2)pos + Vector2.left * hw + Vector2.up * hh;
        if( ClipToInsidePolygon2D( Global.instance.CameraBounds, ref UL ) )
        {
          if( pos.y > UL.y - hh ) pos.y = UL.y - hh;
          if( pos.x < UL.x + hw ) pos.x = UL.x + hw;
        }

        Vector2 UR = (Vector2)pos + Vector2.right * hw + Vector2.up * hh;
        if( ClipToInsidePolygon2D( Global.instance.CameraBounds, ref UR ) )
        {
          if( pos.y > UR.y - hh ) pos.y = UR.y - hh;
          if( pos.x > UR.x - hw ) pos.x = UR.x - hw;
        }

        Vector2 LL = (Vector2)pos + Vector2.left * hw + Vector2.down * hh;
        if( ClipToInsidePolygon2D( Global.instance.CameraBounds, ref LL ) )
        {
          if( pos.y < LL.y + hh ) pos.y = LL.y + hh;
          if( pos.x < LL.x + hw ) pos.x = LL.x + hw;
        }

        Vector2 LR = (Vector2)pos + Vector2.right * hw + Vector2.down * hh;
        if( ClipToInsidePolygon2D( Global.instance.CameraBounds, ref LR ) )
        {
          if( pos.y < LR.y + hh ) pos.y = LR.y + hh;
          if( pos.x > LR.x - hw ) pos.x = LR.x - hw;
        }

        cp3 = pos;
        Debug.DrawLine( new Vector3( -hw, -hh, 0 ) + cp3, new Vector3( -hw, hh, 0 ) + cp3, Color.gray );
        Debug.DrawLine( new Vector3( -hw, hh, 0 ) + cp3, new Vector3( hw, hh, 0 ) + cp3, Color.gray );
        Debug.DrawLine( new Vector3( hw, hh, 0 ) + cp3, new Vector3( hw, -hh, 0 ) + cp3, Color.gray );
        Debug.DrawLine( new Vector3( hw, -hh, 0 ) + cp3, new Vector3( -hw, -hh, 0 ) + cp3, Color.gray );
      }
    }

    pos.z = zOffset;

    if( lerpAlpha > 0 )
      transform.position = Vector3.Lerp( transform.position, pos, Mathf.Clamp01( lerpAlpha * Time.deltaTime ) );
    //transform.position = Vector3.MoveTowards( transform.position, pos, SmoothSpeed * Time.deltaTime );
    else
      transform.position = pos;

  }


  bool ClipToInsidePolygon2D( PolygonCollider2D poly, ref Vector2 cp )
  {
    if( !poly.OverlapPoint( cp ) )
    {
      List<Vector2> pots = new List<Vector2>();
      Vector2[] points = poly.points;
      // add transform position for world space
      for( int i = 0; i < points.Length; i++ )
        points[i] += (Vector2)poly.transform.position;
      for( int i = 0; i < points.Length; i++ )
      {
        int next = (i + 1) % points.Length;
        Vector2 segment = points[next] - points[i];
        Vector2 perp = new Vector2( -segment.y, segment.x );
        Debug.DrawLine( points[i], points[i] + perp, Color.blue );
        Vector2 projectionPerp = Util.Project2D( (cp - points[i]), perp.normalized );
        if( Vector2.Dot( perp.normalized, projectionPerp.normalized ) < 0 )
        {
          Debug.DrawLine( points[i], points[i] + projectionPerp, Color.red );
          Vector2 adjust = cp - projectionPerp;
          if( Vector2.Dot( segment.normalized, (adjust - points[i]).normalized ) > 0 )
          {
            if( (adjust - points[i]).magnitude > segment.magnitude )
              adjust = points[next];
          }
          else
            adjust = points[i];
          pots.Add( adjust );
        }
        else
          Debug.DrawLine( points[i], points[i] + projectionPerp, Color.magenta );
      }

      float distance = Mathf.Infinity;
      Vector2 closest = cp;
      foreach( var p in pots )
      {
        float dist = Vector2.Distance( p, cp );
        if( dist < distance )
        {
          closest = p;
          distance = dist;
        }
      }
      cp = closest;
      return true;
    }
    return false;
  }

}