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

  [Header( "Charge Variant" )]
  public Weapon ChargeVariant;
  public GameObject ChargeEffect;
  public AudioClip soundCharge;
  public AudioClip soundChargeLoop;
  public AudioClip soundChargeShot;

  public void FireWeapon( Character instigator, Vector2 pos, Vector2 shoot )
  {
    if( projectileCount == 1 )
    {
      FireWeaponProjectile( instigator, ProjectilePrefab, pos, shoot );
    }
    else if( projectileCount > 1 )
    {
      // multiple projectiles, with spread
      float inc = spread / (projectileCount - 1);
      float val = -spread * 0.5f;
      bool anyFired = false;
      for( int i = 0; i < projectileCount; i++ )
      {
        if( FireWeaponProjectile( instigator, ProjectilePrefab, pos, Quaternion.Euler( 0, 0, val ) * shoot, false ) )
          anyFired = true;
        val += inc;
      }
      if( anyFired )
        Global.instance.AudioOneShot( StartSound, pos );
    }
  }

  bool FireWeaponProjectile( Character instigator, Projectile projectile, Vector2 pos, Vector2 shoot, bool playSound = true )
  {
    if( weaponType == WeaponType.Projectile )
    {
      Collider2D col = Physics2D.OverlapCircle( pos, projectile.circle.radius, Global.ProjectileNoShootLayers );
      if( col == null )
      {
        GameObject go = Instantiate( projectile.gameObject, pos, Quaternion.identity );
        Projectile p = go.GetComponent<Projectile>();
        p.weapon = this;
        p.instigator = instigator;
        p.velocity = shoot.normalized * (p.speed + speedIncrease);
        foreach( var c in instigator.IgnoreCollideObjects )
          Physics2D.IgnoreCollision( p.circle, c, true );
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
        GameObject go = Instantiate( projectile.gameObject, pos, Quaternion.identity );
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

