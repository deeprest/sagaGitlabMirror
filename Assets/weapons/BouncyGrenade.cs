using UnityEngine;
using System.Collections;

public class BouncyGrenade : Projectile
{
  public GameObject explosion;
  Timer timeoutTimer;

  void Start()
  {
    // weapon plays the sound instead
    //if( StartSound != null )
      //Global.instance.AudioOneShot( StartSound, transform.position );
    GetComponent<Rigidbody2D>().velocity = new Vector2( velocity.x, velocity.y );
    timeoutTimer = new Timer( timeout, null, Boom );
  }

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
  }


  void Boom()
  {
    if( gameObject != null && !gameObject.activeSelf )
      return;
    Instantiate( explosion, transform.position, Quaternion.identity );
    timeoutTimer.Stop( false );
    Global.instance.Destroy( gameObject );
  }

  /*void Update()
  {
    RaycastHit2D hit = Physics2D.CircleCast( transform.position, circle.radius, velocity, raycastDistance, LayerMask.GetMask( BouncyCollideLayers ) );
    if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator )) )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.point;
        dam.TakeDamage( dmg );
      }
      Boom();
    }
  }*/

  private void OnCollisionEnter2D( Collision2D hit )
  {
    if( (LayerMask.GetMask( Global.BouncyCollideLayers ) & (1 << hit.gameObject.layer)) > 0 )
    if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator )) )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.GetContact( 0 ).point;
        dam.TakeDamage( dmg );
      }
      Boom();
    }
  }
}
