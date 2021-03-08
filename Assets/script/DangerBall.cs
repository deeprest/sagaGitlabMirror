using UnityEngine;

public class DangerBall : Entity
{
  [SerializeField] float MinVelDamageThreshold = 0.1f;
  [SerializeField] float fistSpeed = 10;

  [SerializeField] private AnimationCurve shakeCurve;
  [SerializeField] float DamageThreshold = 2;
  public AudioSource audio;
  public AudioClip soundReflect;
  public AudioClip soundSmash;
  public AudioClip soundLaunch;

  private bool launched = false;

  // use to report a hit to the controlling entity
  public System.Action OnHit;

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
    UpdateHit = LocalHit;
    UpdateCollision = BoxCollisionSingle;
  }

  void LocalHit()
  {
    if( !(launched || UseGravity) )
      return;
    if( velocity.magnitude < MinVelDamageThreshold )
      return;
    hitCount = Physics2D.BoxCastNonAlloc( transform.position, box.size, 0, velocity, RaycastHits, Mathf.Max( 0.005f, Time.deltaTime * velocity.magnitude ), Global.DefaultProjectileCollideLayers );
    //hitCount = Physics2D.CircleCastNonAlloc( transform.position, circle.radius, velocity, RaycastHits, Mathf.Max( 0.005f, Time.deltaTime * velocity.magnitude ), Global.DefaultProjectileCollideLayers );
    if( hitCount > 0 )
    {
      for( int i = 0; i < hitCount; i++ )
      {
        hit = RaycastHits[i];

        if( hit.collider.isTrigger || hit.collider.GetInstanceID() == box.GetInstanceID() || (IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider )) )
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
        if( ent != null && !ent.IsStatic )
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

          OnHit();
          break;
        }
      }
    }
  }

  public override bool TakeDamage( Damage damage )
  {
    if( damage.instigator != null && !IsEnemyTeam( damage.instigator.TeamFlags ) )
      return false;
    if( damage.amount < DamageThreshold )
    {
      if( soundReflect != null )
        audio.PlayOneShot( soundReflect );

      Projectile projectile = damage.damageSource.GetComponent<Projectile>();
      if( projectile != null )
      {
        switch( projectile.weapon.weaponType )
        {
          case Weapon.WeaponType.Projectile:
            //projectile.transform.position = transform.position + Vector3.Project( (Vector3)d.point - transform.position, transform.right );
            projectile.velocity = Vector3.Reflect( projectile.velocity, (damage.instigator.transform.position - transform.position).normalized );
            Physics2D.IgnoreCollision( projectile.circle, box, false );

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
    return base.TakeDamage( damage );
  }
}