using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Weapon : ScriptableObject 
{
  public Projectile ProjectilePrefab;
  public float speed = 1;
  public float shootInterval = 0.1f;
  public AudioClip soundXBusterPew;

  public float chargedSpeed = 2;
  public GameObject ChargeEffect;
  public Projectile ChargedProjectilePrefab;
  public AudioClip soundCharge;
  public AudioClip soundChargeLoop;
  public AudioClip soundChargeShot;
}
