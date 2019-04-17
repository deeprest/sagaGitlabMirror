using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

  public Animator animator;
  public CircleCollider2D circle;

  public Transform instigator;

  public Damage ContactDamage;
  public float speed = 1;
  public float raycastDistance = 0.2f;
  public float timeout = 2;
  protected Timer timeoutTimer;
  public Vector3 velocity;
  public static string[] CollideLayers = { "Default", "character", "triggerAndCollision", "enemy" };
  // check first before spawning to avoid colliding with these layers on the first frame
  public static string[] NoShootLayers = { "Default" };


  public AudioClip StartSound;

  public virtual void OnFire()
  {

  }

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
  }

  void Start()
  {
    if( StartSound!=null )
      Global.instance.AudioOneShot( StartSound, transform.position );

    timeoutTimer = new Timer( timeout, null, delegate ()
    {
      if( gameObject != null )
        Destroy( gameObject );
    } );
  }


    private void Update()
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
    }
    else
    {
      transform.position += velocity * Time.deltaTime;
    }
  }
}
