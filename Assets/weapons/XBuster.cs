using UnityEngine;
using System.Collections;
using System.Linq;

public class XBuster : Projectile
{
  public GameObject HitEffect;

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

      Destroy( gameObject );

      enabled = false;
      transform.position = hit.point;


      GameObject hitGO = Instantiate( HitEffect, transform.position, transform.rotation );
      //float duration = hitGO.GetComponent<Animator>().runtimeAnimatorController.animationClips.First( x => x.name == "hit" ).length;
      //new Timer( duration, null, delegate
      //{
      //  if( hitGO != null )
      //    Destroy( hitGO );
      //} );


      /*ParticleSystem ps = go.GetComponent<ParticleSystem>();
      Timer t = new Timer( ps.main.duration, null, delegate
      {
        Destroy( go );
      } );*/

      //timeoutTimer.Stop( false );
    }
    else
    {
      transform.position += velocity * Time.deltaTime;
    }
  }
}