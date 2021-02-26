using UnityEngine;

// TODO dash smoke, dash sound
public class Mech : Entity
{
  public float Scale = 1;
  
  public AudioSource audio;

  public bool facingRight;
  public BoxCollider2D fist;
  public BoxCollider2D torso;

  [SerializeField] float sightRange = 6;
  [SerializeField] float withinpunchRange = 3;
  [SerializeField] float jumpRange = 2;
  [SerializeField] float small = 0.1f;
  [SerializeField] float moveSpeed = 0.5f;
  [SerializeField] float jumpSpeed = 5;
  [SerializeField] float dashSpeed = 5;
  [SerializeField] float shootInterval = 1;
  [SerializeField] float shootUp = 5;
  // durations
  public float jumpDuration = 0.4f;
  public float dashDuration = 1;
  public float landDuration = 0.1f;
  bool onGround;
  Timer jumpTimer = new Timer();
  Timer jumpRepeat = new Timer();
  Timer dashTimer = new Timer();
  Timer dashCooldownTimer = new Timer();
  Timer shootRepeatTimer = new Timer();

  public AudioClip soundJump;
  public AudioClip soundReflect;

  [SerializeField] Weapon weapon;
  [SerializeField] Transform shotOrigin;

  // ignore damage below this value
  [SerializeField] float DamageThreshold = 2;
  int FistDamageLayers;

  // sight, target
  Collider2D[] results = new Collider2D[8];
  int LayerMaskCharacter;
  [SerializeField] Entity Target;
  Timer SightPulseTimer = new Timer();

  protected override void Start()
  {
    base.Start();
    UpdateLogic = BossUpdate;
    UpdateHit = MechHit;
    Physics2D.IgnoreCollision( box, fist );
    Physics2D.IgnoreCollision( box, torso );
    Physics2D.IgnoreCollision( torso, fist );
    LayerMaskCharacter = LayerMask.GetMask( new string[] { "character" } );
    FistDamageLayers = LayerMask.GetMask( new string[] {"character", "destructible"} );
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

  protected override void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    base.OnDestroy();
    jumpTimer.Stop( false );
    jumpRepeat.Stop( false );
    dashTimer.Stop( false );
    dashCooldownTimer.Stop( false );
    shootRepeatTimer.Stop( false );
    SightPulseTimer.Stop( false );
  }

  void BossUpdate()
  {
    if( velocity.y < 0 )
      StopJump();

    if( Target == null )
    {
      velocity.x = 0;
      animator.Play( "idle" );
      // todo wander or patrol
    }
    else
    {
      Vector3 targetpos = Target.transform.position;
      Vector3 delta = targetpos - transform.position;

      //  do not change directions while dashing
      if( !dashTimer.IsActive )
        facingRight = delta.x >= 0;

      if( collideBottom && delta.sqrMagnitude < sightRange * sightRange )
      {
        if( !dashTimer.IsActive && !jumpTimer.IsActive )
        {
          if( delta.y > 1f && delta.y < 5 && dashCooldownTimer.ProgressNormalized > 0.5f && !jumpRepeat.IsActive && !jumpTimer.IsActive && delta.sqrMagnitude < jumpRange * jumpRange )
          {
            StartJump();
          }
          else if( !dashCooldownTimer.IsActive && delta.y > 0f && delta.y < 2 && Mathf.Abs( delta.x ) < withinpunchRange )
          {
            animator.Play( "punchdash" );
            StartDash();
          }
          else if( Mathf.Abs( delta.x ) < small )
          {
            animator.Play( "idle" );
            velocity.x = 0;
          }
          else
          {
            animator.Play( "walk" );
            velocity.x = Mathf.Sign( delta.x ) * moveSpeed;
            if( weapon != null && !shootRepeatTimer.IsActive )
            {
              Shoot( targetpos - shotOrigin.position + Vector3.up * shootUp );
              /*
              // todo check line of site to target
              // todo have a "ready to fire" animation play to warn player
              //hit = Physics2D.LinecastAll( shotOrigin.position, player );
              hit = Physics2D.Linecast( shotOrigin.position, targetpos, LayerMask.GetMask( new string[] { "Default", "character" } ) );
              if( hit.transform == Target.transform )
                Shoot( targetpos - shotOrigin.position + Vector3.up * shootUp );
              
              
              Entity visibleTarget = null;
              // check line of sight to potential target
              hitCount = Physics2D.LinecastNonAlloc( shotOrigin.position, targetpos, RaycastHits, Global.CharacterCollideLayers );
              if( hitCount == 0 ) 
              {
                
              }
              */
            }
          }
        }
      }
    }

    transform.localScale = new Vector3( facingRight ? Scale : -Scale, Scale, 1 );

    bool oldGround = onGround;
    onGround = collideBottom || (collideLeft && collideRight);
    if( onGround && !oldGround )
    {
      //landTimer.Start( landDuration, null, null )
    }
  }
  
  void StartJump()
  {
    velocity.y = jumpSpeed;
    audio.PlayOneShot( soundJump );
    //dashSmoke.Stop();
    animator.Play( "jump" );
    jumpRepeat.Start( 2, null, null );
    jumpTimer.Start( jumpDuration, null, delegate { StopJump(); } );
  }

  void StopJump()
  {
    jumpTimer.Stop( false );
    velocity.y = Mathf.Min( velocity.y, 0 );
  }

  void StartDash()
  {
    dashTimer.Start( dashDuration, ( x ) => {
      velocity.x = (facingRight ? 1 : -1) * dashSpeed;
    }, delegate
    {
      dashCooldownTimer.Start( 2 );
    } );
  }

  void StopDash()
  {
    dashTimer.Stop( false );
  }

  void Shoot( Vector3 shoot )
  {
    shootRepeatTimer.Start( shootInterval, null, null );
    if( !Physics2D.Linecast( transform.position, shotOrigin.position, Global.ProjectileNoShootLayers ) )
      weapon.FireWeapon( this, shotOrigin.position, shoot );
  }


  void MechHit()
  {
    if( ContactDamage == null )
      return;
    // body hit
    hitCount = Physics2D.BoxCastNonAlloc( body.position, box.size, 0, velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.transform.root == transform.root )
        continue;
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

    hitCount = Physics2D.BoxCastNonAlloc( torso.transform.position, torso.size, 0, velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.transform.root == transform.root )
        continue;
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

    // fist hit
    hitCount = Physics2D.BoxCastNonAlloc( fist.transform.position, fist.size, 0, Vector2.zero, RaycastHits, raylength, FistDamageLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.transform.root == transform.root )
        continue;
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


  public override bool TakeDamage( Damage damage )
  {
    if( damage.instigator != null && !IsEnemyTeam( damage.instigator.Team ) )
      return false;
    if( damage.amount < DamageThreshold )
    {
      if( soundReflect != null )
        audio.PlayOneShot( soundReflect );

      Projectile projectile = damage.damageSource.GetComponent<Projectile>();
      if( projectile != null )
      {
        switch( projectile.weapon.weaponType )
        {
          case Weapon.WeaponType.Projectile:
            if( damage.instigator == null )
              projectile.velocity = Vector3.Reflect( projectile.velocity,(damage.point - (Vector2)transform.position).normalized ); 
            else
              projectile.velocity = Vector3.Reflect( projectile.velocity, (damage.instigator.transform.position - transform.position).normalized ); 
            
            Physics2D.IgnoreCollision( projectile.circle, box, false );
            Physics2D.IgnoreCollision( projectile.circle, torso, false );
            Physics2D.IgnoreCollision( projectile.circle, fist, false );

            foreach( var cldr in projectile.instigator.IgnoreCollideObjects )
            {
              if( cldr == null )
                Debug.Log( "ignorecolideobjects null" );
              else
                Physics2D.IgnoreCollision( projectile.circle, cldr, false );
            }

            projectile.instigator = this;
            projectile.ignore.Add( transform );
            break;

            case Weapon.WeaponType.Laser:
            // create second beam
          break;
        }
      }
      return false;
    }
    return base.TakeDamage( damage );
  }
}
