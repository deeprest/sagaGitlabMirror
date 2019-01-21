using UnityEngine;
using System.Collections;

public enum DamageType
{
  Generic
}
  
//[CreateAssetMenu]
public class Damage// : ScriptableObject
{
  public Transform instigator;
  public DamageType type = DamageType.Generic;
  public int amount = 1;
  public Vector3 point;

  public Damage( Transform instigator, DamageType type, int amount )
  {
    this.instigator = instigator;
    this.type = type;
    this.amount = amount;
  }

  public Damage( Transform instigator, DamageType type, int amount, Vector3 point )
  {
    this.instigator = instigator;
    this.type = type;
    this.amount = amount;
    this.point = point;
  }
}
