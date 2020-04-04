using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Airbot : Character
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

  private void OnDestroy()
  {
    hitPauseTimer.Stop( false );
  }

  void Start()
  {
    CharacterStart();
    UpdateLogic = UpdateAirbot;
    UpdateHit = AirbotHit;
  }

  void UpdateAirbot()
  {
    if( Global.instance.CurrentPlayer != null )
    {
      Vector2 player = Global.instance.CurrentPlayer.transform.position;
      Vector2 delta = player - (Vector2)transform.position;
      if( delta.sqrMagnitude < sightRange * sightRange )
      {
        if( !hitpause )
        {
          Transform target = null;
          // for debug
          //target = Global.instance.CurrentPlayer.transform;
          hit = Physics2D.Linecast( sightOrigin.position, player, Global.EnemySightLayers );
          if( hit.transform.root == Global.instance.CurrentPlayer.transform )
            target = hit.transform;

          if( target == null )
          {
            animator.Play( "idle" );
            Wander();
          }
          else
          {
            animator.Play( "alert" );
            SetPath( target.position + Vector3.up * targetOffset );
            speed = AttackSpeed;
          }
        }
      }
      else
      {
        animator.Play( "idle" );
        Wander();
      }
    }
    else
    {
      animator.Play( "idle" );
      Wander();
    }
    UpdatePath();
    velocity = MoveDirection.normalized * speed;
  }

  void Wander()
  {
    speed = WanderSpeed;
    if( !wanderTimer.IsActive && !HasPath )
    {
      SetPath( transform.position + (Vector3)(Random.insideUnitCircle * WanderRadius) );
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
          SetPath( new Vector3( hit.point.x, hit.point.y, 0 ) + Vector3.up * hitPauseOffset );
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

  protected override void Die()
  {
    if( hitPauseTimer != null )
      hitPauseTimer.Stop( false );
    base.Die();
  }
}
