using UnityEngine;
using System.Collections;

public class BasicProjectile : Projectile
{
  public float HitTimeout = 0.1f;
  public new Light light;
  public Vector3 constantAcceleration;
  Timer timeoutTimer;
  int HitCount;
  public int DieAfterHitCount;

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
  }

  void Start()
  {
    // let the weapon play the sound instead
    /*if( StartSound != null )
      Global.instance.AudioOneShot( StartSound, transform.position );*/

    timeoutTimer = new Timer( timeout, null, delegate ()
    {
      if( gameObject != null )
        Destroy( gameObject );
    } );

    transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( velocity.normalized.y, velocity.normalized.x ) ) );
  }

  void Hit( Vector3 position )
  {
    transform.position = position;
    enabled = false;
    light.enabled = false;
    animator.Play( "hit" );
    timeoutTimer.Stop( false );
    timeoutTimer = new Timer( HitTimeout, null, delegate
    {
      if( gameObject != null )
        Destroy( gameObject );
    } );
  }

  void Update()
  {
    RaycastHit2D hit = Physics2D.CircleCast( transform.position, circle.radius, velocity, raycastDistance, LayerMask.GetMask( Global.DefaultProjectileCollideLayers ) );
    if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator )) )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate<Damage>( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.point;
        if( dam.TakeDamage( dmg ) )
        {
          HitCount++;
          if( HitCount >= DieAfterHitCount )
          {
            Hit( hit.point );
          }
        }
      }
      else
      {
        Hit( hit.point );
      }
    }
    else
    {
      velocity += constantAcceleration * Time.deltaTime;
      transform.position += velocity * Time.deltaTime;
    }
    //SpriteRenderer sr = GetComponent<SpriteRenderer>();
    //sr.color = Global.instance.shiftyColor;
    //light.color = Global.instance.shiftyColor;
  }
}