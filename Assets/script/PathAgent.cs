using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class PathAgent
{
  public Transform transform;
  RaycastHit2D[] RaycastHits;
  public Entity Client;

  public bool HasPath;
  public float WaypointRadii = 0.1f;
  public float DestinationRadius = 0.3f;
  [SerializeField] Vector3 DestinationPosition;
  System.Action OnPathEnd;
  System.Action OnPathCancel;
  NavMeshPath nvp;
  float PathEventTime;
  List<Vector3>.Enumerator waypointEnu;
  List<Vector3> waypoint = new List<Vector3>();
#if UNITY_EDITOR
  public List<LineSegment> debugPath = new List<LineSegment>();
#endif
  public int AgentTypeID;
  public Vector2 MoveDirection { get; set; }
  // sidestep
  public bool SidestepAvoidance;
  float SidestepLast;
  Vector2 Sidestep;

  public PathAgent()
  {
    nvp = new NavMeshPath();
  }

  public void UpdatePath()
  {
    if( HasPath )
    {
      if( waypoint.Count > 0 )
      {
        if( Time.time - PathEventTime > Global.instance.RepathInterval )
        {
          PathEventTime = Time.time;
          SetPath( DestinationPosition, OnPathEnd );
        }
        // follow path if waypoints exist
        Vector3 waypointFlat = waypointEnu.Current;
        waypointFlat.z = 0;
        if( Vector3.SqrMagnitude( transform.position - waypointFlat ) > WaypointRadii * WaypointRadii )
        {
          MoveDirection = (Vector2)waypointEnu.Current - (Vector2)transform.position;
        }
        else
        if( waypointEnu.MoveNext() )
        {
          MoveDirection = (Vector2)waypointEnu.Current - (Vector2)transform.position;
        }
        else
        {
          // destination reached
          // clear the waypoints before calling the callback because it may set another path and you do not want them to accumulate
          waypoint.Clear();
#if UNITY_EDITOR
          debugPath.Clear();
#endif
          HasPath = false;
          DestinationPosition = transform.position;
          // do this to allow OnPathEnd to become null because the callback may set another path without a callback.
          System.Action temp = OnPathEnd;
          OnPathEnd = null;
          if( temp != null )
            temp.Invoke();
        }

        //velocity = MoveDirection.normalized * speed;
      }

#if UNITY_EDITOR
      // draw path
      if( debugPath.Count > 0 )
      {
        Color pathColor = Color.white;
        if( nvp.status == NavMeshPathStatus.PathInvalid )
          pathColor = Color.red;
        if( nvp.status == NavMeshPathStatus.PathPartial )
          pathColor = Color.gray;
        foreach( var ls in debugPath )
        {
          Debug.DrawLine( ls.a, ls.b, pathColor );
        }
      }
#endif

    }
    else
    {
      // no path
      MoveDirection = Vector2.zero;
    }
/*
    if( Global.instance.GlobalSidestepping && SidestepAvoidance )
    {
      if( Time.time - SidestepLast > Global.instance.SidestepInterval )
      {
        Sidestep = Vector3.zero;
        SidestepLast = Time.time;
        if( MoveDirection.magnitude > 0.001f )
        {
          float distanceToWaypoint = Vector3.Distance( waypointEnu.Current, transform.position );
          if( distanceToWaypoint > Global.instance.SidestepIgnoreWithinDistanceToGoal )
          {
            float raycastDistance = Mathf.Min( distanceToWaypoint, Global.instance.SidestepRaycastDistance );
            int count = Physics2D.CircleCastNonAlloc( transform.position, 0.5f, MoveDirection.normalized, RaycastHits, raycastDistance, Global.CharacterSidestepLayers );
            for( int i = 0; i < count; i++ )
            {
              Entity other = RaycastHits[i].transform.root.GetComponent<Entity>();
              if( other != null && other != Client )
              {
                Vector3 delta = other.transform.position - transform.position;
                Sidestep = ((transform.position + Vector3.Project( delta, MoveDirection.normalized )) - other.transform.position).normalized * Global.instance.SidestepDistance;
                break;
              }
            }
          }
        }
      }
      MoveDirection += Sidestep;
    }
*/

#if UNITY_EDITOR
    Debug.DrawLine( transform.position, (Vector2)transform.position + MoveDirection.normalized * 0.5f, Color.magenta );
    //Debug.DrawLine( transform.position, transform.position + FaceDirection.normalized, Color.red );
#endif
  }

  public void ClearPath()
  {
    HasPath = false;
    waypoint.Clear();
#if UNITY_EDITOR
    debugPath.Clear();
#endif
    OnPathEnd = null;
    DestinationPosition = transform.position;
  }

  public bool SetPath( Vector3 TargetPosition, System.Action onArrival = null )
  {
    // WARNING!! DO NOT set path from within Start(). The nav meshes are not guaranteed to exist during Start()
    OnPathEnd = onArrival;

    Vector3 EndPosition = TargetPosition;
    NavMeshHit navhit;
    if( NavMesh.SamplePosition( TargetPosition, out navhit, 1.0f, NavMesh.AllAreas ) )
      EndPosition = navhit.position;
    DestinationPosition = EndPosition;

    Vector3 StartPosition = transform.position;
    if( NavMesh.SamplePosition( StartPosition, out navhit, 1.0f, NavMesh.AllAreas ) )
      StartPosition = navhit.position;


    NavMeshQueryFilter filter = new NavMeshQueryFilter();
    filter.agentTypeID = AgentTypeID;
    filter.areaMask = NavMesh.AllAreas;
    nvp.ClearCorners();
    if( NavMesh.CalculatePath( StartPosition, EndPosition, filter, nvp ) )
    {
      if( nvp.status == NavMeshPathStatus.PathComplete || nvp.status == NavMeshPathStatus.PathPartial )
      {
        if( nvp.corners.Length > 0 )
        {
          Vector3 prev = StartPosition;
#if UNITY_EDITOR
          debugPath.Clear();
#endif
          foreach( var p in nvp.corners )
          {
            LineSegment seg = new LineSegment();
            seg.a = prev;
            seg.b = p;
#if UNITY_EDITOR
            debugPath.Add( seg );
#endif
            prev = p;
          }
          waypoint = new List<Vector3>( nvp.corners );
          waypointEnu = waypoint.GetEnumerator();
          waypointEnu.MoveNext();
          PathEventTime = Time.time;
          HasPath = true;
          return true;
        }
        else
        {
          Debug.Log( "corners is zero path to: " + TargetPosition );
        }
      }
      else
      {
        Debug.Log( "invalid path to: " + TargetPosition );
      }
    }
    return false;
  }


}
