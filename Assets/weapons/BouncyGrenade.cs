using UnityEngine;
using System.Collections;

public class BouncyGrenade : Projectile
{
  public GameObject explosion;

  Timer timeoutTimer;

  //public override void OnFire()
  void Start()
  {
    GetComponent<Rigidbody2D>().velocity = new Vector2( velocity.x, velocity.y );

    timeoutTimer = new Timer( timeout, null, delegate ()
    {
      if( gameObject != null )
        Boom();
    } );
  }

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
  }


  void Boom()
  {
    Instantiate( explosion, transform.position, Quaternion.identity );
    // HACK due to a Unity RIGIDBODY2D RemoveContact() crash bug, deactivate gameobject before a delayed destruction.
    gameObject.SetActive( false );
    Destroy( gameObject, 1f );
  }

  void Update()
  {
    /*RaycastHit2D hit = Physics2D.CircleCast( transform.position, circle.radius, velocity, raycastDistance, LayerMask.GetMask( CollideLayers ) );
    if( hit.transform != null && (instigator == null || hit.transform != instigator) )
    {
      Boom();
    }*/
  }

}
