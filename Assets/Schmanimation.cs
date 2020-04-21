using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( Schmanimation ) )]
public class Schmeditor : Editor
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();
    Schmanimation ani = target as Schmanimation;

    EditorGUILayout.LabelField( "Head" );
    if( GUI.Button( EditorGUILayout.GetControlRect(), "idle" ) )
      ani.head.Play( "idle" );
    if( GUI.Button( EditorGUILayout.GetControlRect(), "talk" ) )
      ani.head.Play( "talk" );

    EditorGUILayout.LabelField( "Body" );
    if( GUI.Button( EditorGUILayout.GetControlRect(), "idle" ) )
      ani.body.Play( "idle" );
    if( GUI.Button( EditorGUILayout.GetControlRect(), "run" ) )
      ani.body.Play( "run" );
    
  }
}
#endif

public class Schmanimation : MonoBehaviour
{
  public Animator body;
  public Animator head;

}
