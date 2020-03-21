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

  void OnEnable()
  {
#if UNITY_EDITOR
    if( Global.instance != null )
      cam = Global.instance.CameraController.transform;
    else if( Camera.main != null )
      cam = Camera.main.transform;
    else
      cam = SceneView.GetAllSceneCameras()[0].transform;
#else
		cam = Global.instance.CameraController.transform;
#endif
  }

  void LateUpdate()
  {
    if( cam != null )
      transform.position = Vector3.Scale( cam.position, Scale ) * (Reverse ? -1f : 1f);
  }
}
