using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Serialization;

public class BlasterProjectile : Projectile, IDamage
{
  public Light2D light;
  Timer timeoutTimer;
  int HitCount;
  public int DieAfterHitCount;
  public bool AlignRotationToVelocity = true;
  [SerializeField] GameObject hitPrefab;
  [SerializeField] IndexedColors indexedColors;

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
    {
      GameObject go = Instantiate( hitPrefab, transform.position, transform.rotation );
      if( indexedColors != null )
      {
        IndexedColors ic = go.GetComponent<IndexedColors>();
        if( ic != null )
        {
          indexedColors.colors.CopyTo( ic.colors, 0 );
          ic.ExplicitUpdate();
        }
      }
    }
  }

  void Update()
  {
    if( AlignRotationToVelocity )
      transform.rotation = Quaternion.Euler( new Vector3( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( velocity.normalized.y, velocity.normalized.x ) ) );

    if( instigator != null )
    {
      hitCount = Physics2D.CircleCastNonAlloc( transform.position, circle.radius, velocity, RaycastHits, velocity.magnitude*Time.deltaTime, Global.DefaultProjectileCollideLayers );
      for( int i = 0; i < hitCount; i++ )
      {
        hit = RaycastHits[i];
        if( hit.transform != null && (instigator == null || !hit.transform.IsChildOf( instigator.transform )) && !ignore.Contains( hit.transform ) )
        {
          IDamage dam = hit.transform.GetComponent<IDamage>();
          if( dam != null )
          {
            Damage dmg = Instantiate( ContactDamage );
            dmg.amount = Mathf.FloorToInt( Scale * ContactDamage.amount );
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
    }

    transform.position += (Vector3)velocity * Time.deltaTime;

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