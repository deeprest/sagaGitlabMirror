using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject 
{
  public Sprite icon;
  public Sprite cursor;

  public float shootInterval = 0.1f;
  public float speedIncrease;
  public Projectile ProjectilePrefab;

  [Header("Charge")]
  public float chargedSpeed = 2;
  public GameObject ChargeEffect;
  public Projectile ChargedProjectilePrefab;
  public AudioClip soundCharge;
  public AudioClip soundChargeLoop;
  public AudioClip soundChargeShot;

  public void FireWeapon( Character instigator, Vector3 pos, Vector3 shoot )
  {
    FireWeaponProjectile( instigator, ProjectilePrefab, pos, shoot );
  }

  public void FireWeaponCharged( Character instigator, Vector3 pos, Vector3 shoot )
  {
    FireWeaponProjectile( instigator, ChargedProjectilePrefab, pos, shoot );
  }

  public void FireWeaponProjectile( Character instigator, Projectile projectile, Vector3 pos, Vector3 shoot )
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
    }
  }
}

