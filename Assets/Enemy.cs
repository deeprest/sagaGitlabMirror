using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour , IDamage
{
  public Vector3 velocity = Vector3.zero;
  public Vector3 inertia = Vector3.zero;
  public float friction = 0.5f;
  public float raylength = 0.1f;
  public Vector2 box = new Vector2( 0.3f, 0.3f );
  public float contactSeparation = 0.01f;
  string[] CollideLayers = new string[] { "foreground" };
  public bool collideRight = false;
  public bool collideLeft = false;
  public bool collideHead = false;
  public bool collideFeet = false;
  RaycastHit2D hitRight;
  RaycastHit2D hitLeft;

  public SpriteAnimator animator;
  string anim = "idle";

  int health = 5;
  public GameObject explosion;
  public float hitPush = 4;

  void Update()
  {
    collideRight = false;
    collideLeft = false;
    collideHead = false;
    collideFeet = false;

    RaycastHit2D[] hits;

    hits = Physics2D.BoxCastAll( transform.position, box * 2, 0, velocity, raylength, LayerMask.GetMask( "trigger" ) );
    foreach( var hit in hits )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
        dam.TakeDamage( new Damage( transform, DamageType.Generic, 1 ) );
    }

    const float corner = 0.707f;
    Vector2 adjust = transform.position;

    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.down, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), LayerMask.GetMask( CollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y > corner )
      {
        collideFeet = true;
        adjust.y = hit.point.y + box.y + contactSeparation;
        break;
      }
    }
    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.up, Mathf.Max( raylength, velocity.y * Time.deltaTime ), LayerMask.GetMask( CollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y < -corner )
      {
        collideHead = true;
        adjust.y = hit.point.y - box.y - contactSeparation;
        break;
      }
    }
    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.left, Mathf.Max( raylength, -velocity.x * Time.deltaTime ), LayerMask.GetMask( CollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        hitLeft = hit;
        adjust.x = hit.point.x + box.x + contactSeparation;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.right, Mathf.Max( raylength, velocity.x * Time.deltaTime ), LayerMask.GetMask( CollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        hitRight = hit;
        adjust.x = hit.point.x - box.x - contactSeparation;
        break;
      }
    }
    transform.position = adjust;

    velocity.y += -Global.Gravity * Time.deltaTime;

    if( collideFeet )
    {
      velocity.x -= (velocity.x * friction) * Time.deltaTime;
      velocity.y = Mathf.Max( velocity.y, 0 );
    }
    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
    }
    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
    }

    animator.Play( anim );

    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
    transform.position += velocity * Time.deltaTime;
  }



  public void TakeDamage( Damage d )
  {
    health -= d.amount;
    velocity += ( transform.position - d.point ) * hitPush;
    GameObject.Instantiate( explosion, transform.position, Quaternion.identity );
    if( health <= 0 )
      Destroy( gameObject );
  }
}
