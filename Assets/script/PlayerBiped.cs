//#define PICKLE
//#define BODY_PART_HUNT

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBiped : Pawn
{
  [Header( "PlayerBiped" )]

  /*public float slidingOffset = 1;
  public float slidingOffsetTarget = 1;
  public float slidingOffsetRate = 4;*/
  //[SerializeField] private float DownOffset = BipedDownOffset;

#if BODY_PART_HUNT
  const float BipedDownOffset = 0.1f;
  public float SpiderDownOffset = 0.1f;
#endif

  public float Scale = 1;
  // smaller head box allows for easier jump out and up onto an overhead wall when standing on a ledge.
  public Vector2 headbox = new Vector2( .1f, .1f );
  public float headboxy = -0.1f;
  const float downslopefudge = 0.2f;
  private const float corner = 0.707106769f;
  const float bottomCornerNormalY = 0.65f;
  const float sideCornerNormalX = 0.707f;
  
  Vector2 shoot;

  [Header( "State" )]
  [SerializeField] bool facingRight = true;

  [SerializeField] bool onGround;
  [SerializeField] bool jumping;
  [SerializeField] bool walljumping;
  [SerializeField] bool wallsliding;
  [SerializeField] bool landing;
  [SerializeField] bool dashing;

  Vector3 hitBottomNormal;
  Vector2 wallSlideNormal;
  private Vector2 wallSlideTargetNormal;
  Timer dashTimer = new Timer();
  Timer jumpTimer = new Timer();
  Timer jumpRepeatTimer = new Timer();
  Timer landTimer = new Timer();
  Timer walljumpTimer = new Timer();


  [Header( "DEV" )]
  public float MoveScale = 0.5f;
  public float JumpScale = 0.5f;
  public AnimationCurve OrthoScale;
  
  [Header( "Setting" )]
  public float spiderMoveSpeed = 5;
  public float speedFactorNormalized = 1;

  // movement
  public float moveVelMin = 1.5f;
  public float moveVelMax = 3;
  // PICKLE
  public float moveSpeed
  {
    get
    {
      return (moveVelMin + (moveVelMax - moveVelMin) * speedFactorNormalized)
#if PICKLE
      * 1f + (Scale - 1f) * MoveScale; 
#endif
      ; }
  }

  public float jumpVelMin = 5;
  public float jumpVelMax = 10;
  // PICKLE
  public float jumpSpeed
  {
    get { return (jumpVelMin + (jumpVelMax - jumpVelMin) * speedFactorNormalized) 
#if PICKLE 
        * 1f + (Scale - 1f) * JumpScale;
#endif
      ; }
  }

  public float jumpDuration = 0.4f;
  public float jumpRepeatInterval = 0.1f;
  public float dashVelMin = 3;
  public float dashVelMax = 10;
  // PICKLE
  public float dashSpeed
  {
    get { return (dashVelMin + (jumpVelMax - jumpVelMin) * speedFactorNormalized) 
#if PICKLE 
* 1f + (Scale - 1f) * JumpScale; 
#endif
      ; }
  }

  public float dashDuration = 1;
  public float wallJumpPushVelocity = 1.5f;
  public float wallJumpPushDuration = 0.1f;
  public float wallSlideFactor = 0.5f;
  public float wallSlideRotateSpeed = 1;
  public float wallSlideDownSpeed = 1;
  public float landDuration = 0.1f;


  public ParticleSystem dashSmoke;
  public GameObject dashflashPrefab;
  public GameObject walljumpEffect;
  [SerializeField] Transform WallkickPosition;
  public Transform arm;
  

  [Header( "Cursor" )]
  [SerializeField] Transform Cursor;
  public Transform CursorSnapped;
  public Transform CursorAutoAim;
  public float CursorScale = 2;
  public float SnapAngleDivide = 8;
  public float SnapCursorDistance = 1;
  public float AutoAimCircleRadius = 1;
  public float AutoAimDistance = 5;
  public float DirectionalMinimum = 0.3f;
  bool CursorAboveMinimumDistance;

  // show aim path
  [SerializeField] LineRenderer lineRenderer;

  [Header( "Weapon" )]
  public Weapon weapon;
  public int CurrentWeaponIndex;
  [SerializeField] List<Weapon> weapons;

  Timer shootRepeatTimer = new Timer();

  // charged shots
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
  float chargeStart;
  GameObject chargeEffectGO;

  [Header( "Ability" )]
  [SerializeField] Ability ability;

  [SerializeField] List<Ability> abilities;
  public int CurrentAbilityIndex;

  [Header( "Sound" )]
  public AudioSource audio;
  public AudioSource audio2;
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
  const float damageLift = 0;
  public float damagePushAmount = 1f;
  [SerializeField] private AnimationCurve damageShakeCurve;
  public ParticleSystem damageSmoke;

  // cached for optimization
  int HitLayers;
  RaycastHit2D hitRight;
  RaycastHit2D hitLeft;

  // World Selection
  [SerializeField] float selectRange = 3;
  IWorldSelectable closestISelect;
  IWorldSelectable WorldSelection;
  List<Component> pups = new List<Component>();

  [SerializeField] GameObject SpiderbotPrefab;
  public SpiderPawn spider;

  public bool IsBiped { get { return partLegs.enabled;  } }
  public bool grapPulling
  {
    get { return ability != null && ability is GraphookAbility && ability.IsActive;  }
  }

  protected override void Awake()
  {
    // do not add to the Limit
    // EntityAwake();
    base.Awake();
    InitializeParts();
  }

  protected override void Start()
  {
#if BODY_PART_HUNT    
    DownOffset = BipedDownOffset;
#endif
    HitLayers = Global.TriggerLayers | Global.CharacterDamageLayers;
    IgnoreCollideObjects.AddRange( GetComponentsInChildren<Collider2D>() );
    spriteRenderers.AddRange( GetComponentsInChildren<SpriteRenderer>() );
    if( weapons.Count > 0 )
      weapon = weapons[CurrentWeaponIndex % weapons.Count];
    // unpack
    InteractIndicator.SetActive( false );
    InteractIndicator.transform.SetParent( null );
  }

  public override void OnControllerAssigned()
  {
    // settings are read before player is created, so set player settings here.
    speedFactorNormalized = Global.instance.FloatSetting["PlayerSpeedFactor"].Value;
    controller.CursorInfluence = Global.instance.BoolSetting["CursorInfluence"].Value;
  }

  public override void PreSceneTransition()
  {
    velocity = Vector2.zero;
    StopCharge();
    InteractIndicator.transform.SetParent( gameObject.transform );
    if( ability != null )
      ability.PreSceneTransition();
    WorldSelection = null;
    closestISelect = null;
  }

  public override void PostSceneTransition()
  {
    InteractIndicator.SetActive( false );
    InteractIndicator.transform.SetParent( null );
    if( ability != null )
      ability.PostSceneTransition();
  }

  protected override void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    base.OnDestroy();
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

  bool AddWeapon( Weapon wpn )
  {
    if( weapons.Contains( wpn ) )
      return false;
    weapons.Add( wpn );
    CurrentWeaponIndex = weapons.Count - 1;
    AssignWeapon( weapons[ CurrentWeaponIndex ] );
    return true;
  }
  
  void AssignWeapon( Weapon wpn )
  {
    weapon = wpn;
    Global.instance.weaponIcon.sprite = weapon.icon;
    Cursor.GetComponent<SpriteRenderer>().sprite = weapon.cursor;
    StopCharge();
    foreach( var sr in spriteRenderers )
    {
      sr.material.SetColor( "_IndexColor", weapon.Color0 );
      sr.material.SetColor( "_IndexColor2", weapon.Color1 );
    }
  }

  void NextWeapon()
  {
    if( weapons.Count == 0 )
      return;
    CurrentWeaponIndex = (CurrentWeaponIndex + 1) % weapons.Count;
    AssignWeapon( weapons[CurrentWeaponIndex] );
  }

  bool AddAbility( Ability alt )
  {
    if( abilities.Contains( alt ) )
      return false;
    abilities.Add( alt );
    alt.OnAcquire( this );
    CurrentAbilityIndex = abilities.Count - 1;
    AssignAbility( abilities[ CurrentAbilityIndex ] );
    return true;
  }
  
  void AssignAbility( Ability alt )
  {
    if( ability != null )
      ability.Unequip();
    ability = alt;
    ability.Equip( armMount );
    Global.instance.abilityIcon.sprite = ability.icon;
    if( ability.cursor != null )
      Cursor.GetComponent<SpriteRenderer>().sprite = ability.cursor;
  }

  void NextAbility()
  {
    if( abilities.Count == 0 )
      return;
    CurrentAbilityIndex = (CurrentAbilityIndex + 1) % abilities.Count;
    AssignAbility( abilities[CurrentAbilityIndex] );
  }

  public override void UnselectWorldSelection()
  {
    if( WorldSelection != null )
    {
      WorldSelection.Unselect();
      WorldSelection = null;
    }
  }

  private Vector2 pos;

  new void UpdateHit( float dT )
  {
    pups.Clear();
    hitCount = Physics2D.BoxCastNonAlloc( pos, box.size, 0, velocity, RaycastHits, Mathf.Max( raylength, velocity.magnitude * dT ), HitLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.transform.IsChildOf( transform ) )
        continue;
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
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = this;
          dmg.damageSource = transform;
          dmg.point = hit.point;
          dam.TakeDamage( dmg );
        }
      }
    }

    pups.Clear();
    hitCount = Physics2D.CircleCastNonAlloc( transform.position, selectRange, Vector3.zero, RaycastHits, 0, Global.WorldSelectableLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      IWorldSelectable pup = hit.transform.GetComponent<IWorldSelectable>();
      if( pup != null )
      {
        pups.Add( (Component) pup );
        /*if( !highlightedPickups.Contains( pup ) )
        {
          pup.Highlight();
          highlightedPickups.Add( pup );
        }*/
      }
    }
    //WorldSelectable closest = (WorldSelectable)FindClosest( transform.position, pups.ToArray() );
    IWorldSelectable closest = (IWorldSelectable) Util.FindSmallestAngle( transform.position, shoot, pups.ToArray() );
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
  }


  new void UpdateCollision( float dT )
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    adjust = pos;

#region multisampleAttempt

    /*
    // Avoid the (box-to-box) standing-on-a-corner-and-moving-means-momentarily-not-on-ground bug by 'sampling' the ground at multiple points
    RaycastHit2D right = Physics2D.Raycast( adjust + Vector2.right * box.x, Vector2.down, Mathf.Max( raylength, -velocity.y * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    RaycastHit2D left = Physics2D.Raycast( adjust + Vector2.left * box.x, Vector2.down, Mathf.Max( raylength, -velocity.y * dT ), LayerMask.GetMask( PlayerCollideLayers ) );
    if( right.transform != null )
      rightFoot = right.point;
    else
      rightFoot = adjust + ( Vector2.right * box.x ) + ( Vector2.down * Mathf.Max( raylength, -velocity.y * dT ) );
    if( left.transform != null )
      leftFoot = left.point;
    else
      leftFoot = adjust + ( Vector2.left * box.x ) + ( Vector2.down * Mathf.Max( raylength, -velocity.y * dT ) );

    if( right.transform != null || left.transform != null )
    {
      Vector2 across = rightFoot - leftFoot;
      Vector3 sloped = Vector3.Cross( across.normalized, Vector3.back );
      if( sloped.y > corner )
      {
        collideBottom = true;
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

#endregion
    
    // slidingOffsetTarget = 1;
    string temp = "";
    //float down = (jumping || walljumping) ? contactSeparation : DownOffset + contactSeparation + raylength;
    //float down = (jumping || walljumping) ? 0 : DownOffset;
    
    const float raydown = 0.2f;
    const float downOffset = 0.12f;
    float down = jumping ? raydown - downOffset : raydown;
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size * Scale, 0, Vector2.down, RaycastHits, Mathf.Max( down, -velocity.y * dT ), Global.CharacterCollideLayers );
    temp += "down: " + hitCount + " ";
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y > bottomCornerNormalY )
      {
        collideBottom = true;
        /*
        // sliding offset. This moves the player downward a little bit on slopes to
        // close the gap created by the corners of the box during boxcast.
        if( Mathf.Abs( hit.normal.x ) > 0 )
        {
          slidingOffsetTarget = 1 - Mathf.Abs( hit.normal.x ) / hit.normal.y;
          slidingOffset = Mathf.MoveTowards( slidingOffset, slidingOffsetTarget, slidingOffsetRate * dT );
        }
        else
        {
          slidingOffset = 1;
        }
        adjust.y = hit.point.y + box.size.y * 0.5f + slidingOffset * downOffset;
        */
        adjust.y = hit.point.y + box.size.y * 0.5f * Scale + downOffset;

        hitBottomNormal = hit.normal;
        // moving platforms
        Entity cha = hit.transform.GetComponent<Entity>();
        if( cha != null )
        {
#if UNITY_EDITOR
          if( cha.GetInstanceID() == GetInstanceID() )
          {
            Debug.LogError( "character set itself as carry character", gameObject );
            Debug.Break();
          }
#endif
          carryCharacter = cha;
        }
        break;
      }
      /*
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        hitLeft = hit;
        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        wallSlideNormal = hit.normal;
        //break;
      }
      
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        hitRight = hit;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        wallSlideNormal = hit.normal;
        // break;
      }
      
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        // break;
      }
      */
    }

    hitCount = Physics2D.BoxCastNonAlloc( adjust + Vector2.up * headboxy, headbox, 0, Vector2.up, RaycastHits, Mathf.Max( raylength, velocity.y * dT ), Global.CharacterCollideLayers );
    temp += "up: " + hitCount + " ";
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        break;
      }
    }

    // prefer more-horizontal walls for wall sliding
    // float prefer = 2;
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.left, RaycastHits, Mathf.Max( raylength, -velocity.x * dT ), Global.CharacterCollideLayers );
    temp += "left: " + hitCount + " ";
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.x > sideCornerNormalX /*corner*/ )
      {
        // if( hit.normal.x < prefer )
        // {
          collideLeft = true;
          hitLeft = hit;
          adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
          wallSlideTargetNormal = hit.normal;
          // prevent clipping through angled walls when falling fast.
          velocity.y -= Util.Project2D( velocity, hit.normal ).y;
        //   prefer = hit.normal.x;
        // }
      }
    }

    // start out of normal range
    // prefer = -2;
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.right, RaycastHits, Mathf.Max( raylength, velocity.x * dT ), Global.CharacterCollideLayers );
    temp += "right: " + hitCount + " ";
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.x < -sideCornerNormalX /*-corner*/ )
      {
        // if( hit.normal.x > prefer )
        // {
          collideRight = true;
          hitRight = hit;
          adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
          wallSlideTargetNormal = hit.normal;
          // prevent clipping through angled walls when falling fast.
          velocity.y -= Util.Project2D( velocity, hit.normal ).y;
        //   prefer = hit.normal.x;
        // }
      }
    }
    
    // Debug.Log( temp );

    pos = adjust;
    //transform.position = adjust;

  }
  
  public override Vector2 GetShotOriginPosition()
  {
    return (Vector2) arm.position + shoot.normalized * armRadius * Scale;
  }

  public override Vector2 GetAimVector()
  {
    return shoot;
  }

  void Shoot()
  {
    if( weapon == null || (weapon.HasInterval && shootRepeatTimer.IsActive) )
      return;
    if( !weapon.fullAuto )
      input.Fire = false;
    if( weapon.HasInterval )
      shootRepeatTimer.Start( weapon.shootInterval, null, null );
    Vector2 pos = GetShotOriginPosition();
    // PICKLE
    if( !Physics2D.Linecast( transform.position, pos, Global.ProjectileNoShootLayers ) )
      weapon.FireWeapon( this, pos, shoot
#if PICKLE 
, Scale 
#endif
    );
  }

  void ShootCharged()
  {
    if( weapon == null || weapon.ChargeVariant == null )
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
        Vector3 pos = GetShotOriginPosition();
        if( !Physics2D.Linecast( transform.position, pos, Global.ProjectileNoShootLayers ) )
          weapon.ChargeVariant.FireWeapon( this, pos, shoot );
      }
    }
    StopCharge();
  }

  void UseAbility()
  {
    ability.Activate( GetShotOriginPosition(), shoot );
  }

  void UpdateCursor()
  {
    Vector2 AimPosition = Vector2.zero;
    Vector2 cursorOrigin = arm.position;
    Vector2 cursorDelta = input.Aim;

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
      AimPosition = cursorOrigin + cursorDelta;
      CursorWorldPosition = AimPosition;
    }

    if( Global.instance.AutoAim )
    {
      CursorWorldPosition = cursorOrigin + cursorDelta;
      RaycastHit2D[] hits = Physics2D.CircleCastAll( transform.position, AutoAimCircleRadius, cursorDelta, AutoAimDistance, LayerMask.GetMask( new string[] {"enemy"} ) );
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
        AimPosition = closest.position;
        CursorAutoAim.position = AimPosition;
      }
    }
    else
    {
      CursorAutoAim.gameObject.SetActive( false );
    }

    shoot = AimPosition - (Vector2) arm.position;
    // if no inputs override, then default to facing the aim direction
    if( shoot.sqrMagnitude < 0.0001f )
      shoot = facingRight ? Vector2.right : Vector2.left;
  }

  private bool previousWallsliding;
  Vector2 previousWallSlideTargetNormal;
  
  public override void EntityUpdate( )
  {
    if( Global.Paused )
      return;

    string anim = "idle";
    bool previousGround = onGround;
    onGround = collideBottom || (collideLeft && collideRight);
    if( onGround && !previousGround )
    {
      landing = true;
      landTimer.Start( landDuration, null, delegate { landing = false; } );
    }
    previousWallsliding = wallsliding;
    previousWallSlideTargetNormal = wallSlideTargetNormal; 
    wallsliding = false;

    // must have input (or push) to move horizontally, so allow no persistent horizontal velocity (without push)
    if( grapPulling )
      velocity = Vector3.zero;
    else if( !(input.MoveRight || input.MoveLeft) )
      velocity.x = inertia.x;

    if( partArmBack.enabled )
    {
      // WEAPONS / ABILITIES
      if( input.Fire )
        Shoot();

      if( input.ChargeStart )
        StartCharge();

      if( input.ChargeEnd )
        ShootCharged();
    }

    if( ability != null && partArmFront.enabled )
    {
      if( input.Ability )
        UseAbility();
    }

    if( input.NextWeapon )
      NextWeapon();

    if( input.NextAbility )
      NextAbility();

    if( input.MoveDown )
      hanging = false;

    if( input.Interact )
    {
      if( WorldSelection != null && WorldSelection != closestISelect )
        WorldSelection.Unselect();
      WorldSelection = closestISelect;
      if( WorldSelection != null )
      {
        WorldSelection.Select();
        if( WorldSelection is Pickup )
        {
          Pickup pickup = (Pickup) WorldSelection;
          if( pickup.weapon != null )
          {
            if( AddWeapon( pickup.weapon ) )
              Destroy( pickup.gameObject );
          }
          else if( pickup.ability != null )
          {
            if( AddAbility( pickup.ability ) )
              Destroy( pickup.gameObject );
          }
          else if( pickup.PickupPart != PickupPart.None )
          {
            switch( pickup.PickupPart )
            {
              case PickupPart.Legs:
                EnablePart( partLegs );
                break;
              case PickupPart.Head:
                EnablePart( partHead );
                break;
              case PickupPart.ArmFront:
                EnablePart( partArmFront );
                break;
              case PickupPart.ArmBack:
                EnablePart( partArmBack );
                break;
            }
          }
        }
      }
    }

    if( partLegs.enabled )
    {
      if( takingDamage )
      {
        velocity.y = 0;
      }
      else
      {
        if( input.MoveRight && input.MoveLeft )
        {
          velocity.x = 0;
        }
        else if( input.MoveRight )
        {
          facingRight = true;
          inertia.x = Mathf.Max( inertia.x, 0 );
          // move along floor if angled downwards
          Vector3 hitnormalCross = Vector3.Cross( hitBottomNormal, Vector3.forward );
          if( onGround && hitnormalCross.y < 0 )
            // add a small downward vector for curved surfaces
            velocity = hitnormalCross * moveSpeed + Vector3.down * downslopefudge;
          else
            velocity.x = moveSpeed + inertia.x;
          if( !facingRight && onGround )
            StopDash();
        }
        else if( input.MoveLeft )
        {
          facingRight = false;
          inertia.x = Mathf.Min( inertia.x, 0 );
          // move along floor if angled downwards
          Vector3 hitnormalCross = Vector3.Cross( hitBottomNormal, Vector3.back );
          if( onGround && hitnormalCross.y < 0 )
            // add a small downward vector for curved surfaces
            velocity = hitnormalCross * moveSpeed + Vector3.down * downslopefudge;
          else
            velocity.x = -moveSpeed + inertia.x;
          if( facingRight && onGround )
            StopDash();
        }

        if( input.DashStart && (onGround || collideLeft || collideRight) )
          StartDash();

        if( input.DashEnd && !jumping )
          StopDash();

        if( dashing )
        {
          if( onGround && !previousGround )
          {
            StopDash();
          }
          else if( facingRight )
          {
            if( onGround || input.MoveRight )
              velocity.x = dashSpeed;
            if( (onGround || collideRight) && !dashTimer.IsActive )
              StopDash();
          }
          else
          {
            if( onGround || input.MoveLeft )
              velocity.x = -dashSpeed;
            if( (onGround || collideLeft) && !dashTimer.IsActive )
              StopDash();
          }
        }

        if( onGround && input.JumpStart )
          StartJump();
        else if( input.JumpEnd )
          StopJump();
        else if( collideRight && /*input.MoveRight &&*/ hitRight.normal.y >= 0 )
        {
          if( input.JumpStart )
          {
            walljumping = true;
            velocity.y = jumpSpeed;
            OverrideVelocity( Vector2.left * (input.DashStart ? dashSpeed : wallJumpPushVelocity), wallJumpPushDuration );
            jumpRepeatTimer.Start( jumpRepeatInterval );
            walljumpTimer.Start( wallJumpPushDuration, null, delegate { walljumping = false; } );
            audio.PlayOneShot( soundJump );
            Instantiate( walljumpEffect, WallkickPosition.position, Quaternion.identity );
          }
          else if( !jumping && !walljumping && !onGround && velocity.y < 0 )
          {
            facingRight = true;
            Wallslide();
          }
        }
        else if( collideLeft && /*input.MoveLeft &&*/ hitLeft.normal.y >= 0 )
        {
          if( input.JumpStart )
          {
            walljumping = true;
            velocity.y = jumpSpeed;
            OverrideVelocity( Vector2.right * (input.DashStart ? dashSpeed : wallJumpPushVelocity), wallJumpPushDuration );
            jumpRepeatTimer.Start( jumpRepeatInterval );
            walljumpTimer.Start( wallJumpPushDuration, null, delegate { walljumping = false; } );
            audio.PlayOneShot( soundJump );
            Instantiate( walljumpEffect, WallkickPosition.position, Quaternion.identity );
          }
          else if( !jumping && !walljumping && !onGround && velocity.y < 0 )
          {
            facingRight = false;
            Wallslide();
          }
        }
      }

      if( wallsliding && input.MoveDown )
      {
        if( wallSlideNormal.x > 0 )
          velocity += new Vector2( wallSlideTargetNormal.y, -wallSlideTargetNormal.x ) * wallSlideDownSpeed;
        else
          velocity += new Vector2( -wallSlideTargetNormal.y, wallSlideTargetNormal.x ) * wallSlideDownSpeed;
      }

      if( velocity.y < 0 )
      {
        jumping = false;
        walljumping = false;
        walljumpTimer.Stop( false );
      }

      if( !((onGround && dashing) || wallsliding) )
        dashSmoke.Stop();
    }

    if( ability != null )
      ability.UpdateAbility( this );

    // // add gravity before velocity limits
    if( UseGravity )
      velocity.y -= Global.Gravity * Time.deltaTime;

    if( !grapPulling && overrideVelocityTimer.IsActive )
      velocity.x = overrideVelocity.x;

    if( hanging )
      velocity = Vector3.zero;

    // limit velocity before adding to position
    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
      inertia.x = Mathf.Min( inertia.x, 0 );
    }
    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
      inertia.x = Mathf.Max( inertia.x, 0 );
    }
    // NOTE: "onGround" is not the same as "collideBottom"
    if( onGround )
    {
      velocity.y = Mathf.Max( velocity.y, 0 );
      //inertia.x -= (inertia.x * friction) * dT;
      inertia.x = 0;
    }
    if( collideTop )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }

    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
    // value is adjusted later by collision
    pos = transform.position + (Vector3) Velocity * Time.deltaTime;

    Entity previousCarry = carryCharacter;
    carryCharacter = null;

    UpdateHit( Time.deltaTime );
    // update collision flags, and adjust position
    UpdateCollision( Time.deltaTime );

    // must be after collision
    if( wallsliding )
    {
      float angle = Vector2.Angle( previousWallSlideTargetNormal, wallSlideTargetNormal );
      if( previousWallsliding && angle < wallSlideHardAngleTHreshold )
        wallSlideNormal = Vector2.MoveTowards( wallSlideNormal, wallSlideTargetNormal, wallSlideRotateSpeed * Time.deltaTime );
      else
        wallSlideNormal = wallSlideTargetNormal;
    }

    transform.position = pos;

    // carry momentum when jumping from moving platforms
    if( previousCarry != null && carryCharacter == null )
      inertia = previousCarry.Velocity;

    UpdateCursor();

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
      else if( input.MoveRight || input.MoveLeft )
        anim = "run";
      else if( landing )
        anim = "land";
      else
      {
        anim = "idle";
        // when idling, always face aim direction
        facingRight = shoot.x >= 0;
      }
    }
    else if( !jumping )
      anim = "fall";

    Play( anim );
    transform.localScale = (new Vector3( facingRight ? 1 : -1, 1, 1 )) * Scale;
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

  void LateUpdate()
  {
    if( !Global.instance.Updating )
      return;

    UpdateParts();

    Cursor.position = CursorWorldPosition;

    if( Global.instance.ShowAimPath && CursorAboveMinimumDistance )
    {
      lineRenderer.enabled = true;
      Vector3[] trail = weapon.GetTrajectory( GetShotOriginPosition(), shoot );
      lineRenderer.positionCount = trail.Length;
      lineRenderer.SetPositions( trail );
    }
    else
    {
      lineRenderer.enabled = false;
    }
  }

  public float wallSlideHardAngleTHreshold = 20;
  void Wallslide()
  {
    wallsliding = true;
    velocity.y += (-velocity.y * wallSlideFactor) * Time.deltaTime;
    dashSmoke.transform.localPosition = new Vector3( 0.2f, -0.2f, 0 );
    if( !dashSmoke.isPlaying )
      dashSmoke.Play();
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
          //sr.color = chargeColor;
          sr.material.SetColor( "_FlashColor", chargeColor );
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
      else
        foreach( var sr in spriteRenderers )
          sr.enabled = false;
      DamagePulseFlip();
    } );
  }
  


  public void ScaleChange( float scale )
  {
#if PICKLE    
    Scale *= scale;
    
    DownOffset = 0.1f * Scale;
    raylength = 0.05f * Scale;
    contactSeparation = 0.01f * Scale;
    SetAnimatorSpeed( 1f + (1f - Scale) * MoveScale );
    
    Health = MaxHealth;
    dashSmoke.transform.localScale = Vector3.one * Scale;
    damageSmoke.transform.localScale = Vector3.one * Scale;
    Global.instance.CameraController.orthoTarget = OrthoScale.Evaluate( Scale );
    // HACK hardcoded value
    Global.instance.CameraController.yHalfWidth = 0.5f * Scale; 
    // damage smoke
    ParticleSystem.MainModule damageSmokeMain = damageSmoke.main;
    ParticleSystem.MinMaxCurve rateCurve = damageSmokeMain.startSpeed;
    // HACK hardcoded value
    rateCurve.constant = 0.5f * Scale;
    damageSmokeMain.startSpeed = rateCurve;
    
    // todo 
    // dash effect
    // no jump
    // scale debris
    // projectile speed
#endif
  }
  
  protected override void Die()
  {
    Instantiate( spawnWhenDead, transform.position, Quaternion.identity );
    // todo death sound
    
    /*
    if( spider == null )
    {
      GameObject go = Instantiate( SpiderbotPrefab, transform.position, Quaternion.identity );
      spider = go.GetComponent<SpiderPawn>();
    }
    controller.AssignPawn( spider );
    */

    // BODY PART HUNT EXPERIMENT
#if BODY_PART_HUNT
    StopCharge();
    damageSmoke.Stop();
    dashSmoke.Stop();
    DisablePart( partHead );
    DisablePart( partLegs );
    DisablePart( partArmBack );
    DisablePart( partArmFront );
#endif
    
#if PICKLE
    // PICKLE RICK EXPERIMENT
    ScaleChange( 0.5f );
#else
    
    // RESPAWN
    controller.RemovePawn();
    Destroy( gameObject );
    new Timer( 5, null, Global.instance.SpawnPlayer );
    
#endif

  }

  public override bool TakeDamage( Damage d )
  {
    if( !CanTakeDamage || damagePassThrough || Health <= 0 )
      return false;
    if( d.instigator != null && !IsEnemyTeam( d.instigator.Team ) )
      return false;
    Health -= d.amount;
    if( Health <= 0 )
    {
      flashTimer.Stop( false );
      Die();
      return true;
    }
    damageSmoke.Play();
    
    ParticleSystem.EmissionModule damageSmokeEmission = damageSmoke.emission;
    ParticleSystem.MinMaxCurve rateCurve = damageSmokeEmission.rateOverTime;
    rateCurve.constant = 20.0f - 20.0f * ((float)Health / (float)MaxHealth);
    damageSmokeEmission.rateOverTime = rateCurve;
    
    StopCharge();
    //partHead.transform.localScale = Vector3.one * (1 + (Health / MaxHealth) * 10);
    audio.PlayOneShot( soundDamage );
    CameraShake shaker = Global.instance.CameraController.GetComponent<CameraShake>();
    shaker.amplitude = 0.3f;
    shaker.duration = 0.5f;
    shaker.rate = 100;
    shaker.intensityCurve = damageShakeCurve;
    shaker.enabled = true;
    Play( "damage" );
    float sign = Mathf.Sign( d.damageSource.position.x - transform.position.x );
    facingRight = sign > 0;
    velocity.y = 0;
    OverrideVelocity( new Vector2( -sign * damagePushAmount, damageLift ), damageDuration );
    arm.gameObject.SetActive( false );
    takingDamage = true;
    damagePassThrough = true;
    if( ability != null )
      ability.Deactivate();
    damageTimer.Start( damageDuration, delegate( Timer t )
    {
      //push.x = -sign * damagePushAmount;
    }, delegate()
    {
      takingDamage = false;
      arm.gameObject.SetActive( true );
      DamagePulseFlip();
      damageTimer.Start( damageBlinkDuration, null, delegate()
      {
        foreach( var sr in spriteRenderers )
          sr.enabled = true;
        damagePassThrough = false;
        damagePulseTimer.Stop( false );
      } );
    } );
    /*
    // color pulse
    flip = false;
    foreach( var sr in spriteRenderers )
      sr.material.SetFloat( "_FlashAmount", flashOn );
    flashTimer.Start( flashCount * 2, flashInterval, delegate ( Timer t )
    {
      flip = !flip;
      if( flip )
        foreach( var sr in spriteRenderers )
          sr.material.SetFloat( "_FlashAmount", flashOn );
      else
        foreach( var sr in spriteRenderers )
          sr.material.SetFloat( "_FlashAmount", 0 );
    }, delegate
    {
      foreach( var sr in spriteRenderers )
        sr.material.SetFloat( "_FlashAmount", 0 );
    } );
    */

    return true;
  }

  public void DoorTransition( bool right, float duration, float distance )
  {
    enabled = false;
    damageTimer.Stop( true );
    facingRight = right;
    transform.localScale = new Vector3( facingRight ? 1 : -1, 1, 1 );
    renderer.material.SetInt( "_FlipX", facingRight ? 0 : 1 );
    arm.localScale = new Vector3( facingRight ? 1 : -1, 1, 1 );
    SetAnimatorUpdateMode( AnimatorUpdateMode.UnscaledTime );
    Play( "run" );
    LerpToTarget lerp = gameObject.GetComponent<LerpToTarget>();
    if( lerp == null )
      lerp = gameObject.AddComponent<LerpToTarget>();
    lerp.moveTransform = transform;
    lerp.WorldTarget = true;
    lerp.targetPositionWorld = transform.position + ((right ? Vector3.right : Vector3.left) * distance);
    lerp.duration = duration;
    lerp.unscaledTime = true;
    lerp.enabled = true;
    lerp.OnLerpEnd = delegate
    {
      enabled = true;
      SetAnimatorUpdateMode( AnimatorUpdateMode.Normal );
    };
  }


  [FormerlySerializedAs( "partBody" )]
  [Header( "Character Parts player biped" )]
  public CharacterPart partLegs;

  public CharacterPart partHead;
  public CharacterPart partArmBack;
  public CharacterPart partArmFront;
  public CharacterPart partSpider;

  // Call from Awake()
  public override void InitializeParts()
  {
    CharacterParts = new List<CharacterPart> {partLegs, partHead, partArmBack, partArmFront, partSpider};
    // testing
    for( int i = 0; i < CharacterParts.Count; i++ )
    {
      CharacterPart part = CharacterParts[i];
      part.enabled = true;
      CharacterParts[i] = part;
    }
  }

  /*
    // Call from LateUpdate()
    void UpdateParts()
    {
      foreach( var part in CharacterParts )
        part.renderer.sortingOrder = CharacterLayer + part.layerAnimated;
    }
  */
  void Play( string anim )
  {
    partLegs.animator.Play( anim );
    /*
    foreach( var part in CharacterParts )
      if( part.animator != null && part.animator.HasState( 0, Animator.StringToHash( anim ) ) )
        part.animator.Play( anim );
        */
    //#if UNITY_EDITOR
    //      else
    //        Debug.LogWarning( "anim " + anim + " not found on " + part.transform.name );
    //#endif
  }

  void SetAnimatorUpdateMode( AnimatorUpdateMode mode )
  {
    // for changing the time scale to/from unscaled
    foreach( var part in CharacterParts )
      if( part.animator != null )
        part.animator.updateMode = mode;
  }

  void SetAnimatorSpeed( float speed )
  {
    foreach( var part in CharacterParts )
      if( part.animator != null )
        part.animator.speed = speed;
  }


  void EnablePart( CharacterPart part )
  {
    part.enabled = true;

    if( part.animator != null )
      part.animator.enabled = true;

    if( part.transform == partLegs.transform )
    {
      // GO INTO BIPED MODE!!!!
#if BODY_PART_HUNT
      DownOffset = BipedDownOffset;
#endif
      part.transform.GetComponent<BoxCollider2D>().size = new Vector2( 0.2f, 0.3f );
      part.renderer.enabled = true;
      Global.instance.CameraController.orthoTarget = 3;
      Global.instance.CameraController.UseVerticalRange = true;
      CanTakeDamage = true;
      Health = MaxHealth;
      UseGravity = true;
    }
    part.transform.gameObject.SetActive( true );
  }

  void DisablePart( CharacterPart part )
  {
    part.enabled = false;

    if( part.animator != null )
      part.animator.enabled = false;

    if( part.transform == partLegs.transform )
    {
      // GO INTO SPIDER MODE!!!
#if BODY_PART_HUNT      
      DownOffset = SpiderDownOffset;
#endif
      part.transform.GetComponent<BoxCollider2D>().size = new Vector2( 0.15f, 0.15f );
      part.renderer.enabled = false;
      Global.instance.CameraController.orthoTarget = 1;
      Global.instance.CameraController.UseVerticalRange = false;
      CanTakeDamage = false;
    }
    else
    {
      part.transform.gameObject.SetActive( false );
    }
  }
}