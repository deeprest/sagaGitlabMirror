using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  public Transform instigator;
  public Damage ContactDamage;
  public SpriteAnimator animator;
  public float raycastDistance = 0.2f;
  public float timeout = 2;
  Timer timeoutTimer;
  public Vector3 velocity;
  public CircleCollider2D circle;
  public static string[] CollideLayers = { "Default" ,"character", "triggerAndCollision", "enemy" };
  // check first before spawning to avoid colliding with these layers on the first frame
  public static string[] NoShootLayers = { "Default"};
  public AnimSequence HitEffect;
  public bool AlignXToMovementDirection = false;

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
  }

  void Start()
  {
    timeoutTimer = new Timer( timeout, null, delegate()
    {
      if( gameObject!=null )
        Destroy( gameObject );
    } );

    if( AlignXToMovementDirection )
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
      transform.position = hit.point;
      animator.Play( HitEffect );
      //GameObject go = GameObject.Instantiate( HitEffect, transform.position, Quaternion.identity );
      //SpriteAnimator sa = go.GetComponent<SpriteAnimator>();
      float duration = animator.CurrentSequence.GetDuration();
      new Timer( duration, null, delegate
      {
        Destroy( gameObject );
      } );
        
      /*ParticleSystem ps = go.GetComponent<ParticleSystem>();
      Timer t = new Timer( ps.main.duration, null, delegate
      {
        Destroy( go );
      } );*/

      timeoutTimer.Stop( false );
    }
    else
    {
      transform.position += velocity * Time.deltaTime;
    }
  }


}
