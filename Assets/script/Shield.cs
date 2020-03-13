using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Shield : MonoBehaviour, IDamage
{
  [SerializeField] Character character;
  [SerializeField] new Collider2D collider;
  public AudioSource source;
  public AudioClip soundHit;
  public Damage damage;
  public float lightIntensity;
  public new Light2D light;
  public SpriteRenderer sr;
  public float pulseDuration = 1;
  Timer pulseTimer = new Timer();
  public float hitPushSpeed = 1;
  public float hitPushDuration = 1;
  public float reflectOffset = 0.1f;

  void OnDestroy()
  {
    pulseTimer.Stop( false );
  }

  void Awake()
  {
    sr.material.SetFloat( "_FlashAmount", 0 );
    light.intensity = 0;
    character = GetComponentInParent<Character>();
  }

  void Update()
  {
    sr.flipX = transform.up.x < 0;
  }

  public bool TakeDamage( Damage d )
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

    Character chr = d.instigator.GetComponent<Character>();
    if( chr != null )
    {
      if( damage != null )
        chr.TakeDamage( damage );
      chr.Push( hitPushSpeed * ((Vector2)(d.instigator.transform.position - transform.position)).normalized, hitPushDuration );
    }

    Projectile projectile = d.instigator.GetComponent<Projectile>();
    if( projectile != null )
    {
      switch( projectile.weapon.weaponType )
      {
        case Weapon.WeaponType.Projectile:
        projectile.transform.position = transform.position + Vector3.Project( (Vector3)d.point - transform.position, transform.right );
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

    return false;
  }


}
