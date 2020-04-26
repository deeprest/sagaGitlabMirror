using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum Team
{
  None,
  GoodGuys,
  BadDudes,
  Hostile
}

public class Character : MonoBehaviour, IDamage
{
  public Team Team;
  public Rigidbody2D body;
  public BoxCollider2D box;
  public new SpriteRenderer renderer;
  public Animator animator;

  public List<Collider2D> IgnoreCollideObjects;
  public List<SpriteRenderer> spriteRenderers;
  // when Characters are composite under same transform root
  Character parentCharacter;

  public bool UseGravity = true;
  public bool IsStatic = false;
  public Vector2 velocity = Vector2.zero;
  public Vector2 Velocity
  {
    get
    {
      if( carryCharacter == null )
        return velocity;
      else
        return velocity + carryCharacter.Velocity;
    }
  }
  public Vector2 pushVelocity = Vector2.zero;
  protected Timer pushTimer = new Timer();
  // basic support for moving platforms / stacking characters
  public Character carryCharacter;
  public float friction = 0.05f;
  public float raylength = 0.01f;
  public float contactSeparation = 0.01f;
  // collision flags
  protected bool collideRight = false;
  protected bool collideLeft = false;
  protected bool collideTop = false;
  protected bool collideBottom = false;
  // cached for optimization - to avoid allocating every frame
  protected RaycastHit2D[] RaycastHits;
  // cached
  protected RaycastHit2D hit;
  protected int hitCount;
  protected Vector2 adjust;
  Vector2 boxOffset;

  [Header( "Pathing" )]
  public bool EnablePathing = false;
  public string AgentTypeName = "Small";

  [Header( "Damage" )]
  public bool CanTakeDamage = true;
  public int health = 5;
  public int MaxHealth = 5;
  public GameObject explosion;
  public AudioClip soundHit;
  public GameObject spawnWhenDead;
  public int spawnChance = 1;
  // FLASH
  protected Timer flashTimer = new Timer();
  public float flashInterval = 0.05f;
  public int flashCount = 5;
  protected bool flip = false;
  protected readonly float flashOn = 1f;
  // deal this damage on collision
  public Damage ContactDamage;
  public UnityEvent EventDestroyed;

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

  protected virtual void Awake()
  {
    RaycastHits = new RaycastHit2D[8];
  }

  protected virtual void Start()
  {
    if( transform.parent != null )
      parentCharacter = transform.parent.GetComponent<Character>();
    IgnoreCollideObjects.AddRange( GetComponentsInChildren<Collider2D>() );
    spriteRenderers.AddRange( GetComponentsInChildren<SpriteRenderer>() );
    if( !IsStatic )
    {
      UpdateHit = BoxHit;
      UpdateCollision = BoxCollision;
      UpdatePosition = BasicPosition;
    }
    if( EnablePathing )
    {
      pathAgent = new PathAgent();
      pathAgent.Client = this;
      pathAgent.transform = transform;
      pathAgent.AgentTypeID = Global.instance.AgentType[AgentTypeName];
    }
  }

  protected virtual void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    flashTimer.Stop( false );
    pushTimer.Stop( false );
    if( parentCharacter != null )
      parentCharacter.RemoveChild( this );
    parentCharacter = null;
  }

  void Update()
  {
    if( Global.Paused )
      return;

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
    if( ContactDamage != null )
    {
      hitCount = Physics2D.BoxCastNonAlloc( body.position, box.size, 0, velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
      for( int i = 0; i < hitCount; i++ )
      {
        hit = RaycastHits[i];

        IDamage dam = hit.transform.GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = this;
          dmg.damageSource = transform;
          dmg.point = hit.point;
          dam.TakeDamage( dmg );
        }

      }
    }
  }

  public void Push( Vector2 pVelocity, float duration )
  {
    pushVelocity = pVelocity;
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

    //velocity -= (velocity * airFriction) * Time.deltaTime;
    transform.position += (Vector3)Velocity * Time.deltaTime;
    carryCharacter = null;
  }

  protected void BoxCollision()
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    const float corner = 0.707f;
    boxOffset.x = box.offset.x * Mathf.Sign( transform.localScale.x );
    boxOffset.y = box.offset.y;
    adjust = (Vector2)transform.position + boxOffset;

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.down, RaycastHits, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation;
        // moving platforms
        Character cha = hit.transform.GetComponent<Character>();
        if( cha != null )
          carryCharacter = cha;
        break;
      }
    }
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.up, RaycastHits, Mathf.Max( raylength, velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        break;
      }
    }
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.left, RaycastHits, Mathf.Max( raylength, -velocity.x * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        //hitLeft = hit;
        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        break;
      }
    }

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.right, RaycastHits, Mathf.Max( raylength, velocity.x * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        //hitRight = hit;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        break;
      }
    }

    transform.position = adjust - boxOffset;
  }

  protected virtual void Die()
  {
    Instantiate( explosion, transform.position, Quaternion.identity );
    if( spawnWhenDead != null && Random.Range( 0, spawnChance ) == 0 )
      Instantiate( spawnWhenDead, transform.position, Quaternion.identity );
    Destroy( gameObject );
    EventDestroyed?.Invoke();
  }

  public virtual bool TakeDamage( Damage d )
  {
    // dead characters will not absorb projectiles
    if( !CanTakeDamage || health <= 0 )
      return false;
    if( d.instigator != null && d.instigator.Team == Team )
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

  // Composite characters
  public void RemoveChild( Character child )
  {
    Collider2D[] clds = IgnoreCollideObjects.ToArray();
    for( int i = 0; i < clds.Length; i++ )
    {
      if( clds[i].transform == child.transform )
      {
        IgnoreCollideObjects.Remove( clds[i] );
        break;
      }
    }

    SpriteRenderer[] srs = spriteRenderers.ToArray();
    for( int i = 0; i < srs.Length; i++ )
    {
      if( srs[i].transform == child.transform )
      {
        spriteRenderers.Remove( srs[i] );
        break;
      }
    }
  }

  #region Pathing

  public PathAgent pathAgent;

  public class PathAgent
  {
    public Transform transform;
    RaycastHit2D[] RaycastHits;
    public Character Client;

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
              int count = Physics2D.CircleCastNonAlloc( transform.position, 0.5f/*box.edgeRadius*/, MoveDirection.normalized, RaycastHits, raycastDistance, Global.CharacterSidestepLayers );
              for( int i = 0; i < count; i++ )
              {
                Character other = RaycastHits[i].transform.root.GetComponent<Character>();
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
  #endregion

  // for EventDestroyed unity events
  public void DestroyGameObject( GameObject go )
  {
    Destroy( go );
  }

  public virtual bool IsEnemyTeam( Team other )
  {
    return Team != Team.None && other != Team.None && other != Team;
  }
}
