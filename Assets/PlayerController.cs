﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : Character, IDamage
{
  new public AudioSource audio;
  public AudioSource audio2;
  public SpriteAnimator animator;
  public ParticleSystem dashSmoke;
  public Transform arm;

  // settings
  public float raydown = 0.2f;
  public float downOffset = 0.16f;
  public float raylength = 0.01f;
  public float contactSeparation = 0.01f;
  public float sideraylength = 0.01f;
  public float sidecontactSeparation = 0.01f;

  public float downslopefudge = 0f;
  const float corner = 0.707f;
  public Vector2 box = new Vector2( 0.3f, 0.3f );

  // velocities
  public float moveVel = 2;
  public float jumpVel = 5;
  public float dashVel = 5;
  // durations
  public float jumpDuration = 0.4f;
  public float dashDuration = 1;
  public float landDuration = 0.1f;
  public float wallSlideFactor = 0.5f;
  // state
  public bool facingRight = true;
  public bool inputRight = false;
  public bool inputLeft = false;
  public bool inputJumpStart = false;
  public bool inputJumpEnd = false;
  public bool onGround = false;
  public bool jumping = false;
  public bool landing = false;
  public bool dashing = false;
  public bool hanging = false;

  public Vector3 velocity = Vector3.zero;
  public Vector3 push = Vector3.zero;
  public float friction = 1f;
  public float momentumTest = 2;
  float dashStart;
  float jumpStart;
  float landStart;

  string[] PlayerCollideLayers = new string[] { "Default", "triggerAndCollision" };
  string[] TriggerLayers = new string[] { "trigger", "triggerAndCollision" };
  public bool collideRight = false;
  public bool collideLeft = false;
  public bool collideHead = false;
  public bool collideFeet = false;
  RaycastHit2D hitRight;
  RaycastHit2D hitLeft;

  Vector3 hitBottomNormal;

  public Damage ContactDamage;
  public Weapon weapon;
  Timer shootRepeatTimer = new Timer();
  ParticleSystem chargeEffect = null;
  float chargeAmount = 0;
  public float chargeMin = 0.3f;
  public float armRadius = 0.3f;
  Timer chargePulse = new Timer();
  Timer chargeStartDelay = new Timer();
  Timer chargeSoundLoopDelay = new Timer();
  public float chargeDelay = 0.2f;
  public bool chargePulseOn = true;
  public float chargePulseInterval = 0.1f;
  public Color chargeColor = Color.white;

  public AudioClip soundJump;
  public AudioClip soundDash;

  //  public Vector2 rightFoot;
  //  public Vector2 leftFoot;

  void UpdateCollision( float dT )
  {
    collideRight = false;
    collideLeft = false;
    collideHead = false;
    collideFeet = false;

    RaycastHit2D[] hits;

    hits = Physics2D.BoxCastAll( transform.position, box * 2, 0, velocity, Mathf.Max( raylength, velocity.magnitude * dT ), LayerMask.GetMask( TriggerLayers ) );
    foreach( var hit in hits )
    {
      ITrigger tri = hit.transform.GetComponent<ITrigger>();
      if( tri != null )
      {
        tri.Trigger( transform );
      }
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        // maybe only damage other if charging weapon or has active powerup?
        if( ContactDamage != null )
        {
          Damage dmg = Instantiate<Damage>( ContactDamage );
          dmg.instigator = transform;
          dmg.point = hit.point;
          dam.TakeDamage( dmg );
        }
      }
    }

    Vector2 adjust = transform.position;
    /*
    // Avoid the (box-to-box) standing-on-a-corner-and-moving-means-momentarily-not-on-ground bug by 'sampling' the ground at multiple points
    RaycastHit2D right = Physics2D.Raycast( adjust + Vector2.right * box.x, Vector2.down, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), LayerMask.GetMask( PlayerCollideLayers ) );
    RaycastHit2D left = Physics2D.Raycast( adjust + Vector2.left * box.x, Vector2.down, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), LayerMask.GetMask( PlayerCollideLayers ) );
    if( right.transform != null )
      rightFoot = right.point;
    else
      rightFoot = adjust + ( Vector2.right * box.x ) + ( Vector2.down * Mathf.Max( raylength, -velocity.y * Time.deltaTime ) );
    if( left.transform != null )
      leftFoot = left.point;
    else
      leftFoot = adjust + ( Vector2.left * box.x ) + ( Vector2.down * Mathf.Max( raylength, -velocity.y * Time.deltaTime ) );

    if( right.transform != null || left.transform != null )
    {
      Vector2 across = rightFoot - leftFoot;
      Vector3 sloped = Vector3.Cross( across.normalized, Vector3.back );
      if( sloped.y > corner )
      {
        collideFeet = true;
        adjust.y = ( leftFoot + across * 0.5f ).y + contactSeparation + verticalOffset;
        hitBottomNormal = sloped;
      }
    }

    if( left.transform != null )
      Debug.DrawLine( adjust + Vector2.left * box.x, leftFoot, Color.green );
    else
      Debug.DrawLine( adjust + Vector2.left * box.x, leftFoot, Color.grey );
    if( right.transform != null )
      Debug.DrawLine( adjust + Vector2.right * box.x, rightFoot, Color.green );
    else
      Debug.DrawLine( adjust + Vector2.right * box.x, rightFoot, Color.grey );

    */

    float down = jumping ? raydown - downOffset : raydown;
    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.down, Mathf.Max( down, -velocity.y * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y > corner )
      {
        collideFeet = true;
        adjust.y = hit.point.y + box.y + downOffset;
        hitBottomNormal = hit.normal;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.up, Mathf.Max( raylength, velocity.y * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y < -corner )
      {
        collideHead = true;
        adjust.y = hit.point.y - box.y - contactSeparation;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.left, Mathf.Max( sideraylength, -velocity.x * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        hitLeft = hit;
        adjust.x = hit.point.x + box.x + sidecontactSeparation;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.right, Mathf.Max( sideraylength, velocity.x * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        hitRight = hit;
        adjust.x = hit.point.x - box.x - sidecontactSeparation;
        break;
      }
    }

    transform.position = new Vector3( adjust.x, adjust.y, transform.position.z );
  }


  void Update()
  {
    if( Global.Paused )
      return;

    Vector3 cursorDelta = Camera.main.ScreenToWorldPoint( Global.instance.cursor.anchoredPosition ) - arm.position;
    cursorDelta.z = 0;
    arm.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, cursorDelta ) );
    Vector3 shoot;
    if( Global.instance.UsingKeyboard )
    {
      shoot = cursorDelta;
    }
    else
    {
      shoot = new Vector3( Input.GetAxisRaw( Global.instance.icsCurrent.axisMap["ShootX"] ), -Input.GetAxisRaw( Global.instance.icsCurrent.axisMap["ShootY"] ), 0 );
      if( shoot.sqrMagnitude > Global.deadZone * Global.deadZone )
      {
        if( !shootRepeatTimer.IsActive )
        {
          shootRepeatTimer.Start( weapon.shootInterval, null, null );
          Vector3 pos = arm.position + shoot.normalized * armRadius;
          if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Projectile.NoShootLayers ) ) )
            weapon.FireWeapon( this, pos, shoot );
        }
      }
      else
      {
        // todo change arm sprite
      }
    }

    if( Input.GetKey( Global.instance.icsCurrent.keyMap["Fire"] ) )
    {
      if( !shootRepeatTimer.IsActive )
      {
        shootRepeatTimer.Start( weapon.shootInterval, null, null );
        Vector3 pos = arm.position + shoot.normalized * armRadius;
        if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Projectile.NoShootLayers ) ) )
          weapon.FireWeapon( this, pos, shoot );
      }
    }

    if( Input.GetKeyDown( Global.instance.icsCurrent.keyMap["Charge"] ) )
    {
      chargeStartDelay.Start( chargeDelay, null, delegate
      {
        audio.PlayOneShot( weapon.soundCharge );
        audio2.clip = weapon.soundChargeLoop;
        audio2.loop = true;
        audio2.PlayScheduled( AudioSettings.dspTime + weapon.soundCharge.length );

        animator.material.SetColor( "_BlendColor", chargeColor );
        ChargePulseFlip();
        GameObject geffect = Instantiate( weapon.ChargeEffect, transform );
        chargeEffect = geffect.GetComponent<ParticleSystem>();
      } );
    }
    else
    if( Input.GetKey( Global.instance.icsCurrent.keyMap["Charge"] ) )
    {
      // charge weapon
      if( chargeEffect != null )
      {
        chargeAmount += Time.deltaTime;
      }
    }
    else
    if( Input.GetKeyUp( Global.instance.icsCurrent.keyMap["Charge"] ) )
    {
      if( chargeEffect != null )
      {
        audio.Stop();
        audio.PlayOneShot( weapon.soundChargeShot );
        audio2.Stop();

        Destroy( chargeEffect.gameObject );
        if( chargeAmount > chargeMin )
        {
          Vector3 pos = arm.position + cursorDelta.normalized * armRadius;
          if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Projectile.NoShootLayers ) ) )
            weapon.FireWeaponCharged( this, pos, shoot );
        }
      }
      chargeStartDelay.Stop( false );
      chargePulse.Stop( false );
      animator.material.SetFloat( "_BlendAmount", 0 );
      chargeEffect = null;
      chargeAmount = 0;
    }

    // INPUT

    if( Input.GetKeyUp( Global.instance.icsCurrent.keyMap["Down"] ) )
      hanging = false;

    inputRight = Input.GetKey( Global.instance.icsCurrent.keyMap["MoveRight"] );
    if( inputRight )
    {
      // move along floor if angled downwards
      Vector3 hitnormalCross = Vector3.Cross( hitBottomNormal, Vector3.forward );
      if( onGround && hitnormalCross.y < 0 )
        // add a small downward vector for curved surfaces
        velocity = hitnormalCross * moveVel + Vector3.down * downslopefudge;
      else
        velocity.x = moveVel;
      if( !facingRight && onGround )
        StopDash();
      facingRight = true;
    }

    inputLeft = Input.GetKey( Global.instance.icsCurrent.keyMap["MoveLeft"] );
    if( inputLeft )
    {
      // move along floor if angled downwards
      Vector3 hitnormalCross = Vector3.Cross( hitBottomNormal, Vector3.back );
      if( onGround && hitnormalCross.y < 0 )
        // add a small downward vector for curved surfaces
        velocity = hitnormalCross * moveVel + Vector3.down * downslopefudge;
      else
        velocity.x = -moveVel;
      if( facingRight && onGround )
        StopDash();
      facingRight = false;
    }


    if( Input.GetKeyDown( Global.instance.icsCurrent.keyMap["Dash"] ) )
    {
      if( onGround || collideLeft || collideRight )
      {
        StartDash();
      }
    }
    else if( Input.GetKeyUp( Global.instance.icsCurrent.keyMap["Dash"] ) )
    {
      if( !jumping )
        StopDash();
    }

    if( dashing )
    {
      if( facingRight )
      {
        if( onGround || inputRight )
          velocity.x = dashVel;
        if( onGround && Time.time - dashStart >= dashDuration )
          StopDash();
      }
      else
      {
        if( onGround || inputLeft )
          velocity.x = -dashVel;
        if( onGround && Time.time - dashStart >= dashDuration )
          StopDash();
      }
    }

    if( Input.GetKeyDown( Global.instance.icsCurrent.keyMap["Jump"] ) )
    {
      if( onGround || (inputRight && collideRight) || (inputLeft && collideLeft) )
      {
        jumping = true;
        jumpStart = Time.time;
        velocity.y = jumpVel;
        audio.PlayOneShot( soundJump );
        dashSmoke.Stop();
      }
    }
    else
    if( Input.GetKeyUp( Global.instance.icsCurrent.keyMap["Jump"] ) )
    {
      jumping = false;
      velocity.y = Mathf.Min( velocity.y, 0 );
    }


    if( velocity.y < 0 )
      jumping = false;

    if( jumping && (Time.time - jumpStart >= jumpDuration) )
      jumping = false;

    if( landing && (Time.time - landStart >= landDuration) )
      landing = false;

    animator.flipX = !facingRight;

    string anim = "idle";

    if( jumping )
      anim = "jump";
    else
    if( onGround )
    {
      if( dashing )
        anim = "dash";
      else
      if( inputRight || inputLeft )
        anim = "run";
      else
      if( landing )
        anim = "land";
    }
    else
    if( !jumping )
      anim = "fall";


    if( collideRight )
    {
      if( inputRight && hitRight.normal.y >= 0 )
      {
        if( jumping )
        {
          anim = "walljump";
        }
        else
        if( inputRight && !onGround && velocity.y < 0 )
        {
          velocity.y += (-velocity.y * wallSlideFactor) * Time.deltaTime;
          anim = "wallslide";
          dashSmoke.transform.localPosition = new Vector3( 0.2f, -0.2f, 0 );
          if( !dashSmoke.isPlaying )
            dashSmoke.Play();
        }
      }
      else
      {
        dashSmoke.Stop();
      }
    }

    if( collideLeft )
    {
      if( inputLeft && hitLeft.normal.y >= 0 )
      {
        if( jumping )
        {
          anim = "walljump";
        }
        else
        if( inputLeft && !onGround && velocity.y < 0 )
        {
          velocity.y += (-velocity.y * wallSlideFactor) * Time.deltaTime;
          anim = "wallslide";
          dashSmoke.transform.localPosition = new Vector3( -0.2f, -0.2f, 0 );
          if( !dashSmoke.isPlaying )
            dashSmoke.Play();
        }
      }
      else
      {
        dashSmoke.Stop();
      }
    }

    if( takingDamage )
    {
      anim = "damage";
    }

    velocity += push;
    // vertically push is one-time instantaneous only
    push.y = 0;
    // add gravity before velocity limits
    velocity.y -= Global.Gravity * Time.deltaTime;
    // limit velocity before adding to position
    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
      push.x = Mathf.Min( push.x, 0 );
    }
    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
      push.x = Mathf.Max( push.x, 0 );
    }
    // "onGround" is not the same as "collideFeet"
    if( onGround )
    {
      velocity.y = Mathf.Max( velocity.y, 0 );
      push.x -= (push.x * friction) * Time.deltaTime;
    }
    if( collideHead )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }

    if( hanging )
      velocity = Vector3.zero;

    if( anim != animator.CurrentSequence.name )
      animator.Play( anim );

    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
    transform.position += velocity * Time.deltaTime;
    // must hold button to move horizontally, so allow no persistent horizontal velocity
    velocity.x = 0;
    // update collision flags, and adjust position before render
    UpdateCollision( Time.deltaTime );

    bool oldGround = onGround;
    onGround = collideFeet || (collideLeft && collideRight);
    if( onGround && !oldGround )
    {
      StopDash();
      landing = true;
      landStart = Time.time;
    }

  }

  void StartDash()
  {
    if( !dashing )
    {
      dashing = true;
      dashStart = Time.time;
      if( onGround )
        audio.PlayOneShot( soundDash, 0.5f );
      if( facingRight )
        dashSmoke.transform.localPosition = new Vector3( -0.36f, -0.2f, 0 );
      else
        dashSmoke.transform.localPosition = new Vector3( 0.36f, -0.2f, 0 );
      dashSmoke.Play();
    }
  }

  void StopDash()
  {
    dashing = false;
    dashSmoke.Stop();
  }

  void ChargePulseFlip()
  {
    chargePulse.Start( chargePulseInterval, null, delegate
    {
      chargePulseOn = !chargePulseOn;
      if( chargePulseOn )
        animator.material.SetFloat( "_BlendAmount", 0.5f );
      else
        animator.material.SetFloat( "_BlendAmount", 0 );
      ChargePulseFlip();
    } );
  }


  [Header( "Damage" )]
  bool takingDamage;
  bool invulnerable;
  Timer damageTimer = new Timer();
  public Color damagePulseColor = Color.white;
  bool damagePulseOn;
  Timer damagePulseTimer = new Timer();
  public float damagePulseInterval = .1f;
  public float damageBlinkDuration = 1f;
  public float damageLift = 1f;
  public float damagePush = 1f;

  void DamagePulseFlip()
  {
    damagePulseTimer.Start( damagePulseInterval, null, delegate
    {
      damagePulseOn = !damagePulseOn;
      if( damagePulseOn )
        animator.material.SetFloat( "_BlendAmount", 0.5f );
      else
        animator.material.SetFloat( "_BlendAmount", 0 );
      DamagePulseFlip();
    } );
  }

  public void TakeDamage( Damage d )
  {
    if( invulnerable )
      return;
    //print( "take damage from " + d.instigator.name );
    //Global.instance.CameraController.GetComponent<CameraShake>().enabled = true;

    float sign = Mathf.Sign( d.instigator.position.x - transform.position.x );
    facingRight = sign > 0;
    push.y = damageLift;
    velocity.y = 0;

    takingDamage = true;
    invulnerable = true;
    animator.Play( "damage" );
    damageTimer.Start( animator.CurrentSequence.GetDuration(), delegate ( Timer t )
    {
      push.x = -sign * damagePush;

    }, delegate ()
    {
      takingDamage = false;
      animator.material.SetFloat( "_BlendAmount", 0 );
      animator.material.SetColor( "_BlendColor", damagePulseColor );
      DamagePulseFlip();
      damageTimer.Start( damageBlinkDuration, null, delegate ()
      {
      animator.material.SetFloat( "_BlendAmount", 0 );
      invulnerable = false;
      damagePulseTimer.Stop( false );
    } );
    } );

  }

}
