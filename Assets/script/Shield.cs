using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IDamage
{
  [SerializeField] Character character;
  [SerializeField] Collider2D collider;
  public AudioSource source;
  public AudioClip soundHit;
  public Damage damage;
  public float lightIntensity;
  public Light light;
  public SpriteRenderer sr;
  public float pulseDuration = 1;
  Timer pulseTimer = new Timer();
  public float hitPushSpeed = 1;
  public float hitPushDuration = 1;
  public float reflectOffset = 0.1f;

  void Awake()
  {
    sr.material.SetFloat( "_FlashAmount", 0 );
    light.intensity = 0;
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
      //Global.instance.AudioOneShot( soundHit, transform.position );
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
      // Instantiate the projectile because the original has called Physics2D.IgnoreCollision() and cannot be undone.
      Vector2 pos = transform.position + Vector3.Project( (Vector3)d.point - transform.position, transform.right );
      GameObject go = Instantiate( projectile.gameObject, pos, Quaternion.identity );
      Projectile newProjectile = go.GetComponent<Projectile>();
      if( character != null )
        newProjectile.instigator = character;
      newProjectile.velocity = Vector3.Reflect( newProjectile.velocity, transform.up );
      newProjectile.transform.position = pos;
      newProjectile.ignore.Add( transform );

      switch( projectile.weapon.weaponType )
      {
        case Weapon.WeaponType.Projectile:
        Destroy( projectile.gameObject );
        break;

        case Weapon.WeaponType.Laser:
        break;
      }

    }

    return false;
  }


}
