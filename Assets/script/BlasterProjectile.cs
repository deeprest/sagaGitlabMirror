using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

public class BlasterProjectile : Projectile, IDamage
{
  public new Light2D light;
  Timer timeoutTimer;
  int HitCount;
  public int DieAfterHitCount;
  public bool AlignRotationToVelocity = true;
  [SerializeField] GameObject hitPrefab;

  void OnDestroy()
  {
    timeoutTimer.Stop( false );
  }

  void Start()
  {
    timeoutTimer = new Timer( timeout, null, delegate ()
    {
      if( gameObject != null )
        Destroy( gameObject );
    } );
    if( AlignRotationToVelocity )
      transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( velocity.normalized.y, velocity.normalized.x ) ) );
  }

  void Hit( Vector3 position )
  {
    enabled = false;
    transform.position = position;
    velocity = Vector2.zero;
    light.enabled = false;
    /*animator.Play( "hit" );*/
    Destroy( gameObject );

    if( hitPrefab != null )
      Instantiate( hitPrefab, transform.position, transform.rotation );
  }

  void FixedUpdate()
  {
    if( AlignRotationToVelocity )
      transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( velocity.normalized.y, velocity.normalized.x ) ) );

    hitCount = Physics2D.CircleCastNonAlloc( transform.position, circle.radius, velocity, RaycastHits, raycastDistance, Global.DefaultProjectileCollideLayers );
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
          {
            HitCount++;
            if( HitCount >= DieAfterHitCount )
            {
              Hit( hit.point );
              return;
            }
          }
        }
        else
        {
          Hit( hit.point );
          return;
        }
      }
    }
    transform.position += (Vector3)velocity * Time.fixedDeltaTime;

    /*SpriteRenderer sr = GetComponent<SpriteRenderer>();
    sr.color = Global.instance.shiftyColor;
    light.color = Global.instance.shiftyColor;*/
  }

  public bool TakeDamage( Damage damage )
  {
    Hit( damage.point );
    return true;
  }

}