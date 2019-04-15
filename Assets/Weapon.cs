using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject 
{
  public Projectile ProjectilePrefab;
  public float speed = 1;
  public float shootInterval = 0.1f;
  public AudioClip soundXBusterPew;

  [Header("Charge")]
  public float chargedSpeed = 2;
  public GameObject ChargeEffect;
  public Projectile ChargedProjectilePrefab;
  public AudioClip soundCharge;
  public AudioClip soundChargeLoop;
  public AudioClip soundChargeShot;

  public void FireWeapon( Character instigator, Vector3 pos, Vector3 shoot )
  {
    Collider2D col = Physics2D.OverlapCircle( pos, ProjectilePrefab.circle.radius, LayerMask.GetMask( Projectile.NoShootLayers ) );
    if( col == null )
    {
      GameObject go = Instantiate( ProjectilePrefab.gameObject, pos, Quaternion.identity );
      Projectile p = go.GetComponent<Projectile>();
      p.instigator = instigator.transform;
      p.velocity = shoot.normalized * speed;
      Physics2D.IgnoreCollision( p.circle, instigator.collider );
      Global.instance.AudioOneShot( soundXBusterPew, pos );
      if( p.type == Projectile.ProjectileType.Bounce )
        p.GetComponent<Rigidbody2D>().velocity = new Vector2( p.velocity.x, p.velocity.y );
    }
  }

  public void FireWeaponCharged( Character instigator, Vector3 pos, Vector3 shoot )
  {
    Collider2D col = Physics2D.OverlapCircle( pos, ChargedProjectilePrefab.circle.radius, LayerMask.GetMask( Projectile.NoShootLayers ) );
    if( col == null )
    {
      GameObject go = Instantiate( ChargedProjectilePrefab.gameObject, pos, Quaternion.identity );
      Projectile p = go.GetComponent<Projectile>();
      p.instigator = instigator.transform;
      p.velocity = shoot.normalized * chargedSpeed;
      Physics2D.IgnoreCollision( p.circle, instigator.collider );
      Global.instance.AudioOneShot( soundChargeShot, pos );
      if( p.type == Projectile.ProjectileType.Bounce )
        p.GetComponent<Rigidbody2D>().velocity = new Vector2( p.velocity.x, p.velocity.y );
    }
  }
}

