using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class SceneParams : ScriptableObject
{
  public string SceneName;
  public float CameraFOV = 20;
}
