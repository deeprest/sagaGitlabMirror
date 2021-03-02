using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  [SerializeField] Camera cam;
  public Controller LookTarget;
  public LerpToTarget lerp;
  public bool UseVerticalRange = true;
  public bool CursorInfluence;
  public float cursorAlpha = 0.5f;
  public float lerpAlpha = 50;
  public float SmoothSpeed = 1;
  public float zOffset;
  public float yHalfWidth = 2;
  // lerp target position
  Vector3 pos;

  public float orthoTarget = 1;
  public float orthoSpeed = 1;

  [SerializeField] CameraZone ActiveCameraZone;
  public bool CameraZoneOverride;

  public void AssignOverrideCameraZone(CameraZone zone)
  {
    ActiveCameraZone = zone;
    CameraZoneOverride = zone!=null;
  }

  public void CameraLateUpdate()
  {
    float ortho = orthoTarget;
    pos.z = zOffset;

    if( !lerp.enabled && LookTarget != null && LookTarget.pawn != null )
    {
      Vector3 lookTarget = LookTarget.pawn.transform.position;
      
      if( CursorInfluence )
      {
        lookTarget = Vector3.Lerp( LookTarget.pawn.transform.position, LookTarget.pawn.CursorWorldPosition, cursorAlpha );
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

      // auto-switch to zone the player enters (respects the ignore flag on the zone)
      if( !CameraZoneOverride )
      {
        CameraZone zone = null;
        CameraZone.DoesOverlapAnyZone( LookTarget.pawn.transform.position, ref zone );
        ActiveCameraZone = zone;
      }

      if( ActiveCameraZone != null )
      {
        if( ActiveCameraZone.SetOrtho )
          ortho = ActiveCameraZone.orthoTarget;
        
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
        Vector2 origin = LookTarget.pawn.transform.position;

        if( CameraZoneOverride ) //ActiveCameraZone.ConfineToZone )
        {
          // if the origin(lookTarget) is outside of all the colliders, then clip to inside.
          Collider2D overlap = null;
          List<Vector2> points = new List<Vector2>();
          foreach( var cld in ActiveCameraZone.colliders )
          {
            if( cld.OverlapPoint( origin ) )
            {
              overlap = cld;
              break;
            }
            else
            {
              points.Add( cld.ClosestPoint( origin ) );
            }
          }
          if( overlap == null )
          {
            Vector2 neworigin = Util.FindClosest( origin, points.ToArray() );
            origin = neworigin + (neworigin - origin).normalized;
          }
        }

        // TODO prevent multiple colliders within same zone from overriding clip points of one another.
        foreach( var cld in ActiveCameraZone.colliders )
        {
          if( !CameraZoneOverride && !cld.OverlapPoint( origin ) )
            continue;
          Vector2 UL = (Vector2)pos + Vector2.left * hw + Vector2.up * hh;
          if( ClipToInsideCollider2D( cld, ref UL, origin ) )
          {
            if( pos.y > UL.y - hh ) pos.y = UL.y - hh;
            if( pos.x < UL.x + hw ) pos.x = UL.x + hw;
          }

          Vector2 UR = (Vector2)pos + Vector2.right * hw + Vector2.up * hh;
          if( ClipToInsideCollider2D( cld, ref UR, origin ) )
          {
            if( pos.y > UR.y - hh ) pos.y = UR.y - hh;
            if( pos.x > UR.x - hw ) pos.x = UR.x - hw;
          }

          Vector2 LL = (Vector2)pos + Vector2.left * hw + Vector2.down * hh;
          if( ClipToInsideCollider2D( cld, ref LL, origin ) )
          {
            if( pos.y < LL.y + hh ) pos.y = LL.y + hh;
            if( pos.x < LL.x + hw ) pos.x = LL.x + hw;
          }

          Vector2 LR = (Vector2)pos + Vector2.right * hw + Vector2.down * hh;
          if( ClipToInsideCollider2D( cld, ref LR, origin ) )
          {
            if( pos.y < LR.y + hh ) pos.y = LR.y + hh;
            if( pos.x > LR.x - hw ) pos.x = LR.x - hw;
          }
        }

#if UNITY_EDITOR
        Vector3 cp3 = transform.position;
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
