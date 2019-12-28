using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Profiling;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof( Level ) )]
public class LevelEditor : Editor
{
  public override void OnInspectorGUI()
  {
    Level level = target as Level;
    EditorGUILayout.BeginHorizontal();
    if( GUI.Button( EditorGUILayout.GetControlRect(), "Generate" ) )
      level.Generate();
    if( GUI.Button( EditorGUILayout.GetControlRect(), "+1" ) )
    {
      level.seed++;
      level.Generate();
    }
    EditorGUILayout.EndHorizontal();
    if( GUI.Button( EditorGUILayout.GetControlRect(), "Destroy" ) )
      level.DestroyAll();
    DrawDefaultInspector();
  }
}
#endif

public class Column
{
  public int xindex;
  public List<LevelNode> nodes = new List<LevelNode>();
}

public static class PixelBit
{
  public const byte None = 0;
  //public const byte Door = 32;
  //public const byte Wall = 64;
  public const byte Building = 128;
}

[ExecuteInEditMode]
public class Level : MonoBehaviour
{
  public Vector2Int dimension = new Vector2Int( 1, 1 );


  string StructurePath { get { return Application.persistentDataPath + "/" + name + "/" + "structure.png"; } }
  Texture2D StructureTexture;
  Dictionary<int, List<GameObject>> built = new Dictionary<int, List<GameObject>>();
  Color32[] structureData;

  [Header( "Marching Squares" )]
  // marching squares
  public Vector2Int cellsize = new Vector2Int( 10, 10 );
  public List<Column> columns = new List<Column>();
  public int GroundY = 2;
  public MarchingSquareData building;
  public MarchingSquareData underground;
  List<MarchingSquareData> msd;

  // nodelinks
  List<GameObject> gens = new List<GameObject>();
  List<GameObject> gos;
  public int max = 20;
  List<LineSegment> debugSegments = new List<LineSegment>();
  public int seed;

  public GameObject spawnPointPrefab;

  private void Awake()
  {
#if UNITY_EDITOR
    EditorApplication.playModeStateChanged += EditorApplication_PlayModeStateChanged;
#endif
  }

  private void Update()
  {
    foreach( var segment in debugSegments )
      Debug.DrawLine( segment.a, segment.b, Color.red );
  }

  Bounds bounds = new Bounds();

  void AddObject( GameObject go )
  {
    Collider2D cld = go.GetComponent<Collider2D>();
    if( cld != null )
      bounds.Encapsulate( cld.bounds );
    gens.Add( go );
  }

  public void Generate()
  {
    Profiler.BeginSample( "Level Generation" );

    DestroyAll();
    Random.InitState( seed );
    bounds.Encapsulate( Vector3.right * dimension.x * cellsize.x );
    bounds.Encapsulate( Vector3.up * dimension.y * cellsize.y );

    InitializeStructure();
    Vector2Int pos = new Vector2Int( 1, 1 );
    for( int x = 0; x <= dimension.x; x++ )
    {
      Column column = new Column();
      columns.Add( column );
      column.xindex = pos.x;
      if( x == 0 || x == dimension.x )
      {
        for( int y = 0; y < dimension.y; y++ )
          SetStructureValue( x, y, 0 );
        pos.x++;
      }
      else
      {
        int height = Mathf.CeilToInt( Random.Range( 0, dimension.y - 1 ) * Mathf.Sin( ((float)pos.x / (float)dimension.x) * Mathf.PI ) + 0.5f );
        for( int y = 0; y < height; y++ )
        {
          SetStructureBitOn( pos.x, pos.y, PixelBit.Building );
          pos.y++;
        }
        pos.x += Random.Range( 1, 3 );
      }
      pos.y = 1;
    }
    UpdateStructure( 0, 0, dimension.x, dimension.y );

    //AddObject( Instantiate( spawnPointPrefab, new Vector3( 2, (GroundY+1)*cellsize.y, 0 ), Quaternion.identity ) );
    GenerateChain( "street bg", false, 0, 3 );
    GenerateChain( "street", true );

    /*
    SceneScript ss = FindObjectOfType<SceneScript>();
    if( ss != null )
    {
      if( ss.sb != null && ss.sb is BoxCollider2D )
      {
        //bounds.Encapsulate( ss.sb.bounds );
        BoxCollider2D box = ss.sb as BoxCollider2D;
        box.size = bounds.size;
        box.transform.position = bounds.center;

        ss.NavmeshBox.size = new Vector3( bounds.size.x, .1f, bounds.size.y );
        ss.NavmeshBox.transform.position = bounds.center;
      }
    }
    */
    Profiler.EndSample();
  }

#if UNITY_EDITOR
  void EditorApplication_PlayModeStateChanged( PlayModeStateChange obj )
  {
    if( obj == PlayModeStateChange.ExitingEditMode )
    {
      DestroyAll();
    }
    if( obj == PlayModeStateChange.ExitingPlayMode )
    {
      // restore objects from ids
      DestroyAll();
    }
  }
#endif

  void GenerateChain( string folder, bool overlapCheck = true, float xpos = 0, float zDepth = 0 )
  {
    gos = new List<GameObject>( Resources.LoadAll<GameObject>( "LevelFreeNode/" + folder ) );
    List<NodeLink> links = new List<NodeLink>();

    GameObject firstPrefab = gos[0];
    GameObject first = Instantiate( firstPrefab, new Vector3( xpos, Random.Range( 10, 20 ), zDepth ), Quaternion.identity );
    first.name = firstPrefab.name;
    AddObject( first );
    links.AddRange( first.GetComponentsInChildren<NodeLink>() );

    for( int i = 0; i < max; i++ )
    {
      List<NodeLink> newlinks = new List<NodeLink>();
      List<NodeLink> removelinks = new List<NodeLink>();
      foreach( var link in links )
      {
        if( link.transform.position.x > cellsize.x * dimension.x )
          continue;
        GameObject prefab = link.AllowedToLink[Random.Range( 0, link.AllowedToLink.Length )];

        if( overlapCheck )
        {
          Bounds bounds = prefab.GetComponent<PrefabAABB>().bounds;
          LineSegment seg;
          seg.a = link.transform.position + bounds.min;
          seg.b = link.transform.position + bounds.max;
          debugSegments.Add( seg );

          Collider2D[] overlap = Physics2D.OverlapAreaAll( seg.a, seg.b );
          Collider2D[] ignore = link.transform.root.GetComponentsInChildren<Collider2D>();
          List<Collider2D> conflict = new List<Collider2D>( overlap );
          for( int c = 0; c < overlap.Length; c++ )
          {
            for( int g = 0; g < ignore.Length; g++ )
            {
              if( overlap[c] == ignore[g] )
              {
                conflict.Remove( ignore[g] );
                break;
              }
            }
          }

          if( conflict.Count == 0 )
          {
            GameObject go = Instantiate( prefab, link.transform.position, Quaternion.identity );
            go.name = prefab.name;
            AddObject( go );
            newlinks.AddRange( go.GetComponentsInChildren<NodeLink>() );
          }
          else
          {
            string conflictnames = "";
            foreach( var cn in conflict )
              conflictnames += cn.name + " ";
            Debug.Log( "prefab " + prefab.name + " will not spawn due to overlap with " + conflictnames, conflict[0] );
          }
        }
        else
        {
          GameObject go = Instantiate( prefab, link.transform.position, Quaternion.identity );
          go.name = prefab.name;
          AddObject( go );
          newlinks.AddRange( go.GetComponentsInChildren<NodeLink>() );
        }
        removelinks.Add( link );
      }
      links.AddRange( newlinks );
      foreach( var ugh in removelinks )
      {
        ugh.GetComponent<SpriteRenderer>().enabled = false;
        links.Remove( ugh );
      }
    }
  }

  public void DestroyAll()
  {
    // node links
    debugSegments.Clear();
    for( int i = 0; i < gens.Count; i++ )
      DestroyImmediate( gens[i] );
    gens.Clear();

    // marching square built
    foreach( var pair in built )
      if( built[pair.Key] != null && built[pair.Key].Count > 0 )
        for( int i = 0; i < built[pair.Key].Count; i++ )
        {
          DestroyImmediate( built[pair.Key][i] );

        }
    built.Clear();
  }


  public void InitializeStructure()
  {
    msd = new List<MarchingSquareData>();
    msd.Add( building );
    //msd.Add( wall );
    foreach( var ms in msd )
      ms.indexBuffer = new int[dimension.x, dimension.y];
    StructureTexture = new Texture2D( dimension.x + 1, dimension.y + 1 );
    structureData = new Color32[(dimension.x + 1) * (dimension.y + 1)];
  }

  public void DeserializeStructure()
  {
    if( File.Exists( StructurePath ) )
    {
      byte[] bytes = File.ReadAllBytes( StructurePath );
      if( bytes.Length > 0 )
      {
        StructureTexture.filterMode = FilterMode.Point;
        StructureTexture.LoadImage( bytes );
        structureData = StructureTexture.GetPixels32();
      }
    }
    else
    {
      Color32 black = new Color32( 0, 0, 0, 255 );
      for( int i = 0; i < dimension.x * dimension.y; i++ )
        structureData[i] = black;
    }
    UpdateStructure( 0, 0, dimension.x, dimension.y );
  }

  public void SetStructureValue( int x, int y, byte value )
  {
    x = Mathf.Clamp( x, 0, dimension.x );
    y = Mathf.Clamp( y, 0, dimension.y );
    structureData[x + y * dimension.x].r = value;
  }

  public void SetStructureBitOn( int x, int y, byte value )
  {
    x = Mathf.Clamp( x, 0, dimension.x );
    y = Mathf.Clamp( y, 0, dimension.y );
    structureData[x + y * dimension.x].r |= value;
  }


  public void UpdateStructure( int ox, int oy, int w, int h )
  {
    ox = Mathf.Clamp( ox, 0, dimension.x );
    oy = Mathf.Clamp( oy, 0, dimension.y );
    if( ox > 0 )
    {
      ox -= 1;
      w += 1;
    }
    if( oy > 0 )
    {
      oy -= 1;
      h += 1;
    }
    // first pass
    for( int y = oy; y < oy + h && y < dimension.y; y++ )
    {
      for( int x = ox; x < ox + w && x < dimension.x; x++ )
      {
        int key = x + y * dimension.x;
        if( built.ContainsKey( key ) && built[key] != null )
        {
          for( int i = 0; i < built[key].Count; i++ )
            Destroy( built[key][i] );
          built[key].Clear();
        }
        foreach( var ms in msd )
        {
          // getpixels returns:
          // 23
          // 01
          // marching squares bit values:
          // 24
          // 18
          int index = 0;
          if( (structureData[key].r & ms.bit) > 0 )
            index += 1;
          if( (structureData[(x + 1) + y * dimension.x].r & ms.bit) > 0 )
            index += 8;
          if( (structureData[x + (y + 1) * dimension.x].r & ms.bit) > 0 )
            index += 2;
          if( (structureData[(x + 1) + (y + 1) * dimension.x].r & ms.bit) > 0 )
            index += 4;
          ms.indexBuffer[x, y] = index;
        }
      }
    }

    // second pass: neighbors
    /* foreach( var ms in msd )
    {
      for( int y = 1; y < BlockSize - 1; y++ )
      {
        for( int x = 1; x < BlockSize - 1; x++ )
        {
          //int index = x + y * BlockSize;
          int[] n = new int[9];
          n[ 0 ] = ms.indexBuffer[ x - 1, y - 1 ];
          n[ 1 ] = ms.indexBuffer[ x , y - 1 ];
          n[ 2 ] = ms.indexBuffer[ x + 1, y - 1 ];
          n[ 3 ] = ms.indexBuffer[ x - 1, y ];
          n[ 4 ] = ms.indexBuffer[ x, y ];
          n[ 5 ] = ms.indexBuffer[ x + 1, y ];
          n[ 6 ] = ms.indexBuffer[ x - 1, y + 1 ];
          n[ 7 ] = ms.indexBuffer[ x, y + 1 ];
          n[ 8 ] = ms.indexBuffer[ x + 1, y + 1 ];

        }
      }
    }*/


    for( int y = oy; y < oy + h && y < dimension.y; y++ )
    {
      for( int x = ox; x < ox + w && x < dimension.x; x++ )
      {
        int key = x + y * dimension.x;

        int buildingIndex = building.indexBuffer[x, y];
        if( IsUnderground( x, y ) )
        {
          GameObject[] prefabs = underground.ms.Cells[buildingIndex].prefab;
          if( prefabs.Length > 0 )
            SingleCell( key, x, y, prefabs[Random.Range( 0, prefabs.Length )] );
        }
        else
        {
          GameObject[] prefabs = building.ms.Cells[buildingIndex].prefab;
          if( prefabs.Length > 0 )
            SingleCell( key, x, y, prefabs[Random.Range( 0, prefabs.Length )] );
        }
        /*
        foreach( var ms in msd )
        {
          int buildingIndex = ms.indexBuffer[x, y];
          SingleCell( key, x, y, ms.ms.Cells[buildingIndex].bottom, null );
        }

        /*MarchingSquareCell cell = building.ms.Cells[building.indexBuffer[x, y]];
        if( cell != null )
        {
          int doorIndex = 0;
          if( (structureData[key].r & PixelBit.Door) > 0 )
            doorIndex += 1;
          if( (structureData[(x + 1) + y * BlockSize].r & PixelBit.Door) > 0 )
            doorIndex += 8;
          if( (structureData[x + (y + 1) * BlockSize].r & PixelBit.Door) > 0 )
            doorIndex += 2;
          if( (structureData[(x + 1) + (y + 1) * BlockSize].r & PixelBit.Door) > 0 )
            doorIndex += 4;

          if( doorIndex > 0 && cell.door != null )
            SingleCell( key, x, y, cell.door, cell.top );
          else
            SingleCell( key, x, y, cell.bottom, cell.top );
        }

        cell = wall.ms.Cells[wall.indexBuffer[x, y]];
        if( cell != null )
          SingleCell( key, x, y, cell.bottom, cell.top );
          */
      }
    }

  }

  void SingleCell( int key, int x, int y, GameObject prefab )
  {
    if( prefab != null )
    {
      GameObject go = null;
      Vector3 pos = new Vector3( x * cellsize.x, y * cellsize.y, 0 );
      if( Application.isPlaying )
        go = Global.instance.Spawn( prefab, pos, Quaternion.identity, null, false, true );
      else
        go = Instantiate( prefab, pos, Quaternion.identity, null );

      GenerationVariant[] vars = go.GetComponentsInChildren<GenerationVariant>();
      foreach( var cmp in vars )
        cmp.Generate();

      if( !built.ContainsKey( key ) )
        built[key] = new List<GameObject>();
      built[key].Add( go );
    }
  }

  bool IsUnderground( int x, int y )
  {
    return y <= GroundY;
  }

}