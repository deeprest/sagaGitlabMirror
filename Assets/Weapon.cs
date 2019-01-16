using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Weapon : ScriptableObject 
{
  public GameObject ProjectilePrefab;
  public float speed = 1;
  public float shootInterval = 0.1f;

  public float chargedSpeed = 2;
  public GameObject ChargeEffect;
  public GameObject ChargedProjectilePrefab;
}
