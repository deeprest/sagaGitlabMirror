using UnityEngine;

public class Airbot : Entity
{
  Timer hitPauseTimer = new Timer();
  const float small = 0.1f;
  const float hitPauseOffset = 0.5f;
  private Vector2 direction;

  // Attack
  [SerializeField] float AttackSpeed = 3;
  private float speed;
  public float targetOffset = 0.5f;
  Vector3 targetPosition;
  [SerializeField] Entity NearbyTarget;
  public Vector2 lastKnownTargetDirection;
  public Vector2 lastKnownTargetPosition;

  // Wander
  [SerializeField] float WanderSpeed = 1.5f;
  Timer wanderTimer = new Timer();
  public float WanderRadius = 0.5f;
  public float WanderInterval = 1f;
  public float WanderAngleChange = 15;


  // sight, target
  [SerializeField] Transform sightOrigin;
  public float sightRange = 6;
  Collider2D[] results = new Collider2D[32];

  Timer SightPulseTimer = new Timer();

  protected override void Start()
  {
    base.Start();
    UpdateLogic = AirbotLogic;
    UpdateHit = AirbotHit;
    UpdateCollision = CircleCollisionVelocity;

    SightPulseTimer.Start( int.MaxValue, 1, ( x ) => { SightPulse(); }, null );
  }

  void SightPulse()
  {
    // reaffirm target
    NearbyTarget = null;
    int count = Physics2D.OverlapCircleNonAlloc( transform.position, sightRange, results, Global.EnemySightLayers );
    for( int i = 0; i < count; i++ )
    {
      Collider2D cld = results[i];
      //Character character = results[i].transform.root.GetComponentInChildren<Character>();
      Entity potentialTarget = results[i].GetComponent<Entity>();
      if( potentialTarget != null && IsEnemyTeam( potentialTarget.Team ) )
      {
        NearbyTarget = potentialTarget;
        break;
      }
    }
  }

  protected override void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    base.OnDestroy();
    hitPauseTimer.Stop( false );
    SightPulseTimer.Stop( false );
  }

  void AirbotLogic()
  {
    if( NearbyTarget == null )
    {
      animator.Play( "idle" );
      Wander();
    }
    else
    {
      Vector2 playerpos = NearbyTarget.transform.position;
      Vector2 delta = playerpos - (Vector2) transform.position;
      if( delta.sqrMagnitude < circle.radius * circle.radius )
      {
        HitPause();
      }
      else if( delta.sqrMagnitude > sightRange * sightRange )
      {
        animator.Play( "idle" );
        Wander();
      }
      else if( !hitPauseTimer.IsActive )
      {
        Entity visibleTarget = null;
        // check line of sight to potential target
        hitCount = Physics2D.LinecastNonAlloc( sightOrigin.position, playerpos, RaycastHits, Global.EnemySightLayers );
        for( int a = 0; a < hitCount; a++ )
        {
          hit = RaycastHits[a];
          if( hit.transform == transform )
            continue;
          if( hit.transform.root == NearbyTarget.transform )
          {
            visibleTarget = hit.transform.GetComponent<Entity>();
            lastKnownTargetDirection = visibleTarget.velocity;
            lastKnownTargetPosition = visibleTarget.transform.position;
            animator.Play( "alert" );
            speed = AttackSpeed;
            pathAgent.SetPath( visibleTarget.transform.position + Vector3.up * targetOffset, null );
          }
          // early out on first visible thing
          break;
        }
        if( visibleTarget == null )
          Search();
      }
    }
    pathAgent.UpdatePath();
    velocity = pathAgent.MoveDirection.normalized * speed;
  }

  void Wander()
  {
    speed = WanderSpeed;
    if( !wanderTimer.IsActive && !pathAgent.HasPath )
    {
      direction = Quaternion.AngleAxis( WanderAngleChange * (Random.value * 2 - 1), Vector3.forward ) * direction.normalized * WanderRadius;
      //direction = Random.insideUnitCircle * WanderRadius;
      pathAgent.SetPath( transform.position + (Vector3) direction );
      wanderTimer.Start( WanderInterval );
    }
  }

  // Search
  public float SearchSpeed = 2.5f;
  public float SearchRadius = 2;
  Timer fanTimer = new Timer();
  public float anglerange;


  void Search()
  {
    if( !fanTimer.IsActive )
      fanTimer.Start( 10, null, null );
#if UNITY_EDITOR
    if( fanTimer.IsActive )
    {
      Vector3 vecMax, vecMin;
      anglerange = 180f * fanTimer.ProgressNormalized;
      vecMax = Quaternion.AngleAxis( anglerange, Vector3.forward ) * lastKnownTargetDirection.normalized * SearchRadius;
      vecMin = Quaternion.AngleAxis( -anglerange, Vector3.forward ) * lastKnownTargetDirection.normalized * SearchRadius;
      Debug.DrawLine( transform.position, transform.position + vecMax, Color.white );
      Debug.DrawLine( transform.position, transform.position + vecMin, Color.white );
      Debug.DrawLine( transform.position, transform.position + (Vector3) direction, Color.green );
    }
#endif
    SightPulse();
    speed = SearchSpeed;
    if( !pathAgent.HasPath )
    {
      if( fanTimer.IsActive )
        direction = Quaternion.AngleAxis( 180f * fanTimer.ProgressNormalized * (Random.value * 2 - 1), Vector3.forward ) * lastKnownTargetDirection.normalized * SearchRadius;
      pathAgent.SetPath( transform.position + (Vector3) direction );
    }
  }

  void AirbotHit()
  {
    hitCount = Physics2D.CircleCastNonAlloc( transform.position, circle.radius, velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
    //hitCount = Physics2D.BoxCastNonAlloc( transform.position, box.size, 0, velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
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
        if( dam.TakeDamage( dmg ) )
        {
          pathAgent.SetPath( new Vector3( hit.point.x, hit.point.y, 0 ) + Vector3.up * hitPauseOffset );
          HitPause();
        }
      }
    }
  }

  void HitPause()
  {
    animator.Play( "laugh" );
    hitPauseTimer.Start( 1, null, delegate { animator.Play( "idle" ); } );
  }
}