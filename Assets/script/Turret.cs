﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Entity
{
  [Header( "Turret" )]
  public bool AutoFire;
  bool cAutoFire;
  public Vector2 AutoFireDirection;
  [SerializeField] float InitialShotRepeatDelay;
  [SerializeField] Transform cannon;
  [SerializeField] float sightRange = 6;
  [SerializeField] float small = 0.1f;
  [SerializeField] float rotspeed = 20;
  Timer shootRepeatTimer = new Timer();
  [SerializeField] Transform shotOrigin;
  [SerializeField] Weapon weapon;
  [SerializeField] Transform sightOrigin;
  [SerializeField] float sightStartRadius = 0.5f;
  Timer returnToDefaultRotationTimer = new Timer();

  [SerializeField] float min = -90;
  [SerializeField] float max = 90;
  public float maxShootAngle = 5;

  // sight, target
  Collider2D[] results = new Collider2D[8];
  [SerializeField] Entity PotentialTarget;
  Transform targetPrev;
  Timer SightPulseTimer = new Timer();

  [SerializeField] IndexedColors indexedColors;

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
    UpdateCollision = null;
    if( indexedColors != null )
    {
      Global.instance.GetTeam( TeamFlags ).color.CopyTo( indexedColors.colors, 0 );
      indexedColors.ExplicitUpdate();
    }

    SightPulseTimer.Start( int.MaxValue, 2, ( x ) =>
    {
      // reaffirm target
      PotentialTarget = null;
      int count = Physics2D.OverlapCircleNonAlloc( transform.position, sightRange, results, Global.EnemyInterestLayers );
      for( int i = 0; i < count; i++ )
      {
        Collider2D cld = results[i];
        //Character character = results[i].transform.root.GetComponentInChildren<Character>();
        Entity character = results[i].GetComponent<Entity>();
        if( character != null && IsEnemyTeam( character.TeamFlags ) )
        {
          PotentialTarget = character;
          break;
        }
      }
    }, null );

    shootRepeatTimer.Start( InitialShotRepeatDelay );
  }

  void UpdateTurret()
  {
    if( cAutoFire != AutoFire )
    {
      cAutoFire = AutoFire;
      if( AutoFire )
        AutoFireOn( AutoFireDirection );
      else
        AutoFireOff();
    }

    if( AutoFire )
    {
      Vector2 local = transform.worldToLocalMatrix.MultiplyVector( AutoFireDirection );
      // prevent cannon from rotating outside of 180 degree range 
      float angle = Mathf.Clamp( Util.NormalizeAngle( Mathf.Rad2Deg * Mathf.Atan2( local.y, local.x ) - 90 ), min, max );
      cannon.localRotation = Quaternion.Euler( 0, 0, Mathf.MoveTowardsAngle( cannon.localRotation.eulerAngles.z, angle, rotspeed * Time.deltaTime ) );
      Vector2 aim = cannon.transform.up;
      if( Vector2.Angle( AutoFireDirection, aim ) < maxShootAngle )
        if( !shootRepeatTimer.IsActive )
          Shoot( AutoFireDirection );
    }
    else if( PotentialTarget == null )
    {
      animator.Play( "idle" );
      if( !returnToDefaultRotationTimer.IsActive )
        cannon.localRotation = Quaternion.Euler( 0, 0, Mathf.MoveTowardsAngle( cannon.localRotation.eulerAngles.z, 0, rotspeed * Time.deltaTime ) );
    }
    else
    {
      Vector2 pos = cannon.position;
      Vector2 player = PotentialTarget.transform.position;
      Vector2 delta = player - pos;
      if( delta.sqrMagnitude < sightRange * sightRange )
      {
        Transform target = null;
        hitCount = Physics2D.LinecastNonAlloc( (Vector2) sightOrigin.position + delta.normalized * sightStartRadius, player, RaycastHits, Global.SightObstructionLayers );
        if( hitCount == 0 )
          target = PotentialTarget.transform;
        if( target == null )
        {
          animator.Play( "idle" );
          if( targetPrev != null )
            returnToDefaultRotationTimer.Start( 4, null, null );
          if( !returnToDefaultRotationTimer.IsActive )
            cannon.localRotation = Quaternion.Euler( 0, 0, Mathf.MoveTowardsAngle( cannon.localRotation.eulerAngles.z, 0, rotspeed * Time.deltaTime ) );
        }
        else
        {
          animator.Play( "alert" );
          Vector2 local = transform.worldToLocalMatrix.MultiplyVector( delta );
          // prevent cannon from rotating outside of 180 degree range 
          float angle = Mathf.Clamp( Util.NormalizeAngle( Mathf.Rad2Deg * Mathf.Atan2( local.y, local.x ) - 90 ), min, max );
          cannon.localRotation = Quaternion.Euler( 0, 0, Mathf.MoveTowardsAngle( cannon.localRotation.eulerAngles.z, angle, rotspeed * Time.deltaTime ) );
          Vector2 aim = cannon.transform.up;
          if( target != null && target.IsChildOf( PotentialTarget.transform ) )
          {
            if( Vector2.Angle( delta, aim ) < maxShootAngle )
              if( !shootRepeatTimer.IsActive )
                Shoot( aim );
          }
        }
        targetPrev = target;
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

  public void AutoFireOn( Vector2 direction )
  {
    AutoFire = true;
    cAutoFire = true;
    AutoFireDirection = direction;
  }

  public void AutoFireOff()
  {
    AutoFire = false;
    cAutoFire = false;
  }
}