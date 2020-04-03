using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Malee;

#if UNITY_EDITOR
using UnityEditor;
/*
using Malee.Editor;
[CustomEditor( typeof( BuildMetaData ) )]
public class BuildMetaDataEditor : Editor
{
  ReorderableList list;
  void OnEnable()
  {
    list = new ReorderableList( serializedObject.FindProperty( "scenes" ) );
    // onChangedCallback
    list.onReorderCallback += ( x ) => { (target as BuildMetaData).SetBuildOptions(); };
  }

  private void List_onReorderCallback( ReorderableList list )
  {
    throw new System.NotImplementedException();
  }

  public override void OnInspectorGUI()
  {
    BuildMetaData obj = target as BuildMetaData;
    if( GUI.Button( EditorGUILayout.GetControlRect(), "populate from scenes folder" ) )
      obj.FromFolder();

    //if( GUI.Button( EditorGUILayout.GetControlRect(), "Set Build Options" ) )
    //  obj.SetBuildOptions();
    serializedObject.Update();
    //draw the list using GUILayout, you can of course specify your own position and label
    list.DoLayoutList();
    serializedObject.ApplyModifiedProperties();

    //DrawDefaultInspector();
  }
}
[System.Serializable]
public class SceneMetaArray : ReorderableArray<SceneMeta> { }
*/
#endif


[System.Serializable]
public class SceneMeta
{
  public string name;
}

[CreateAssetMenu]
public class BuildMetaData : ScriptableObject
{
  [SerializeField]
  public List<SceneMeta> scenes;
  /*
#if UNITY_EDITOR
  public void FromFolder()
  {
    scenes.Clear();
    List<Object> gos = new List<Object>();
    string[] guids = AssetDatabase.FindAssets( "t:scene", new string[] { "Assets/scene" } ); //Util.GetCurrentAssetDirectory()
    int i = 0;
    foreach( string guid in guids )
    {
      Object obj = AssetDatabase.LoadAssetAtPath<Object>( AssetDatabase.GUIDToAssetPath( guid ) );
      gos.Add( obj );
      scenes.Add( new SceneMeta { name = obj.name } );
    }
    EditorUtility.SetDirty( this );
  }
#endif
*/
}