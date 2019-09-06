﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Airbot : Character
{
  public float sightRange = 6;
  public float flySpeed = 2;
  public float targetOffset = 0.5f;
  public float hitPauseOffset = 1;
  Vector3 target;
  Timer hitPauseTimer;
  bool hitpause = false;
  const float small = 0.1f;

  void Start()
  {
    CharacterStart();
    UpdateLogic = UpdateAirbot;
    UpdateHit = AirbotHit;
    hitPauseTimer = new Timer();
  }

  void UpdateAirbot()
  {
    if( Global.instance.CurrentPlayer != null )
    {
      if( !hitpause )
        target = Global.instance.CurrentPlayer.transform.position + Vector3.up * targetOffset;
      Vector3 delta = target - transform.position;
      if( delta.sqrMagnitude < small * small )
        WaypointVector = Vector3.zero;
      else if( delta.sqrMagnitude < sightRange * sightRange )
        SetPath( Global.instance.CurrentPlayer.transform.position );
      //velocity = delta.normalized * flySpeed;
    }
    UpdatePath();
    velocity = WaypointVector.normalized * flySpeed;
  }


  void AirbotHit()
  {
    hits = Physics2D.BoxCastAll( transform.position, box.size, 0, velocity, raylength, LayerMask.GetMask( DamageLayers ) );
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
    }
  }

  protected override void Die()
  {
    if( hitPauseTimer != null )
      hitPauseTimer.Stop( false );
    base.Die();
  }
}
