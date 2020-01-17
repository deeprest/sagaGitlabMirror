﻿using UnityEngine;
using System.Collections;

public class StickyBomb : Projectile, IDamage
{
  public GameObject explosion;
  Timer timeoutTimer = new Timer();
  Timer pulseTimer = new Timer();
  [SerializeField] float pulseInterval = 0.2f;
  [SerializeField] Light light;
  [SerializeField] float radiusFudge;
  public bool AlignRotationToVelocity = true;
  [SerializeField] float AttachDuration = 2;
  Rigidbody2D body;
  Transform hitTransform;
  [SerializeField] float BoomRadius = 1;

  void Start()
  {
    body = GetComponent<Rigidbody2D>();
    body.velocity = new Vector2( velocity.x, velocity.y );
    timeoutTimer.Start( timeout, null, delegate { TakeDamage( ContactDamage ); } );
    pulseTimer.Start( int.MaxValue, pulseInterval, delegate ( Timer obj ) { light.enabled = !light.enabled; }, null );
  }

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
    pulseTimer.Stop( false );
  }


  void FixedUpdate()
  {
    // +X = forward
    if( AlignRotationToVelocity )
      transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( body.velocity.normalized.y, body.velocity.normalized.x ) ) );
  }

  bool alreadyBoom = false;

  public bool TakeDamage( Damage damage )
  {
    if( gameObject != null && !gameObject.activeSelf )
      return false;
    if( alreadyBoom )
      return false;
    alreadyBoom = true;
    timeoutTimer.Stop( false );

    Collider2D[] clds = Physics2D.OverlapCircleAll( transform.position, BoomRadius, LayerMask.GetMask( Global.StickyBombCollideLayers ) );
    for( int i = 0; i < clds.Length; i++ )
    {
      if( clds[i] != null )
      {
        IDamage dam = clds[i].GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = transform;
          dmg.point = transform.position;
          dam.TakeDamage( dmg );
        }
      }
    }

    Destroy( gameObject );
    Instantiate( explosion, transform.position, Quaternion.identity );
    return true;
  }

  void OnCollisionEnter2D( Collision2D hit )
  {
    if( (LayerMask.GetMask( Global.StickyBombCollideLayers ) & (1 << hit.gameObject.layer)) > 0 )
      if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator.transform )) && !ignore.Contains( hit.transform ) )
      {
        AlignRotationToVelocity = false;
        transform.parent = hit.transform;
        // +X = forward
        transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( -hit.contacts[0].normal.y, -hit.contacts[0].normal.x ) ) );
        body.bodyType = RigidbodyType2D.Static;
        //body.freezeRotation = true;
        animator.Play( "flash" );
        hitTransform = hit.transform;
        timeoutTimer.Start( AttachDuration, null, delegate
        {
          if( alreadyBoom )
            return;
          Damage selfDamage = Instantiate( ContactDamage );
          selfDamage.instigator = transform;
          selfDamage.point = transform.position;
          TakeDamage( selfDamage );
        } );
      }
  }
}
