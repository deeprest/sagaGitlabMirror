using UnityEngine;
using System.Collections;

public enum DamageType
{
  Generic
}
  
[CreateAssetMenu]
public class Damage : ScriptableObject
{
  public DamageType type = DamageType.Generic;
  public int amount = 1;
  // optional instigator
  public Character instigator;
  // the projectile, etc
  public Transform damageSource;
  public Vector2 point;

  public Damage( Transform instigator, DamageType type, int amount )
  {
    this.damageSource = instigator;
    this.type = type;
    this.amount = amount;
  }

  public Damage( Transform instigator, DamageType type, int amount, Vector3 point )
  {
    this.damageSource = instigator;
    this.type = type;
    this.amount = amount;
    this.point = point;
  }
}
