#define DEBUG_LINES
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour, IDamage
{
  public Rigidbody2D body;
  public BoxCollider2D box;
  public new SpriteRenderer renderer;
  public Animator animator;

  public Collider2D[] colliders;
  public SpriteRenderer[] spriteRenderers;

  public bool UseGravity = true;
  public Vector2 velocity = Vector2.zero;
  public Vector2 pushVelocity = Vector2.zero;
  public Vector2 inertia = Vector2.zero;
  public float friction = 0.05f;
  public float airFriction = 0.05f;
  public float raylength = 0.01f;
  public float contactSeparation = 0.01f;

  public List<Transform> IgnoreCollideObjects;
  protected bool collideRight = false;
  protected bool collideLeft = false;
  protected bool collideTop = false;
  protected bool collideBottom = false;
  protected RaycastHit2D[] hits;
  protected RaycastHit2D hitRight;
  protected RaycastHit2D hitLeft;

  [Header( "Pathing" )]
  public bool HasPath = false;
  public float WaypointRadii = 0.1f;
  public float DestinationRadius = 0.3f;
  public Vector3 DestinationPosition;
  public System.Action OnPathEnd;
  public System.Action OnPathCancel;
  public NavMeshPath nvp;
  float PathEventTime;
  public List<Vector3>.Enumerator waypointEnu;
  public List<Vector3> waypoint = new List<Vector3>();
  public List<LineSegment> debugPath = new List<LineSegment>();
  int AgentTypeID;
  public string AgentTypeName = "Small";
  public Vector2 MoveDirection;
  // sidestep
  public bool SidestepAvoidance = true;
  bool DefaultSidestepAvoidance = true;
  float SidestepLast;
  Vector2 Sidestep;
  RaycastHit2D[] RaycastHits;

  [Header("Damage")]
  public bool CanTakeDamage = true;
  public int health = 5;
  public GameObject explosion;
  public AudioClip soundHit;

  // FLASH
  Timer flashTimer = new Timer();
  public float flashInterval = 0.05f;
  public int flashCount = 5;
  bool flip = false;
  readonly float flashOn = 1f;

  public Damage ContactDamage;

  // "collision" impedes this object's movement
  protected System.Action UpdateCollision;
  // "hit" inflicts damage on others
  protected System.Action UpdateHit;
  // integrate forces into position
  protected System.Action UpdatePosition;
  // brains!!
  protected System.Action UpdateLogic;

  public virtual void PreSceneTransition() { }
  public virtual void PostSceneTransition() { }

  protected void CharacterStart()
  {
    colliders = GetComponentsInChildren<Collider2D>();
    foreach( var cld in colliders )
      IgnoreCollideObjects.Add( cld.transform );
    spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    UpdateHit = BoxHit;
    UpdateCollision = BoxCollision;
    UpdatePosition = BasicPosition;
    //if( animator != null )
    //animator.Play( "idle" );
    nvp = new NavMeshPath();
    AgentTypeID = Global.instance.AgentType[AgentTypeName];
    RaycastHits = new RaycastHit2D[4];
  }

  void Update()
  {
    if( UpdateLogic != null )
      UpdateLogic();

    if( UpdateHit != null )
      UpdateHit();

    if( UpdatePosition != null )
      UpdatePosition();

    if( UpdateCollision != null )
      UpdateCollision();

    //body.MovePosition( transform.position );
  }

  protected void BoxHit()
  {
    if( ContactDamage == null )
      return;
    hits = Physics2D.BoxCastAll( body.position, box.size, 0, velocity, raylength, LayerMask.GetMask( Global.CharacterDamageLayers ) );
    foreach( var hit in hits )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.point;
        dam.TakeDamage( dmg );
      }
    }
  }

  Timer pushTimer = new Timer();

  public void Push( Vector2 pVelocity, float duration )
  {
    pushVelocity = pVelocity;
    pushTimer.Stop( false );
    pushTimer.Start( duration );
  }

  protected void BasicPosition()
  {
    if( pushTimer.IsActive )
      velocity = pushVelocity;

    if( UseGravity )
      velocity.y += -Global.Gravity * Time.deltaTime;

    if( collideTop )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
    if( collideBottom )
    {
      velocity.x -= (velocity.x * friction) * Time.deltaTime;
      velocity.y = Mathf.Max( velocity.y, 0 );
    }
    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
    }
    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
    }

    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
    velocity -= (velocity * airFriction) * Time.deltaTime;
    transform.position += (Vector3)velocity * Time.deltaTime;
  }

  protected void BoxCollision()
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;

    const float corner = 0.707f;

    Vector2 boxOffset = box.offset;
    boxOffset.x *= Mathf.Sign( transform.localScale.x );
    Vector2 adjust = (Vector2)transform.position + boxOffset;

    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.down, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.transform ) )
        continue;
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation;
        break;
      }
    }
    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.up, Mathf.Max( raylength, velocity.y * Time.deltaTime ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.transform ) )
        continue;
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        break;
      }
    }
    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.left, Mathf.Max( raylength, -velocity.x * Time.deltaTime ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.transform ) )
        continue;
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        hitLeft = hit;
        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.right, Mathf.Max( raylength, velocity.x * Time.deltaTime ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.transform ) )
        continue;
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        hitRight = hit;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        break;
      }
    }
    transform.position = (Vector3)(adjust - boxOffset);
  }

  protected virtual void Die()
  {
    Instantiate( explosion, transform.position, Quaternion.identity );
    //Global.instance.Destroy( gameObject );
    Destroy( gameObject );
  }

  public bool TakeDamage( Damage d )
  {
    if( !CanTakeDamage || health <= 0 )
      return false;
    health -= d.amount;
    if( health <= 0 )
    {
      flashTimer.Stop( false );
      Die();
    }
    else
    {
      if( soundHit != null )
        Global.instance.AudioOneShot( soundHit, transform.position );
      // color pulse
      flip = false;
      foreach( var sr in spriteRenderers )
        sr.material.SetFloat( "_FlashAmount", flashOn );
      flashTimer.Start( flashCount * 2, flashInterval, delegate ( Timer t )
      {
        flip = !flip;
        if( flip )
          foreach( var sr in spriteRenderers )
            sr.material.SetFloat( "_FlashAmount", flashOn );
        else
          foreach( var sr in spriteRenderers )
            sr.material.SetFloat( "_FlashAmount", 0 );
      }, delegate
      {
        foreach( var sr in spriteRenderers )
          sr.material.SetFloat( "_FlashAmount", 0 );
      } );
    }
    return true;
  }
  #region Pathing
  protected void UpdatePath()
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
        if( Vector3.SqrMagnitude( transform.position - waypointFlat ) > (Time.timeScale * WaypointRadii) * (WaypointRadii * Time.timeScale) )
        {
          MoveDirection = (Vector2)waypointEnu.Current - (Vector2)transform.position;
        }
        else
        if( waypointEnu.MoveNext() && Vector3.SqrMagnitude( transform.position - DestinationPosition ) > DestinationRadius * DestinationRadius )
        {
          MoveDirection = (Vector2)waypointEnu.Current - (Vector2)transform.position;
        }
        else
        {
          // destination reached
          // clear the waypoints before calling the callback because it may set another path and you do not want them to accumulate
          waypoint.Clear();
          debugPath.Clear();
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

#if DEBUG_LINES
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
            int count = Physics2D.CircleCastNonAlloc( transform.position, 0.5f/*box.edgeRadius*/, MoveDirection.normalized, RaycastHits, raycastDistance, LayerMask.GetMask( Global.CharacterSidestepLayers ) );
            if( count > 0 )
            {
              for( int i = 0; i < count; i++ )
              {
                RaycastHit2D hit = RaycastHits[i];
                Character other = hit.transform.root.GetComponent<Character>();
                if( other != null && other != this )
                {
                  Vector3 delta = other.transform.position - transform.position;
                  Sidestep = ((transform.position + Vector3.Project( delta, MoveDirection.normalized )) - other.transform.position).normalized * Global.instance.SidestepDistance;
                  break;
                }
              }
            }
          }
        }
      }
      MoveDirection += Sidestep;
    }



#if DEBUG_LINES
    Debug.DrawLine( transform.position, (Vector2)transform.position + MoveDirection.normalized * 0.5f, Color.magenta );
    //Debug.DrawLine( transform.position, transform.position + FaceDirection.normalized, Color.red );
#endif
  }

  void ClearPath()
  {
    HasPath = false;
    waypoint.Clear();
    debugPath.Clear();
    OnPathEnd = null;
    DestinationPosition = transform.position;
  }

  public bool SetPath( Vector3 TargetPosition, System.Action onArrival = null )
  {
    OnPathEnd = onArrival;

    Vector3 EndPosition = TargetPosition;
    NavMeshHit hit;
    if( NavMesh.SamplePosition( TargetPosition, out hit, 5.0f, NavMesh.AllAreas ) )
      EndPosition = hit.position;
    DestinationPosition = EndPosition;

    Vector3 StartPosition = transform.position;
    if( NavMesh.SamplePosition( StartPosition, out hit, 5.0f, NavMesh.AllAreas ) )
      StartPosition = hit.position;


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
          debugPath.Clear();
          foreach( var p in nvp.corners )
          {
            LineSegment seg = new LineSegment();
            seg.a = prev;
            seg.b = p;
            debugPath.Add( seg );
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
#endregion

}
