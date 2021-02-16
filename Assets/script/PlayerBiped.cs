//#define optimized_collision

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
  const float raydown = 0.2f;
  public Vector2 headbox = new Vector2( .1f, .1f );
  public float headboxy = -0.1f;
  const float downslopefudge = 0.2f;
  //private const float corner = 0.707106769f;
  const float collisionCornerTop = -0.658504546f;
  const float collisionCornerBottom = 0.658504546f;
  const float collisionCornerSide = 0.752576649f;
  
  Vector2 shoot;

  [Header( "State" )]
  [SerializeField] bool facingRight = true;

  bool onGround;
  bool previousGround;
  [SerializeField] bool jumping;
  [SerializeField] bool walljumping;
  [SerializeField] bool wallsliding;
  [SerializeField] bool landing;
  [SerializeField] bool dashing;
  
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
  public float wallSlideDownX = 1;
  public float wallSlideDownY = 1;
  public float wallSlideHardAngleThreshold = 25;
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
  float chargeStartTime;
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
  public AudioClip soundDeath;
  public AudioClip soundPickup;
  public AudioClip soundDenied;
  public AudioClip soundWeaponFail;

  [Header( "PlayerBiped Damage" )]
  [SerializeField] Damage CrushDamage;
  [SerializeField] float damageDuration = 0.5f;
  bool takingDamage;
  bool damageGracePeriod;
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
  RaycastHit2D hitTop;
  RaycastHit2D hitBottom;
  RaycastHit2D hitRight;
  RaycastHit2D hitLeft;

  // World Selection
  [SerializeField] float selectRange = 3;
  IWorldSelectable closestISelect;
  IWorldSelectable WorldSelection;
  List<Component> pups = new List<Component>();

  public bool IsBiped { get { return partLegs.enabled;  } }
  public bool grapPulling
  {
    get { return ability != null && ability is GraphookAbility && ability.IsActive;  }
  }

  protected override void Awake()
  {
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
    // acquire abilities serialized into the prefab
    for( int i = 0; i < abilities.Count; i++ )
      abilities[i].OnAcquire( this );
    if( weapons.Count > 0 )
      Global.instance.weaponIcon.sprite = weapon.icon;
    else
      Global.instance.weaponIcon.sprite = null;
    Global.instance.abilityIcon.sprite = null;
    
    bottomHits = new RaycastHit2D[RaycastHits.Length];
    topHits = new RaycastHit2D[RaycastHits.Length];
    leftHits = new RaycastHit2D[RaycastHits.Length];
    rightHits = new RaycastHit2D[RaycastHits.Length];
  }

  public override void OnControllerAssigned()
  {
    // settings are read before player is created, so set player settings here.
    speedFactorNormalized = Global.instance.FloatSetting["PlayerSpeedFactor"].Value;
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
    if( ability != null )
      ability.Deactivate();
  }

  bool AddWeapon( Weapon wpn )
  {
    if( weapons.Contains( wpn ) )
    {
      audio.PlayOneShot( soundDenied );
      return false;
    }
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
    {
      audio.PlayOneShot( soundDenied );
      return false;
    }
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
    hitCount = Physics2D.BoxCastNonAlloc( pos, box.size + Vector2.up * DownOffset*2 + Vector2.right*0.1f, 0, velocity, RaycastHits, Mathf.Max( raylength, velocity.magnitude * dT ), HitLayers );
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
      Pickup pickup = hit.transform.GetComponent<Pickup>();
      if( pickup != null && pickup.SelectOnContact )
      {
        if( pickup.unique != UniquePickupType.None )
        {
          switch( pickup.unique )
          {
            case UniquePickupType.Health: 
              AddHealth( pickup.uniqueInt0 ); 
              break;
            case UniquePickupType.SpeedFactorNormalized:  
              speedFactorNormalized += pickup.uniqueFloat0; 
              break;
          }
        }
        // this should destroy the pickup
        pickup.Select();
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

    IWorldSelectable closest = (IWorldSelectable) Util.FindSmallestAngle( transform.position, shoot, pups.ToArray() );
    if( closest == null )
    {
      if( closestISelect != null )
      {
        closestISelect.Unhighlight();
        closestISelect = null;
        /*InteractIndicator.SetActive( false );*/
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
      {
        closestISelect.Unhighlight();
        /*InteractIndicator.SetActive( false );*/
      }
      closestISelect = closest;
      closestISelect.Highlight();
      // Not everything is indicated the same way
      /*InteractIndicator.SetActive( true );
      InteractIndicator.transform.position = closestISelect.GetPosition();*/
    }
    else
    {
      InteractIndicator.transform.position = closestISelect.GetPosition();
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
  

#if optimized_collision

  new void UpdateCollision( float dT )
  {
    // HACK
    contactSeparation = 0;
    raylength = 0;
    // HACK
    
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    adjust = pos;
    string temp = "";
    bottomHitCount=0;
    topHitCount=0;
    rightHitCount=0;
    leftHitCount=0;
    
    float preferBottom = float.MinValue;
    float preferLeft = 2;
    float preferRight = -2;
    float down = jumping ? 0.08f : DownOffset; //0.23f;
    float offset = Mathf.Max( down, -velocity.y * dT ) * 0.5f;
    Vector2 debugOrigin = adjust + offset * Vector2.down;
    Vector2 debugBox = (box.size + 
      Vector2.up * Mathf.Max( down, -velocity.y * dT ) +
        Vector2.right *  Mathf.Max( 0, Mathf.Abs(velocity.x * dT) )
        );
    
    Bounds bounds = new Bounds( debugOrigin, debugBox );
    Debug.DrawLine( new Vector3( bounds.min.x, bounds.min.y ), new Vector3( bounds.min.x, bounds.max.y ), Color.red );
    Debug.DrawLine( new Vector3( bounds.min.x, bounds.max.y ), new Vector3( bounds.max.x, bounds.max.y ), Color.red );
    Debug.DrawLine( new Vector3( bounds.max.x, bounds.max.y ), new Vector3( bounds.max.x, bounds.min.y ), Color.red );
    Debug.DrawLine( new Vector3( bounds.max.x, bounds.min.y ), new Vector3( bounds.min.x, bounds.min.y ), Color.red );
    
    hitCount = Physics2D.BoxCastNonAlloc( debugOrigin, debugBox, transform.rotation.eulerAngles.z, Vector2.zero, RaycastHits, 0, Global.CharacterCollideLayers );
    //Debug.Log("hitCount: " + hitCount);
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;

      if( hit.normal.y > 0 && hit.normal.y > collisionCornerBottom && hit.point.y > preferBottom )
      {
        preferBottom = hit.point.y;
        collideBottom = true;
        hitBottom = hit;
        velocity.y = Mathf.Max( velocity.y, 0 );
        inertia.x = 0;
        adjust.y = hit.point.y + debugBox.y * 0.5f + offset;
        bottomHits[bottomHitCount++] = hit;

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
      }

      if( hit.normal.y < collisionCornerTop )
      {
        collideTop = true;
        hitTop = hit;
        velocity.y = Mathf.Min( velocity.y, 0 );
        adjust.y = hit.point.y - debugBox.y * 0.5f;
        topHits[topHitCount++] = hit;
      }
      
      if( hit.normal.x > collisionCornerSide && hit.normal.x < preferLeft )
      {
        preferLeft = hit.normal.x;
        collideLeft = true;
        hitLeft = hit;
      
        velocity.x = Mathf.Max( velocity.x, 0 );
        inertia.x = Mathf.Max( inertia.x, 0 );
        adjust.x = hit.point.x + debugBox.x * 0.5f;
        leftHits[leftHitCount++] = hit;
        
        wallSlideTargetNormal = hit.normal;
        // prevent clipping through angled walls when falling fast.
        velocity.y -= Util.Project2D( velocity, hit.normal ).y;
      }
      
      if( hit.normal.x < -collisionCornerSide && hit.normal.x > preferRight )
      {
        preferRight = hit.normal.x;
        collideRight = true;
        hitRight = hit;
      
        velocity.x = Mathf.Min( velocity.x, 0 );
        inertia.x = Mathf.Min( inertia.x, 0 );
        adjust.x = hit.point.x - debugBox.x * 0.5f;
        rightHits[rightHitCount++] = hit;
        
        wallSlideTargetNormal = hit.normal;
        // prevent clipping through angled walls when falling fast.
        velocity.y -= Util.Project2D( velocity, hit.normal ).y;
      }
      
    }
    
    pos = adjust;
  }
  
#else
  
  new void UpdateCollision( float dT )
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    adjust = pos;

    bottomHitCount=0;
    topHitCount=0;
    rightHitCount=0;
    leftHitCount=0;
    
#if multisample

    Vector2 rightFoot;
    Vector2 leftFoot;
    // Avoid the (box-to-box) standing-on-a-corner-and-moving-means-momentarily-not-on-ground issue by 'sampling' the ground at multiple points
    RaycastHit2D right = Physics2D.Raycast( adjust + Vector2.right * box.size.x, Vector2.down, Mathf.Max( down, -velocity.y * dT ), Global.CharacterCollideLayers );
    RaycastHit2D left = Physics2D.Raycast( adjust + Vector2.left * box.size.x, Vector2.down, Mathf.Max( down, -velocity.y * dT ), Global.CharacterCollideLayers );
    if( right.transform != null )
      rightFoot = right.point;
    else
      rightFoot = adjust + ( Vector2.right * box.size.x ) + ( Vector2.down * Mathf.Max( raylength, -velocity.y * dT ) );
    if( left.transform != null )
      leftFoot = left.point;
    else
      leftFoot = adjust + ( Vector2.left * box.size.x ) + ( Vector2.down * Mathf.Max( raylength, -velocity.y * dT ) );

    if( right.transform != null || left.transform != null )
    {
      Vector2 across = rightFoot - leftFoot;
      Vector3 sloped = Vector3.Cross( across.normalized, Vector3.back );
      if( sloped.y > corner )
      {
        collideBottom = true;
        adjust.y = ( leftFoot + across * 0.5f ).y + downOffset;
        hitBottomNormal = sloped;
      }
    }

    if( left.transform != null )
      Debug.DrawLine( adjust + Vector2.left * box.size.x, leftFoot, Color.green );
    else
      Debug.DrawLine( adjust + Vector2.left * box.size.x, leftFoot, Color.grey );
    if( right.transform != null )
      Debug.DrawLine( adjust + Vector2.right * box.size.x, rightFoot, Color.green );
    else
      Debug.DrawLine( adjust + Vector2.right * box.size.x, rightFoot, Color.grey );

#endif
    
    // slidingOffsetTarget = 1;
    string temp = "";
    float down = jumping ? raydown - DownOffset : raydown;
    float prefer;
   
    Bounds bounds = new Bounds( adjust + Vector2.down * Mathf.Max( down, -velocity.y * dT ) * 0.5f, box.size + Vector2.up * Mathf.Max( down, -velocity.y * dT ) );
    Debug.DrawLine( new Vector3( bounds.min.x, bounds.min.y ), new Vector3( bounds.min.x, bounds.max.y ), Color.red );
    Debug.DrawLine( new Vector3( bounds.min.x, bounds.max.y ), new Vector3( bounds.max.x, bounds.max.y ), Color.red );
    Debug.DrawLine( new Vector3( bounds.max.x, bounds.max.y ), new Vector3( bounds.max.x, bounds.min.y ), Color.red );
    Debug.DrawLine( new Vector3( bounds.max.x, bounds.min.y ), new Vector3( bounds.min.x, bounds.min.y ), Color.red );
    
    // BOTTOM
    prefer = float.MinValue;
    // todo fix exception that occurs after death
    bottomHitCount = Physics2D.BoxCastNonAlloc( adjust, box.size * Scale, transform.rotation.eulerAngles.z, -(Vector2)transform.up, bottomHits, Mathf.Max( down, -velocity.y * dT ), Global.CharacterCollideLayers );
    temp += "down: " + bottomHitCount + " ";
    for( int i = 0; i < bottomHitCount; i++ )
    {
      hit = bottomHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y > 0 && hit.normal.y > collisionCornerBottom && hit.point.y > prefer )
      {
        prefer = hit.point.y;
        collideBottom = true;
        hitBottom = hit;
        velocity.y = Mathf.Max( velocity.y, 0 );
        inertia.x = 0;
#if false
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
#endif
        adjust.y = hit.point.y + box.size.y * 0.5f * Scale + DownOffset;
        
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
      }
    }

    // TOP
    topHitCount = Physics2D.BoxCastNonAlloc( adjust + Vector2.up * headboxy, headbox, 0, Vector2.up, topHits, Mathf.Max( raylength, velocity.y * dT ), Global.CharacterCollideLayers );
    temp += "up: " + topHitCount + " ";
    for( int i = 0; i < topHitCount; i++ )
    {
      hit = topHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y < collisionCornerTop )
      {
        collideTop = true;
        hitTop = hit;
        velocity.y = Mathf.Min( velocity.y, 0 );
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
      }
    }

    // LEFT
    // Prefer more-horizontal walls for wall sliding. Start beyond normal range.
    prefer = 2;
    leftHitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.left, leftHits, Mathf.Max( raylength, -velocity.x * dT ), Global.CharacterCollideLayers );
    temp += "left: " + leftHitCount + " ";
    for( int i = 0; i < leftHitCount; i++ )
    {
      hit = leftHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.x > collisionCornerSide && hit.normal.x < prefer )
      {
        prefer = hit.normal.x;
        collideLeft = true;
        hitLeft = hit;
        
        velocity.x = Mathf.Max( velocity.x, 0 );

        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        wallSlideTargetNormal = hit.normal;
        // prevent clipping through angled walls when falling fast.
        velocity.y -= Util.Project2D( velocity, hit.normal ).y;
      }
    }

    // RIGHT
    prefer = -2;
    rightHitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.right, rightHits, Mathf.Max( raylength, velocity.x * dT ), Global.CharacterCollideLayers );
    temp += "right: " + rightHitCount + " ";
    for( int i = 0; i < rightHitCount; i++ )
    {
      hit = rightHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.x < -collisionCornerSide && hit.normal.x > prefer )
      {
        prefer = hit.normal.x;
        collideRight = true;
        hitRight = hit;
        
        velocity.x = Mathf.Min( velocity.x, 0 );

        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        wallSlideTargetNormal = hit.normal;
        // prevent clipping through angled walls when falling fast.
        velocity.y -= Util.Project2D( velocity, hit.normal ).y;
      }
    }

    //Debug.Log( temp );
    pos = adjust;
  }
  #endif
  

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
    if( weapon == null ||
      !weapon.fullAuto && pinput.Fire ||
      shootRepeatTimer.IsActive ) 
      return;
    if( weapon.shootInterval > 0 )
      shootRepeatTimer.Start( weapon.shootInterval, null, null );
    Vector2 pos = GetShotOriginPosition();
    // PICKLE
    if( !Physics2D.Linecast( transform.position, pos, Global.ProjectileNoShootLayers ) )
      weapon.FireWeapon( this, pos, shoot
#if PICKLE
, Scale
#endif
      );
    else
      audio.PlayOneShot( soundWeaponFail );
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
      if( (Time.time - chargeStartTime) > chargeMin )
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
    Vector2 cursorOrigin = transform.position;
    Vector2 inputAim = input.Aim;

    CursorAboveMinimumDistance = inputAim.magnitude > DirectionalMinimum;
    Cursor.gameObject.SetActive( CursorAboveMinimumDistance );

    if( Global.instance.AimSnap )
    {
      if( CursorAboveMinimumDistance )
      {
        // set cursor
        CursorSnapped.gameObject.SetActive( true );
        float angle = Mathf.Atan2( inputAim.x, inputAim.y ) / Mathf.PI;
        float snap = Mathf.Round( angle * SnapAngleDivide ) / SnapAngleDivide;
        Vector2 snapped = new Vector2( Mathf.Sin( snap * Mathf.PI ), Mathf.Cos( snap * Mathf.PI ) );
        AimPosition = cursorOrigin + snapped * SnapCursorDistance;
        CursorSnapped.position = AimPosition;
        CursorSnapped.rotation = Quaternion.LookRotation( Vector3.forward, snapped );
        CursorWorldPosition = cursorOrigin + inputAim;
      }
      else
      {
        CursorSnapped.gameObject.SetActive( false );
      }
    }
    else
    {
      CursorSnapped.gameObject.SetActive( false );
      AimPosition = cursorOrigin + inputAim;
      CursorWorldPosition = AimPosition;
    }

    if( Global.instance.AutoAim )
    {
      CursorWorldPosition = cursorOrigin + inputAim;
      RaycastHit2D[] hits = Physics2D.CircleCastAll( transform.position, AutoAimCircleRadius, inputAim, AutoAimDistance, LayerMask.GetMask( new string[] {"enemy"} ) );
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

  bool jumpStart { get { return input.Jump && !pinput.Jump;  } }
  bool jumpStop { get { return !input.Jump && pinput.Jump;  } }
  bool dashStart { get { return (input.Dash && !pinput.Dash); } }
  bool dashStop { get { return (!input.Dash && pinput.Dash); } }
  bool chargeStart { get { return input.Charge && !pinput.Charge;  } }
  bool chargeStop { get { return !input.Charge && pinput.Charge;  } }

  public override void EntityUpdate( )
  {
    if( Global.Paused || Health <= 0 )
      return;

    string anim = "idle";
    bool previousGround = onGround;
    onGround = collideBottom || (collideLeft && hitLeft.normal.y > 0 && collideRight && hitRight.normal.y > 0 );
    if( onGround && !previousGround )
    {
      landing = true;
      landTimer.Start( landDuration, null, delegate { landing = false; } );
    }
    
    if( wallsliding )
    {
      float angle = Vector2.Angle( previousWallSlideTargetNormal, wallSlideTargetNormal );
      if( previousWallsliding && angle < wallSlideHardAngleThreshold )
        wallSlideNormal = Vector2.MoveTowards( wallSlideNormal, wallSlideTargetNormal, wallSlideRotateSpeed * Time.deltaTime );
      else
        wallSlideNormal = wallSlideTargetNormal;
    }
    previousWallsliding = wallsliding;
    previousWallSlideTargetNormal = wallSlideTargetNormal; 
    wallsliding = false;

    // must have input (or inertia) to move horizontally, so allow no persistent horizontal velocity (without inertia)
    if( grapPulling )
      velocity = Vector3.zero;
    else if( !(input.MoveRight || input.MoveLeft) )
      velocity.x = inertia.x;

    // WEAPONS / ABILITIES
    if( weapon != null )
    {
      if( input.Fire )
        Shoot();

      if( weapon.HasChargeVariant )
      {
        if( chargeStart )
          StartCharge();
        if( chargeStop )
          ShootCharged();
      }
    }

    if( ability != null && partArmFront.enabled && input.Ability )
      UseAbility();

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
              pickup.Select();
          }
          else if( pickup.ability != null )
          {
            if( AddAbility( pickup.ability ) )
              pickup.Select();
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
          Vector3 hitnormalCross = Vector3.Cross( hitBottom.normal, Vector3.forward );
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
          Vector3 hitnormalCross = Vector3.Cross( hitBottom.normal, Vector3.back );
          if( onGround && hitnormalCross.y < 0 )
            // add a small downward vector for curved surfaces
            velocity = hitnormalCross * moveSpeed + Vector3.down * downslopefudge;
          else
            velocity.x = -moveSpeed + inertia.x;
          if( facingRight && onGround )
            StopDash();
        }

        if( dashStart && (onGround || collideLeft || collideRight) )
          StartDash();

        if( dashStop && !jumping )
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

        if( onGround && jumpStart )
          StartJump();
        else if( jumpStop )
          StopJump();
        else if( collideRight && /*input.MoveRight &&*/ hitRight.normal.y >= 0 )
        {
          if( jumpStart && input.MoveRight )
          {
            walljumping = true;
            velocity.y = jumpSpeed;
            OverrideVelocity( Vector2.left * (input.Dash ? dashSpeed : wallJumpPushVelocity), wallJumpPushDuration );
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
        else if( collideLeft && /*input.MoveLeft &&*/ hitLeft.normal.y >= 0 )
        {
          if( jumpStart && input.MoveLeft )
          {
            walljumping = true;
            velocity.y = jumpSpeed;
            OverrideVelocity( Vector2.right * (input.Dash ? dashSpeed : wallJumpPushVelocity), wallJumpPushDuration );
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
      }

      if( wallsliding && input.MoveDown )
      {
        Vector2 wsvel;
        wsvel = Util.Project2D( -(Vector2) transform.up, wallSlideNormal.x > 0 ? Vector2.right : Vector2.left) * wallSlideDownX +
          Vector2.down * wallSlideDownY;
        Debug.DrawLine( (Vector2) transform.position, (Vector2) transform.position + wsvel, Color.yellow );
        velocity += wsvel;
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
      ability.UpdateAbility( );

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
    
    transform.position = pos;

    // preserve inertia when jumping from moving platforms
    if( previousCarry != null && carryCharacter == null )
      inertia = previousCarry.Velocity;
    
    bool crushed = false;
    const float crushMinSpeed = 0.005f;
    if( collideRight && hitRight.normal.y <= 0 && collideLeft && hitLeft.normal.y <= 0 )
    {
      Entity entity;
      for( int i = 0; i < leftHitCount; i++ )
        if( leftHits[i].transform.TryGetComponent( out entity ) && entity.Velocity.x > crushMinSpeed )
          crushed = true;
      for( int i = 0; i < rightHitCount; i++ )
        if( rightHits[i].transform.TryGetComponent( out entity ) && entity.Velocity.x < -crushMinSpeed )
          crushed = true;
    }
    if( collideTop && collideBottom )
    {
      Entity entity;
      for( int i = 0; i < topHitCount; i++ )
        if( topHits[i].transform.TryGetComponent( out entity ) && entity.Velocity.y < -crushMinSpeed )
          crushed = true;
      for( int i =0; i < bottomHitCount; i++ )
        if( bottomHits[i].transform.TryGetComponent( out entity ) && entity.Velocity.y > crushMinSpeed )
          crushed = true;
    }
    if( crushed )
    {
      //Die();
      if( !takingDamage )
        TakeDamage( CrushDamage );
    }

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
      else if( (input.MoveRight || input.MoveLeft) && Mathf.Abs(velocity.x)>0 )
        anim = "run";
      else if( landing )
        anim = "land";
      else
      {
        anim = "idle";
        // when idling, always face aim direction
        facingRight = CursorWorldPosition.x >= transform.position.x;
      }
    }
    else if( !jumping )
      anim = "fall";

    Play( anim );
    transform.localScale = (new Vector3( facingRight ? 1 : -1, 1, 1 )) * Scale;
    renderer.material.SetInt( "_FlipX", facingRight ? 0 : 1 );

    arm.localScale = new Vector3( facingRight ? 1 : -1, 1, 1 );
    arm.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, shoot ) );
    if( wallsliding && wallSlideNormal.x < 0 )
      transform.rotation = Quaternion.LookRotation( Vector3.forward, new Vector3( wallSlideNormal.y, -wallSlideNormal.x ) );
    else if( wallsliding && wallSlideNormal.x > 0 )
      transform.rotation = Quaternion.LookRotation( Vector3.forward, new Vector3( -wallSlideNormal.y, wallSlideNormal.x ) );
    else
      transform.rotation = Quaternion.Euler( 0, 0, 0 );

    ResetInput();
    
#if debugdraw || true
    Debug.DrawLine( (Vector2)transform.position, (Vector2)transform.position + velocity, Color.magenta );
#endif
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
  
  void Wallslide()
  {
    wallsliding = true;
    velocity.y += (-velocity.y * wallSlideFactor) * Time.deltaTime;
    dashSmoke.transform.localPosition = new Vector3( -0.2f, -0.2f, 0 );
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
        chargeStartTime = Time.time;
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
    if( SpawnWhenDead.Length > 0 )
      Instantiate( SpawnWhenDead[Random.Range( 0, SpawnWhenDead.Length )], transform.position, Quaternion.identity );
    
    audio.PlayOneShot( soundDeath );
    
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

  public override void AddHealth( int amount )
  {
    Health = Mathf.Clamp( Health + amount, 0, MaxHealth );
    
    if( Health >= MaxHealth ) 
    {
      damageSmoke.Stop();
    }
    else
    {
      damageSmoke.Play();
      ParticleSystem.EmissionModule damageSmokeEmission = damageSmoke.emission;
      ParticleSystem.MinMaxCurve rateCurve = damageSmokeEmission.rateOverTime;
      rateCurve.constant = 20.0f - 20.0f * ((float)Health / (float)MaxHealth);
      damageSmokeEmission.rateOverTime = rateCurve;
    }
  }
  
  public override bool TakeDamage( Damage damage )
  {
    if( !CanTakeDamage || damageGracePeriod || Health <= 0 )
      return false;
    if( damage.instigator != null && !IsEnemyTeam( damage.instigator.Team ) )
      return false;
    AddHealth( -damage.amount );
    if( Health <= 0 )
    {
      flashTimer.Stop( false );
      Die();
      return true;
    }
    
    // damageSmoke.Play();
    // ParticleSystem.EmissionModule damageSmokeEmission = damageSmoke.emission;
    // ParticleSystem.MinMaxCurve rateCurve = damageSmokeEmission.rateOverTime;
    // rateCurve.constant = 20.0f - 20.0f * ((float)Health / (float)MaxHealth);
    // damageSmokeEmission.rateOverTime = rateCurve;
    
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
    float sign = 0;
    if( damage.damageSource != null )
      sign = Mathf.Sign( damage.damageSource.position.x - transform.position.x );
    facingRight = sign > 0;
    velocity.y = 0;
    OverrideVelocity( new Vector2( -sign * damagePushAmount, damageLift ), damageDuration );
    arm.gameObject.SetActive( false );
    takingDamage = true;
    damageGracePeriod = true;
    if( ability != null )
      ability.Deactivate();
    damageTimer.Start( damageDuration, null, delegate()
    {
      takingDamage = false;
      arm.gameObject.SetActive( true );
      DamagePulseFlip();
      damageTimer.Start( damageBlinkDuration, null, delegate()
      {
        foreach( var sr in spriteRenderers )
          sr.enabled = true;
        damageGracePeriod = false;
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
      //Global.instance.CameraController.orthoTarget = 3;
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
      //Global.instance.CameraController.orthoTarget = 1;
      Global.instance.CameraController.UseVerticalRange = false;
      CanTakeDamage = false;
    }
    else
    {
      part.transform.gameObject.SetActive( false );
    }
  }
}