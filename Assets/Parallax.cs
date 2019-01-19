using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
  public Vector2 rate;
  public float depth = 1;
  public Vector3 offset;

  void LateUpdate()
  {
    //if( Application.isEditor && Application.isPlaying )
    {
      if( Camera.main != null )
      {
        Vector3 pos = Camera.main.transform.position;
        transform.position = offset + new Vector3( pos.x * rate.x, pos.y * rate.y, depth );
      }
    }
  }

}
