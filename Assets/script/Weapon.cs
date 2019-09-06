using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
  public Sprite icon;
  public Sprite cursor;

  public Projectile ProjectilePrefab;
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

  public void FireWeapon( Character instigator, Vector3 pos, Vector3 shoot )
  {
    if( projectileCount == 1 )
    {
      FireWeaponProjectile( instigator, ProjectilePrefab, pos, shoot );
    }
    else if( projectileCount > 1 )
    {
      float inc = spread / (projectileCount-1);
      float val = -spread * 0.5f;
      for( int i = 0; i < projectileCount; i++ )
      {
        FireWeaponProjectile( instigator, ProjectilePrefab, pos, shoot + Vector3.Cross( shoot, Vector3.forward ) * val, false );
        val += inc;
      }
      Global.instance.AudioOneShot( ProjectilePrefab.StartSound, pos );
    }
  }

  void FireWeaponProjectile( Character instigator, Projectile projectile, Vector3 pos, Vector3 shoot, bool playSound = true )
  {
    Collider2D col = Physics2D.OverlapCircle( pos, projectile.circle.radius, LayerMask.GetMask( Projectile.NoShootLayers ) );
    if( col == null )
    {
      GameObject go = Instantiate( projectile.gameObject, pos, Quaternion.identity );
      Projectile p = go.GetComponent<Projectile>();
      p.instigator = instigator.transform;
      p.velocity = shoot.normalized * (p.speed + speedIncrease);
      foreach( var c in instigator.colliders )
        Physics2D.IgnoreCollision( p.circle, c );
      //SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
      //if( sr != null )
      //sr.color = Global.instance.shiftyColor;
      if( playSound )
        Global.instance.AudioOneShot( ProjectilePrefab.StartSound, pos );
    }
  }
}

