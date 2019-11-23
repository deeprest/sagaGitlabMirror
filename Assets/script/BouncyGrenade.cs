﻿using UnityEngine;
using System.Collections;

public class BouncyGrenade : Projectile, IDamage
{
  public GameObject explosion;
  Timer timeoutTimer;
  Timer pulseTimer;
  [SerializeField] float pulseInterval = 0.2f;
  [SerializeField] Light light;
  [SerializeField] float radiusFudge;

  void Start()
  {
    // weapon plays the sound instead
    //if( StartSound != null )
    //Global.instance.AudioOneShot( StartSound, transform.position );
    GetComponent<Rigidbody2D>().velocity = new Vector2( velocity.x, velocity.y );
    timeoutTimer = new Timer( timeout, null, Boom );
    pulseTimer = new Timer( int.MaxValue, pulseInterval,delegate(Timer obj){ light.enabled = !light.enabled; }, null );
  }

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
    pulseTimer.Stop( false );
  }

  void Boom()
  {
    if( gameObject != null && !gameObject.activeSelf )
      return;
    Instantiate( explosion, transform.position, Quaternion.identity );
    timeoutTimer.Stop( false );
    //Global.instance.Destroy( gameObject );
    Destroy( gameObject );
  }

  void FixedUpdate()
  {
    RaycastHit2D hit = Physics2D.CircleCast( transform.position, circle.radius + radiusFudge, velocity, raycastDistance, LayerMask.GetMask( Global.BouncyGrenadeCollideLayers ) );
    if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator )) )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.point;
        if( dam.TakeDamage( dmg ) )
          Boom();
      }
    }
  }

  public bool TakeDamage( Damage damage )
  {
    Boom();
    return true;
  }

  // UNITY CRASH BUG: Destroying this gameObject from within this callback causes a crash in Unity 2019.2.6f1
  // Even deferred destruction in Global.cs caused a crash.
  /*void OnCollisionEnter2D( Collision2D hit )
  {
    if( (LayerMask.GetMask( Global.BouncyGrenadeCollideLayers ) & (1 << hit.gameObject.layer)) > 0 )
    if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator )) )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.GetContact( 0 ).point;
        dam.TakeDamage( dmg );
      }
      Boom();
    }
  }*/
}
