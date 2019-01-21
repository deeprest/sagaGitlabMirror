using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  public GameObject instigator;
  public SpriteAnimator animator;
  public float raycastDistance = 0.2f;
  public float timeout = 2;
  Timer timeoutTimer;
  public Vector3 velocity;
  public CircleCollider2D circle;
  public static string[] CollideLayers = new string[] { "foreground" ,"character"};
  // check first before spawning to avoid colliding with these layers on the first frame
  public static string[] NoShootLayers = new string[] { "foreground"};
  public AnimSequence HitEffect;
  public bool AlignXToMovementDirection = false;

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
  // Update is called once per frame
  void Update()
  {
    RaycastHit2D hit = Physics2D.CircleCast( transform.position, circle.radius, velocity, raycastDistance, LayerMask.GetMask( CollideLayers ) );
    if( hit.transform != null && hit.transform.gameObject != instigator )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
        dam.TakeDamage( new Damage( transform, DamageType.Generic, 1, hit.point ) );
      
      enabled = false;
      transform.position = hit.point;
      animator.Play( HitEffect, true );
      //GameObject go = GameObject.Instantiate( HitEffect, transform.position, Quaternion.identity );
      //SpriteAnimator sa = go.GetComponent<SpriteAnimator>();
      float duration = ( 1.0f / animator.CurrentSequence.fps ) * animator.CurrentSequence.sprites.Length;
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
    transform.position += velocity * Time.deltaTime;
  }


}
