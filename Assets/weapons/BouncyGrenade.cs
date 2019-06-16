using UnityEngine;
using System.Collections;

public class BouncyGrenade : Projectile
{
  public GameObject explosion;
  Timer timeoutTimer;
  static string[] BouncyCollideLayers = { "character", "triggerAndCollision", "enemy", "projectile" };

  void Start()
  {
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
    // HACK due to a Unity RIGIDBODY2D RemoveContact() crash bug, deactivate gameobject before a delayed destruction.
    gameObject.SetActive( false );
    Destroy( gameObject );
    //Destroy( gameObject, 1f );
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
    if( (LayerMask.GetMask( BouncyCollideLayers ) & (1 << hit.gameObject.layer)) > 0 )
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
