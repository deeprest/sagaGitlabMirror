using UnityEngine;
using System.Collections;

public enum DamageType
{
  Generic,
  Fire,
  Axe
}
  
[CreateAssetMenu]
public class Damage : ScriptableObject
{
  public Transform instigator;
  public DamageType type = DamageType.Generic;
  public int amount = 1;

  public Damage( Transform instigator, DamageType type, int amount )
  {
    this.instigator = instigator;
    this.type = type;
    this.amount = amount;
  }
}
