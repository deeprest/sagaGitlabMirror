using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof( MarchingSquare ) )]
public class MarchingSquareEditor : Editor
{
  public override void OnInspectorGUI()
  {
    MarchingSquare ms = target as MarchingSquare;
    if( GUI.Button( EditorGUILayout.GetControlRect(), "prefab to cell" ) )
      ms.PrefabsToCells();
    DrawDefaultInspector();
  }
}
#endif

[System.Serializable]
public class MarchingSquareData
{
  public byte bit = 0;
  public MarchingSquare ms;
  // transient
  public int[,] indexBuffer;
}

[System.Serializable]
public class MarchingSquareCell
{
  public GameObject bottom;
  //public GameObject top;
  //public GameObject door;
}

[CreateAssetMenu]
public class MarchingSquare : ScriptableObject
{
  public MarchingSquareCell[] Cells = new MarchingSquareCell[16];
  public MarchingSquareCell[] Inside = new MarchingSquareCell[16];

  [Tooltip( "These are parsed by name. Example: \"shack-3\" or \"wall-15-top\"" )]
  public GameObject[] Prefabs;

  public void PrefabsToCells()
  {
    if( Prefabs != null && Prefabs.Length > 0 )
    {
      // parse prefab names
      foreach( var go in Prefabs )
      {
        string[] tokens = go.name.Split( new char[]{ '-' } );
        int index = int.Parse( tokens[ 1 ] );
        if( tokens.Length == 2 )
          Cells[ index ].bottom = go;
        /*else
        if( tokens.Length == 3 )
        {
          if( tokens[ 2 ] == "top" )
            Cells[ index ].top = go;
          if( tokens[ 2 ] == "door" )
            Cells[ index ].door = go;
        }*/
        else
          Debug.LogError( "parsing of marching squares" );
      }
    }
  }
}

