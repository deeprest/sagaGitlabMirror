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
  // lerp target position
  Vector3 pos;
  public bool EncompassBounds = false;

  public float orthoTarget = 1;
  public float orthoSpeed = 1;

  public bool snapToPixel = true;
  const float pixelDensity = 64;
  public float snapDivide = 1;
  public float pixelSnap;
  public float presnap;
  public float snapped;


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
    float ortho = orthoTarget;
    pos.z = zOffset;

    if( !lerp.enabled && LookTarget != null )
    {
      Vector3 lookTarget = LookTarget.transform.position;

      if( CursorInfluence )
      {
        lookTarget = Vector3.Lerp( LookTarget.transform.position, Global.instance.CursorWorldPosition, cursorAlpha );
        lookTarget.z = zOffset;
      }
      pos.x = lookTarget.x;
      //pos.z = zOffset;

      if( UseVerticalRange )
      {
        if( lookTarget.y > pos.y + yHalfWidth )
          pos.y = lookTarget.y - yHalfWidth;
        if( lookTarget.y < pos.y - yHalfWidth )
          pos.y = lookTarget.y + yHalfWidth;
      }
      else
      {
        pos.y = lookTarget.y;
      }

      if( Global.instance.CameraPoly != null )
      {
        float hh, hw, xangle = 0, yangle = 0;
        if( Camera.main.orthographic )
        {
          hh = Camera.main.orthographicSize;
          hw = Camera.main.orthographicSize * Camera.main.aspect;
        }
        else
        {
          // draw gray lines showing camera rectangle on z=0
          yangle = Mathf.Deg2Rad * Camera.main.fieldOfView * 0.5f;
          hh = Mathf.Tan( yangle ) * -transform.position.z;
          xangle = yangle * ((float)Camera.main.pixelWidth / (float)Camera.main.pixelHeight);
          hw = Mathf.Tan( xangle ) * -transform.position.z;
        }

        if( Global.instance.CameraPoly is PolygonCollider2D )
        {
          PolygonCollider2D poly = Global.instance.CameraPoly as PolygonCollider2D;
          Vector2 UL = (Vector2)pos + Vector2.left * hw + Vector2.up * hh;
          if( ClipToInsidePolygon2D( poly, ref UL ) )
          {
            if( pos.y > UL.y - hh ) pos.y = UL.y - hh;
            if( pos.x < UL.x + hw ) pos.x = UL.x + hw;
          }

          Vector2 UR = (Vector2)pos + Vector2.right * hw + Vector2.up * hh;
          if( ClipToInsidePolygon2D( poly, ref UR ) )
          {
            if( pos.y > UR.y - hh ) pos.y = UR.y - hh;
            if( pos.x > UR.x - hw ) pos.x = UR.x - hw;
          }

          Vector2 LL = (Vector2)pos + Vector2.left * hw + Vector2.down * hh;
          if( ClipToInsidePolygon2D( poly, ref LL ) )
          {
            if( pos.y < LL.y + hh ) pos.y = LL.y + hh;
            if( pos.x < LL.x + hw ) pos.x = LL.x + hw;
          }

          Vector2 LR = (Vector2)pos + Vector2.right * hw + Vector2.down * hh;
          if( ClipToInsidePolygon2D( poly, ref LR ) )
          {
            if( pos.y < LR.y + hh ) pos.y = LR.y + hh;
            if( pos.x > LR.x - hw ) pos.x = LR.x - hw;
          }
        }

#if UNITY_EDITOR
        Vector3 cp3 = transform.position;
        Debug.DrawLine( new Vector3( -hw, -hh, 0 ) + cp3, new Vector3( -hw, hh, 0 ) + cp3, Color.blue );
        Debug.DrawLine( new Vector3( -hw, hh, 0 ) + cp3, new Vector3( hw, hh, 0 ) + cp3, Color.blue );
        Debug.DrawLine( new Vector3( hw, hh, 0 ) + cp3, new Vector3( hw, -hh, 0 ) + cp3, Color.blue );
        Debug.DrawLine( new Vector3( hw, -hh, 0 ) + cp3, new Vector3( -hw, -hh, 0 ) + cp3, Color.blue );
        cp3 = pos;
        Debug.DrawLine( new Vector3( -hw, -hh, 0 ) + cp3, new Vector3( -hw, hh, 0 ) + cp3, Color.gray );
        Debug.DrawLine( new Vector3( -hw, hh, 0 ) + cp3, new Vector3( hw, hh, 0 ) + cp3, Color.gray );
        Debug.DrawLine( new Vector3( hw, hh, 0 ) + cp3, new Vector3( hw, -hh, 0 ) + cp3, Color.gray );
        Debug.DrawLine( new Vector3( hw, -hh, 0 ) + cp3, new Vector3( -hw, -hh, 0 ) + cp3, Color.gray );
#endif
        if( EncompassBounds )
        {
          Bounds bounds = Global.instance.CameraBounds;
          if( Global.instance.CameraPoly is PolygonCollider2D )
          {
            // PolygonCollider2D bounds does not have a usable center value
            pos.x = Global.instance.CameraPoly.transform.position.x + bounds.center.x;
            pos.y = Global.instance.CameraPoly.transform.position.y + bounds.center.y;
          }
          else
          {
            // BoxCollider2D bounds has a center value
            pos.x = bounds.center.x;
            pos.y = bounds.center.y;
          }
          if( Camera.main.orthographic )
          {
            if( bounds.extents.y > bounds.extents.x )
              ortho = bounds.extents.y;
            else
              ortho = bounds.extents.x / Camera.main.aspect;
          }
          else
            pos.z = -Mathf.Max( bounds.extents.x / Mathf.Sin( xangle ), bounds.extents.y / Mathf.Sin( yangle ) );
          Debug.DrawLine( new Vector3( bounds.min.x, bounds.min.y ), new Vector3( bounds.min.x, bounds.max.y ), Color.yellow );
          Debug.DrawLine( new Vector3( bounds.min.x, bounds.max.y ), new Vector3( bounds.max.x, bounds.max.y ), Color.yellow );
          Debug.DrawLine( new Vector3( bounds.max.x, bounds.max.y ), new Vector3( bounds.max.x, bounds.min.y ), Color.yellow );
          Debug.DrawLine( new Vector3( bounds.max.x, bounds.min.y ), new Vector3( bounds.min.x, bounds.min.y ), Color.yellow );
        }
      }
    }

    if( Camera.main.orthographic )
    {
      //Camera.main.orthographicSize = Mathf.MoveTowards( Camera.main.orthographicSize, ortho, orthoSpeed * Time.deltaTime );
      Camera.main.orthographicSize = Mathf.Lerp( Camera.main.orthographicSize, ortho, Mathf.Clamp01( orthoSpeed * Time.deltaTime ) );
    }

    if( lerpAlpha > 0 )
    {
      transform.position = Vector3.Lerp( transform.position, pos, Mathf.Clamp01( lerpAlpha * Time.deltaTime ) );
      //transform.position = Vector3.MoveTowards( transform.position, pos, SmoothSpeed * Time.deltaTime );
    }
    else
      transform.position = pos;


    /*if( snapToPixel )
    {
      float yangle = Mathf.Deg2Rad * Camera.main.fieldOfView * 0.5f;
      float worldCameraHeight = 2f * Mathf.Tan( yangle ) * -transform.position.z;
      float xangle = yangle * ((float)Camera.main.pixelWidth / (float)Camera.main.pixelHeight);
      float hw = Mathf.Tan( xangle ) * -transform.position.z;
      pixelSnap = worldCameraHeight / pixelDensity * snapDivide; //(float)cam.pixelHeight;
      presnap = transform.position.x;
      transform.position = new Vector3( (Mathf.Floor( transform.position.x / pixelSnap ) + 0.5f) * pixelSnap, (Mathf.Floor( transform.position.y / pixelSnap ) + 0.5f) * pixelSnap, transform.position.z );
      snapped = transform.position.x;
    }*/
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