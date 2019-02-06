using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Rain : MonoBehaviour
{
  public Transform follow;
  public Vector3 offset;
  public float ratio = 1;

  void Update()
  {
    if( follow != null )
      transform.position = follow.position + offset * Camera.main.orthographicSize;
    var sm = GetComponent<ParticleSystem>().shape;
    sm.radius = Camera.main.orthographicSize * ratio;

  }
}
