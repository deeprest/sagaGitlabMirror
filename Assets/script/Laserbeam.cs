﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Laserbeam : Projectile//, IDamage
{
  [SerializeField] SpriteRenderer renderer;
  public GameObject lightPrefab;
  public float lightInterval;
  [SerializeField] GameObject hitPrefab;
  GameObject hitObject;
  [SerializeField] float beamRadius = 0.2f;
  List<Light> lights = new List<Light>();
  Timer timeoutTimer;

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
  }

  void Start()
  {
    timeoutTimer = new Timer( timeout, null, delegate ()
    {
      if( gameObject != null )
        Destroy( gameObject );
    } );
    renderer.drawMode = SpriteDrawMode.Tiled;
    transform.rotation = Quaternion.LookRotation( Vector3.forward, velocity );
  }

  void Hit( Vector3 position )
  {
    if( hitPrefab != null && hitObject == null )
      hitObject = Instantiate( hitPrefab, transform.position, transform.rotation );
  }

  void FixedUpdate()
  {
    transform.rotation = Quaternion.LookRotation( Vector3.forward, velocity );
    float distance = raycastDistance;
    //transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( velocity.normalized.y, velocity.normalized.x ) ) );
    hitCount = Physics2D.CircleCastNonAlloc( transform.position, beamRadius, velocity, RaycastHits, raycastDistance, Global.DefaultProjectileCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.transform != null && !ignore.Contains( hit.transform ) && !ignore.Contains( hit.transform ) )
      {
        /*
        float f = 0;
        int i = 0;
        for(; f <= distance; f+=1.0f, i++ )
        {
          Light light = null;
          if( lights.Count <= (int)i )
          {
            GameObject go = Instantiate( lightPrefab, transform.position, Quaternion.identity, transform );
            light = go.GetComponent<Light>();
            lights.Add( light );
          }
          light.transform.position = transform.position + (Vector3)velocity * f;
        }
        for( int asdf = i; asdf < lights.Count; asdf++ )
        {
          lights.RemoveAt( lights.Count - 1 );
        }
        */
        if( instigator == null || !hit.transform.IsChildOf( instigator.transform ) )
        {
          IDamage dam = hit.transform.GetComponent<IDamage>();
          if( dam != null )
          {
            Damage dmg = Instantiate( ContactDamage );
            dmg.instigator = instigator;
            dmg.damageSource = transform;
            dmg.point = hit.point;
            if( dam.TakeDamage( dmg ) )
            {
              Hit( hit.point );
            }
          }
          else
          {
            distance = Mathf.Min( distance, Vector2.Distance( hit.point, transform.position ) );
            Hit( hit.point );
          }
        }
      }
    }
    renderer.size = new Vector2( 0.2f, distance );
  }

  //public bool TakeDamage( Damage damage )
  //{
  //  Hit( damage.point );
  //  return true;
  //}
}