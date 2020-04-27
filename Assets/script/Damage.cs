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

 
}


// todo convert SOs to structs
/*
public struct Damage
{
  public DamageType type;
  public int amount;
  // optional instigator
  public Character instigator;
  // the projectile, etc
  public Transform damageSource;
  public Vector2 point;
}
*/