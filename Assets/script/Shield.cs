using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Shield : MonoBehaviour, IDamage
{
  [SerializeField] Entity character;
  [SerializeField] Collider2D collider;
  public AudioSource source;
  public AudioClip soundHit;
  public Damage damage;
  public float lightIntensity;
  public Light2D light;
  public SpriteRenderer sr;
  public float pulseDuration = 1;
  Timer pulseTimer = new Timer();
  public float hitPushSpeed = 1;
  public float hitPushDuration = 1;
  public float reflectOffset = 0.1f;
  [SerializeField] private float MaximumDamage = 2;

  void OnDestroy()
  {
    pulseTimer.Stop( false );
  }

  void Awake()
  {
    sr.material.SetFloat( "_FlashAmount", 0 );
    light.intensity = 0;
    character = GetComponentInParent<Entity>();
  }

  void Update()
  {
    sr.flipX = transform.up.x < 0;
  }

  public bool TakeDamage( Damage damage )
  {
    if( soundHit != null )
    {
      source.clip = soundHit;
      source.Play();
    }

    sr.material.SetFloat( "_FlashAmount", 1 );
    light.intensity = lightIntensity;

    pulseTimer.Start( pulseDuration, delegate ( Timer tmr )
    {
      sr.material.SetFloat( "_FlashAmount", 1.0f - tmr.ProgressNormalized );
      light.intensity = (1.0f - tmr.ProgressNormalized) * lightIntensity;
    }, delegate
    {
      sr.material.SetFloat( "_FlashAmount", 0 );
      light.intensity = 0;
    } );

    Entity chr = damage.damageSource.GetComponent<Entity>();
    if( chr != null )
    {
      if( this.damage != null )
        chr.TakeDamage( this.damage );
      chr.OverrideVelocity( hitPushSpeed * ((Vector2)(damage.damageSource.transform.position - transform.position)).normalized, hitPushDuration );
    }

    if( damage.amount <= MaximumDamage )
    {
      Projectile projectile = damage.damageSource.GetComponent<Projectile>();
      if( projectile != null )
      {
        switch( projectile.weapon.weaponType )
        {
          case Weapon.WeaponType.Projectile:
            projectile.transform.position = transform.position + Vector3.Project( (Vector3) damage.point - transform.position, transform.right );
            projectile.velocity = Vector3.Reflect( projectile.velocity, transform.up );
            Physics2D.IgnoreCollision( projectile.circle, collider, false );
            foreach( var cldr in projectile.instigator.IgnoreCollideObjects )
            {
              if( cldr == null )
                Debug.Log( "ignorecolideobjects null" );
              else
                Physics2D.IgnoreCollision( projectile.circle, cldr, false );
            }
            if( character != null )
              projectile.instigator = character;
            projectile.ignore.Add( transform );
            break;

          case Weapon.WeaponType.Laser:
            // create second beam
            break;
        }

      }
    }

    return false;
  }

}
