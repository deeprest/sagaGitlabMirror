using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
  static int Active;
  const int MaxExplosionSoundsAtOnce = 2;

  [SerializeField] float timeout = 0.5f;
  public bool playSound = true;

  void Start()
  {
    Active++;
    Destroy( gameObject, timeout );
    if( Active <= MaxExplosionSoundsAtOnce )
    {
      Debug.Log( "boom sound: "+ Active );
      GetComponent<AudioSource>().Play();
    }
  }

  private void OnDestroy()
  {
    Active--;
  }
}
