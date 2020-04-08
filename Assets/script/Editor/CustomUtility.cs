#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEditor.Build.Reporting;

public static class ShowGUIDUtility
{
  [MenuItem( "Assets/ShowGUID", true )]
  static bool CanExecute()
  {
    return Selection.assetGUIDs.Length > 0;
  }

  [MenuItem( "Assets/ShowGUID" )]
  static void Execute()
  {
    for( int i = 0; i < Selection.objects.Length; i++ )
      Debug.Log( Selection.objects[i].GetType().ToString() + " " + Selection.objects[i].name + " " + Selection.assetGUIDs[i] );
  }
}

public class CustomUtility : EditorWindow
{
  static string todo;

  [MenuItem( "Tool/Utility Window" )]
  static void Init()
  {
    // Get existing open window or if none, make a new one:
    CustomUtility window = (CustomUtility)EditorWindow.GetWindow( typeof( CustomUtility ) );
    window.Show();
  }

  private void OnEnable()
  {
    todo = File.ReadAllText( Application.dataPath + "/../todo" );
  }

  bool developmentBuild;
  bool buildMacOS = true;
  bool buildLinux = false;
  bool buildWebGL = false;
  bool buildWindows = false;

  string progressMessage = "";
  float progress = 0;
  bool processing = false;
  System.Action ProgressUpdate;
  System.Action ProgressDone;
  int index = 0;
  int length = 0;

  List<AudioSource> auds;
  List<string> scenes;
  List<GameObject> gos;
  List<Object> objects;
  List<string> assetPaths;
  int layer;

  string replaceName;
  GameObject replacePrefab;
  bool replaceInScene;
  bool replaceSelected;
  bool fixInScene;
  bool fixSpriteInScene;
  string fixSpriteName;
  string fixSpriteSubobjectName;
  Sprite sprite;
  Material material;
  string subobjectName;
  string[] guids;
  float fudgeFactor;

  // font output
  Texture2D fontImage;

  bool foldTodo = true;
  bool foldBuild;
  bool foldGeneric;
  bool foldReplace;
  bool foldSprite;
  bool foldFont;

  void StartJob( string message, int count, System.Action update, System.Action done )
  {
    progressMessage = message;
    processing = true;
    index = 0;
    progress = 0;
    length = count;
    ProgressUpdate = update;
    ProgressDone = done;
  }

  void OnGUI()
  {
    if( processing )
    {
      if( index == length )
      {
        processing = false;
        progress = 1;
        ProgressUpdate = null;
        if( ProgressDone != null )
          ProgressDone();
        progressMessage = "Done";
      }
      else
      {
        ProgressUpdate();
        progress = (float)index++ / (float)length;
      }
      // this call forces OnGUI to be called repeatedly instead of on-demand
      Repaint();
    }

    /*
    if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "NEW ACTION" ) )
    {
      int count = 0;
      if( Selection.objects.Length > 0 )
      {
        objects = new List<Object>( Selection.objects );
        count = objects.Count;
      }
      if( count > 0 )
      {
        StartJob( "Searching...", count, delegate ()
        {
          
            //objects[index]
          
        }, null );
      }
    }
    */

    EditorGUILayout.Space();
    EditorGUI.ProgressBar( EditorGUILayout.GetControlRect( false, 30 ), progress, progressMessage );

    EditorGUILayout.Space();
    foldTodo = EditorGUILayout.Foldout( foldTodo, "Todo" );
    if( foldTodo )
    {
      string td = EditorGUILayout.TextArea( todo, new GUILayoutOption[] { GUILayout.Height( 200 ) } );
      if( td != todo )
      {
        todo = td;
        File.WriteAllText( Application.dataPath + "/../todo", todo );
      }
    }

    EditorGUILayout.Space();
    foldBuild = EditorGUILayout.Foldout( foldBuild, "Build" );
    if( foldBuild )
    {
      /*if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Run WebGL Build" ) )
      {
        string path = EditorUtility.OpenFolderPanel( "Select WebGL build folder", "", "" );
        if( path.Length != 0 )
        {
          Debug.Log( path );
          Util.Execute( new Util.Command { cmd = "exec /usr/local/bin/static", args = "", dir = path }, true );
        }
      }*/
      developmentBuild = EditorGUILayout.ToggleLeft( "Development Build", developmentBuild );
      EditorGUILayout.BeginHorizontal();
      buildMacOS = EditorGUILayout.ToggleLeft( "MacOS", buildMacOS, GUILayout.MaxWidth( 60 ) );
      buildLinux = EditorGUILayout.ToggleLeft( "Linux", buildLinux, GUILayout.MaxWidth( 60 ) );
      buildWindows = EditorGUILayout.ToggleLeft( "Windows", buildWindows, GUILayout.MaxWidth( 60 ) );
      buildWebGL = EditorGUILayout.ToggleLeft( "WebGL", buildWebGL, GUILayout.MaxWidth( 60 ) );
      EditorGUILayout.EndHorizontal();
      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Build" ) )
      {
        if( BuildPipeline.isBuildingPlayer )
          return;

        
        List<string> buildnames = new List<string>();
        /*
        for( int i = 0; i < EditorBuildSettings.scenes.Length; i++ )
        {
          if( EditorBuildSettings.scenes[i].enabled )
          {
            buildInfo.scenes[i].name = Path.GetFileNameWithoutExtension( EditorBuildSettings.scenes[i].path );
            buildnames.Add( EditorBuildSettings.scenes[i].path );
          }
        }
        */

        /*
        // Trying to get a scene build index from the build settings is hopeless.
        for( int i = 0; i < SceneManager.sceneCountInBuildSettings; i++ )
        {
          string scenepath = SceneUtility.GetScenePathByBuildIndex( i );
          if( scenepath != null )
          {
            int buildIndex = SceneManager.GetSceneByPath( scenepath ).buildIndex;
            ///
            // UNITY BUG: second scene in build settings has index of -1 !!
            ///
            if( buildIndex >= 0 )
            {
              buildnames.Add( scenepath );
              buildInfo.scenes.Add( new BuildMetaData.SceneMeta { buildIndex = buildIndex, name = Path.GetFileNameWithoutExtension( scenepath ) } );
            }
          }
          //string sceneName = path.Substring( 0, path.Length - 6 ).Substring( path.LastIndexOf( '/' ) + 1 );
        }*/

        BuildPlayerOptions bpo = new BuildPlayerOptions();
        bpo.scenes = buildnames.ToArray();
        if( developmentBuild )
          bpo.options = BuildOptions.Development | BuildOptions.AutoRunPlayer;
        else
          bpo.options = BuildOptions.CompressWithLz4;

        if( buildLinux )
        {
          bpo.targetGroup = BuildTargetGroup.Standalone;
          bpo.target = BuildTarget.StandaloneLinux64;
          string outDir = Directory.GetParent( Application.dataPath ).FullName + "/build/Linux";
          outDir += "/Saga." + Util.Timestamp();
          Directory.CreateDirectory( outDir );
          bpo.locationPathName = outDir + "/" + (developmentBuild ? "sagaDEV" : "Saga") + ".x86_64";
          BuildPipeline.BuildPlayer( bpo );
          Debug.Log( bpo.locationPathName );
          // copy to shared folder
          string shareDir = System.Environment.GetFolderPath( System.Environment.SpecialFolder.UserProfile ) + "/SHARE";
          Util.DirectoryCopy( outDir, Path.Combine( shareDir, (developmentBuild ? "sagaDEV." : "Saga.") + Util.Timestamp() ) );
        }
        if( buildWindows )
        {
          bpo.targetGroup = BuildTargetGroup.Standalone;
          bpo.target = BuildTarget.StandaloneWindows64;
          string outDir = Directory.GetParent( Application.dataPath ).FullName + "/build/Windows";
          outDir += "/Saga." + Util.Timestamp();
          Directory.CreateDirectory( outDir );
          bpo.locationPathName = outDir + "/" + (developmentBuild ? "sagaDEV" : "Saga") + ".exe";
          BuildReport report = BuildPipeline.BuildPlayer( bpo );
          Debug.Log( bpo.locationPathName );
        }
        if( buildWebGL )
        {
          bpo.targetGroup = BuildTargetGroup.WebGL;
          bpo.target = BuildTarget.WebGL;
          string outDir = Directory.GetParent( Application.dataPath ).FullName + "/build/WebGL";
          Directory.CreateDirectory( outDir );
          bpo.locationPathName = outDir + "/" + (developmentBuild ? "sagaDEV" : "Saga") + "." + Util.Timestamp();
          BuildPipeline.BuildPlayer( bpo );
          Debug.Log( bpo.locationPathName );
        }
        if( buildMacOS )
        {
          bpo.targetGroup = BuildTargetGroup.Standalone;
          bpo.target = BuildTarget.StandaloneOSX;
          string outDir = Directory.GetParent( Application.dataPath ).FullName + "/build/MacOS/";
          Directory.CreateDirectory( outDir );
          // the extension is replaced with ".app" by Unity
          bpo.locationPathName = outDir += (developmentBuild ? "sagaDEV" : "Saga") + "." + Util.Timestamp() + ".extension";
          BuildPipeline.BuildPlayer( bpo );
          Debug.Log( bpo.locationPathName );
          System.Diagnostics.Process.Start( "open", bpo.locationPathName );
        }
      }
    }

    GUILayout.Space( 10 );
    foldGeneric = EditorGUILayout.Foldout( foldGeneric, "Generic Utils" );
    if( foldGeneric )
    {
      //GUILayout.Label( "Generic Utils", EditorStyles.boldLabel );
      GUILayout.Label( "Character", EditorStyles.boldLabel );
      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Select Player Character" ) )
        if( Application.isPlaying )
          Selection.activeGameObject = Global.instance.CurrentPlayer.gameObject;

      if( Selection.activeGameObject != null )
      {
        //      Character selected = Selection.activeGameObject.GetComponent<Character>();
        //      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Dance, monkey, dance!" ) )
        //      {
        //        // there are no monkeys
        //      }
      }

      layer = (LayerMask)EditorGUILayout.IntField( "layer", layer );
      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Find GameObjects by Layer" ) )
      {
        gos = new List<GameObject>( Resources.FindObjectsOfTypeAll<GameObject>() );
        List<GameObject> found = new List<GameObject>();
        StartJob( "Searching...", gos.Count, delegate ()
        {
          GameObject go = gos[index];
          if( go.layer == layer )
            found.Add( go );
        },
          delegate ()
          {
            foreach( var go in found )
            {
              Debug.Log( "Found: " + go.name, go );
            }
          } );
      }


      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Show GUID of selected assets" ) )
      {
        for( int i = 0; i < Selection.assetGUIDs.Length; i++ )
          Debug.Log( Selection.objects[i].GetType().ToString() + " " + Selection.objects[i].name + " " + Selection.assetGUIDs[i] );
      }

      fixInScene = EditorGUILayout.Toggle( "Fix in Scene", fixInScene );
      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Fix all Nav Obstacles" ) )
      {
        int count;
        if( fixInScene )
        {
          gos = new List<GameObject>( FindObjectsOfType<GameObject>() );
          count = gos.Count;
        }
        else
        {
          assetPaths = new List<string>();
          guids = AssetDatabase.FindAssets( replaceName + " t:prefab", new string[] { "Assets" } );
          foreach( string guid in guids )
            assetPaths.Add( AssetDatabase.GUIDToAssetPath( guid ) );
          count = assetPaths.Count;
        }
        StartJob( "Searching...", count, delegate ()
        {
          GameObject go;
          if( fixInScene )
          {
            go = gos[index];
          }
          else
          {
            go = PrefabUtility.LoadPrefabContents( assetPaths[index] );
          }
          NavMeshObstacle[] navs = go.GetComponentsInChildren<NavMeshObstacle>();
          foreach( var nav in navs )
          {
            SpriteRenderer rdr = nav.gameObject.GetComponent<SpriteRenderer>();
            if( rdr != null )
            {
              nav.carving = true;
              nav.center = rdr.transform.worldToLocalMatrix.MultiplyPoint( rdr.bounds.center );
              nav.size = new Vector3( rdr.size.x, rdr.size.y, 0.3f );
            }
          }
          if( !fixInScene )
          {
            PrefabUtility.SaveAsPrefabAsset( go, assetPaths[index] );
            PrefabUtility.UnloadPrefabContents( go );
          }
        },
        null );
      }

      fudgeFactor = EditorGUILayout.FloatField( "fudge", fudgeFactor );

      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Generate Edge Collider Nav Obstacles" ) )
      {
        GameObject go = new GameObject( "Nav Obstacle" );
        go.transform.position = Vector3.back;
        go.layer = LayerMask.NameToLayer( "Ignore Raycast" );
        //go.AddComponent<MeshRenderer>();
        //MeshFilter mf = go.AddComponent<MeshFilter>();

        List<CombineInstance> combine = new List<CombineInstance>();
        EdgeCollider2D[] edges = FindObjectsOfType<EdgeCollider2D>();
        for( int e = 0; e < edges.Length; e++ )
        {
          EdgeCollider2D edge = edges[e];
          /*
          Vector3[] v = new Vector3[(edge.points.Length - 1) * 4];
          int[] indices = new int[(edge.points.Length) * 4];
          Vector3[] n = new Vector3[v.Length];
          for( int i = 0; i < edge.points.Length - 1; i++ )
          {
            Vector2 a = edge.points[i];
            Vector2 b = edge.points[i + 1];
            Vector2 segment = b - a;
            Vector2 fudge = segment.normalized * fudgeFactor;
            Vector2 cross = (new Vector2( -segment.y, segment.x )).normalized * (edge.edgeRadius + fudgeFactor);
            v[i * 4] = a + cross - fudge;
            v[i * 4 + 1] = a + cross + segment + fudge;
            v[i * 4 + 2] = a - cross + segment + fudge;
            v[i * 4 + 3] = a - cross - fudge;
            n[i * 4] = Vector3.back;
            n[i * 4 + 1] = Vector3.back;
            n[i * 4 + 2] = Vector3.back;
            n[i * 4 + 3] = Vector3.back;
            indices[i * 4] = i * 4;
            indices[i * 4 + 1] = i * 4 + 1;
            indices[i * 4 + 2] = i * 4 + 2;
            indices[i * 4 + 3] = i * 4 + 3;
          }
          */
          CombineInstance ci = new CombineInstance();
          ci.transform = edge.transform.localToWorldMatrix;
          //ci.mesh = new Mesh();
          ci.mesh = edge.CreateMesh( false, false );
          //ci.mesh.vertices = v;
          //ci.mesh.normals = n;
          //ci.mesh.SetIndices( indices, MeshTopology.Quads, 0 );
          combine.Add( ci );
        }

        Mesh mesh = new Mesh();
        if( combine.Count == 1 )
        {
          mesh = combine[0].mesh;
          //go.transform.position = (Vector3)combine[0].transform.GetColumn( 3 );
        }
        else
          mesh.CombineMeshes( combine.ToArray(), false, false );

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        //mf.sharedMesh = mesh;
      }


#if false
    if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Zip Data" ) )
    {
      //string[] scenes = new string[]{ "home" };
      scenes = new List<string>();

      string[] dirs = Directory.GetDirectories( Application.persistentDataPath );
      foreach( var dir in dirs )
      {
        string basename = Path.GetFileName( dir );
        if( basename == "Unity" )
          continue;
        scenes.Add( basename );
      }

      StartJob( "Zipping...", scenes.Count, delegate()
      {
        string basename = scenes[ index ];
        Debug.Log( "Zipping level: " + basename );
        string[] files = Directory.GetFiles( Application.persistentDataPath + "/" + basename );
        ZipUtil.Zip( Application.dataPath + "/Resources/zone/" + basename + ".bytes", files );
      },
        delegate()
        {
          List<string> persistentFilenames = new List<string>();
          foreach( string pfn in Global.persistentFilenames )
            persistentFilenames.Add( Application.persistentDataPath + "/" + pfn );
          ZipUtil.Zip( Application.dataPath + "/Resources/persistent.bytes", persistentFilenames.ToArray() );

          UnityEditor.AssetDatabase.Refresh();
          // highlight folder in project view
          UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath( "Assets/Resources/level/" + scenes[ 0 ] + ".bytes", typeof(UnityEngine.Object) );
          //    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/level/home.bytes", typeof(UnityEngine.Object));
          Selection.activeObject = obj;
          EditorGUIUtility.PingObject( obj );
        } );
    }
#endif

      /*GUILayout.Space( 10 );
      GUILayout.Label( "Audio Sources", EditorStyles.boldLabel );
      audioDistanceMin = EditorGUILayout.IntField( "audioDistanceMin", audioDistanceMin );
      audioDistanceMax = EditorGUILayout.IntField( "audioDistanceMax", audioDistanceMax );
      audioRolloff = (AudioRolloffMode)EditorGUILayout.EnumPopup( "rolloff", audioRolloff );
      audioDoppler = EditorGUILayout.IntField( "doppler", audioDoppler );
      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Apply to All Prefabs!" ) )
      {
        auds = new List<AudioSource>();
        string[] guids = AssetDatabase.FindAssets( "t:prefab" );
        foreach( string guid in guids )
        {
          GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>( AssetDatabase.GUIDToAssetPath( guid ) );
          AudioSource[] source = prefab.GetComponentsInChildren<AudioSource>();
          foreach( var ass in source )
            auds.Add( ass );
        }
        StartJob( "Applying...", auds.Count, delegate ()
        {
          AudioSource ass = auds[index];
          Debug.Log( "audio source modified in prefab: " + ass.gameObject.name, ass.gameObject );
          ass.minDistance = audioDistanceMin;
          ass.maxDistance = audioDistanceMax;
          ass.rolloffMode = audioRolloff;
          ass.dopplerLevel = audioDoppler;
        }, null );
      }*/

    }

    EditorGUILayout.Space( 10 );
    foldReplace = EditorGUILayout.Foldout( foldReplace, "Replace" );
    if( foldReplace )
    {
      //GUILayout.Label( "Replace Common Settings", EditorStyles.boldLabel );
      replaceInScene = EditorGUILayout.Toggle( "Replace in Scene", replaceInScene );
      replaceSelected = EditorGUILayout.Toggle( "Replace Selected", replaceSelected );
      replaceName = EditorGUILayout.TextField( "Find Assets/Objects (leave blank for all scene objects)", replaceName );
      subobjectName = EditorGUILayout.TextField( "SubobjectName", subobjectName );
      replacePrefab = (GameObject)EditorGUILayout.ObjectField( "Replace Prefab", replacePrefab, typeof( GameObject ), false, GUILayout.MinWidth( 50 ) );
      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Replace GameObjects with Prefabs" ) )
      {
        int count = 0;
        if( replaceSelected )
        {
          if( Selection.gameObjects.Length > 0 )
          {
            gos = new List<GameObject>( Selection.gameObjects );
            count = gos.Count;
          }
        }
        else if( replaceInScene )
        {
          gos = new List<GameObject>( FindObjectsOfType<GameObject>() );
          count = gos.Count;
        }
        else
        {
          assetPaths = new List<string>();
          guids = AssetDatabase.FindAssets( replaceName + " t:prefab" );
          foreach( string guid in guids )
            assetPaths.Add( AssetDatabase.GUIDToAssetPath( guid ) );
          count = assetPaths.Count;
        }
        if( count > 0 )
        {
          StartJob( "Searching...", count, delegate ()
          {
            GameObject go;
            if( replaceInScene || replaceSelected )
            {
              go = gos[index];
              if( go.name.StartsWith( replaceName ) )
                ReplaceObjectWithPrefabInstance( go, replacePrefab );
            }
            else
            {
              go = PrefabUtility.LoadPrefabContents( assetPaths[index] );
              for( int i = 0; i < go.transform.childCount; i++ )
              {
                Transform tf = go.transform.GetChild( i );
                if( tf.name.StartsWith( subobjectName ) )
                {
                  ReplaceObjectWithPrefabInstance( tf.gameObject, replacePrefab );
                }
              }
              PrefabUtility.SaveAsPrefabAsset( go, assetPaths[index] );
              PrefabUtility.UnloadPrefabContents( go );
            }
          },
          null );
        }
      }
    }

    EditorGUILayout.Space( 10 );
    foldSprite = EditorGUILayout.Foldout( foldSprite, "Fix Sprite" );
    if( foldSprite )
    {
      GUILayout.Label( "Fix Sprite", EditorStyles.boldLabel );
      fixSpriteInScene = EditorGUILayout.Toggle( "In Scene", fixSpriteInScene );
      fixSpriteName = EditorGUILayout.TextField( "Find Prefabs/Scene Objects", fixSpriteName );
      fixSpriteSubobjectName = EditorGUILayout.TextField( "SubobjectName", fixSpriteSubobjectName );
      sprite = (Sprite)EditorGUILayout.ObjectField( "Replace Sprite", sprite, typeof( Sprite ), false );
      material = (Material)EditorGUILayout.ObjectField( "Replace Material", material, typeof( Material ), false );

      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Fix Sprites" ) )
      {
        int count;
        if( fixSpriteInScene )
        {
          gos = new List<GameObject>( FindObjectsOfType<GameObject>() );
          count = gos.Count;
        }
        else
        {
          assetPaths = new List<string>();
          guids = AssetDatabase.FindAssets( fixSpriteName + " t:prefab" );
          foreach( string guid in guids )
            assetPaths.Add( AssetDatabase.GUIDToAssetPath( guid ) );
          count = assetPaths.Count;
        }
        StartJob( "Searching...", count, delegate ()
        {
          GameObject go;
          if( replaceInScene )
            go = gos[index];
          else
            go = PrefabUtility.LoadPrefabContents( assetPaths[index] );
          Transform[] tfs = go.GetComponentsInChildren<Transform>();
          foreach( var tf in tfs )
          {
            if( fixSpriteSubobjectName.Length == 0 || tf.name.StartsWith( fixSpriteSubobjectName ) )
            {
              SpriteRenderer sr = tf.GetComponent<SpriteRenderer>();
              if( sr != null )
              {
                sr.sprite = sprite;
                sr.material = material;
              }
            }
          }
        },
        AssetDatabase.SaveAssets );
      }
    }

    EditorGUILayout.Space( 10 );
    foldFont = EditorGUILayout.Foldout( foldFont, "Pixel Font" );
    if( foldFont )
    {
      // tested on Unity 2019.2.6f1
      fontImage = (Texture2D)EditorGUILayout.ObjectField( "Pixel Font Image", fontImage, typeof( Texture2D ), false );
      if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Create Pixel Font" ) )
      {
        string letter = " !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        const int imageSize = 256;
        const int cols = 16;
        const int charWith = imageSize / cols;
        string output = "info font=\"Base 5\" size=32 bold=0 italic=0 charset=\"\" unicode=0 stretchH=100 smooth=1 aa=1 padding=0,0,0,0 spacing=2,2\n" +
          "common lineHeight=10 base=12 scaleW=" + imageSize + " scaleH=" + imageSize + " pages=1 packed=0\n" +
          "page id=0 file=\"" + fontImage.name + ".png\"\nchars count=" + letter.Length + "\n";
        for( int i = 0; i < letter.Length; i++ )
          output += "char id=" + i + " x=" + (i % cols) * charWith + " y=" + (i / cols) * charWith + " width=14 height=14 xoffset=1 yoffset=1 xadvance=16 page=0 chnl=0 letter=\"" + letter[i] + "\"\n";
        output += "kernings count=0";
        File.WriteAllText( Application.dataPath + "/font/" + fontImage.name + ".fnt", output );
        AssetDatabase.SaveAssets();

        string fntpath = Path.ChangeExtension( AssetDatabase.GetAssetPath( fontImage ), "fnt" );
        Debug.Log( fntpath );
        AssetDatabase.ImportAsset( fntpath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport );
        AssetDatabase.Refresh();
        litefeel.BFImporter.DoImportBitmapFont( fntpath );

        // set default font values
        string fontsettings = Path.ChangeExtension( AssetDatabase.GetAssetPath( fontImage ), "fontsettings" );
        Font font = AssetDatabase.LoadAssetAtPath<Font>( fontsettings );
        SerializedObject so = new SerializedObject( font );
        so.Update();
        so.FindProperty( "m_AsciiStartOffset" ).intValue = 32;
        so.FindProperty( "m_Tracking" ).floatValue = 0.5f;
        so.FindProperty( "m_CharacterRects" ).GetArrayElementAtIndex( 0 ).FindPropertyRelative( "advance" ).floatValue = 16;
        so.ApplyModifiedProperties();
        so.SetIsDifferentCacheDirty();
      }
    }


  }


  void ReplaceObjectWithPrefabInstance( GameObject replaceThis, GameObject prefab )
  {
    GameObject go = (GameObject)PrefabUtility.InstantiatePrefab( prefab, replaceThis.transform.parent );
    go.transform.position = replaceThis.transform.position;
    go.transform.rotation = replaceThis.transform.rotation;
    SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
    SpriteRenderer tfsr = replaceThis.GetComponent<SpriteRenderer>();
    if( sr != null && tfsr != null )
    {
      sr.size = tfsr.size;
      PrefabUtility.RecordPrefabInstancePropertyModifications( sr );
    }
    NavMeshObstacle gonob = go.GetComponent<NavMeshObstacle>();
    NavMeshObstacle nob = replaceThis.GetComponent<NavMeshObstacle>();
    if( gonob != null && nob != null )
    {
      gonob.size = nob.size;
      gonob.carving = nob.carving;
    }
    PrefabUtility.RecordPrefabInstancePropertyModifications( go.transform );
    DestroyImmediate( replaceThis.gameObject );
  }
}

#if false
void ClearGroundImages()
{
  string[] dirs = Directory.GetDirectories( Application.persistentDataPath );
  foreach( var dir in dirs )
  {
    Debug.Log( dir );
    string[] files = Directory.GetFiles( dir, "*.png" );
    foreach( var f in files )
    {
      File.Delete( f );
    }
  }
}

void ClearGroundOverlayImages()
{
  string[] dirs = Directory.GetDirectories( Application.persistentDataPath );
  foreach( var dir in dirs )
  {
    Debug.Log( dir );
    string[] files = Directory.GetFiles( dir, "*-dirt.png" );
    foreach( var f in files )
    {
      File.Delete( f );
    }
  }
}



// I wrote this for someone on the Unity forums
public class ProgressUpdateExample : EditorWindow
{
  [MenuItem("Tool/ProgressUpdateExample")]
  static void Init()
  {
    ProgressUpdateExample window = (ProgressUpdateExample)EditorWindow.GetWindow(typeof(ProgressUpdateExample));
    window.Show();
  }

  System.Action ProgressUpdate;
  bool processing = false;
  float progress = 0;
  int index=0;
  int length=0;

  List<GameObject> gos = new List<GameObject>();

  void OnGUI()
  {
    if( processing )
    {
      if( index == length )
      {
        processing = false;
        progress = 1;
        ProgressUpdate = null;
      }
      else
      {
        ProgressUpdate();
        progress = (float)index++ / (float)length;
      }
      // IMPORTANT: while processing, this call "drives" OnGUI to be called repeatedly instead of on-demand.
      Repaint();
    }

    EditorGUI.ProgressBar( EditorGUILayout.GetControlRect( false, 30 ), progress, "progress" );

    if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "List all Prefabs" ) )
    {
      // gather prefabs into list
      gos.Clear();
      string[] guids = AssetDatabase.FindAssets("t:prefab");
      foreach (string guid in guids)
      {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>( AssetDatabase.GUIDToAssetPath( guid ) );
        gos.Add( prefab );
      }

      // initialize progress update
      length = gos.Count;
      index = 0;
      progress = 0;
      processing = true;
      ProgressUpdate = delegate() {
        GameObject go = gos[index];
        Debug.Log("prefab: " + go.name, go );
      };
    }
  }
}
#endif


#endif