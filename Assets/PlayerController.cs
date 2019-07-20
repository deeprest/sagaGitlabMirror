#define ANIM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : Character, IDamage
{
  new public AudioSource audio;
  public AudioSource audio2;
  public ParticleSystem dashSmoke;
  public Transform arm;

  // settings
  public float raydown = 0.2f;
  public float downOffset = 0.16f;

  public float downslopefudge = 0f;
  const float corner = 0.707f;

  // velocities
  public float moveVel = 2;
  public float jumpVel = 5;
  public float dashVel = 5;
  // durations
  public float jumpDuration = 0.4f;
  public float dashDuration = 1;
  public float landDuration = 0.1f;
  public float wallSlideFactor = 0.5f;
  // input / control
  public bool playerInput = true;
  public bool inputRight;
  public bool inputLeft;
  public bool inputJumpStart;
  public bool inputJumpEnd;
  public bool inputDashStart;
  public bool inputDashEnd;
  public bool inputChargeStart;
  public bool inputCharge;
  public bool inputChargeEnd;
  public bool inputGraphook;
  // state
  [SerializeField] bool facingRight = true;
  [SerializeField] bool onGround;
  [SerializeField] bool jumping;
  [SerializeField] bool landing;
  [SerializeField] bool dashing;
  public bool hanging { get; set; }

  public Vector2 push = Vector2.zero;
  float dashStart;
  float jumpStart;
  float landStart;

  string[] PlayerCollideLayers = { "Default", "triggerAndCollision" };
  string[] TriggerLayers = { "trigger", "triggerAndCollision" };

  Vector3 hitBottomNormal;

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
  public AudioClip soundDamage;

  //  public Vector2 rightFoot;
  //  public Vector2 leftFoot;

  [Header( "Damage" )]
  [SerializeField] float damageDuration = 0.5f;
  bool takingDamage;
  bool invulnerable;
  Timer damageTimer = new Timer();
  public Color damagePulseColor = Color.white;
  bool damagePulseOn;
  Timer damagePulseTimer = new Timer();
  public float damagePulseInterval = .1f;
  public float damageBlinkDuration = 1f;
  public float damageLift = 1f;
  public float damagePushAmount = 1f;

  void Start()
  {
    colliders = GetComponentsInChildren<Collider2D>();
    spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    graphookTip.SetActive( false );
    grapCableRender.gameObject.SetActive( false );
  }

  void OnDestroy()
  {
    damageTimer.Stop( false );
    damagePulseTimer.Stop( false );
    shootRepeatTimer.Stop( false );
    chargePulse.Stop( false );
    chargeSoundLoopDelay.Stop( false );
    chargeStartDelay.Stop( false );
  }


  [SerializeField] float highlightPickupRange = 3;
  Pickup closestPickup;
  //List<Pickup> highlightedPickups = new List<Pickup>();
  List<Pickup> pups = new List<Pickup>();
  //List<Pickup> highlightedPickupsRemove = new List<Pickup>();


  Component FindClosest( Vector3 position, Component[] cmps )
  {
    float distance = Mathf.Infinity;
    Component closest = null;
    foreach( var cmp in cmps )
    {
      float dist = Vector3.Distance( cmp.transform.position, position );
      if( dist < distance )
      {
        closest = cmp;
        distance = dist;
      }
    }
    return closest;
  }

  new void UpdateCollision( float dT )
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;

    RaycastHit2D[] hits;

    hits = Physics2D.BoxCastAll( transform.position, box.size, 0, velocity, Mathf.Max( raylength, velocity.magnitude * dT ), LayerMask.GetMask( TriggerLayers ) );
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

    pups.Clear();
    hits = Physics2D.CircleCastAll( transform.position, highlightPickupRange, Vector2.zero, 0, LayerMask.GetMask( new string[] { "pickup" } ) );
    foreach( var hit in hits )
    {
      Pickup pup = hit.transform.GetComponent<Pickup>();
      if( pup != null )
      {
        pups.Add( pup );
        /*if( !highlightedPickups.Contains( pup ) )
        {
          pup.Highlight();
          highlightedPickups.Add( pup );
        }*/
      }
    }
    Pickup closest = (Pickup)FindClosest( transform.position, pups.ToArray() );
    if( closest == null )
    {
      if( closestPickup != null )
      {
        closestPickup.Unhighlight();
        closestPickup = null;
      }
    }
    else if( closest != closestPickup )
    {
      if( closestPickup != null )
        closestPickup.Unhighlight();
      closestPickup = closest;
      closestPickup.Highlight();
    }
    /*highlightedPickupsRemove.Clear();
    foreach( var pup in highlightedPickups )
      if( !pups.Contains( pup ) )
      {
        pup.Unhighlight();
        highlightedPickupsRemove.Add( pup );
      }
    foreach( var pup in highlightedPickupsRemove )
      highlightedPickups.Remove( pup );
    */

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
    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.down, Mathf.Max( down, -velocity.y * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + downOffset;
        hitBottomNormal = hit.normal;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.up, Mathf.Max( raylength, velocity.y * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.left, Mathf.Max( contactSeparation, -velocity.x * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        hitLeft = hit;
        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.right, Mathf.Max( contactSeparation, velocity.x * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        hitRight = hit;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        break;
      }
    }

    transform.position = adjust;
  }

  void Shoot()
  {
    shootRepeatTimer.Start( weapon.shootInterval, null, null );
    Vector3 pos = arm.position + shoot.normalized * armRadius;
    if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Projectile.NoShootLayers ) ) )
      weapon.FireWeapon( this, pos, shoot );
  }

  [Header( "Graphook" )]
  [SerializeField] GameObject graphookTip;
  [SerializeField] SpriteRenderer grapCableRender;
  public float grapDistance = 10;
  public float grapSpeed = 5;
  public float grapTimeout = 5;
  public float grapPullSpeed = 10;
  public float grapStopDistance = 0.1f;
  Timer grapTimer = new Timer();
  Timer grapPullTimer = new Timer();
  Vector2 grapSize;
  Vector3 graphitpos;
  public bool grapShooting;
  public bool grapPulling;
  public AudioClip grapShotSound;
  public AudioClip grapHitSound;

  void ShootGraphook()
  {
    if( grapShooting )
      return;
    if( grapPulling )
      StopGrap();
    Vector3 pos = arm.position + shoot.normalized * armRadius;
    if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Projectile.NoShootLayers ) ) )
    {
      RaycastHit2D hit = Physics2D.Raycast( pos, shoot, grapDistance, LayerMask.GetMask( PlayerCollideLayers ) );
      if( hit )
      {
        //Debug.DrawLine( pos, hit.point, Color.red );
        grapShooting = true;
        graphitpos = hit.point;
        graphookTip.SetActive( true );
        graphookTip.transform.parent = null;
        graphookTip.transform.localScale = Vector3.one;
        graphookTip.transform.position = pos;
        graphookTip.transform.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, (graphitpos - transform.position) ) ); ;
        grapTimer.Start( grapTimeout, delegate
        {
          pos = arm.position + shoot.normalized * armRadius;
          graphookTip.transform.position = Vector3.MoveTowards( graphookTip.transform.position, graphitpos, grapSpeed * Time.deltaTime );
          //grap cable
          grapCableRender.gameObject.SetActive( true );
          grapCableRender.transform.parent = null;
          grapCableRender.transform.localScale = Vector3.one;
          grapCableRender.transform.position = pos;
          grapCableRender.transform.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, (graphookTip.transform.position - pos) ) );
          grapSize = grapCableRender.size;
          grapSize.x = Vector3.Distance( graphookTip.transform.position, pos );
          grapCableRender.size = grapSize;

          if( Vector3.Distance( graphookTip.transform.position, graphitpos ) < 0.01f )
          {
            grapShooting = false;
            grapPulling = true;
            grapTimer.Stop( false );
            grapTimer.Start( grapTimeout, null, StopGrap );
            audio.PlayOneShot( grapHitSound );
          }
        },
        StopGrap );
        audio.PlayOneShot( grapShotSound );
      }
    }
  }

  void StopGrap()
  {
    grapShooting = false;
    grapPulling = false;
    graphookTip.SetActive( false );
    grapCableRender.gameObject.SetActive( false );
    // avoid grapsize.y == 0 if StopGrap is called before grapSize is assigned
    grapSize.y = grapCableRender.size.y;
    grapSize.x = 0;
    grapCableRender.size = grapSize;
    grapTimer.Stop( false );
  }

  void ShootCharged()
  {
    if( chargeEffect != null )
    {
      audio.Stop();
      audio.PlayOneShot( weapon.soundChargeShot );
      if( chargeAmount > chargeMin )
      {
        Vector3 pos = arm.position + cursorDelta.normalized * armRadius;
        if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Projectile.NoShootLayers ) ) )
          weapon.FireWeaponCharged( this, pos, shoot );
      }
    }
    StopCharge();
  }

  Vector3 cursorDelta;
  Vector3 shoot;

  void UpdatePlayerInput()
  {
    cursorDelta = Global.instance.AimPosition - (Vector2)arm.position;
    cursorDelta.z = 0;
    arm.localScale = new Vector3( facingRight ? 1 : -1, 1, 1 );
    arm.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, cursorDelta ) );


    if( GameInput.UsingKeyboard )
    {
      shoot = cursorDelta;
    }
    else
    {
      shoot = new Vector3( GameInput.GetAxisRaw( "ShootX" ), -GameInput.GetAxisRaw( "ShootY" ), 0 );
      if( shoot.sqrMagnitude > GameInput.deadZone * GameInput.deadZone )
      {
        if( !shootRepeatTimer.IsActive )
        {
          Shoot();
        }
      }
      else
      {
        // todo change arm sprite
        shoot = Vector3.right;
      }
    }

    if( GameInput.GetKey( "Fire" ) )
    {
      if( !shootRepeatTimer.IsActive )
      {
        Shoot();
      }
    }

    if( GameInput.GetKeyDown( "Pickup" ) )
    {
      if( closestPickup != null )
      {
        closestPickup.Selected();
        weapon = closestPickup.weapon;
        Global.instance.weaponIcon.sprite = weapon.icon;
        Global.instance.SetCursor( weapon.cursor );
      }
    }

    if( GameInput.GetKeyUp( "graphook" ) )
      inputGraphook = true;

    if( GameInput.GetKeyDown( "Charge" ) )
      inputChargeStart = true;
    else
    if( GameInput.GetKey( "Charge" ) )
      inputCharge = true;
    else
    if( GameInput.GetKeyUp( "Charge" ) )
      inputChargeEnd = true;

    // INPUT

    if( GameInput.GetKeyUp( "Down" ) )
      hanging = false;

    inputRight = GameInput.GetKey( "MoveRight" );
    inputLeft = GameInput.GetKey( "MoveLeft" );

    if( GameInput.GetKeyDown( "Dash" ) )
      inputDashStart = true;
    else if( GameInput.GetKeyUp( "Dash" ) )
      inputDashEnd = true;

    if( GameInput.GetKeyDown( "Jump" ) )
      inputJumpStart = true;
    else
    if( GameInput.GetKeyUp( "Jump" ) )
      inputJumpEnd = true;

  }

  void ResetInput()
  {
    inputRight = false;
    inputLeft = false;
    inputJumpStart = false;
    inputJumpEnd = false;
    inputDashStart = false;
    inputDashEnd = false;
    inputChargeStart = false;
    inputCharge = false;
    inputChargeEnd = false;
    inputGraphook = false;
  }

  void Update()
  {
    if( Global.Paused )
      return;

    if( playerInput )
      UpdatePlayerInput();

    if( inputGraphook )
      ShootGraphook();

    if( inputChargeStart )
      StartCharge();
    if( inputCharge && chargeEffect != null )
      chargeAmount += Time.deltaTime;
    if( inputChargeEnd )
      ShootCharged();

    if( !takingDamage )
    {
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

      if( inputDashStart )
      {
        if( onGround || collideLeft || collideRight )
        {
          StartDash();
        }
      }
      if( inputDashEnd )
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

      if( inputJumpStart )
      {
        if( onGround || (inputRight && collideRight) || (inputLeft && collideLeft) )
        {
          StartJump();
        }
      }
      if( inputJumpEnd )
        StopJump();

    }
    if( velocity.y < 0 )
      jumping = false;

    if( jumping && (Time.time - jumpStart >= jumpDuration) )
      jumping = false;

    if( landing && (Time.time - landStart >= landDuration) )
      landing = false;

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

    if( takingDamage )
    {
      anim = "damage";
      velocity.y = 0;
    }

    if( grapPulling )
    {
      Vector3 armpos = arm.position + shoot.normalized * armRadius;
      grapCableRender.transform.position = armpos;
      grapCableRender.transform.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, (graphookTip.transform.position - armpos) ) );
      grapSize = grapCableRender.size;
      grapSize.x = Vector3.Distance( graphookTip.transform.position, armpos );
      grapCableRender.size = grapSize;

      Vector3 grapDelta = graphitpos - transform.position;
      if( grapDelta.magnitude < grapStopDistance )
        StopGrap();
      else if( grapDelta.magnitude > 0.01f )
        velocity = grapDelta.normalized * grapPullSpeed; // * Time.deltaTime;
    }
    else
    {
      velocity += push;
      // vertical push is one-time instantaneous only (so far only damage does this)
      push.y = 0;
    }

    animator.Play( anim );

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
    if( collideTop )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }

    if( hanging )
      velocity = Vector3.zero;

    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
    transform.position += (Vector3)velocity * Time.deltaTime;
    transform.localScale = new Vector3( facingRight ? 1 : -1, 1, 1 );

    if( grapPulling )
      velocity = Vector3.zero;
    else
      // must have input (or push) to move horizontally, so allow no persistent horizontal velocity
      velocity.x = 0;

    // update collision flags, and adjust position before render
    UpdateCollision( Time.deltaTime );
    //body.MovePosition( transform.position );

    bool oldGround = onGround;
    onGround = collideBottom || (collideLeft && collideRight);
    if( onGround && !oldGround )
    {
      StopDash();
      landing = true;
      landStart = Time.time;
    }

    ResetInput();

  }

  void StartJump()
  {
    jumping = true;
    jumpStart = Time.time;
    velocity.y = jumpVel;
    audio.PlayOneShot( soundJump );
    dashSmoke.Stop();
  }

  void StopJump()
  {
    jumping = false;
    velocity.y = Mathf.Min( velocity.y, 0 );
  }

  void StartDash()
  {
    if( !dashing )
    {
      dashing = true;
      dashStart = Time.time;
      if( onGround )
        audio.PlayOneShot( soundDash, 0.5f );
      dashSmoke.transform.localPosition = new Vector3( -0.38f, -0.22f, 0 );
      dashSmoke.Play();
    }
  }

  void StopDash()
  {
    dashing = false;
    dashSmoke.Stop();
  }

  void StartCharge()
  {
    chargeStartDelay.Start( chargeDelay, null, delegate
    {
      audio.PlayOneShot( weapon.soundCharge );
      audio2.clip = weapon.soundChargeLoop;
      audio2.loop = true;
      audio2.PlayScheduled( AudioSettings.dspTime + weapon.soundCharge.length );
      foreach( var sr in spriteRenderers )
        sr.material.SetColor( "_FlashColor", chargeColor );
      ChargePulseFlip();
      GameObject geffect = Instantiate( weapon.ChargeEffect, transform );
      chargeEffect = geffect.GetComponent<ParticleSystem>();
    } );
  }

  void StopCharge()
  {
    if( chargeEffect != null )
    {
      audio2.Stop();
      Destroy( chargeEffect.gameObject );
    }
    chargeStartDelay.Stop( false );
    chargePulse.Stop( false );
    foreach( var sr in spriteRenderers )
      sr.material.SetFloat( "_FlashAmount", 0 );
    chargeEffect = null;
    chargeAmount = 0;
  }

  void ChargePulseFlip()
  {
    chargePulse.Start( chargePulseInterval, null, delegate
    {
      chargePulseOn = !chargePulseOn;
      if( chargePulseOn )
        foreach( var sr in spriteRenderers )
          sr.material.SetFloat( "_FlashAmount", 0.5f );
      else
        foreach( var sr in spriteRenderers )
          sr.material.SetFloat( "_FlashAmount", 0 );
      ChargePulseFlip();
    } );
  }


  void DamagePulseFlip()
  {
    damagePulseTimer.Start( damagePulseInterval, null, delegate
    {
      damagePulseOn = !damagePulseOn;
      if( damagePulseOn )
        foreach( var sr in spriteRenderers )
          sr.enabled = true;
      //animator.material.SetFloat( "_FlashAmount", 0.5f );
      else
        foreach( var sr in spriteRenderers )
          sr.enabled = false;
      //animator.material.SetFloat( "_FlashAmount", 0 );
      DamagePulseFlip();
    } );
  }

  new public void TakeDamage( Damage d )
  {
    if( invulnerable )
      return;

    //StopCharge();
    audio.PlayOneShot( soundDamage );
    Global.instance.CameraController.GetComponent<CameraShake>().enabled = true;

    float sign = Mathf.Sign( d.instigator.position.x - transform.position.x );
    facingRight = sign > 0;
    push.y = damageLift;
    velocity.y = 0;
    arm.gameObject.SetActive( false );
    takingDamage = true;
    invulnerable = true;
    animator.Play( "damage" );
    damageTimer.Start( damageDuration, (System.Action<Timer>)delegate ( Timer t )
    {
      this.push.x = -sign * this.damagePushAmount;

    }, delegate ()
    {
      takingDamage = false;
      arm.gameObject.SetActive( true );
      //animator.material.SetFloat( "_FlashAmount", 0 );
      //animator.material.SetColor( "_FlashColor", damagePulseColor );
      DamagePulseFlip();
      damageTimer.Start( damageBlinkDuration, null, delegate ()
      {
        //animator.material.SetFloat( "_FlashAmount", 0 );
        foreach( var sr in spriteRenderers )
          sr.enabled = true;
        invulnerable = false;
        damagePulseTimer.Stop( false );
      } );
    } );

  }

  public override void PreSceneTransition()
  {
    StopCharge();
    StopGrap();
    // "pack" the graphook with this gameobject
    graphookTip.transform.parent = gameObject.transform;
    grapCableRender.transform.parent = gameObject.transform;
  }

  public override void PostSceneTransition()
  {

  }
}
