using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character, IDamage
{
  public bool UseGravity = true;
  public Vector3 velocity = Vector3.zero;
  public Vector3 inertia = Vector3.zero;
  public float friction = 0.05f;
  public float airFriction = 0.05f;
  public float raylength = 0.01f;
  public float contactSeparation = 0.01f;
  public Vector2 box = new Vector2( 0.3f, 0.3f );
  public  string[] CollideLayers = { "Default" };
  public  string[] DamageLayers = { "character" };
  public bool collideRight = false;
  public bool collideLeft = false;
  public bool collideTop = false;
  public bool collideBottom = false;
  protected RaycastHit2D[] hits;
  RaycastHit2D hitRight;
  RaycastHit2D hitLeft;

  public new SpriteRenderer renderer;
  public Animator animator;

  public int health = 5;
  public GameObject explosion;
  public AudioClip soundHit;
  public float hitPush = 4;
  Timer flashTimer = new Timer();
  public float flashInterval = 0.05f;
  public int flashCount = 5;
  bool flip = false;
  readonly float flashOn = 1f;

  public Damage ContactDamage;

  protected System.Action UpdateEnemy;
  protected System.Action UpdateCollision;
  protected System.Action UpdatePosition;
  protected System.Action UpdateHit;

  protected void EnemyStart()
  {
    animator.Play( "idle" );
    //renderer.material.SetFloat( "_FlashAmount", 0 );
    UpdateHit = BoxHit;
    UpdateCollision = BoxCollision;
    UpdatePosition = BasicPosition;
  }

  void Update()
  {
    if( UpdateEnemy != null )
      UpdateEnemy();

    if( UpdateHit != null )
      UpdateHit();

    if( UpdateCollision !=null )
      UpdateCollision();

    if( UpdatePosition != null )
      UpdatePosition();
  }

  protected void BoxHit()
  {
    hits = Physics2D.BoxCastAll( transform.position, box * 2, 0, velocity, raylength, LayerMask.GetMask( DamageLayers ) );
    foreach( var hit in hits )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.point;
        dam.TakeDamage( dmg );
      }
    }
  }

  protected void BasicPosition()
  {
    if( UseGravity )
      velocity.y += -Global.Gravity * Time.deltaTime;

    if( collideTop )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
    if( collideBottom )
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

    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
    velocity -= (velocity * airFriction) * Time.deltaTime;
    transform.position += velocity * Time.deltaTime;
  }

  protected void BoxCollision()
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;

    const float corner = 0.707f;
    Vector2 adjust = transform.position;

    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.down, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), LayerMask.GetMask( CollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + box.y + contactSeparation;
        break;
      }
    }
    hits = Physics2D.BoxCastAll( adjust, box * 2, 0, Vector2.up, Mathf.Max( raylength, velocity.y * Time.deltaTime ), LayerMask.GetMask( CollideLayers ) );
    foreach( var hit in hits )
    {
      if( hit.normal.y < -corner )
      {
        collideTop = true;
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

  }

  protected virtual void Die()
  {
    Instantiate( explosion, transform.position, Quaternion.identity );
    Destroy( gameObject );
  }

  public void TakeDamage( Damage d )
  {
    health -= d.amount;
    velocity += (transform.position - d.point) * hitPush;
    if( health <= 0 )
    {
      flashTimer.Stop( false );
      Die();
    }
    else
    {
      Global.instance.AudioOneShot( soundHit, transform.position );

      // color pulse
      flip = false;
      renderer.material.SetFloat( "_FlashAmount", flashOn );
      flashTimer.Start( flashCount * 2, flashInterval, delegate ( Timer t )
      {
        flip = !flip;
        if( flip )
          renderer.material.SetFloat( "_FlashAmount", flashOn );
        else
          renderer.material.SetFloat( "_FlashAmount", 0 );
      }, delegate
      {
        renderer.material.SetFloat( "_FlashAmount", 0 );
      } );

    }
  }


}
