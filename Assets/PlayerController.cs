using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour, IDamage
{
  public void TakeDamage( Damage d )
  {
    print( "take damage from " + d.instigator );
  }

  new public BoxCollider2D collider;
  new public SpriteRenderer renderer;
  new public AudioSource audio;
  public SpriteAnimator animator;
  public ParticleSystem dashSmoke;

  // settings
  public float raylength = 0.1f;
  public Vector2 box = new Vector2( 0.3f, 0.3f );
  //public Vector2 hitOffset = new Vector2( 0.3f, 0.3f );
  public float contactSeparation = 0.01f;

  // velocities
  public float MaxVelocity = 50;
  public float gravity = 16;
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
  public bool onGround = false;
  public bool jumping = false;
  public bool landing = false;
  public bool dashing = false;
  public bool hanging = false;

  public Vector3 velocity = Vector3.zero;
  public Vector3 inertia = Vector3.zero;
  public float momentumDecay = 0.5f;
  public float momentumTest = 2;
  float dashStart;
  float jumpStart;
  float landStart;

  public bool collideRight = false;
  public bool collideLeft = false;
  public bool collideHead = false;
  public bool collideFeet = false;

  string[] PlayerCollideLayers = new string[] { "foreground" };
  public Weapon weapon;
  Timer shootRepeatTimer = new Timer();
  ParticleSystem chargeEffect = null;
  float chargeAmount = 0;
  public float chargeMin = 0.3f;
  public float armRadius = 0.3f;
  Timer chargePulse = new Timer();
  Timer chargeStartDelay = new Timer();
  public float chargeDelay = 0.2f;
  public bool chargePulseOn = true;
  public float chargePulseInterval = 0.1f;
  public Color chargeColor = Color.white;
  public AudioClip soundXBusterPew;
  public AudioClip soundJump;
  public AudioClip soundDash;

  void Awake()
  {
    audio = GetComponent < AudioSource>();
    collider.size = box * 2;
  }

  RaycastHit2D hitRight;
  RaycastHit2D hitLeft;
  RaycastHit2D hitTop;
  RaycastHit2D hitBottom;

  void Update()
  {
    if( Global.Paused )
      return;

    inertia -= (inertia * momentumDecay) * Time.deltaTime;
    velocity.x = inertia.x;

    const float deadZone = 0.3f;

    if( !Global.instance.UsingKeyboard )
    {
      Vector3 shoot = new Vector3( Input.GetAxisRaw( Global.instance.icsCurrent.axisMap[ "ShootX" ] ), -Input.GetAxisRaw( Global.instance.icsCurrent.axisMap[ "ShootY" ] ), 0 );
      if( shoot.sqrMagnitude > deadZone * deadZone )
      {
        if( !shootRepeatTimer.IsActive )
        {
          shootRepeatTimer.Start( weapon.shootInterval, null, null );
          GameObject go = GameObject.Instantiate( weapon.ProjectilePrefab.gameObject, transform.position + shoot.normalized * armRadius, Quaternion.identity );
          Projectile p = go.GetComponent<Projectile>();
          p.instigator = gameObject;
          p.velocity = shoot.normalized * weapon.speed;
          Physics2D.IgnoreCollision( p.circle, collider );
        }
      }
    }
      
    if( Input.GetKeyDown( Global.instance.icsCurrent.keyMap[ "Fire" ] ) )
    {
      if( !shootRepeatTimer.IsActive )
      {
        shootRepeatTimer.Start( weapon.shootInterval, null, null );

        Vector3 pos = transform.position + Global.instance.cursorDelta.normalized * armRadius;
        Collider2D col = Physics2D.OverlapCircle( pos, weapon.ProjectilePrefab.circle.radius, LayerMask.GetMask( Projectile.NoShootLayers) );
        if( col == null )
        {
          GameObject go = GameObject.Instantiate( weapon.ProjectilePrefab.gameObject, pos, Quaternion.identity );
          Projectile p = go.GetComponent<Projectile>();
          p.instigator = gameObject;
          p.velocity = Global.instance.cursorDelta.normalized * weapon.speed;
          Physics2D.IgnoreCollision( p.circle, collider );
          audio.PlayOneShot( soundXBusterPew );
          chargeStartDelay.Start( chargeDelay, null, delegate
          {
            renderer.material.SetColor( "_BlendColor", chargeColor );
            ChargePulseFlip();
            GameObject geffect = GameObject.Instantiate( weapon.ChargeEffect, transform );
            chargeEffect = geffect.GetComponent<ParticleSystem>();
          } );
        }
      }
    }
    else
    if( Input.GetKey( Global.instance.icsCurrent.keyMap[ "Fire" ] ) )
    {
      // charge weapon
      if( chargeEffect != null )
      {
        chargeAmount += Time.deltaTime;
      }

    }
    else
    if( Input.GetKeyUp( Global.instance.icsCurrent.keyMap[ "Fire" ] ) )
    {
      if( chargeEffect != null )
      {
        Destroy( chargeEffect.gameObject );
        if( chargeAmount > chargeMin )
        {
          GameObject go = GameObject.Instantiate( weapon.ChargedProjectilePrefab.gameObject, transform.position + Global.instance.cursorDelta.normalized * armRadius, Quaternion.identity );
          Projectile p = go.GetComponent<Projectile>();
          p.instigator = gameObject;
          p.velocity = Global.instance.cursorDelta.normalized * weapon.chargedSpeed;
          Physics2D.IgnoreCollision( p.circle, collider );
        }
      }
      chargeStartDelay.Stop( false );
      chargePulse.Stop( false );
      renderer.material.SetFloat( "_BlendAmount", 0 );
      chargeEffect = null;
      chargeAmount = 0;
    }

    // INPUT
    if( Input.GetKey( Global.instance.icsCurrent.keyMap[ "MoveRight" ] ) )
    {
      inputRight = true;
      velocity.x = moveVel;
      if( !facingRight && onGround )
        dashing = false;
      facingRight = true;
    }
    else
    {
      inputRight = false;
    }

    if( Input.GetKey( Global.instance.icsCurrent.keyMap[ "MoveLeft" ] ) )
    {
      inputLeft = true;
      velocity.x = -moveVel;
      if( facingRight && onGround )
        dashing = false;
      facingRight = false;
    }
    else
    {
      inputLeft = false;
    }

    if( Input.GetKeyDown( Global.instance.icsCurrent.keyMap[ "Jump" ] ) )
    {
      if( onGround || ( inputRight && collideRight ) || ( inputLeft && collideLeft ) )
      {
        jumping = true;
        jumpStart = Time.time;
        velocity.y = jumpVel;
        audio.PlayOneShot( soundJump );
      }
    }
    else
    if( Input.GetKeyUp( Global.instance.icsCurrent.keyMap[ "Jump" ] ) )
    {
      jumping = false;
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
        
    if( Input.GetKeyDown( Global.instance.icsCurrent.keyMap[ "Dash" ] ) )
    {
      if( onGround || collideLeft || collideRight )
      {
        dashing = true;
        dashStart = Time.time;
        audio.PlayOneShot( soundDash );
      }
    }
    else
    if( Input.GetKeyUp( Global.instance.icsCurrent.keyMap[ "Dash" ] ) )
    {
      if( !jumping )
        dashing = false;
    }

    if( Input.GetKeyUp( Global.instance.icsCurrent.keyMap[ "Down" ] ) )
    {
      hanging = false;
      // TEMP tunnel test
      //velocity = Vector2.down * MaxVelocity;
//      if( hangingFromChopper )
//        momentum.x += chopperSpeed;
    }

    if( velocity.y < 0 )
      jumping = false;

    if( jumping && ( Time.time - jumpStart >= jumpDuration ) )
      jumping = false;

    if( landing && ( Time.time - landStart >= landDuration ) )
      landing = false;

    if( collideFeet && !onGround )
    {
      dashing = false;
      landing = true;
      landStart = Time.time;
    }
      
    if( dashing )
    {
      if( facingRight )
      {
        velocity.x = dashVel;
        dashSmoke.transform.localPosition = new Vector3( -0.36f, -0.2f, 0 );
      }
      else
      {
        velocity.x = -dashVel;
        dashSmoke.transform.localPosition = new Vector3( 0.36f, -0.2f, 0 );
      }
    }
      
    if( collideFeet )
      inertia.x = 0;

    if( facingRight )
      renderer.flipX = false;
    else
      renderer.flipX = true;

    string anim = "idle";
    if( jumping )
      anim = "jump";
    
    if( !onGround && !jumping )
      anim = "fall";
    
    if( landing )
      anim = "land";

    if( onGround && ( inputRight || inputLeft ) )
      anim = "run";

    if( dashing )
    {
      if( onGround )
      {
        anim = "dash";
        if( Time.time - dashStart >= dashDuration )
        {
          dashing = false;
          dashSmoke.Stop();
        }
      }
    }

    if( !hanging )
    {
      velocity.y += -gravity * Time.deltaTime;
      velocity.y = Mathf.Max( velocity.y, -MaxVelocity );
    }

    if( Input.GetKey( KeyCode.G ) )
      inertia.x = momentumTest;
     
    if( collideFeet )
    {
      onGround = true;
      velocity.y = Mathf.Max( velocity.y, 0 );
    }
    else
    {
      onGround = false;
    }

    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
      inertia.x = 0;
      if( inputRight && hitRight.normal.y >= 0 )
      {
        if( jumping )
        {
          anim = "walljump";
        }
        else
        if( inputRight && !onGround && velocity.y < 0 )
        {
          velocity.y *= wallSlideFactor;
          anim = "wallslide";
          dashSmoke.transform.localPosition = new Vector3( 0.2f, -0.2f, 0 );
        }
      }
    }

    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
      inertia.x = 0;
      if( inputLeft && hitLeft.normal.y >= 0 )
      {
        if( jumping )
        {
          anim = "walljump";
        }
        else
        if( inputLeft && !onGround && velocity.y < 0 )
        {
          velocity.y *= wallSlideFactor;
          anim = "wallslide";
          dashSmoke.transform.localPosition = new Vector3( -0.2f, -0.2f, 0 );
        }
      }
    }

    if( collideHead )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
     
   
    if( anim == "wallslide" || anim == "dash" )
      dashSmoke.Play();
    else
      dashSmoke.Stop();
    
    animator.Play( anim );

    transform.position += velocity * Time.deltaTime;

    collideRight = false;
    collideLeft = false;
    collideHead = false;
    collideFeet = false;

    RaycastHit2D[] hits;

    hits = Physics2D.BoxCastAll( transform.position, box * 2, 0, velocity, raylength, LayerMask.GetMask( "trigger" ) );
    foreach( var hit in hits )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
        dam.TakeDamage( new Damage( transform, DamageType.Generic, 1 ) );
    }

    const float corner = 0.707f;
    Vector2 adjust = transform.position;

    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.down, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y > corner )
      {
        collideFeet = true;
        hitBottom = hit;
        adjust.y = hit.point.y + box.y + contactSeparation;
        break;
      }
    }
    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.up, Mathf.Max( raylength, velocity.y * Time.deltaTime ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y < -corner )
      {
        collideHead = true;
        hitTop = hit;
        adjust.y = hit.point.y - box.y - contactSeparation;
        break;
      }
    }
    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.left, Mathf.Max( raylength, -velocity.x * Time.deltaTime ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        hitLeft = hit;
        adjust.x = hit.point.x + box.x + contactSeparation;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.right, Mathf.Max( raylength, velocity.x * Time.deltaTime ), LayerMask.GetMask( PlayerCollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        hitRight = hit;
        adjust.x = hit.point.x - box.x - contactSeparation;
        break;
      }
    }

    transform.position = adjust;
  }

  void ChargePulseFlip()
  {
    chargePulse.Start( chargePulseInterval, null, delegate
    {
      chargePulseOn = !chargePulseOn;
      if( chargePulseOn )
        renderer.material.SetFloat( "_BlendAmount", 0.5f );
      else
        renderer.material.SetFloat( "_BlendAmount", 0 );
      ChargePulseFlip();
    } );
  }
}
