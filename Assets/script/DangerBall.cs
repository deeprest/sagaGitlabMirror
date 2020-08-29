﻿using UnityEngine;

public class DangerBall : Entity
{
  [Header( "DangerBall" )] [SerializeField]
  float fistSpeed = 10;

  [SerializeField] private AnimationCurve shakeCurve;
  [SerializeField] private Vector3 smashDirection;
  [SerializeField] float DamageThreshold = 2;
  public AudioSource audio;
  public AudioClip soundReflect;
  public AudioClip soundSmash;
  public AudioClip soundLaunch;

  private bool launched = false;

  // use to report a hit to the controlling entity
  public System.Action<RaycastHit2D> OnHit;

  public void Launch( Vector2 direction, float speed )
  {
    launched = true;
    UseGravity = false;
    fistSpeed = speed;
    velocity = direction.normalized * fistSpeed;

    audio.loop = true;
    audio.clip = soundLaunch;
    audio.Play();
  }

  public void Stop()
  {
    audio.Stop();
    launched = false;
    velocity = Vector2.zero;
  }

  protected override void Start()
  {
    base.Start();
    UpdateLogic = null;
    UpdateHit = null;
    UpdateCollision = LocalCollision;
    UpdatePosition = BasicPosition;
  }

  void LocalCollision()
  {
    if( !launched )
      return;
    // hitCount = Physics2D.BoxCastNonAlloc( box.transform.position, box.size, 0, smashDirection, RaycastHits, Mathf.Max(0.01f, Time.deltaTime * fistSpeed), Global.DefaultProjectileCollideLayers );
    hitCount = Physics2D.CircleCastNonAlloc( transform.position, circle.radius + 0.002f, smashDirection, RaycastHits, Mathf.Max( 0.005f, Time.deltaTime * fistSpeed ), Global.DefaultProjectileCollideLayers );
    if( hitCount > 0 )
    {
      for( int i = 0; i < hitCount; i++ )
      {
        hit = RaycastHits[i];
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;

        IDamage dam = hit.transform.GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = this;
          dmg.damageSource = transform;
          dmg.point = hit.point;
          dam.TakeDamage( dmg );
        }

        bool isStatic = true;
        Entity ent = hit.transform.GetComponent<Entity>();
        if( ent!=null && !ent.IsStatic )
          isStatic = false;
        Rigidbody2D rb2d = hit.rigidbody;
        if( rb2d != null && rb2d.bodyType != RigidbodyType2D.Static )
          isStatic = false;
        
        if( isStatic )
        {
          CameraShake shaker = Global.instance.CameraController.GetComponent<CameraShake>();
          shaker.amplitude = 0.05f;
          shaker.duration = 0.4f;
          shaker.rate = 100;
          shaker.intensityCurve = shakeCurve;
          shaker.enabled = true;

          Stop();
          // after Stop() to avoid audio conflict
          audio.PlayOneShot( soundSmash );
          
          OnHit( hit );
          break;
        }
      }
    }
  }

  public override bool TakeDamage( Damage d )
  {
    if( d.instigator != null && !IsEnemyTeam( d.instigator.Team ) )
      return false;
    if( d.amount < DamageThreshold )
    {
      if( soundReflect != null )
        audio.PlayOneShot( soundReflect );

      Projectile projectile = d.damageSource.GetComponent<Projectile>();
      if( projectile != null )
      {
        switch( projectile.weapon.weaponType )
        {
          case Weapon.WeaponType.Projectile:
            //projectile.transform.position = transform.position + Vector3.Project( (Vector3)d.point - transform.position, transform.right );
            projectile.velocity = Vector3.Reflect( projectile.velocity, (d.instigator.transform.position - transform.position).normalized );
            Physics2D.IgnoreCollision( projectile.circle, circle, false );

            foreach( var cldr in projectile.instigator.IgnoreCollideObjects )
            {
              if( cldr == null )
                Debug.Log( "ignorecolideobjects null" );
              else
                Physics2D.IgnoreCollision( projectile.circle, cldr, false );
            }

            projectile.instigator = this;
            projectile.ignore.Add( transform );
            break;

          case Weapon.WeaponType.Laser:
            // create second beam
            break;
        }
      }
      return false;
    }
    return base.TakeDamage( d );
  }
}