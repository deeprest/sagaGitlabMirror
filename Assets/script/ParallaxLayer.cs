using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
  public Vector2 Scale;
  public bool Reverse;
  private Transform cam;

  void Start()
  {
    if( Application.isPlaying )
      cam = Global.instance.CameraController.transform;
  }

  Vector3 cached;
  void LateUpdate()
  {
#if UNITY_EDITOR 
    if( cam == null )
      cam = SceneView.GetAllSceneCameras()[0].transform;
#endif
    if( cam != null )
      transform.position = Vector3.Scale( cam.position, Scale ) * (Reverse ? -1f : 1f);
  }
}
