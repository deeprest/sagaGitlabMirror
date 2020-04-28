using UnityEngine;
using System.Collections;

public enum DamageType
{
  Generic,
  Fire
}

[CreateAssetMenu]
public class Damage : ScriptableObject
{
  public DamageType type = DamageType.Generic;
  public int amount = 1;
  // optional instigator
  [HideInInspector]
  public Character instigator;
  // the projectile, etc
  [HideInInspector]
  public Transform damageSource;
  [HideInInspector]
  public Vector2 point;
 
}
