using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
// todo brains and pawns
// todo PlayerController -> PlayerPawn
// Separate inputs from actions.
// Bind the same action functions to input delegates driven by an AI brain.

public class PawnController
{
  public Pawn pawn;
  public virtual void Initialize() { }
  public virtual void Update() { }
}

public class NewPlayerController : PawnController
{
  public override void Initialize()
  {
    // without callback context
    Global.instance.Controls.BipedActions.Fire.performed += ( obj ) => pawn.Fire_Performed();
  }
}

public class AIController : PawnController
{
  public override void Update()
  {
    // logic drives inputs

    //InputAction.CallbackContext ctx = new InputAction.CallbackContext();
    pawn.Fire_Performed( );
  }
}

// todo make Character a subclass of Pawn
public class Pawn : Character, IDamage
{
  public PawnController controller;

  new public bool TakeDamage( Damage d )
  {
    return true;
  }

  //public void Fire_Performed( InputAction.CallbackContext obj )
  public void Fire_Performed()
  {
    // shoot
  }
}

/*
// todo call Update directly from global
// todo instead of assigning animation every frame, only change when needed
// todo refactor logic: inputs, collision, velocity, state vars, timers -> state vars, anims, velocity

inputs
collision flags
logic for changing state

result of current state ->
animation state
velocity
position
collision update

state
  onground
  facingRight
  takingdamage
  aim / shoot
  jumping
  wallslide
  dash
  graphook
  shield

*/

public class PlayerController : Character, IDamage
{
  new public AudioSource audio;
  public AudioSource audio2;
  public ParticleSystem dashSmoke;
  public GameObject dashflashPrefab;
  public Transform arm;

  const float raydown = 0.2f;
  const float downOffset = 0.12f;
  // smaller head box allows for easier jump out and up onto wall from vertically-aligned ledge.
  public Vector2 headbox = new Vector2( .1f, .1f );
  const float headboxy = -0.1f;
  const float downslopefudge = 0.2f;
  const float corner = 0.707f;

  [Header( "Setting" )]
  public float speedFactorNormalized = 1;
  public float SpeedFactorNormalized
  {
    get { return speedFactorNormalized; }
    set
    {
      speedFactorNormalized = value;
      //animator.speed = value;
    }
  }
  // velocities
  public float moveVelMin = 1.5f;
  public float moveVelMax = 3;
  public float moveSpeed { get { return moveVelMin + (moveVelMax - moveVelMin) * speedFactorNormalized; } }
  public float jumpVelMin = 5;
  public float jumpVelMax = 10;
  public float jumpSpeed { get { return jumpVelMin + (jumpVelMax - jumpVelMin) * speedFactorNormalized; } }
  public float dashVelMin = 3;
  public float dashVelMax = 10;
  public float dashSpeed { get { return dashVelMin + (jumpVelMax - jumpVelMin) * speedFactorNormalized; } }
  public float wallJumpPushVelocity = 1.5f;
  public float wallJumpPushDuration = 0.1f;
  // durations
  public float jumpDuration = 0.4f;
  public float jumpRepeatInterval = 0.1f;
  public float dashDuration = 1;
  public float landDuration = 0.1f;
  public float wallSlideFactor = 0.5f;

  [Header( "Input" )]
  // input / control
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
  //public bool inputShield;
  public bool inputFire;
  Vector3 shoot;

  [Header( "State" )]
  // state
  [SerializeField] bool facingRight = true;
  [SerializeField] bool onGround;
  [SerializeField] bool jumping;
  [SerializeField] bool walljumping;
  [SerializeField] bool wallsliding;
  [SerializeField] bool landing;
  [SerializeField] bool dashing;
  public bool hanging { get; set; }
  Vector3 hitBottomNormal;
  Vector2 wallSlideNormal;
  Timer dashTimer = new Timer();
  Timer jumpTimer = new Timer();
  Timer jumpRepeatTimer = new Timer();
  Timer landTimer = new Timer();
  Timer walljumpTimer = new Timer();

  [Header( "Weapon" )]
  public Weapon weapon;
  public int CurrentWeaponIndex;
  [SerializeField] List<Weapon> weapons;
  Timer shootRepeatTimer = new Timer();
  ParticleSystem chargeEffect = null;
  public float chargeMin = 0.3f;
  public float armRadius = 0.3f;
  Timer chargePulse = new Timer();
  Timer chargeStartDelay = new Timer();
  public float chargeDelay = 0.2f;
  public bool chargePulseOn = true;
  public float chargePulseInterval = 0.1f;
  public Color chargeColor = Color.white;
  public Transform armMount;
  [SerializeField] Ability secondary;

  [Header( "Sound" )]
  public AudioClip soundJump;
  public AudioClip soundDash;
  public AudioClip soundDamage;

  [Header( "Damage" )]
  [SerializeField] float damageDuration = 0.5f;
  bool takingDamage;
  bool damagePassThrough;
  Timer damageTimer = new Timer();
  public Color damagePulseColor = Color.white;
  bool damagePulseOn;
  Timer damagePulseTimer = new Timer();
  public float damagePulseInterval = .1f;
  public float damageBlinkDuration = 1f;
  public float damageLift = 1f;
  public float damagePushAmount = 1f;

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


  void Awake()
  {
    BindControls();
  }

  void Start()
  {
    IgnoreCollideObjects.AddRange( GetComponentsInChildren<Collider2D>() );
    spriteRenderers.AddRange( GetComponentsInChildren<SpriteRenderer>() );
    graphookTip.SetActive( false );
    grapCableRender.gameObject.SetActive( false );
    if( weapons.Count > 0 )
      weapon = weapons[CurrentWeaponIndex % weapons.Count];
    // unpack
    InteractIndicator.SetActive( false );
    InteractIndicator.transform.SetParent( null );
  }

  public override void PreSceneTransition()
  {
    StopCharge();
    StopGrap();
    // "pack" the graphook with this gameobject
    // graphook is unpacked when used
    graphookTip.transform.parent = gameObject.transform;
    grapCableRender.transform.parent = gameObject.transform;
    InteractIndicator.transform.parent = gameObject.transform;
  }

  public override void PostSceneTransition()
  {
    InteractIndicator.SetActive( false );
    InteractIndicator.transform.SetParent( null );
  }

  void OnDestroy()
  {
    damageTimer.Stop( false );
    damagePulseTimer.Stop( false );
    shootRepeatTimer.Stop( false );
    if( chargeEffect != null )
    {
      audio2.Stop();
      Destroy( chargeEffectGO );
    }
    chargeStartDelay.Stop( false );
    chargePulse.Stop( false );
  }

  void AssignWeapon( Weapon wpn )
  {
    weapon = wpn;
    Global.instance.weaponIcon.sprite = weapon.icon;
    Cursor.GetComponent<SpriteRenderer>().sprite = weapon.cursor;
    StopCharge();
  }

  void NextWeapon()
  {
    CurrentWeaponIndex = (CurrentWeaponIndex + 1) % weapons.Count;
    AssignWeapon( weapons[CurrentWeaponIndex] );
  }

  [SerializeField] float selectRange = 3;
  WorldSelectable closestISelect;
  WorldSelectable WorldSelection;
  List<WorldSelectable> pups = new List<WorldSelectable>();
  //List<Pickup> highlightedPickups = new List<Pickup>();
  //List<Pickup> highlightedPickupsRemove = new List<Pickup>();

  public void UnselectWorldSelection()
  {
    if( WorldSelection != null )
    {
      WorldSelection.Unselect();
      WorldSelection = null;
    }
  }

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

  Component FindSmallestAngle( Vector3 position, Vector3 direction, Component[] cmps )
  {
    float angle = Mathf.Infinity;
    Component closest = null;
    foreach( var cmp in cmps )
    {
      float dist = Vector3.Angle( cmp.transform.position - position, direction );
      if( dist < angle )
      {
        closest = cmp;
        angle = dist;
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

    //RaycastHit2D[] hits;
    hits = Physics2D.BoxCastAll( transform.position, box.size, 0, velocity, Mathf.Max( raylength, velocity.magnitude * dT ), LayerMask.GetMask( Global.TriggerLayers ) );
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
    hits = Physics2D.CircleCastAll( transform.position, selectRange, Vector3.zero, 0, LayerMask.GetMask( new string[] { "worldselect" } ) );
    foreach( var hit in hits )
    {
      WorldSelectable pup = hit.transform.GetComponent<WorldSelectable>();
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
    //WorldSelectable closest = (WorldSelectable)FindClosest( transform.position, pups.ToArray() );
    WorldSelectable closest = (WorldSelectable)FindSmallestAngle( transform.position, shoot, pups.ToArray() );
    if( closest == null )
    {
      if( closestISelect != null )
      {
        closestISelect.Unhighlight();
        closestISelect = null;
      }
      if( WorldSelection != null )
      {
        WorldSelection.Unselect();
        WorldSelection = null;
      }
    }
    else if( closest != closestISelect )
    {
      if( closestISelect != null )
        closestISelect.Unhighlight();
      closestISelect = closest;
      closestISelect.Highlight();
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
    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.down, Mathf.Max( down, -velocity.y * dT ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + downOffset;
        hitBottomNormal = hit.normal;
        // moving platforms
        Character cha = hit.transform.GetComponent<Character>();
        if( cha != null )
          carryCharacter = cha;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust + Vector2.down * headboxy, headbox, 0, Vector2.up, Mathf.Max( raylength, velocity.y * dT ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.left, Mathf.Max( contactSeparation, -velocity.x * dT ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        hitLeft = hit;
        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        wallSlideNormal = hit.normal;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.right, Mathf.Max( contactSeparation, velocity.x * dT ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        hitRight = hit;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        wallSlideNormal = hit.normal;
        break;
      }
    }

    transform.position = adjust;
  }

  void Shoot()
  {
    if( !CanShoot || weapon == null || (weapon.HasInterval && shootRepeatTimer.IsActive) )
      return;
    if( !weapon.fullAuto )
      inputFire = false;
    if( weapon.HasInterval )
      shootRepeatTimer.Start( weapon.shootInterval, null, null );
    Vector3 pos = arm.position + shoot.normalized * armRadius;
    if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Global.ProjectileNoShootLayers ) ) )
      weapon.FireWeapon( this, pos, shoot );
  }

  void ShootCharged()
  {
    if( !CanShoot || weapon == null || weapon.ChargeVariant == null )
    {
      StopCharge();
      return;
    }
    // hack: chargeEffect is used to indicate charged state
    if( chargeEffect != null )
    {
      audio.Stop();
      if( (Time.time - chargeStart) > chargeMin )
      {
        audio.PlayOneShot( weapon.soundChargeShot );
        Vector3 pos = arm.position + shoot.normalized * armRadius;
        if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Global.ProjectileNoShootLayers ) ) )
          weapon.ChargeVariant.FireWeapon( this, pos, shoot );
      }
    }
    StopCharge();
  }

  void ShootGraphook()
  {
    if( grapShooting )
      return;
    if( grapPulling )
      StopGrap();
    Vector3 pos = arm.position + shoot.normalized * armRadius;
    if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Global.ProjectileNoShootLayers ) ) )
    {
      RaycastHit2D hit = Physics2D.Raycast( pos, shoot, grapDistance, LayerMask.GetMask( new string[] { "Default", "triggerAndCollision", "enemy" } ) );
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

  void BindControls()
  {
    Global.instance.Controls.BipedActions.Fire.started += ( obj ) => inputFire = true;
    Global.instance.Controls.BipedActions.Fire.canceled += ( obj ) => inputFire = false;
    Global.instance.Controls.BipedActions.Jump.started += ( obj ) => inputJumpStart = true;
    Global.instance.Controls.BipedActions.Jump.canceled += ( obj ) => inputJumpEnd = true;
    Global.instance.Controls.BipedActions.Dash.started += ( obj ) => inputDashStart = true;
    Global.instance.Controls.BipedActions.Dash.canceled += ( obj ) => inputDashEnd = true;
    //Global.instance.Controls.BipedActions.Shield.started += ( obj ) => inputShield = true;
    //Global.instance.Controls.BipedActions.Shield.canceled += ( obj ) => inputShield = false;
    Global.instance.Controls.BipedActions.Graphook.performed += ( obj ) => inputGraphook = true; ;
    Global.instance.Controls.BipedActions.NextWeapon.performed += ( obj ) => NextWeapon();
    Global.instance.Controls.BipedActions.Charge.started += ( obj ) => inputChargeStart = true;
    Global.instance.Controls.BipedActions.Charge.canceled += ( obj ) => inputChargeEnd = true;
    Global.instance.Controls.BipedActions.Down.performed += ( obj ) => hanging = false;

    Global.instance.Controls.BipedActions.Interact.performed += ( obj ) => {
      if( WorldSelection != null && WorldSelection != closestISelect )
        WorldSelection.Unselect();
      WorldSelection = closestISelect;
      if( WorldSelection != null )
      {
        WorldSelection.Select();
        if( WorldSelection is Pickup )
        {
          Pickup pickup = (Pickup)closestISelect;
          if( pickup.weapon != null )
          {
            if( !weapons.Contains( pickup.weapon ) )
            {
              weapons.Add( pickup.weapon );
              AssignWeapon( pickup.weapon );
            }
          }
          else if( pickup.ability != null )
          {
            secondary = pickup.ability;
            secondary.Activate( this );
            Destroy( pickup.gameObject );
          }
        }
      }
    };
  }

  void ResetInput()
  {
    //inputFire = false;
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

    // INPUTS
    shoot = AimPosition - (Vector2)arm.position;
    shoot.z = 0;

    // if player controlled
    if( Global.instance.Controls.BipedActions.MoveRight.ReadValue<float>() > 0.5f )
      inputRight = true;
    if( Global.instance.Controls.BipedActions.MoveLeft.ReadValue<float>() > 0.5f )
      inputLeft = true;


    string anim = "idle";
    bool previousGround = onGround;
    onGround = collideBottom || (collideLeft && collideRight);
    if( onGround && !previousGround )
    {
      landing = true;
      landTimer.Start( landDuration, null, delegate { landing = false; } );
    }
    wallsliding = false;
    // must have input (or push) to move horizontally, so allow no persistent horizontal velocity (without push)
    if( grapPulling )
      velocity = Vector3.zero;
    else if( !(inputRight || inputLeft) )
      velocity.x = 0;

    //if( carryCharacter != null )
    //  velocity = carryCharacter.velocity;
    //carryCharacter = null;

    // WEAPONS / ABILITIES
    if( inputFire )
      Shoot();

    if( inputGraphook )
      ShootGraphook();

    //Shield.SetActive( inputShield );

    if( inputChargeStart )
      StartCharge();

    if( inputChargeEnd )
      ShootCharged();

    if( takingDamage )
    {
      velocity.y = 0;
    }
    else
    {
      if( inputRight )
      {
        facingRight = true;
        // move along floor if angled downwards
        Vector3 hitnormalCross = Vector3.Cross( hitBottomNormal, Vector3.forward );
        if( onGround && hitnormalCross.y < 0 )
          // add a small downward vector for curved surfaces
          velocity = hitnormalCross * moveSpeed + Vector3.down * downslopefudge;
        else
          velocity.x = moveSpeed;
        if( !facingRight && onGround )
          StopDash();
      }

      if( inputLeft )
      {
        facingRight = false;
        // move along floor if angled downwards
        Vector3 hitnormalCross = Vector3.Cross( hitBottomNormal, Vector3.back );
        if( onGround && hitnormalCross.y < 0 )
          // add a small downward vector for curved surfaces
          velocity = hitnormalCross * moveSpeed + Vector3.down * downslopefudge;
        else
          velocity.x = -moveSpeed;
        if( facingRight && onGround )
          StopDash();
      }

      if( inputDashStart && (onGround || collideLeft || collideRight) )
        StartDash();

      if( inputDashEnd && !jumping )
        StopDash();

      if( dashing )
      {
        if( onGround && !previousGround )
        {
          StopDash();
        }
        else if( facingRight )
        {
          if( onGround || inputRight )
            velocity.x = dashSpeed;
          if( (onGround || collideRight) && !dashTimer.IsActive )
            StopDash();
        }
        else
        {
          if( onGround || inputLeft )
            velocity.x = -dashSpeed;
          if( (onGround || collideLeft) && !dashTimer.IsActive )
            StopDash();
        }
      }

      if( onGround && inputJumpStart )
        StartJump();
      else
      if( inputJumpEnd )
        StopJump();
      else
      if( collideRight && inputRight && hitRight.normal.y >= 0 )
      {
        if( inputJumpStart )
        {
          walljumping = true;
          velocity.y = jumpSpeed;
          Push( Vector2.left * (inputDashStart ? dashSpeed : wallJumpPushVelocity), wallJumpPushDuration );
          jumpRepeatTimer.Start( jumpRepeatInterval );
          walljumpTimer.Start( wallJumpPushDuration, null, delegate { walljumping = false; } );
        }
        else if( !jumping && !walljumping && !onGround && velocity.y < 0 )
        {
          velocity.y += (-velocity.y * wallSlideFactor) * Time.deltaTime;
          wallsliding = true;
          dashSmoke.transform.localPosition = new Vector3( 0.2f, -0.2f, 0 );
          if( !dashSmoke.isPlaying )
            dashSmoke.Play();
        }
      }
      else if( collideLeft && inputLeft && hitLeft.normal.y >= 0 )
      {
        if( inputJumpStart )
        {
          walljumping = true;
          velocity.y = jumpSpeed;
          Push( Vector2.right * (inputDashStart ? dashSpeed : wallJumpPushVelocity), wallJumpPushDuration );
          jumpRepeatTimer.Start( jumpRepeatInterval );
          walljumpTimer.Start( wallJumpPushDuration, null, delegate { walljumping = false; } );
        }
        else if( !jumping && !walljumping && !onGround && velocity.y < 0 )
        {
          velocity.y += (-velocity.y * wallSlideFactor) * Time.deltaTime;
          wallsliding = true;
          dashSmoke.transform.localPosition = new Vector3( 0.2f, -0.2f, 0 );
          if( !dashSmoke.isPlaying )
            dashSmoke.Play();
        }
      }

    }

    if( velocity.y < 0 )
    {
      jumping = false;
      walljumping = false;
      walljumpTimer.Stop( false );
    }

    if( !((onGround && dashing) || wallsliding) )
      dashSmoke.Stop();

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
        velocity = grapDelta.normalized * grapPullSpeed;
    }

    // add gravity before velocity limits
    velocity.y -= Global.Gravity * Time.deltaTime;
    // grap force
    if( !grapPulling && pushTimer.IsActive )
      velocity.x = pushVelocity.x;

    if( hanging )
      velocity = Vector3.zero;

    // limit velocity before adding to position
    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
      pushVelocity.x = Mathf.Min( pushVelocity.x, 0 );
    }
    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
      pushVelocity.x = Mathf.Max( pushVelocity.x, 0 );
    }
    // "onGround" is not the same as "collideFeet"
    if( onGround )
    {
      velocity.y = Mathf.Max( velocity.y, 0 );
      pushVelocity.x -= (pushVelocity.x * friction) * Time.deltaTime;
    }
    if( collideTop )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );

    transform.position += (Vector3)Velocity * Time.deltaTime;
    carryCharacter = null;
    // update collision flags, and adjust position before render
    UpdateCollision( Time.deltaTime );

    if( takingDamage )
      anim = "damage";
    else if( walljumping )
      anim = "walljump";
    else if( jumping )
      anim = "jump";
    else if( wallsliding )
      anim = "wallslide";
    else if( onGround )
    {
      if( dashing )
        anim = "dash";
      else if( inputRight || inputLeft )
        anim = "run";
      else if( landing )
        anim = "land";
      else
        anim = "idle";
    }
    else if( !jumping )
      anim = "fall";

    animator.Play( anim );
    transform.localScale = new Vector3( facingRight ? 1 : -1, 1, 1 );
    renderer.material.SetInt( "_FlipX", facingRight ? 0 : 1 );
    arm.localScale = new Vector3( facingRight ? 1 : -1, 1, 1 );
    arm.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, shoot ) );
    if( wallsliding && collideRight )
      transform.rotation = Quaternion.LookRotation( Vector3.forward, new Vector3( wallSlideNormal.y, -wallSlideNormal.x ) );
    else if( wallsliding && collideLeft )
      transform.rotation = Quaternion.LookRotation( Vector3.forward, new Vector3( -wallSlideNormal.y, wallSlideNormal.x ) );
    else
      transform.rotation = Quaternion.Euler( 0, 0, 0 );

    ResetInput();
  }


  [Header( "Cursor" )]
  public GameObject InteractIndicator;
  [SerializeField] Transform Cursor;
  public Transform CursorSnapped;
  public Transform CursorAutoAim;
  Vector2 AimPosition;
  public Vector2 CursorWorldPosition;
  public float CursorScale = 2;
  public float SnapAngleDivide = 8;
  public float SnapCursorDistance = 1;
  public float AutoAimCircleRadius = 1;
  public float AutoAimDistance = 5;
  public float DirectionalMinimum = 0.3f;
  public float DirectionalCursorDistance = 3;

  bool CanShoot { get { return true; } }// CursorAboveMinimumDistance; } }
  Vector2 cursorDelta;
  Vector2 cursorOrigin;
  bool CursorAboveMinimumDistance;

  void LateUpdate()
  {
    if( !Global.instance.Updating )
      return;

#if UNITY_WEBGL && !UNITY_EDITOR
      Vector2 delta = Controls.BipedActions.Aim.ReadValue<Vector2>() * CursorSensitivity;
      delta.y = -delta.y;
      if( UsingGamepad )
        cursorDelta = delta * DirectionalCursorDistance;
      else
        cursorDelta += delta;
#else
    if( Global.instance.UsingGamepad )
    {
      cursorDelta = Global.instance.Controls.BipedActions.Aim.ReadValue<Vector2>() * DirectionalCursorDistance;
    }
    else
    {
      cursorDelta += Global.instance.Controls.BipedActions.Aim.ReadValue<Vector2>() * Global.instance.CursorSensitivity;
      cursorDelta = cursorDelta.normalized * Mathf.Max( Mathf.Min( cursorDelta.magnitude, Camera.main.orthographicSize * Camera.main.aspect * Global.instance.CursorOuter ), 0.1f );
    }
#endif
    cursorOrigin = arm.position;
    CursorAboveMinimumDistance = cursorDelta.magnitude > DirectionalMinimum;
    Cursor.gameObject.SetActive( CursorAboveMinimumDistance );

    if( Global.instance.AimSnap )
    {
      if( CursorAboveMinimumDistance )
      {
        // set cursor
        CursorSnapped.gameObject.SetActive( true );
        float angle = Mathf.Atan2( cursorDelta.x, cursorDelta.y ) / Mathf.PI;
        float snap = Mathf.Round( angle * SnapAngleDivide ) / SnapAngleDivide;
        Vector2 snapped = new Vector2( Mathf.Sin( snap * Mathf.PI ), Mathf.Cos( snap * Mathf.PI ) );
        AimPosition = cursorOrigin + snapped * SnapCursorDistance;
        CursorSnapped.position = AimPosition;
        CursorSnapped.rotation = Quaternion.LookRotation( Vector3.forward, snapped );
        CursorWorldPosition = cursorOrigin + cursorDelta;
      }
      else
      {
        CursorSnapped.gameObject.SetActive( false );
      }
    }
    else
    {
      CursorSnapped.gameObject.SetActive( false );
      if( CursorAboveMinimumDistance )
        AimPosition = cursorOrigin + cursorDelta;
      else
        AimPosition = cursorOrigin + (facingRight^wallsliding ? Vector2.right : Vector2.left);
      CursorWorldPosition = AimPosition;
    }

    if( Global.instance.AutoAim )
    {
      CursorWorldPosition = cursorOrigin + cursorDelta;
      RaycastHit2D[] hits = Physics2D.CircleCastAll( transform.position, AutoAimCircleRadius, cursorDelta, AutoAimDistance, LayerMask.GetMask( new string[] { "enemy" } ) );
      float distance = Mathf.Infinity;
      Transform closest = null;
      foreach( var hit in hits )
      {
        float dist = Vector2.Distance( CursorWorldPosition, hit.transform.position );
        if( dist < distance )
        {
          closest = hit.transform;
          distance = dist;
        }
      }

      if( closest == null )
      {
        CursorAutoAim.gameObject.SetActive( false );
      }
      else
      {
        CursorAutoAim.gameObject.SetActive( true );
        // todo adjust for flight path
        //Rigidbody2D body = CurrentPlayer.weapon.ProjectilePrefab.GetComponent<Rigidbody2D>();
        AimPosition = closest.position;
        CursorAutoAim.position = AimPosition;
      }
    }
    else
    {
      CursorAutoAim.gameObject.SetActive( false );
    }

    Cursor.position = CursorWorldPosition;
  }

  void StartJump()
  {
    jumping = true;
    jumpTimer.Start( jumpDuration, null, StopJump );
    jumpRepeatTimer.Start( jumpRepeatInterval );
    velocity.y = jumpSpeed;
    audio.PlayOneShot( soundJump );
    dashSmoke.Stop();
  }

  void StopJump()
  {
    jumping = false;
    velocity.y = Mathf.Min( velocity.y, 0 );
    jumpTimer.Stop( false );
  }

  void StartDash()
  {
    if( !dashing )
    {
      dashing = true;
      dashTimer.Start( dashDuration );
      if( onGround )
        audio.PlayOneShot( soundDash, 0.5f );
      dashSmoke.transform.localPosition = new Vector3( -0.38f, -0.22f, 0 );
      dashSmoke.Play();
      if( onGround )
      {
        GameObject go = Instantiate( dashflashPrefab, transform.position + new Vector3( facingRight ? -0.25f : 0.25f, -0.25f, 0 ), Quaternion.identity );
        go.transform.localScale = facingRight ? Vector3.one : new Vector3( -1, 1, 1 );
      }
    }
  }

  void StopDash()
  {
    dashing = false;
    dashSmoke.Stop();
    dashTimer.Stop( false );
  }

  float chargeStart;
  GameObject chargeEffectGO;

  void StartCharge()
  {
    if( weapon == null )
      return;
    if( weapon.ChargeVariant != null )
    {
      chargeStartDelay.Start( chargeDelay, null, delegate
      {
        audio.PlayOneShot( weapon.soundCharge );
        audio2.clip = weapon.soundChargeLoop;
        audio2.loop = true;
        audio2.PlayScheduled( AudioSettings.dspTime + weapon.soundCharge.length );
        foreach( var sr in spriteRenderers )
          sr.color = chargeColor;
          //sr.material.SetColor( "_FlashColor", chargeColor );
        ChargePulseFlip();
        if( chargeEffectGO != null )
          Destroy( chargeEffectGO );
        chargeEffectGO = Instantiate( weapon.ChargeEffect, transform );
        chargeEffect = chargeEffectGO.GetComponent<ParticleSystem>();
        chargeStart = Time.time;
      } );
    }
  }

  void StopCharge()
  {
    if( chargeEffect != null )
    {
      audio2.Stop();
      Destroy( chargeEffectGO );
    }
    chargeStartDelay.Stop( false );
    chargePulse.Stop( false );
    foreach( var sr in spriteRenderers )
      sr.material.SetFloat( "_FlashAmount", 0 );
    chargeEffect = null;
  }

  void ChargePulseFlip()
  {
    chargePulse.Start( chargePulseInterval, null, delegate
    {
      chargePulseOn = !chargePulseOn;
      if( chargePulseOn )
        foreach( var sr in spriteRenderers )
          sr.material.SetFloat( "_FlashAmount", 1 );
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

  public override bool TakeDamage( Damage d )
  {
    if( !CanTakeDamage || damagePassThrough )
      return false;

    //StopCharge();
    audio.PlayOneShot( soundDamage );
    Global.instance.CameraController.GetComponent<CameraShake>().enabled = true;

    float sign = Mathf.Sign( d.instigator.position.x - transform.position.x );
    facingRight = sign > 0;
    //push.y = damageLift;
    velocity.y = 0;
    arm.gameObject.SetActive( false );
    takingDamage = true;
    damagePassThrough = true;
    animator.Play( "damage" );
    Push( new Vector2( -sign * damagePushAmount, damageLift ), damageDuration );
    StopGrap();
    damageTimer.Start( damageDuration, delegate ( Timer t )
    {
      //push.x = -sign * damagePushAmount;
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
        damagePassThrough = false;
        damagePulseTimer.Stop( false );
      } );
    } );
    return true;
  }



}
