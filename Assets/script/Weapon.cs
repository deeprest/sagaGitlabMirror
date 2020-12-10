using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
  public enum WeaponType
  {
    Projectile,
    Laser
  }
  public WeaponType weaponType;
  public Sprite icon;
  public Sprite cursor;
  public Color Color0;
  public Color Color1;

  public Projectile ProjectilePrefab;
  public AudioClip StartSound;
  Projectile projectileInstance;
  public bool fullAuto;
  public bool HasInterval = true;
  public float shootInterval = 0.1f;
  public float speedIncrease;
  public int projectileCount = 1;
  public float spread = 0.5f;
  public float AimDistanceMax = 2;

  [Header( "Charge Variant" )]
  public Weapon ChargeVariant;
  public GameObject ChargeEffect;
  public AudioClip soundCharge;
  public AudioClip soundChargeLoop;
  public AudioClip soundChargeShot;

  const int Maxpoints = 1000;

  Vector2 GetInitialVelocity( Projectile projectile, Vector2 shoot )
  {
    /*Vector2 velocity;
    Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
    if( rb == null )
      velocity = shoot.normalized * (projectile.speed + speedIncrease);
    else
      velocity = shoot.normalized * Mathf.Min( shoot.magnitude / AimDistanceMax, 1 ) * (projectile.speed + speedIncrease);
    return velocity;*/
    return shoot.normalized * (projectile.speed + speedIncrease);
  }

  // for viewing the arc of physics projectiles
  public Vector3[] GetTrajectory( Vector2 origin, Vector2 shoot )
  {
    Projectile projectile = ProjectilePrefab;
    Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
    Vector2 vel = GetInitialVelocity( projectile, shoot );
    List<Vector3> points = new List<Vector3>();
    Vector2 pos = origin;
    points.Add( pos );
    for( int i = 0; i < Maxpoints; i++ )
    {
      pos += vel * Time.fixedDeltaTime;
      vel += (rb != null ? Vector2.up * Physics2D.gravity * Time.fixedDeltaTime : Vector2.zero);
      points.Add( pos );
    }
    return points.ToArray();
  }
  // PICKLE arg scale
  public void FireWeapon( Entity instigator, Vector2 pos, Vector2 shoot, float scale = 1 )
  {
    if( projectileCount == 1 )
    {
      FireWeaponProjectile( instigator, ProjectilePrefab, pos, shoot, true, scale );
    }
    else if( projectileCount > 1 )
    {
      // multiple projectiles, with spread
      float inc = spread / (projectileCount - 1);
      float val = -spread * 0.5f;
      bool anyFired = false;
      for( int i = 0; i < projectileCount; i++ )
      {
        if( FireWeaponProjectile( instigator, ProjectilePrefab, pos, Quaternion.Euler( 0, 0, val ) * shoot, false, scale ) )
          anyFired = true;
        val += inc;
      }
      if( anyFired )
        Global.instance.AudioOneShot( StartSound, pos );
    }
  }
  // PICKLE arg scale
  bool FireWeaponProjectile( Entity instigator, Projectile projectilePrefab, Vector2 pos, Vector2 shoot, bool playSound = true, float scale = 1 )
  {
    if( weaponType == WeaponType.Projectile )
    {
      Collider2D col = Physics2D.OverlapCircle( pos, projectilePrefab.circle.radius * scale, Global.ProjectileNoShootLayers );
      if( col == null )
      {
        GameObject go = Instantiate( projectilePrefab.gameObject, pos, Quaternion.identity );
        go.transform.localScale = Vector3.one * scale;
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.weapon = this;
        projectile.instigator = instigator;
        projectile.velocity = GetInitialVelocity( projectile, shoot );
        projectile.Scale = scale;

        foreach( var c in instigator.IgnoreCollideObjects )
          Physics2D.IgnoreCollision( projectile.circle, c, true );
        if( playSound && StartSound != null )
          Global.instance.AudioOneShot( StartSound, pos );
        // color shifting
        /*SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if( sr != null )
          sr.color = Global.instance.shiftyColor;
        Light light = go.GetComponentInChildren<Light>();
        if( light != null )
          light.color = Global.instance.shiftyColor;*/
        return true;
      }
    }
    else if( weaponType == WeaponType.Laser )
    {
      if( projectileInstance == null )
      {
        GameObject go = Instantiate( projectilePrefab.gameObject, pos, Quaternion.identity );
        go.transform.localScale = Vector3.one * scale;
        projectileInstance = go.GetComponent<Projectile>();
        projectileInstance.weapon = this;
        projectileInstance.instigator = instigator;
        projectileInstance.velocity = shoot.normalized * (projectileInstance.speed + speedIncrease);
        //foreach( var c in instigator.colliders )
        //Physics2D.IgnoreCollision( projectileInstance.circle, c );
        if( playSound && StartSound != null )
          Global.instance.AudioOneShot( StartSound, pos );
      }
      else
      {
        projectileInstance.velocity = shoot;
        projectileInstance.transform.position = pos;
      }
    }
    return false;
  }
}

