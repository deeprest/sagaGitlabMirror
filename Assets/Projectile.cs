﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  public float raycastDistance = 0.2f;
  public float timeout = 2;
  Timer timeoutTimer;
  public Vector3 velocity;
  public CircleCollider2D circle;
  string[] CollideLayers = new string[] { "foreground" };
  public GameObject HitEffect;
  public bool AlignXToMovementDirection = false;

  void Start()
  {
    timeoutTimer = new Timer( timeout, null, delegate()
    {
      if( gameObject!=null )
        Destroy( gameObject );
    } );

    if( AlignXToMovementDirection )
      transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( velocity.normalized.y, velocity.normalized.x ) ) );
  }
  // Update is called once per frame
  void Update()
  {
    transform.position += velocity * Time.deltaTime;
    RaycastHit2D hit = Physics2D.CircleCast( transform.position, circle.radius, velocity, raycastDistance, LayerMask.GetMask( CollideLayers ) );
    if( hit.transform != null )
    {
      //print( "hit " + hit.transform.name );
      transform.position = hit.point;
      GameObject go = GameObject.Instantiate( HitEffect, transform.position, Quaternion.identity );
      SpriteAnimator sa = go.GetComponent<SpriteAnimator>();
      float duration = ( 1.0f / sa.CurrentSequence.fps ) * sa.CurrentSequence.sprites.Length;
      Timer t = new Timer( duration, null, delegate
      {
        Destroy( go );
      } );
        
      /*ParticleSystem ps = go.GetComponent<ParticleSystem>();
      Timer t = new Timer( ps.main.duration, null, delegate
      {
        Destroy( go );
      } );*/

      timeoutTimer.Stop( false );
      Destroy( gameObject );
    }
  }


}
