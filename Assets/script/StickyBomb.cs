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
  Transform hitTransform;
  [SerializeField] float BoomRadius = 1;
  bool alreadyBoom = false;

  void Start()
  {
    body = GetComponent<Rigidbody2D>();
    body.velocity = new Vector2( velocity.x, velocity.y );
    timeoutTimer.Start( timeout, null, delegate { TakeDamage( ContactDamage ); } );
    pulseTimer.Start( int.MaxValue, pulseInterval, delegate ( Timer obj ) { light.enabled = !light.enabled; }, null );
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

  void Boom()
  {
    if( alreadyBoom )
      return;
    alreadyBoom = true;
    // disable collider before explosion to avoid unnecessary OnCollisionEnter2D() calls
    GetComponent<Collider2D>().enabled = false;
    timeoutTimer.Stop( false );
    Collider2D[] clds = Physics2D.OverlapCircleAll( transform.position, BoomRadius, Global.StickyBombCollideLayers );
    for( int i = 0; i < clds.Length; i++ )
    {
      if( clds[i] != null )
      {
        IDamage dam = clds[i].GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
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

  void OnCollisionEnter2D( Collision2D hit )
  {
    if( (Global.StickyBombCollideLayers & (1 << hit.gameObject.layer)) > 0 )
      if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator.transform )) && !ignore.Contains( hit.transform ) )
      {
        // ignore projectiles from this instigator
        Projectile projectile = hit.transform.GetComponent<Projectile>();
        if( projectile != null )
        {
          // stickybomb will simply bounce off of other stickybomb
          if( instigator != null && projectile.instigator != null && projectile.instigator == instigator )
            return;
          //if( projectile.instigator != instigator )
            Boom();
        }

        AlignRotationToVelocity = false;
        transform.parent = hit.transform;
        // +X = forward
        transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( -hit.contacts[0].normal.y, -hit.contacts[0].normal.x ) ) );
        //body.simulated = false;
        body.bodyType = RigidbodyType2D.Static;
        animator.Play( "flash" );
        hitTransform = hit.transform;
        timeoutTimer.Start( AttachDuration, null, delegate
        {
          Boom();
          /*Damage selfDamage = Instantiate( ContactDamage );
          selfDamage.instigator = transform;
          selfDamage.point = transform.position;
          TakeDamage( selfDamage );*/
        } );
      }
  }
}
