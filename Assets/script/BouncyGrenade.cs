using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BouncyGrenade : Projectile, IDamage
{
  public GameObject explosion;
  Timer timeoutTimer = new Timer();
  Timer pulseTimer = new Timer();
  [SerializeField] float pulseInterval = 0.2f;
  [SerializeField] Light2D light;
  [SerializeField] float radiusFudge;

  void Start()
  {
    GetComponent<Rigidbody2D>().velocity = new Vector2( velocity.x, velocity.y );
    timeoutTimer.Start( timeout, null, Boom );
    pulseTimer.Start( int.MaxValue, pulseInterval,delegate(Timer obj){ light.enabled = !light.enabled; }, null );
  }

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
    pulseTimer.Stop( false );
  }

  void Boom()
  {
    if( gameObject != null && !gameObject.activeSelf )
      return;
    Instantiate( explosion, transform.position, Quaternion.identity );
    timeoutTimer.Stop( false );
    Destroy( gameObject );
  }

  void FixedUpdate()
  {
    hitCount = Physics2D.CircleCastNonAlloc( transform.position, circle.radius + radiusFudge, velocity, RaycastHits, raycastDistance, Global.DamageCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator.transform )) && !ignore.Contains( hit.transform ) )
      {
        IDamage dam = hit.transform.GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = instigator;
          dmg.damageSource = transform;
          dmg.point = hit.point;
          if( dam.TakeDamage( dmg ) )
            Boom();
          break;
        }
      }
    }
  }

  public bool TakeDamage( Damage damage )
  {
    Boom();
    return true;
  }

  // UNITY CRASH BUG: Destroying this gameObject from within this callback causes a crash in Unity 2019.2.6f1
  // Even deferred destruction in Global.cs caused a crash.
  /*void OnCollisionEnter2D( Collision2D hit )
  {
    if( (LayerMask.GetMask( Global.BouncyGrenadeCollideLayers ) & (1 << hit.gameObject.layer)) > 0 )
    if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator )) )
    {
      IDamage dam = hit.transform.GetComponent<IDamage>();
      if( dam != null )
      {
        Damage dmg = Instantiate( ContactDamage );
        dmg.instigator = transform;
        dmg.point = hit.GetContact( 0 ).point;
        dam.TakeDamage( dmg );
      }
      Boom();
    }
  }*/
}
