using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IDamage
{
  public AudioClip soundHit;
  public Damage damage;
  public float lightIntensity;
  public Light light;
  public SpriteRenderer sr;
  public float pulseDuration = 1;
  Timer pulseTimer = new Timer();
  public float hitPushSpeed = 1;
  public float hitPushDuration = 1;

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
      Global.instance.AudioOneShot( soundHit, transform.position );

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
      projectile.velocity = Vector3.Reflect( projectile.velocity, transform.up );
    }

    return false;
  }
}
