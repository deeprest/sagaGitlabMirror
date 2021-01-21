using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
  public Vector2 Scale;
  private Transform cam;
  
  void Start()
  {
    if( Application.isPlaying )
      cam = Global.instance.CameraController.transform;
  }
  
  void LateUpdate()
  {
#if UNITY_EDITOR
    if( !Application.isPlaying )
    {
      // avoid changing the transform unnecessarily, which makes the scene dirty.
      if( cam == null )
        cam = SceneView.GetAllSceneCameras()[0].transform;
    }
#endif
    if( cam != null )
      transform.position = Vector3.Scale( cam.position, Scale );
  }
}