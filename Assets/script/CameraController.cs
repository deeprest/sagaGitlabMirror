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

  public float orthoTarget = 1;
  public float orthoSpeed = 1;

  public bool snapToPixel = true;
  const float pixelDensity = 64;
  public float snapDivide = 1;
  public float pixelSnap;
  public float presnap;
  public float snapped;

  public CameraZone ActiveCameraZone;

  /*
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
  */
#if CLIP_POINT_ATTEMPT
  public class ClipPoint
  {
    public Collider2D collider;
    public Vector2 vector;
    public bool clip = false;
    public bool adjust = true;
    public System.Action<Vector2> adjustFunction;
  }
  [SerializeField] float project = 0.1f;
#endif

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

      if( ActiveCameraZone != null )
      {
        float hh, hw, xangle = 0, yangle = 0;
        if( Camera.main.orthographic )
        {
          hh = Camera.main.orthographicSize;
          hw = Camera.main.orthographicSize * Camera.main.aspect;
        }
        else
        {
          // draw gray lines showing camera rectangle
          yangle = Mathf.Deg2Rad * Camera.main.fieldOfView * 0.5f;
          hh = Mathf.Tan( yangle ) * -transform.position.z;
          xangle = yangle * ((float)Camera.main.pixelWidth / (float)Camera.main.pixelHeight);
          hw = Mathf.Tan( xangle ) * -transform.position.z;
        }

        Vector2 debug = pos;
        Vector2 origin = LookTarget.transform.position;

#if CLIP_POINT_ATTEMPT
        /*
        ClipPoint[] clip = new ClipPoint[4];
        // UL
        clip[0] = new ClipPoint
        {
          vector = (Vector2)pos + Vector2.left * hw + Vector2.up * hh, adjustFunction = ( point ) => {
            if( pos.x < point.x + hw ) pos.x = point.x + hw;
            if( pos.y > point.y - hh ) pos.y = point.y - hh;
          }
        };
        // UR
        clip[1] = new ClipPoint
        {
          vector = (Vector2)pos + Vector2.right * hw + Vector2.up * hh, adjustFunction = ( point ) => {
            if( pos.x > point.x - hw ) pos.x = point.x - hw;
            if( pos.y > point.y - hh ) pos.y = point.y - hh;
          }
        };
        // LR
        clip[2] = new ClipPoint
        {
          vector = (Vector2)pos + Vector2.right * hw + Vector2.down * hh, adjustFunction = ( point ) => {
            if( pos.x > point.x - hw ) pos.x = point.x - hw;
            if( pos.y < point.y + hh ) pos.y = point.y + hh;
          }
        };
        // LL
        clip[3] = new ClipPoint
        {
          vector = (Vector2)pos + Vector2.left * hw + Vector2.down * hh, adjustFunction = ( point ) => {
            if( pos.x < point.x + hw ) pos.x = point.x + hw;
            if( pos.y < point.y + hh ) pos.y = point.y + hh;
          }
        };


        for( int i = 0; i < ActiveCameraZone.colliders.Length; i++ )
        {
          for( int a = 0; a < clip.Length; a++ )
          {
            clip[a].clip = false;
            if( ClipToInsideCollider2D( ActiveCameraZone.colliders[i], ref clip[a].vector, origin ) )
            {
              clip[a].collider = ActiveCameraZone.colliders[i];
              clip[a].clip = true;
            }
          }
        }

        for( int a = 0; a < clip.Length; a++ )
        {
          if( clip[a].clip )
          {
            clip[a].adjust = true;
            for( int b = 0; b < ActiveCameraZone.colliders.Length; b++ )
            {
              if( clip[a].collider != ActiveCameraZone.colliders[b] && ActiveCameraZone.colliders[b].OverlapPoint( clip[a].vector ) )
              {
                clip[a].adjust = false;
                break;
              }
            }
          }
        }

        for( int a = 0; a < clip.Length; a++ )
        {
          if( clip[a].adjust )
          {
            clip[a].adjustFunction( clip[a].vector );
          }
        }
        */
#endif

        foreach( var cld in ActiveCameraZone.colliders )
        {
          if( !cld.OverlapPoint( origin ) )
            continue;
          Vector2 UL = (Vector2)pos + Vector2.left * hw + Vector2.up * hh;
          if( ClipToInsideCollider2D( cld, ref UL, LookTarget.transform.position ) )
          {
            if( pos.y > UL.y - hh ) pos.y = UL.y - hh;
            if( pos.x < UL.x + hw ) pos.x = UL.x + hw;
          }

          Vector2 UR = (Vector2)pos + Vector2.right * hw + Vector2.up * hh;
          if( ClipToInsideCollider2D( cld, ref UR, LookTarget.transform.position ) )
          {
            if( pos.y > UR.y - hh ) pos.y = UR.y - hh;
            if( pos.x > UR.x - hw ) pos.x = UR.x - hw;
          }

          Vector2 LL = (Vector2)pos + Vector2.left * hw + Vector2.down * hh;
          if( ClipToInsideCollider2D( cld, ref LL, LookTarget.transform.position ) )
          {
            if( pos.y < LL.y + hh ) pos.y = LL.y + hh;
            if( pos.x < LL.x + hw ) pos.x = LL.x + hw;
          }

          Vector2 LR = (Vector2)pos + Vector2.right * hw + Vector2.down * hh;
          if( ClipToInsideCollider2D( cld, ref LR, LookTarget.transform.position ) )
          {
            if( pos.y < LR.y + hh ) pos.y = LR.y + hh;
            if( pos.x > LR.x - hw ) pos.x = LR.x - hw;
          }
        }

#if UNITY_EDITOR
        Vector3 cp3 = transform.position;
        //Debug.DrawLine( new Vector3( -hw, -hh, 0 ) + cp3, new Vector3( -hw, hh, 0 ) + cp3, Color.blue );
        //Debug.DrawLine( new Vector3( -hw, hh, 0 ) + cp3, new Vector3( hw, hh, 0 ) + cp3, Color.blue );
        //Debug.DrawLine( new Vector3( hw, hh, 0 ) + cp3, new Vector3( hw, -hh, 0 ) + cp3, Color.blue );
        //Debug.DrawLine( new Vector3( hw, -hh, 0 ) + cp3, new Vector3( -hw, -hh, 0 ) + cp3, Color.blue );
        cp3 = pos;
        Debug.DrawLine( new Vector3( -hw, -hh, 0 ) + cp3, new Vector3( -hw, hh, 0 ) + cp3, Color.green );
        Debug.DrawLine( new Vector3( -hw, hh, 0 ) + cp3, new Vector3( hw, hh, 0 ) + cp3, Color.green );
        Debug.DrawLine( new Vector3( hw, hh, 0 ) + cp3, new Vector3( hw, -hh, 0 ) + cp3, Color.green );
        Debug.DrawLine( new Vector3( hw, -hh, 0 ) + cp3, new Vector3( -hw, -hh, 0 ) + cp3, Color.green );
        cp3 = debug;
        Debug.DrawLine( new Vector3( -hw, -hh, 0 ) + cp3, new Vector3( -hw, hh, 0 ) + cp3, Color.blue );
        Debug.DrawLine( new Vector3( -hw, hh, 0 ) + cp3, new Vector3( hw, hh, 0 ) + cp3, Color.blue );
        Debug.DrawLine( new Vector3( hw, hh, 0 ) + cp3, new Vector3( hw, -hh, 0 ) + cp3, Color.blue );
        Debug.DrawLine( new Vector3( hw, -hh, 0 ) + cp3, new Vector3( -hw, -hh, 0 ) + cp3, Color.blue );
#endif
        if( ActiveCameraZone.EncompassBounds )
        {
          Bounds bounds = new Bounds();
          bounds.center = ActiveCameraZone.colliders[0].transform.position;
          foreach( var wtf in ActiveCameraZone.colliders )
            bounds.Encapsulate( wtf.bounds );

          pos.x = bounds.center.x;
          pos.y = bounds.center.y;

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
      Camera.main.orthographicSize = Mathf.Lerp( Camera.main.orthographicSize, ortho, Mathf.Clamp01( orthoSpeed * Time.unscaledDeltaTime ) );
    }

    if( lerpAlpha > 0 )
    {
      transform.position = Vector3.Lerp( transform.position, pos, Mathf.Clamp01( lerpAlpha * Time.unscaledDeltaTime ) );
      //transform.position = Vector3.MoveTowards( transform.position, pos, SmoothSpeed * Time.deltaTime );
    }
    else
      transform.position = pos;
  }

  bool ClipToInsideCollider2D( Collider2D cld, ref Vector2 cp, Vector2 origin )
  {
    if( !cld.OverlapPoint( cp ) )
    {
      Vector2[] points = null;
      if( cld is PolygonCollider2D )
        points = (cld as PolygonCollider2D).points;
      else if( cld is BoxCollider2D )
      {
        BoxCollider2D box = cld as BoxCollider2D;
        points = new Vector2[4];
        points[0] = box.offset + (Vector2.left * box.size.x * 0.5f) + (Vector2.down * box.size.y * 0.5f);
        points[1] = box.offset + (Vector2.right * box.size.x * 0.5f) + (Vector2.down * box.size.y * 0.5f);
        points[2] = box.offset + (Vector2.right * box.size.x * 0.5f) + (Vector2.up * box.size.y * 0.5f);
        points[3] = box.offset + (Vector2.left * box.size.x * 0.5f) + (Vector2.up * box.size.y * 0.5f);
      }
      List<Vector2> pots = new List<Vector2>();
      // add transform position for world space
      for( int i = 0; i < points.Length; i++ )
        points[i] += (Vector2)cld.transform.position;
      for( int i = 0; i < points.Length; i++ )
      {
        int next = (i + 1) % points.Length;
        /*Vector2 intersection = cp;
        if( Util.LineSegmentsIntersectionWithPrecisonControl( points[i], points[next], origin, cp, ref intersection ) )
        {
          pots.Add( intersection );
          Debug.DrawLine( origin, intersection, Color.red );
        }*/

        Vector2 segment = points[next] - points[i];
        if( !Util.DoLinesIntersect( points[i].x, points[i].y, points[next].x, points[next].y, origin.x, origin.y, cp.x, cp.y ) )
          continue;
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
        float dist = Vector2.Distance( p, origin );
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