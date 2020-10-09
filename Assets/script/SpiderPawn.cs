using System.Collections.Generic;
using UnityEngine;

public class SpiderPawn : Pawn
{
  [Header( "Setting" )] public float moveSpeed = 3;
  public float jumpSpeed = 5;

  [Header( "Ability" )] [SerializeField] Ability ability;
  [SerializeField] List<Ability> abilities;
  public int CurrentAbilityIndex;

#if UNITY_EDITOR
  public List<LineSegment> debugPath = new List<LineSegment>();
#endif

  protected override void Start()
  {
    base.Start();
    UpdateCollision = null;
    UpdatePosition = null;
    UpdateLogic = UpdateSpider;
    UpdateHit = CircleHit;
    // unpack
    InteractIndicator.SetActive( false );
    InteractIndicator.transform.SetParent( null );
  }

  public override void OnControllerAssigned()
  {
    controller.CursorInfluence = false;
  }

  public float drtm = 1;

  void UpdateSpider()
  {
    if( Physics2D.CircleCastNonAlloc( transform.position, circle.radius, Vector2.down, RaycastHits, Time.deltaTime * drtm, Global.CharacterCollideLayers ) > 0 )
      collideBottom = true;

    if( Physics2D.CircleCastNonAlloc( transform.position, circle.radius, Vector2.up, RaycastHits, Time.deltaTime * drtm, Global.CharacterCollideLayers ) > 0 )
      collideTop = true;

    if( Physics2D.CircleCastNonAlloc( transform.position, circle.radius, Vector2.left, RaycastHits, Time.deltaTime * drtm, Global.CharacterCollideLayers ) > 0 )
      collideLeft = true;

    if( Physics2D.CircleCastNonAlloc( transform.position, circle.radius, Vector2.right, RaycastHits, Time.deltaTime * drtm, Global.CharacterCollideLayers ) > 0 )
      collideRight = true;

    if( body.bodyType == RigidbodyType2D.Dynamic )
    {
      if( input.MoveRight )
        body.AddForce( (moveSpeed - body.velocity.x) * Vector2.right * body.mass, ForceMode2D.Impulse );
      if( input.MoveLeft )
        body.AddForce( (-moveSpeed - body.velocity.x) * Vector2.right * body.mass, ForceMode2D.Impulse );
      if( !input.MoveRight && !input.MoveLeft )
        body.AddForce( -body.velocity.x * Vector2.right * body.mass, ForceMode2D.Impulse );

      if( collideLeft || collideRight )
      {
        body.gravityScale = 0;
        if( input.MoveUp )
          body.AddForce( (moveSpeed - body.velocity.y) * Vector2.up * body.mass, ForceMode2D.Impulse );
        if( input.MoveDown )
          body.AddForce( (-moveSpeed - body.velocity.y) * Vector2.up * body.mass, ForceMode2D.Impulse );
        if( !input.MoveUp && !input.MoveDown )
          body.AddForce( -body.velocity.y * Vector2.up * body.mass, ForceMode2D.Impulse );
      }
      else if( collideTop )
      {
        body.gravityScale = 0;
      }
      else
      {
        body.gravityScale = 1;
        if( input.JumpEnd )
          body.AddForce( Mathf.Min( 0, -body.velocity.y ) * Vector2.up * body.mass, ForceMode2D.Impulse );
        else if( collideBottom && input.JumpStart )
          body.AddForce( jumpSpeed * Vector2.up * body.mass, ForceMode2D.Impulse );
      }
    }
    else if( body.bodyType == RigidbodyType2D.Kinematic )
    {
      velocity.y += -Global.Gravity * Time.deltaTime;
      if( input.MoveRight ) velocity.x = moveSpeed;
      if( input.MoveLeft ) velocity.x = -moveSpeed;
      if( !input.MoveRight && !input.MoveLeft ) velocity.x = 0;
      if( collideBottom && input.JumpStart ) velocity.y = jumpSpeed;
      else if( input.JumpEnd ) velocity.y = 0;
    }

    // You want to reset the collision flags after each time you "process" the inputs.
    // Otherwise if you reset them in FixedUpdate() you might have what feels like "collision lag"
    collideRight = false;
    collideTop = false;
    collideLeft = false;
    collideBottom = false;
    //collideBodyBottom = null;

#if UNITY_EDITOR
    // draw path
    if( debugPath.Count > 0 )
      foreach( var ls in debugPath )
        Debug.DrawLine( ls.a, ls.b, Color.magenta );
#endif

    UpdateCursor();

    pups.Clear();
    hitCount = Physics2D.CircleCastNonAlloc( transform.position, selectRange, Vector3.zero, RaycastHits, 0, Global.WorldSelectableLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      IWorldSelectable pup = hit.transform.GetComponent<IWorldSelectable>();
      if( pup != null )
        pups.Add( (Component) pup );
    }
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
          if( pickup.ability != null )
          {
            if( !abilities.Contains( pickup.ability ) )
            {
              abilities.Add( pickup.ability );
              AssignAbility( pickup.ability );
            }
            Destroy( pickup.gameObject );
          }
        }
      }

      if( input.Fire )
        Shoot();

      if( input.Ability )
        UseAbility();
    }
  }

  private Vector2 shoot;

  // World Selection
  [SerializeField] float selectRange = 1;
  IWorldSelectable closestISelect;
  IWorldSelectable WorldSelection;
  List<Component> pups = new List<Component>();

  private void FixedUpdate()
  {
    if( body.bodyType == RigidbodyType2D.Kinematic )
    {
      if( collideRight ) velocity.x = Mathf.Min( velocity.x, 0 );
      if( collideLeft ) velocity.x = Mathf.Max( velocity.x, 0 );
      if( collideTop ) velocity.y = Mathf.Min( velocity.y, 0 );
      if( collideBottom )
      {
        //if( collideBodyBottom != null ) velocity.y = Mathf.Max( velocity.y, collideBodyBottom.velocity.y );
        //else velocity.y = Mathf.Max( velocity.y, 0 );
      }
      body.MovePosition( body.position + (velocity * Time.fixedDeltaTime) );
    }
#if UNITY_EDITOR
    debugPath.Clear();
#endif
  }

  private void OnCollisionStay2D( Collision2D collision )
  {
    for( int i = 0; i < collision.contactCount; i++ )
    {
      if( -collision.contacts[i].normal.x > 0.5f ) collideRight = true;
      if( -collision.contacts[i].normal.x < 0.5f ) collideLeft = true;
      if( -collision.contacts[i].normal.y > 0.5f ) collideTop = true;
      if( -collision.contacts[i].normal.y < 0.5f ) collideBottom = true;
#if UNITY_EDITOR
      debugPath.Add( new LineSegment {a = collision.contacts[i].point, b = collision.contacts[i].point + collision.contacts[i].normal} );
#endif
    }
  }

  void CircleHit()
  {
    if( ContactDamage != null )
    {
      hitCount = Physics2D.CircleCastNonAlloc( body.position, circle.radius, body.velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
      for( int i = 0; i < hitCount; i++ )
      {
        if( RaycastHits[i].transform.GetInstanceID() == transform.GetInstanceID() )
          continue;
        hit = RaycastHits[i];
        IDamage dam = hit.transform.GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = this;
          dmg.damageSource = transform;
          dmg.point = hit.point;
          dam.TakeDamage( dmg );
        }
      }
    }
  }

  [Header( "Cursor" )] [SerializeField] Transform Cursor;
  // public Transform CursorSnapped;
  // public Transform CursorAutoAim;
  public float CursorScale = 2;
  public float SnapAngleDivide = 8;
  public float SnapCursorDistance = 1;
  public float AutoAimCircleRadius = 1;
  public float AutoAimDistance = 5;
  public float DirectionalMinimum = 0.3f;
  bool CursorAboveMinimumDistance;

  void UpdateCursor()
  {
    Vector2 AimPosition = Vector2.zero;
    Vector2 cursorOrigin = transform.position;
    Vector2 cursorDelta = input.Aim;

    CursorAboveMinimumDistance = cursorDelta.magnitude > DirectionalMinimum;
    Cursor.gameObject.SetActive( CursorAboveMinimumDistance );
/*
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
    */

    shoot = AimPosition - (Vector2) transform.position;
  }


  public override void PreSceneTransition()
  {
    velocity = Vector2.zero;
    // "pack" the graphook with this gameobject
    // graphook is unpacked when used
    // graphookTip.transform.parent = gameObject.transform;
    // grapCableRender.transform.parent = gameObject.transform;
    InteractIndicator.transform.SetParent( gameObject.transform );
  }

  public override void PostSceneTransition()
  {
    InteractIndicator.SetActive( false );
    InteractIndicator.transform.SetParent( null );
  }


  void AssignAbility( Ability alt )
  {
    if( ability != null )
      ability.Unequip();
    ability = alt;
    ability.Equip( transform );
    Global.instance.abilityIcon.sprite = ability.icon;
    Cursor.GetComponent<SpriteRenderer>().sprite = ability.cursor;
  }

  void NextAbility()
  {
    CurrentAbilityIndex = (CurrentAbilityIndex + 1) % abilities.Count;
    AssignAbility( abilities[CurrentAbilityIndex] );
  }

  void UseAbility()
  {
    if( ability == null )
      return;
    ability.Activate( GetShotOriginPosition(), shoot );
  }

  public override Vector2 GetShotOriginPosition()
  {
    return (Vector2) transform.position + shoot.normalized * 0.5f;
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
    if( !Physics2D.Linecast( transform.position, pos, Global.ProjectileNoShootLayers ) )
      weapon.FireWeapon( this, pos, shoot );
  }
  
  void AssignWeapon( Weapon wpn )
  {
    weapon = wpn;
    Global.instance.weaponIcon.sprite = weapon.icon;
    Cursor.GetComponent<SpriteRenderer>().sprite = weapon.cursor;
    foreach( var sr in spriteRenderers )
    {
      sr.material.SetColor( "_IndexColor", weapon.Color0 );
      sr.material.SetColor( "_IndexColor2", weapon.Color1 );
    }
  }

  void NextWeapon()
  {
    CurrentWeaponIndex = (CurrentWeaponIndex + 1) % weapons.Count;
    AssignWeapon( weapons[CurrentWeaponIndex] );
  }
  
  
  [Header( "Weapon" )] public Weapon weapon;
  public int CurrentWeaponIndex;
  [SerializeField] List<Weapon> weapons;
  Timer shootRepeatTimer = new Timer();


  void LateUpdate()
  {
    if( !Global.instance.Updating )
      return;
    Cursor.position = CursorWorldPosition;
  }
}