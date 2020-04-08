using UnityEngine;
using System.Collections;

// This is a way to serialize a reference to a SceneReference using a ScriptableObject.
// It's for triggers that load a scene, but don't want to use plain strings and
// just assume the scene name has not been changed. This is safer.
[CreateAssetMenu]
public class SceneAssetObject : ScriptableObject
{
  [SerializeField]
  public SceneReference scene;
}

