using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Liftbot : Character
{
  public float sightRange = 6;
  public float flySpeed = 2;
  public float targetOffset = 0.5f;
  public float hitPauseOffset = 1;
  Vector3 target;
  Timer hitPauseTimer;
  bool hitpause = false;
  const float small = 0.1f;
  int pathIndex = 0;
  Vector3[] path;

  void Start()
  {
    CharacterStart();
    UpdateLogic = UpdateAirbot;
    UpdateHit = null;
    UpdateCollision = BoxCollision;
    hitPauseTimer = new Timer();
    CanTakeDamage = false;
    path = new Vector3[] { transform.position, transform.position + Vector3.up * 5 };
  }

  void PathLoop()
  {
    if( !SetPath( path[pathIndex], PathLoop ) )
    {
      //Debug.Log( "Liftbot set path failed.", this );
    }
    pathIndex = ++pathIndex % path.Length;
  }

  void UpdateAirbot()
  {
    if( !HasPath )
      PathLoop();
    UpdatePath();
    velocity = MoveDirection.normalized * flySpeed;
  }


  void AirbotHit()
  {
    /*hits = Physics2D.BoxCastAll( transform.position, box.size, 0, velocity, raylength, LayerMask.GetMask( DamageLayers ) );
    foreach( var hit in hits )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.point;
        dam.TakeDamage( dmg );
        hitpause = true;
        target = new Vector3( hit.point.x, hit.point.y, 0 ) + Vector3.up * hitPauseOffset;
        animator.Play( "laugh" );
        hitPauseTimer.Start( 2, null, delegate
        {
          hitpause = false;
          animator.Play( "idle" );
        } );
      }
    }*/
  }

  protected override void Die()
  {
    base.Die();
    // todo
  }
}
