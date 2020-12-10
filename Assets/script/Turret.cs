﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Entity
{
  [Header( "Turret" )]
  [SerializeField] Transform cannon;
  [SerializeField] float sightRange = 6;
  [SerializeField] float small = 0.1f;
  [SerializeField] float rotspeed = 20;
  Timer shootRepeatTimer = new Timer();
  [SerializeField] Transform shotOrigin;
  [SerializeField] Weapon weapon;
  [SerializeField] Transform sightOrigin;
  [SerializeField] float sightStartRadius = 0.5f;

  [SerializeField] float min = -90;
  [SerializeField] float max = 90;
  public float maxShootAngle = 5;

  
  // sight, target
  Collider2D[] results = new Collider2D[8];
  int LayerMaskCharacter;
  [SerializeField] Entity Target;
  Timer SightPulseTimer = new Timer();

  protected override void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    base.OnDestroy();
    SightPulseTimer.Stop( false );
  }
  
  protected override void Start()
  {
    base.Start();
    UpdateLogic = UpdateTurret;
    UpdatePosition = null;
    UpdateCollision = null;
    
    LayerMaskCharacter = LayerMask.GetMask( new string[] { "character" } );
    SightPulseTimer.Start( int.MaxValue, 3, ( x ) => {
      // reaffirm target
      Target = null;
      int count = Physics2D.OverlapCircleNonAlloc( transform.position, sightRange, results, LayerMaskCharacter );
      for( int i = 0; i < count; i++ )
      {
        Collider2D cld = results[i];
        //Character character = results[i].transform.root.GetComponentInChildren<Character>();
        Entity character = results[i].GetComponent<Entity>();
        if( character != null && IsEnemyTeam( character.Team ) )
        {
          Target = character;
          break;
        }
      }
    }, null );
  }

  void UpdateTurret()
  {
    if( Target == null )
    {
      animator.Play( "idle" );
    }
    else
    {
      Vector2 pos = cannon.position;
      Vector2 player = Target.transform.position;
      Vector2 delta = player - pos;
      if( delta.sqrMagnitude < sightRange * sightRange )
      {
        Transform target = null;
        RaycastHit2D hit = Physics2D.Linecast( (Vector2)sightOrigin.position + delta.normalized * sightStartRadius, player, Global.TurretSightLayers );
        if( hit.transform != null && hit.transform.root == Target.transform )
          target = hit.transform;
        if( target == null )
        {
          animator.Play( "idle" );
          //cannon.rotation = Quaternion.RotateTowards( cannon.rotation, transform.localToWorldMatrix.rotation, rotspeed * Time.deltaTime );
          cannon.localRotation = Quaternion.RotateTowards( cannon.localRotation, Quaternion.identity, rotspeed * Time.deltaTime );
        }
        else
        {
          animator.Play( "alert" );
          Vector2 local = transform.worldToLocalMatrix.MultiplyVector( delta );
          // prevent cannon from rotating outside of 180 degree range 
          float angle = Mathf.Clamp( Util.NormalizeAngle( Mathf.Rad2Deg * Mathf.Atan2( local.y, local.x ) - 90 ), min, max );
          cannon.localRotation = Quaternion.RotateTowards( cannon.localRotation, Quaternion.Euler( 0, 0, angle ), rotspeed * Time.deltaTime );
          //cannon.rotation = Quaternion.RotateTowards( cannon.rotation, Quaternion.Euler( 0, 0, angle ) * transform.localToWorldMatrix.rotation, rotspeed * Time.deltaTime );
          Vector2 aim = cannon.transform.up;
          if( target != null && target.IsChildOf( Target.transform ) )
          {
            if( Vector2.Angle( delta, aim ) < maxShootAngle )
              if( !shootRepeatTimer.IsActive )
                Shoot( aim );
          }
        }

      }
      else
      {
        animator.Play( "idle" );
      }
    }
  }

  void Shoot( Vector3 shoot )
  {
    shootRepeatTimer.Start( weapon.shootInterval, null, null );
    Vector3 pos = shotOrigin.position;
    //if( !Physics2D.Linecast( transform.position, pos, Global.ProjectileNoShootLayers ) )
    weapon.FireWeapon( this, pos, shoot );
  }
}
