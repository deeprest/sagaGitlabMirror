using UnityEngine;

public class BossPrototype : Entity
{
  // sight, target
  [SerializeField] Entity NearbyTarget;
  [SerializeField] Transform SightOrigin;
  public float sightRange = 6;
  Timer SightPulseTimer = new Timer();
  
  Timer hitPauseTimer = new Timer();
  [SerializeField] float hitPause = 3;
  
  // Death
  Timer explosionTimer = new Timer();
  [SerializeField] float explosionInterval = 0.2f;
  [SerializeField] float explosionRange = 1;
  [SerializeField] GameObject junk;

  protected override void Start()
  {
    base.Start();
    UpdateLogic = Logic;
    UpdateHit = Hit;
    UpdateCollision = BoxCollisionSingle;
    SightPulseTimer.Start( int.MaxValue, 1, ( x ) => { SightPulse(); }, null );
  }

  void SightPulse()
  {
    // reaffirm target
    NearbyTarget = null;
    int count = Physics2D.OverlapCircleNonAlloc( SightOrigin.position, sightRange, Global.ColliderResults, Global.EnemyInterestLayers );
    for( int i = 0; i < count; i++ )
    {
      Entity potentialTarget = Global.ColliderResults[i].GetComponent<Entity>();
      if( potentialTarget != null && IsEnemyTeam( potentialTarget.TeamFlags ) )
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

  void Logic()
  {
    if( Health <= 0 )
      return;
    if( NearbyTarget == null )
    {
      animator.Play( "idle" );
    }
    else
    if( !hitPauseTimer.IsActive )
    {
      Vector2 targetpos = NearbyTarget.transform.position;
      Vector2 delta = targetpos - (Vector2) transform.position;
      hitCount = Physics2D.LinecastNonAlloc( transform.position, targetpos, Global.RaycastHits, Global.SightObstructionLayers );
      if( hitCount == 0 )
      {
        if( delta.y < 2 )
        {
          velocity = (delta.x > 0 ? Vector2.right : Vector2.left) * 3;
        }
      }
    }
  }

  void Hit()
  {
    hitCount = Physics2D.BoxCastNonAlloc( transform.position, box.size, 0, velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
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
          HitPause();
        }
      }
    }
  }
  
  void HitPause()
  {
    velocity = Vector2.zero;
    //animator.Play( "laugh" );
    hitPauseTimer.Start( hitPause, null, delegate { animator.Play( "idle" ); } );
  }
  
  protected override void Die( Damage damage )
  {
    velocity = Vector2.zero;
    
    // foreach( var c in IgnoreCollideObjects )
    //   c.enabled = false;
    UpdateHit = null;
    //UpdateCollision = null;
    explosionTimer.Start( 10, explosionInterval,
      delegate
      {
        Instantiate( explosion, transform.position + (Vector3)Random.insideUnitCircle * explosionRange, Quaternion.identity );
        Instantiate( junk, transform.position, Quaternion.identity );
      },
      delegate
      {
        Destroy( gameObject );
        GameObject[] prefab = GetDeathSpawnObjects();
        for( int i = 0; i < prefab.Length; i++ )
          Instantiate( prefab[i], transform.position, Quaternion.identity );
        
      } );
  }
}