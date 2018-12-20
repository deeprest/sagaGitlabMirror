using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
  public Vector2 rate;
  public float depth = 1;

  void Update()
  {
    if( Camera.current != null )
    {
      Vector3 pos = Camera.current.transform.position;
      transform.position = new Vector3( pos.x * rate.x, pos.y * rate.y, depth );
    }
  }

}
