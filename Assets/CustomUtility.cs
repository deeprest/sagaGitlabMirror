﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

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
      Debug.Log( Selection.objects[ i ].GetType().ToString() + " " + Selection.objects[ i ].name + " " + Selection.assetGUIDs[ i ] );
  }
}

public class CustomUtility : EditorWindow
{
  [MenuItem( "Tool/Utility Window" )]
  static void Init()
  {
    // Get existing open window or if none, make a new one:
    CustomUtility window = (CustomUtility)EditorWindow.GetWindow( typeof(CustomUtility) );
    window.Show();
  }

  int audioDoppler = 0;
  int audioDistanceMin = 1;
  int audioDistanceMax = 30;
  AudioRolloffMode audioRolloff = AudioRolloffMode.Logarithmic;

  string progressMessage = "";
  float progress = 0;
  bool processing = false;
  System.Action ProgressUpdate;
  System.Action ProgressDone;
  int index = 0;
  int length = 0;

  List<AudioSource> auds;
  List<string> scenes;
  List<GameObject> gameobjects;
  List<AnimSequence> ans;
  int layer;

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
      // this call "drives" OnGUI to be called repeatedly instead of on-demand
      Repaint();
    }
    EditorGUILayout.Space();
    EditorGUI.ProgressBar( EditorGUILayout.GetControlRect( false, 30 ), progress, progressMessage );

    GUILayout.Label( "Build", EditorStyles.boldLabel );
    if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Build Release" ) )
    {
      if( BuildPipeline.isBuildingPlayer )
        return;
      BuildPlayerOptions bpo = new BuildPlayerOptions ();
      bpo.target = BuildTarget.StandaloneOSX;
      bpo.scenes = new string[]{ "Assets/zero.unity", "Assets/mmx-city.unity" };
      bpo.options = BuildOptions.CompressWithLz4; //BuildOptions.AutoRunPlayer;
      bpo.locationPathName = Directory.GetParent( Application.dataPath ).FullName + "/SagaCity-" + Util.Timestamp();
      Debug.Log( bpo.locationPathName );
      BuildPipeline.BuildPlayer( bpo );
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

   
    GUILayout.Space( 10 );
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
      StartJob( "Applying...", auds.Count, delegate()
      {
        AudioSource ass = auds[ index ];
        Debug.Log( "audio source modified in prefab: " + ass.gameObject.name, ass.gameObject );
        ass.minDistance = audioDistanceMin;
        ass.maxDistance = audioDistanceMax;
        ass.rolloffMode = audioRolloff;
        ass.dopplerLevel = audioDoppler;
      }, null );
    }


    GUILayout.Space( 10 );
    GUILayout.Label( "Generic Utils", EditorStyles.boldLabel );
    layer = (LayerMask)EditorGUILayout.IntField( "layer", layer );
    if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Find GameObjects by Layer" ) )
    {
      gameobjects = new List<GameObject>( Resources.FindObjectsOfTypeAll<GameObject>() );
      List<GameObject> found = new List<GameObject>();
      StartJob( "Searching...", gameobjects.Count, delegate()
      {
        GameObject go = gameobjects[ index ];
        if( go.layer == layer )
          found.Add( go );
      },
        delegate()
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
        Debug.Log( Selection.objects[ i ].GetType().ToString() + " " + Selection.objects[ i ].name + " " + Selection.assetGUIDs[ i ] );
    }

    /*GUILayout.Label( "AnimFix", EditorStyles.boldLabel );
    if( GUI.Button( EditorGUILayout.GetControlRect( false, 30 ), "Apply" ) )
    {
      ans = new List<AnimSequence>();
      string[] guids = AssetDatabase.FindAssets( "t:AnimSequence" );
      foreach( string guid in guids )
      {
        AnimSequence seq = AssetDatabase.LoadAssetAtPath<AnimSequence>( AssetDatabase.GUIDToAssetPath( guid ) );
        ans.Add( seq );
      }
      StartJob( "Applying...", ans.Count, delegate()
      {
        AnimSequence seq = ans[ index ];
        Debug.Log( "modified: " + seq.name + " " + index, seq );
        seq.frames = new AnimFrame[seq.frames.Length];
        for( int i = 0; i < seq.frames.Length; i++ )
        {
          seq.frames[ i ].sprite = seq.frames[i];
        }

      }, null );
    }*/


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
  }

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
}

#if false
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