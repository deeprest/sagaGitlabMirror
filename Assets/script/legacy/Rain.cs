using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class Rain : MonoBehaviour
{
  public Transform follow;
  public Vector3 offset;
  public float ratio = 1;
  Vector3 pos;
  ParticleSystem ps;
  void Start()
  {
    ps = GetComponent<ParticleSystem>();
  }
  void Update()
  {
    if( follow != null )
    {
      pos = follow.position + offset * Camera.main.orthographicSize;
      pos.z = 0;
      transform.position = pos;
    }
    var sm = ps.shape;
    sm.radius = Camera.main.orthographicSize * ratio;

  }
}
