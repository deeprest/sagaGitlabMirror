using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Profiling;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof( SceneCity ) )]
public class SceneCityEditor : Editor
{
  public override void OnInspectorGUI()
  {
    SceneCity level = target as SceneCity;
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
    if( GUI.Button( EditorGUILayout.GetControlRect(), "Write png" ) )
      level.WriteStructurePNG();
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
  public const byte BuildingBackground = 64;
  public const byte Building = 128;
}

[ExecuteInEditMode]
public class SceneCity : SceneScript
{
  [Header( "City" )]
  [SerializeField] Chopper chopper;
  [SerializeField] float runRightDuration = 3;

  public override void StartScene()
  {
    base.StartScene();

    if( GenerateLevelOnStart )
      Generate();

    chopper.StartDrop( Global.instance.CurrentPlayer );
    //Global.instance.ready.SetActive( true );
    Global.instance.Controls.BipedActions.Disable();
    Global.instance.Controls.BipedActions.Aim.Enable();
    Global.instance.Controls.BipedActions.Fire.Enable();

    new Timer( runRightDuration, delegate
    {
      Global.instance.CurrentPlayer.ApplyInput( new InputState { MoveRight = true } );
    }, delegate
    {
      Global.instance.Controls.BipedActions.Enable();
    } );

    Global.instance.CameraController.orthoTarget = 3;
  }


  [Header( "Level Generation" )]
  public bool GenerateLevelOnStart = true;
  public bool RandomSeedOnStart = false;
  public int seed;
  public Vector2Int dimension = new Vector2Int( 20, 8 );
  private Vector2Int cellsize = new Vector2Int( 10, 10 );
  public Bounds bounds;
  public int GroundY = 2;
  public bool UseDensity;
  [Range( 0, 100 )]
  [SerializeField] int density = 60;
  public bool SineCurve;
  [SerializeField] int spaceMax = 2;
  [SerializeField] int buildingWidthMax = 3;
  [SerializeField] float highwayY = 10;
  public int MaxLinkPasses = 30;
  public GameObject spawnPointPrefab;
  // marching square
  string StructurePath { get { return Application.persistentDataPath + "/structure.png"; } }
  [SerializeField] Texture2D StructureTexture;
  Dictionary<int, List<GameObject>> built = new Dictionary<int, List<GameObject>>();
  Color32[] structureData;
  public MarchingSquare.MarchingSquareData building;
  public MarchingSquare.MarchingSquareData buildingBG;
  public MarchingSquare.MarchingSquareData underground;
  List<MarchingSquare.MarchingSquareData> msd;
  // nodelinks
  List<GameObject> gens = new List<GameObject>();
  List<LineSegment> debugSegments = new List<LineSegment>();
  /*
#if UNITY_EDITOR
  private void Awake()
  {
    EditorApplication.playModeStateChanged += EditorApplication_PlayModeStateChanged;
  }

  void EditorApplication_PlayModeStateChanged( PlayModeStateChange obj )
  {
    if( obj == PlayModeStateChange.ExitingEditMode )
    {
      DestroyAll();
      Debug.Log( "destroyed generated objects." );
    }
    if( obj == PlayModeStateChange.ExitingPlayMode )
    {
      // restore objects from ids
      DestroyAll();
      Debug.Log( "returned from play mode" );
    }
  }
#endif
*/

  public void EncapsulateNavmesh( Bounds bounds )
  {
    SceneScript ss = FindObjectOfType<SceneScript>();
    if( ss != null )
    {
      if( ss.NavmeshBox != null )
      {
        ss.NavmeshBox.size = new Vector3( bounds.size.x, 0, bounds.size.y );
        Vector3 boxpos = bounds.center; boxpos.z = 0;
        ss.NavmeshBox.transform.position = boxpos;
      }
    }
  }


  private void Update()
  {
    foreach( var segment in debugSegments )
      Debug.DrawLine( segment.a, segment.b, Color.red );
  }


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
    if( !Application.isEditor && RandomSeedOnStart )
      seed = System.DateTime.Now.Second;
    Random.InitState( seed );
    bounds = new Bounds();
    bounds.Encapsulate( Vector3.right * dimension.x * cellsize.x );
    bounds.Encapsulate( Vector3.up * dimension.y * cellsize.y );
    // encapsulate existing objects in scene
    GameObject[] gos = gameObject.scene.GetRootGameObjects();
    foreach( var go in gos )
    {
      Collider2D[] cld = go.GetComponentsInChildren<Collider2D>();
      foreach( var c in cld )
        bounds.Encapsulate( c.bounds );
    }

    InitializeStructure();
    int buildingWidth = 1;
    int spaceWidth = 0;
    for( int x = 0; x <= dimension.x; x++ )
    {
      SetStructureValue( x, 0, PixelBit.None );
      if( x == 0 || x == dimension.x )
      {
        for( int y = 0; y < dimension.y; y++ )
          SetStructureValue( x, y, PixelBit.None );
      }
      else
      if( spaceWidth > 0 )
      {
        spaceWidth--;
        if( spaceWidth == 0 )
          buildingWidth = Random.Range( 1, buildingWidthMax + 1 );
      }
      else
      if( buildingWidth > 0 )
      {
        for( int y = 1; y < dimension.y; y++ )
        {
          int height = dimension.y;
          if( SineCurve )
            height = Mathf.RoundToInt( dimension.y * Mathf.Sin( ((float)x / (float)dimension.x) * Mathf.PI ) + 0.5f );
          if( y < height )
          {
            if( !UseDensity || Random.Range( 0, 100 ) < density )
              SetStructureBitOn( x, y, PixelBit.Building | PixelBit.BuildingBackground );
            else
              SetStructureBitOn( x, y, PixelBit.BuildingBackground );
          }
        }
        buildingWidth--;
        if( buildingWidth == 0 )
          spaceWidth = Random.Range( 1, spaceMax + 1 );
      }
    }
    UpdateStructure( 0, 0, dimension.x, dimension.y );

    //AddObject( Instantiate( spawnPointPrefab, new Vector3( 2, (GroundY+1)*cellsize.y, 0 ), Quaternion.identity ) );
    GenerateChain( "street bg", new Vector2( 0, highwayY ), false );
    GenerateChain( "street", new Vector2( 0, highwayY ), true );


    SceneScript ss = FindObjectOfType<SceneScript>();
    if( ss != null )
    {
      if( ss.NavmeshBox != null )
      {
        ss.NavmeshBox.size = new Vector3( bounds.size.x, 0, bounds.size.y );
        Vector3 boxpos = bounds.center; boxpos.z = 0;
        ss.NavmeshBox.transform.position = boxpos;
      }
    }

    Profiler.EndSample();
  }

  void GenerateChain( string folder, Vector2 pos, bool overlapCheck = true )
  {
    List<GameObject> gos = new List<GameObject>( Resources.LoadAll<GameObject>( "LevelFreeNode/" + folder ) );
    List<NodeLink> links = new List<NodeLink>();
    // first link
    CreateChainLink( gos[0], pos, links );
    for( int i = 0; i < MaxLinkPasses; i++ )
    {
      List<NodeLink> newlinks = new List<NodeLink>();
      List<NodeLink> removelinks = new List<NodeLink>();
      foreach( var link in links )
      {
        if( link.transform.position.x > cellsize.x * dimension.x )
          continue;
        GameObject prefab = link.linkSet.AllowedToLink[Random.Range( 0, link.linkSet.AllowedToLink.Length )];

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
            CreateChainLink( prefab, link.transform.position, newlinks );
          }
          else
          {
            string conflictnames = "";
            foreach( var cn in conflict )
              conflictnames += cn.name + " ";
            //Debug.Log( "prefab " + prefab.name + " will not spawn due to overlap with " + conflictnames, conflict[0] );
          }
        }
        else
        {
          CreateChainLink( prefab, link.transform.position, newlinks );
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

  void CreateChainLink( GameObject prefab, Vector2 pos, List<NodeLink> newLinks )
  {
    GameObject go = null;
    if( Application.isPlaying )
    {
      go = Instantiate( prefab, pos, Quaternion.identity );
    }
    else
    {
#if UNITY_EDITOR
      go = (GameObject)PrefabUtility.InstantiatePrefab( prefab );
      go.transform.position = pos;
#endif
    }
    go.name = prefab.name;
    AddObject( go );
    newLinks.AddRange( go.GetComponentsInChildren<NodeLink>() );
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
    msd = new List<MarchingSquare.MarchingSquareData>();
    msd.Add( building );
    msd.Add( buildingBG );
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

  public void WriteStructurePNG()
  {
    for( int x = 0; x < StructureTexture.width; x++ )
    {
      for( int y = 0; y < StructureTexture.height; y++ )
      {
        Color32 color32 = structureData[x + y * dimension.x];
        StructureTexture.SetPixel( x, y, new Color( (float)color32.r / 255, 0, 0, 1 ) );
      }
    }
    StructureTexture.Apply();
    File.WriteAllBytes( StructurePath, StructureTexture.EncodeToPNG() );
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
    w = Mathf.Clamp( w, 1, dimension.x - ox );
    h = Mathf.Clamp( h, 1, dimension.y - oy );
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

        {
          int buildingBackgroundIndex = buildingBG.indexBuffer[x, y];
          GameObject[] prefabs = buildingBG.ms.Cells[buildingBackgroundIndex].prefab;
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
      {
        go = Global.instance.Spawn( prefab, pos, Quaternion.identity, null, false, true );
      }
      else
      {
#if UNITY_EDITOR
        go = (GameObject)PrefabUtility.InstantiatePrefab( prefab );
        go.transform.position = pos;
#endif
      }

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


