using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squishtrigger : MonoBehaviour, IDamage
{
  public void TakeDamage( Damage d ){
    GetComponent<ParticleSystem>().Play();
    //GetComponent<BoxCollider2D>().enabled = false;
  }

}
