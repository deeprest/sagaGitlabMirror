using UnityEngine;
using System.Collections;

public class XBuster : Projectile
{
  public float HitTimeout = 0.5f;
  public new Light light;

  Timer timeoutTimer;

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
  }

  void Start()
  {
    if( StartSound != null )
      Global.instance.AudioOneShot( StartSound, transform.position );

    timeoutTimer = new Timer( timeout, null, delegate ()
    {
      if( gameObject != null )
        Destroy( gameObject );
    } );

    transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( velocity.normalized.y, velocity.normalized.x ) ) );
  }

  void Update()
  {
    RaycastHit2D hit = Physics2D.CircleCast( transform.position, circle.radius, velocity, raycastDistance, LayerMask.GetMask( CollideLayers ) );
    if( hit.transform != null && (instigator == null || hit.transform != instigator) )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate<Damage>( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.point;
        dam.TakeDamage( dmg );
      }

      enabled = false;
      light.enabled = false;
      transform.position = hit.point;
      animator.Play( "hit" );
      timeoutTimer.Stop( false );
      timeoutTimer = new Timer( HitTimeout, null, delegate
      {
        if( gameObject != null )
          Destroy( gameObject );
      } );

    }
    else
    {
      transform.position += velocity * Time.deltaTime;
    }
  }
}