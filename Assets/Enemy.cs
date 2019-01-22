using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour , IDamage
{
  public bool UseGravity = true;
  public Vector3 velocity = Vector3.zero;
  public Vector3 inertia = Vector3.zero;
  public float friction = 0.5f;
  public float raylength = 0.01f;
  public float contactSeparation = 0.01f;
  public Vector2 box = new Vector2( 0.3f, 0.3f );
  string[] CollideLayers = new string[] { "foreground" };
  public bool collideRight = false;
  public bool collideLeft = false;
  public bool collideHead = false;
  public bool collideFeet = false;
  RaycastHit2D hitRight;
  RaycastHit2D hitLeft;

  [SerializeField] new SpriteRenderer renderer;
  public SpriteAnimator animator;
  public AnimSequence idle;

  public int health = 5;
  public GameObject explosion;
  public AudioClip soundHit;
  public float hitPush = 4;
  Timer flashTimer = new Timer();
  public float flashInterval = 0.1f;
  public int flashCount = 5;
  bool flip = false;
  public float emissive = 0.5f;

  public Damage ContactDamage;

  void Start()
  {
    renderer.material.SetTexture( "_EmissiveTex", null );
    renderer.material.SetFloat( "_EmissiveAmount", 0 );
  }

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
      {
        Damage dmg = ScriptableObject.Instantiate<Damage>( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.point;
        dam.TakeDamage( dmg );
      }
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

    if( UseGravity )
      velocity.y += -Global.Gravity * Time.deltaTime;

    if( collideFeet )
    {
      velocity.x -= ( velocity.x * friction ) * Time.deltaTime;
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

    animator.Play( idle );

    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
    transform.position += velocity * Time.deltaTime;
  }



  public void TakeDamage( Damage d )
  {
    health -= d.amount;
    velocity += ( transform.position - d.point ) * hitPush;
    if( health <= 0 )
    {
      flashTimer.Stop( false );
      GameObject.Instantiate( explosion, transform.position, Quaternion.identity );
      Destroy( gameObject );
    }
    else
    {
      Global.instance.AudioOneShot( soundHit, transform.position );

      // color pulse
      flip = false;
      renderer.material.SetFloat( "_EmissiveAmount", emissive );
      flashTimer.Start( flashCount * 2, flashInterval, delegate(Timer t )
      {
        flip = !flip;
        if( flip )
          renderer.material.SetFloat( "_EmissiveAmount", emissive );
        else
          renderer.material.SetFloat( "_EmissiveAmount", 0 );
      }, delegate
      {
        renderer.material.SetFloat( "_EmissiveAmount", 0 );
      } );
      
    }
  }


}
