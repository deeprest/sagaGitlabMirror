using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( MarchingSquare ) )]
public class MarchingSquareEditor : Editor
{
  public override void OnInspectorGUI()
  {
    MarchingSquare ms = target as MarchingSquare;
    if( GUI.Button( EditorGUILayout.GetControlRect(), "prefab to cell" ) )
      ms.PrefabsToCells();
    if( GUI.Button( EditorGUILayout.GetControlRect(), "populate from selected folder" ) )
    {
      ms.FromFolder();
      ms.PrefabsToCells();
    }
    DrawDefaultInspector();
  }
}