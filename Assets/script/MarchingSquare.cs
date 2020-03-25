using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class MarchingSquare : ScriptableObject
{
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
    public GameObject[] prefab;
  }

  [SerializeField]
  public MarchingSquareCell[] Cells = new MarchingSquareCell[16];

  [Tooltip( "These are parsed by name. Example: \"shack-3\" or \"wall-15-top\"" )]
  public GameObject[] Prefabs;
#if UNITY_EDITOR
  public void PrefabsToCells()
  {
    if( Prefabs != null && Prefabs.Length > 0 )
    {
      for( int i = 0; i < Cells.Length; i++ )
      {
        ArrayUtility.Clear( ref Cells[i].prefab );
      }
      // parse prefab names
      foreach( var go in Prefabs )
      {
        string[] tokens = go.name.Split( new char[]{ '-' } );
        int index = int.Parse( tokens[ 1 ] );
        if( tokens.Length == 2 )
        {
          ArrayUtility.Add( ref Cells[index].prefab, go );
        }
        else
        if( tokens.Length == 3 )
        {
          ArrayUtility.Add( ref Cells[index].prefab, go );
        }
        else
          Debug.LogError( "parsing of marching squares" );
      }
    }
    EditorUtility.SetDirty( this );
  }

  public void FromFolder()
  {
    List<GameObject> gos = new List<GameObject>();
    string[] guids = AssetDatabase.FindAssets( "t:prefab", new string[] { GetCurrentAssetDirectory() } );
    foreach( string guid in guids )
    {
      GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>( AssetDatabase.GUIDToAssetPath( guid ) );
      gos.Add( go );
    }
    Prefabs = gos.ToArray();
    EditorUtility.SetDirty( this );
  }

  public static string GetCurrentAssetDirectory()
  {
    foreach( var obj in Selection.GetFiltered<Object>( SelectionMode.Assets ) )
    {
      var path = AssetDatabase.GetAssetPath( obj );
      if( string.IsNullOrEmpty( path ) )
        continue;

      if( System.IO.Directory.Exists( path ) )
        return path;
      else if( System.IO.File.Exists( path ) )
        return System.IO.Path.GetDirectoryName( path );
    }

    return "Assets";
  }
#endif
}

