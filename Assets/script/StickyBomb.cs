using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

public class StickyBomb : Projectile, IDamage
{
  public GameObject explosion;
  Timer timeoutTimer = new Timer();
  Timer pulseTimer = new Timer();
  [SerializeField] float pulseInterval = 0.2f;
  [SerializeField] new Light2D light;
  [SerializeField] float radiusFudge;
  public bool AlignRotationToVelocity = true;
  [SerializeField] float AttachDuration = 2;
  Rigidbody2D body;
  [SerializeField] float BoomRadius = 1;
  bool flagBoom = false;
  bool flagHit = false;

  void Start()
  {
    body = GetComponent<Rigidbody2D>();
    body.velocity = new Vector2( velocity.x, velocity.y );
    timeoutTimer.Start( timeout, null, delegate { TakeDamage( ContactDamage ); } );
    pulseTimer.Start( int.MaxValue, pulseInterval, delegate ( Timer obj ) { light.enabled = !light.enabled; }, null );
    dmg = Instantiate( ContactDamage );
  }

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
    pulseTimer.Stop( false );
  }


  void FixedUpdate()
  {
    // +X = forward
    if( AlignRotationToVelocity )
      transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( body.velocity.normalized.y, body.velocity.normalized.x ) ) );
  }

  Collider2D[] clds = new Collider2D[4];
  Damage dmg;

  void Boom()
  {
    if( flagBoom )
      return;
    flagBoom = true;
    // disable collider before explosion to avoid unnecessary OnCollisionEnter2D() calls
    circle.enabled = false;
    timeoutTimer.Stop( false );
    int count = Physics2D.OverlapCircleNonAlloc( transform.position, BoomRadius, clds, Global.DamageCollideLayers );
    for( int i = 0; i < count; i++ )
    {
      if( clds[i] != null )
      {
        IDamage dam = clds[i].GetComponent<IDamage>();
        if( dam != null )
        {
          dmg.instigator = instigator;
          dmg.damageSource = transform;
          dmg.point = transform.position;
          dam.TakeDamage( dmg );
        }
      }
    }
    Destroy( gameObject );
    Instantiate( explosion, transform.position, Quaternion.identity );
  }

  public bool TakeDamage( Damage damage )
  {
    Boom();
    return true;
  }

  void OnCollisionEnter2D( Collision2D collision )
  {
    if( flagHit )
      return;
    if( (Global.StickyBombCollideLayers & (1 << collision.gameObject.layer)) > 0 && collision.transform != null &&
      (instigator == null || !collision.transform.IsChildOf( instigator.transform )) && !ignore.Contains( collision.transform ) )
    {
      flagHit = true;

      Projectile projectile = collision.transform.GetComponent<Projectile>();
      if( projectile != null && (instigator == null || projectile.instigator == null || projectile.instigator != instigator) )
        Boom();

      AlignRotationToVelocity = false;
      transform.parent = collision.transform;
      // +X = forward
      transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( -collision.contacts[0].normal.y, -collision.contacts[0].normal.x ) ) );
      //body.simulated = false;
      body.bodyType = RigidbodyType2D.Static;
      animator.Play( "flash" );
      timeoutTimer.Start( AttachDuration, null, delegate
      {
        Boom();
      } );
    }
  }

}
