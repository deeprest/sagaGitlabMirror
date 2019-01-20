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
  string[] CollideLayers = new string[] { "foreground" ,"character"};
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
    transform.position += velocity * Time.deltaTime;
    RaycastHit2D hit = Physics2D.CircleCast( transform.position, circle.radius, velocity, raycastDistance, LayerMask.GetMask( CollideLayers ) );
    if( hit.transform != null )
    {
      if( hit.transform.gameObject == instigator )
        return;
      
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
        dam.TakeDamage( new Damage( transform, DamageType.Generic, 1 ) );
      
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
  }


}
