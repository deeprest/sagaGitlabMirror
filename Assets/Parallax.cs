using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Parallax : MonoBehaviour
{
  //public Vector2 rate;
  [Range(0,1)]
  public float rate = 1;
  [Range( 0, 1 )]
  public float rateVertical = 0.5f;
  public Vector3 position;
  Vector3 pos;

  private void Start()
  {
    position = transform.position;
  }
  void LateUpdate()
  {
    //if( Application.isEditor && Application.isPlaying )
    {
      if( Camera.main != null )
      {
        pos = Camera.main.transform.position;
        transform.position = position + new Vector3( pos.x * rate, pos.y * rate * rateVertical, Mathf.Max( 0.1f, rate * 10 ));
      }
    }
  }

}
