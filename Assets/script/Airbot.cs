using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Airbot : Entity
{
  [SerializeField] Transform sightOrigin;
  public float sightRange = 6;
  [SerializeField] float WanderSpeed = 1.5f;
  [SerializeField] float AttackSpeed = 3;
  private float speed;
  public float targetOffset = 0.5f;
  const float hitPauseOffset = 0.5f;
  Vector3 targetPosition;
  Timer hitPauseTimer = new Timer();
  bool hitpause = false;
  const float small = 0.1f;
  Timer wanderTimer = new Timer();
  const float WanderRadius = 2;
  const float WanderInterval = 0.5f;

  // sight, target
  Collider2D[] results = new Collider2D[8];
  int LayerMaskCharacter;
  [SerializeField] Entity Target;
  Timer SightPulseTimer = new Timer();

  protected override void Start()
  {
    base.Start();
    UpdateLogic = AirbotLogic;
    UpdateHit = AirbotHit;

    SightPulseTimer.Start( int.MaxValue, 3, ( x ) => {
      // reaffirm target
      Target = null;
      int count = Physics2D.OverlapCircleNonAlloc( transform.position, sightRange, results, LayerMaskCharacter );
      for( int i = 0; i < count; i++ )
      {
        Collider2D cld = results[i];
        //Character character = results[i].transform.root.GetComponentInChildren<Character>();
        Entity potentialTarget = results[i].GetComponent<Entity>();
        if( potentialTarget != null && IsEnemyTeam( potentialTarget.Team ) )
        {
          Target = potentialTarget;
          break;
        }
      }
    }, null );
  }

  protected override void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    base.OnDestroy();
    hitPauseTimer.Stop( false );
  }

  void AirbotLogic()
  {
    if( Target == null )
    {
      animator.Play( "idle" );
      Wander();
    }
    else
    {
      Vector2 player = Target.transform.position;
      Vector2 delta = player - (Vector2)transform.position;
      if( delta.sqrMagnitude > sightRange * sightRange )
      {
        animator.Play( "idle" );
        Wander();
      }
      else if( !hitpause )
      {
        Transform target = null;
        // check line of sight to potential target
        hit = Physics2D.Linecast( sightOrigin.position, player, Global.EnemySightLayers );
        if( hit.transform.root == Target.transform )
          target = hit.transform;

        if( target == null )
        {
          animator.Play( "idle" );
          Wander();
        }
        else
        {
          animator.Play( "alert" );
          pathAgent.SetPath( target.position + Vector3.up * targetOffset );
          speed = AttackSpeed;
        }
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
      pathAgent.SetPath( transform.position + (Vector3)(Random.insideUnitCircle * WanderRadius) );
      wanderTimer.Start( WanderInterval );
    }
  }

  void AirbotHit()
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
          hitpause = true;
          pathAgent.SetPath( new Vector3( hit.point.x, hit.point.y, 0 ) + Vector3.up * hitPauseOffset );
          animator.Play( "laugh" );
          hitPauseTimer.Start( 1, null, delegate
          {
            hitpause = false;
            animator.Play( "idle" );
          } );
        }
      }
    }
  }

}
