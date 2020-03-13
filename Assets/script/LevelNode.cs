using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelNode : MonoBehaviour
{
  [SerializeField] TextMesh label;

  void Awake()
  {
    if( Application.isPlaying && label != null )
      Destroy( label.gameObject );
  }
#if UNITY_EDITOR
  private void OnValidate()
  {
    if( label != null )
      label.text = name;
  }
#endif
} 

