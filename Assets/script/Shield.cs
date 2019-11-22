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
      Vector2 pos = transform.position + Vector3.Project( (Vector3)d.point - transform.position, transform.right );
      projectile.velocity = Vector3.Reflect( projectile.velocity, transform.up );
      projectile.transform.position = pos + projectile.velocity.normalized * projectile.circle.radius * 2;
      Debug.DrawLine( d.point, projectile.velocity, Color.red );
    }

    return false;
  }
  public float reflectOffset = 0.1f;

}
