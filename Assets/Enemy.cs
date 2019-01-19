using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour , IDamage
{
  public GameObject explosion;
  public void TakeDamage( Damage d ){
    GameObject.Instantiate( explosion, transform.position, Quaternion.identity );
  }

}
