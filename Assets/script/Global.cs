// #pragma warning disable 414

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;
using LitJson;
using Ionic.Zip;
// deeprest.SerializedObject
using UnityEngine.Profiling;

using deeprest;
using UnityEngine.Serialization;
using Gizmos = Popcron.Gizmos;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof(Global) )]
public class GlobalEditor : Editor
{
  Global obj;

  public override void OnInspectorGUI()
  {
    //screenshotInterval = EditorGUILayout.IntField( "Screenshot Interval", screenshotInterval );
    obj = target as Global;
    if( obj.ScreenshotTimer.IsActive )
    {
      if( GUI.Button( EditorGUILayout.GetControlRect(), "Stop Screenshot Timer" ) )
        obj.ScreenshotTimer.Stop( false );
    }
    else
    {
      if( GUI.Button( EditorGUILayout.GetControlRect(), "Start Screenshot Timer" ) )
        StartTimer();
    }
    DrawDefaultInspector();
  }

  void StartTimer()
  {
    obj.ScreenshotTimer.Start( obj.screenshotInterval, null, delegate
    {
      Util.Screenshot();
      StartTimer();
    } );
  }
}
#endif

public class Global : MonoBehaviour
{
  public static Global instance;
  public static bool Paused = false;
  public static bool Slowed = false;
  public static bool IsQuiting = false;

  [Header( "Global Settings" )]
  [SerializeField] SceneReference[] sceneRefs;
  public bool RandomSeedOnStart = false;
  [Tooltip( "Pretend this is a build we're running" )]
  public bool SimulatePlayer = false;
  public static float Gravity = 16;
  public const float MaxVelocity = 60;
  [SerializeField] SceneReference InitialScene;
  // screenshot timer
  public int screenshotInterval;
  public Timer ScreenshotTimer = new Timer();
  // timescale when in slo-mo
  [SerializeField] float slowtime = 0.2f;
  Timer fadeTimer = new Timer();
  public float RepathInterval = 1;
  // sidestep
  public bool GlobalSidestepping = true;
  public float SidestepDistance = 1f;
  public float SidestepRaycastDistance = 1f;
  public float SidestepInterval = 1f;
  public float SidestepIgnoreWithinDistanceToGoal = 0.5f;

  public static string[] persistentFilenames = new string[]
  {
    /*"settings.json",
    "characters.json",
    "events.json"*/
  };

  public static int CharacterCollideLayers;
  public static int CharacterSidestepLayers;
  public static int CharacterDamageLayers;
  public static int TriggerLayers;
  public static int WorldSelectableLayers;
  public static int ProjectileNoShootLayers;
  public static int DefaultProjectileCollideLayers;
  public static int FlameProjectileCollideLayers;
  public static int DamageCollideLayers;
  public static int StickyBombCollideLayers;
  public static int EnemyInterestLayers;
  public static int SightObstructionLayers;

  [Header( "Settings" )]
  public GameObject ToggleTemplate;
  public GameObject SliderTemplate;
  public GameObject StringTemplate;

  string settingsPath
  {
    get { return Application.persistentDataPath + "/" + "settings.json"; }
  }

  public Dictionary<string, FloatValue> FloatSetting = new Dictionary<string, FloatValue>();
  public Dictionary<string, BoolValue> BoolSetting = new Dictionary<string, BoolValue>();
  public Dictionary<string, StringValue> StringSetting = new Dictionary<string, StringValue>();
  // screen settings
  public GameObject ScreenSettingsPrompt;
  public Text ScreenSettingsCountdown;
  Timer ScreenSettingsCountdownTimer = new Timer();

  [Header( "References" )]
  public CameraController CameraController;
  [SerializeField] AudioClip VolumeChangeNoise;

  [Header( "Prefabs" )]
  public GameObject audioOneShotPrefab;
  public GameObject AvatarPrefab;

  [Header( "Transient (Assigned at runtime)" )]
  public bool Updating = false;
  public SceneScript sceneScript;
  public Pawn CurrentPlayer;
  public PlayerController PlayerController;
  public Dictionary<string, int> AgentType = new Dictionary<string, int>();
  NavMeshSurface[] meshSurfaces;

  [Header( "UI" )]
  public GameObject UI;
  CanvasScaler CanvasScaler;
  public UIScreen PauseMenu;
  [SerializeField] UIScreen SceneList;
  public GameObject SceneListElementTemplate;
  [SerializeField] UIScreen MusicList;
  public GameObject MusicListElementTemplate;
  [SerializeField] GameObject HUD;
  DiegeticUI ActiveDiegetic;

  bool MenuShowing
  {
    get { return PauseMenu.gameObject.activeInHierarchy; }
  }

  public GameObject LoadingScreen;
  [SerializeField] Image fader;
  public GameObject ready;
  [SerializeField] GameObject OnboardingControls;
  [SerializeField] Image RecordingIndicator;
  // cursor
  public float CursorOuter = 1;
  public float CursorSensitivity = 1;
  public bool AimSnap;
  public bool AutoAim;
  public bool ShowAimPath;
  // status
  public Image weaponIcon;
  public Image abilityIcon;
  // settings
  public GameObject SettingsParent;
  [SerializeField] Selectable previousNavSelectable;
  public UIScreen ConfirmDialog;

  [Header( "Input" )]
  [SerializeField] InputSystemUIInputModule UIInputModule;
  public Controls Controls;
  public bool UsingGamepad;
  public System.Action OnGameControllerChanged;
  public Color ControlNameColor = Color.red;
  Dictionary<string, string> ReplaceControlNames = new Dictionary<string, string>();

  [Header( "Debug" )]
  [SerializeField] Text debugFPS;
  [SerializeField] Text debugText;
  [SerializeField] Text debugText2;
  [SerializeField] Text debugText3;
  [SerializeField] Text debugText4;
  // loading screen
  bool loadingScene;
  float prog = 0;
  [SerializeField] Image progress;
  [SerializeField] float progressSpeed = 0.5f;

  [Header( "Misc" )]
  Timer fpsTimer;
  int frames;
  float zoomDelta = 0;
  // color shift
  public Color shiftyColor = Color.red;
  [SerializeField] float colorShiftSpeed = 1;
  [SerializeField] Image shifty;
  // spinner on loading screen
  float progTarget = 0;
  [SerializeField] GameObject spinner;
  [SerializeField] float spinnerMoveSpeed = 1;
  int spawnCycleIndex = 0;

  public Dictionary<string, GameObject> ResourceLookup = new Dictionary<string, GameObject>();

  [FormerlySerializedAs( "music" ),Header( "Audio" )]
  [SerializeField] AudioLoop[] Music;
  public UnityEngine.Audio.AudioMixer mixer;
  public UnityEngine.Audio.AudioMixerSnapshot snapSlowmo;
  public UnityEngine.Audio.AudioMixerSnapshot snapNormal;
  public UnityEngine.Audio.AudioMixerSnapshot snapMusicSilence;
  public float MusicTransitionDuration = 0.5f;
  public float AudioFadeDuration = 0.1f;
  [SerializeField] AudioSource musicSource0;
  [SerializeField] AudioSource musicSource1;
  AudioSource activeMusicSource;


  [Header( "Speech" )]
  public GameObject SpeechBubble;
  public Text SpeechText;
  public SpriteRenderer SpeechIcon;
  public Animator SpeechAnimator;
  [SerializeField] Camera SpeechIconCamera;
  CharacterIdentity SpeechCharacter;
  int SpeechPriority = 0;
  public float SpeechRange = 8;
  Timer SpeechTimer = new Timer();

  [Header( "Minimap" )]
  [SerializeField] Camera MinimapCamera;
  [SerializeField] GameObject Minimap;
  [FormerlySerializedAs( "bigsheet" ),SerializeField] Material bigsheetMaterial;
  [SerializeField] Material backgroundMaterial;
  [SerializeField] GameObject MinimapScroller;
  [SerializeField] float mmScrollSpeed = 800;

  // This will make sure there is always a GLOBAL object when playing a scene in the editor
  [RuntimeInitializeOnLoadMethod]
  static void OnLoadMethod()
  {
    Application.wantsToQuit += WantsToQuit;
  }

  static bool WantsToQuit()
  {
    IsQuiting = true;
    // do pre-quit stuff here
    if( Global.instance != null )
      Global.instance.WriteSettings();
    return true;
  }

  void Awake()
  {
    if( instance != null )
    {
      Destroy( gameObject );
      return;
    }
    instance = this;
    DontDestroyOnLoad( gameObject );

    // todo see what this does
    //Application.targetFrameRate = 60;

    // note: allowing characters to collide with each other introduces the risk of being forced into a corner.
    CharacterCollideLayers = LayerMask.GetMask( new string[] {"Default", "destructible", "triggerAndCollision"} );
    CharacterSidestepLayers = LayerMask.GetMask( new string[] {"character"} );
    CharacterDamageLayers = LayerMask.GetMask( new string[] {"character", "destructible"} );
    TriggerLayers = LayerMask.GetMask( new string[] {"trigger", "triggerAndCollision"} );
    WorldSelectableLayers = LayerMask.GetMask( new string[] {"worldselect"} );
    ProjectileNoShootLayers = LayerMask.GetMask( new string[] {"Default"} );
    DefaultProjectileCollideLayers = LayerMask.GetMask( new string[] {"Default", "character", "projectileCollisionOnly", "triggerAndCollision", "destructible", "bouncyGrenade"} );
    FlameProjectileCollideLayers = LayerMask.GetMask( new string[] {"Default", "character", "triggerAndCollision", "destructible", "bouncyGrenade"} );
    DamageCollideLayers = LayerMask.GetMask( new string[] {"character", "triggerAndCollision", "projectile", "destructible"} );
    StickyBombCollideLayers = LayerMask.GetMask( new string[] {"Default", "character", "triggerAndCollision", "projectile", "destructible"} );
    EnemyInterestLayers = LayerMask.GetMask( new string[] { "character" } );
    SightObstructionLayers = LayerMask.GetMask( new string[] {"Default", "triggerAndCollision", "destructible"} );

    CanvasScaler = UI.GetComponent<CanvasScaler>();
    InitializeSettings();
    ReadSettings();
    ApplyScreenSettings();
    InitializeInput();

    PlayerController = ScriptableObject.CreateInstance<PlayerController>();
    
    // SCRIPT EXECUTION ORDER Global.cs is first priority so that Awake() called from scene load in editor respects the code below.
    //Entity.Limit.UpperLimit = 1000;
    Entity.Limit.EnforceUpper = false;

    SceneManager.sceneLoaded += delegate( Scene arg0, LoadSceneMode arg1 )
    {
      //Debug.Log( "scene loaded: " + arg0.name );
    };
    SceneManager.activeSceneChanged += delegate( Scene arg0, Scene arg1 )
    {
      //Debug.Log( "active scene changed from " + arg0.name + " to " + arg1.name );
    };

    GameObject[] res = Resources.LoadAll<GameObject>( "" );
    foreach( GameObject go in res )
      ResourceLookup.Add( go.name, go );

    meshSurfaces = FindObjectsOfType<NavMeshSurface>();
    foreach( var mesh in meshSurfaces )
      AgentType[NavMesh.GetSettingsNameFromID( mesh.agentTypeID )] = mesh.agentTypeID;

    fpsTimer = new Timer( int.MaxValue, 1, delegate( Timer tmr )
    {
      debugFPS.text = frames.ToString();
      frames = 0;
    }, null );

    if( Camera.main.orthographic )
      debugText.text = Camera.main.orthographicSize.ToString( "##.#" );
    else
      debugText.text = CameraController.zOffset.ToString( "##.#" );

    if( Camera.main.orthographic )
      Camera.main.orthographicSize = 2;
    else
      Camera.main.fieldOfView = 20;

    RecordingIndicator.gameObject.SetActive( false );

    HideHUD();
    HideMinimap();
    HidePauseMenu();
    HideLoadingScreen();
    SpeechBubble.SetActive( false );
    // musicSource1 is used as the loop source
    activeMusicSource = musicSource1;
    Gizmos.Enabled = false;
  }

  void Start()
  {
#if UNITY_EDITOR
    // workaround for Unity Editor bug where AudioMixer.SetFloat() does not work in Awake()
    mixer.SetFloat( "MasterVolume", Util.DbFromNormalizedVolume( FloatSetting["MasterVolume"].Value ) );
    mixer.SetFloat( "MusicVolume", Util.DbFromNormalizedVolume( FloatSetting["MusicVolume"].Value ) );
    mixer.SetFloat( "SFXVolume", Util.DbFromNormalizedVolume( FloatSetting["SFXVolume"].Value ) );
#endif
    
    if( Application.isEditor && !SimulatePlayer )
    {
      LoadingScreen.SetActive( false );
      fader.color = Color.clear;
      Updating = true;
      sceneScript = FindObjectOfType<SceneScript>();
      if( sceneScript != null )
        sceneScript.StartScene();
      GameObject generatedMeshCollider = Util.GenerateNavMeshForEdgeColliders();
      foreach( var mesh in meshSurfaces )
        mesh.BuildNavMesh();
      Destroy( generatedMeshCollider );
      if( CurrentPlayer == null )
        SpawnPlayer();
    }
    else
    {
      LoadScene( InitialScene, true, true, true, false, null );
    }
    
  }

  public string ReplaceWithControlNames( string source, bool colorize = true )
  {
    // This is slow. DO NOT call this every frame.
    // todo make this less awful
    // todo support composites
    //action.GetBindingDisplayString( InputBinding.DisplayStringOptions.DontUseShortDisplayNames );

    string outstr = "";
    string[] tokens = source.Split( new char[] {'['} );
    foreach( var tok in tokens )
    {
      if( tok.Contains( "]" ) )
      {
        string[] ugh = tok.Split( new char[] {']'} );
        if( ugh.Length > 2 )
          return "BAD FORMAT";
        int inputTypeIndex = UsingGamepad ? 1 : 0;
        // warning!! controls must be in correct order per-action: keyboard+mouse first, then gamepad
        InputAction ia = Controls.BipedActions.Get().FindAction( ugh[0] );
        if( ia == null ) ia = Controls.MenuActions.Get().FindAction( ugh[0] );
        if( ia == null ) ia = Controls.GlobalActions.Get().FindAction( ugh[0] );
        if( ia == null ) return "ACTION NOT FOUND: " + ugh[0];
        if( ia.controls.Count <= inputTypeIndex )
          return "(no binding)";
        InputControl ic = ia.controls[inputTypeIndex];
        string controlName = "BAD NAME";
        if( ic.shortDisplayName != null )
          controlName = ic.shortDisplayName;
        else
          controlName = ic.name.ToUpper();

        if( ReplaceControlNames.ContainsKey( controlName.ToUpper() ) )
          controlName = ReplaceControlNames[controlName.ToUpper()];

        if( colorize )
          outstr += "<color=#" + ColorUtility.ToHtmlStringRGB( ControlNameColor ) + ">" + controlName + "</color>";
        else
          outstr += controlName;
        outstr += ugh[1];
      }
      else
        outstr += tok;
    }
    return outstr;
  }

  void InitializeInput()
  {
    InputSystem.onDeviceChange += ( device, change ) =>
    {
      switch( change )
      {
        case InputDeviceChange.Added:
          Debug.Log( "Device added: " + device );
          break;
        case InputDeviceChange.Removed:
          Debug.Log( "Device removed: " + device );
          break;
        case InputDeviceChange.ConfigurationChanged:
          Debug.Log( "Device configuration changed: " + device );
          break;
      }
    };

    ReplaceControlNames.Add( "DELTA", "Mouse" );
    ReplaceControlNames.Add( "LMB", "Left Mouse Button" );
    ReplaceControlNames.Add( "RMB", "Right Mouse Button" );
    ReplaceControlNames.Add( "LB", "Left Bumper" );
    ReplaceControlNames.Add( "RB", "Right Bumper" );
    ReplaceControlNames.Add( "LT", "Left Trigger" );
    ReplaceControlNames.Add( "RT", "Right Trigger" );

    UIInputModule.enabled = false;
    Controls = new Controls();
    Controls.Enable();
    Controls.MenuActions.Disable();

    Controls.GlobalActions.Any.performed += ( obj ) =>
    {
      bool newvalue = obj.control.device.name.Contains( "Gamepad" );
      if( newvalue != UsingGamepad )
      {
        UsingGamepad = newvalue;
        OnGameControllerChanged.Invoke();
        OnboardingControls.GetComponent<OnboardingControls>().UpdateText();
      }
    };

    Controls.GlobalActions.Menu.performed += ( obj ) => TogglePauseMenu();
    Controls.GlobalActions.Pause.performed += ( obj ) =>
    {
      if( Paused )
        Unpause();
      else
        Pause();
    };

    Controls.GlobalActions.Screenshot.performed += ( obj ) => Util.Screenshot();

    /*Controls.GlobalActions.DEVClone.performed += ( obj ) => {
      GameObject go = Spawn( AvatarPrefab, (Vector2)PlayerController.GetPawn().transform.position + Vector2.right, Quaternion.identity, null, false );
      PlayerBiped pawn = go.GetComponent<PlayerBiped>();
      pawn.SpeedFactorNormalized = ((PlayerBiped)PlayerController.GetPawn()).SpeedFactorNormalized;
      PlayerController.AddMinion( pawn );
    };*/

    Controls.GlobalActions.DEVRespawn.performed += ( obj ) =>
    {
      /*Chopper chopper = FindObjectOfType<Chopper>();
      if( chopper != null )
        chopper.StartDrop( CurrentPlayer );
      else*/
      {
        PlayerController.pawn.transform.position = FindRandomSpawnPosition();
        PlayerController.pawn.velocity = Vector2.zero;
        /*if( sceneScript != null )
          sceneScript.AssignCameraZone( sceneScript.CameraZone );*/
      }
    };

    Controls.GlobalActions.RecordToggle.performed += ( obj ) =>
    {
      PlayerController.RecordToggle();
      RecordingIndicator.gameObject.SetActive( PlayerController.IsRecording() );
    };

    Controls.GlobalActions.RecordPlayback.performed += ( obj ) =>
    {
      PlayerController.PlaybackToggle();
      RecordingIndicator.gameObject.SetActive( PlayerController.IsRecording() );
    };

    Controls.GlobalActions.DevSlowmo.performed += ( obj ) =>
    {
      if( Slowed )
        NoSlow();
      else
        Slow();
    };

    Controls.MenuActions.Back.performed += ( obj ) =>
    {
      if( MenuShowing )
      {
        PauseMenu.Back();
      }
      else
      {
        // ...or if there is none, close diagetic UI
        if( ActiveDiegetic != null && CurrentPlayer != null )
          CurrentPlayer.UnselectWorldSelection();
      }
    };

    // DEVELOPMENT

    Controls.GlobalActions.DEVGizmos.performed += ( obj ) => { Gizmos.Enabled = !Gizmos.Enabled; };
    // Controls.BipedActions.DEVZoom.started += ( obj ) => { zoomDelta += obj.ReadValue<float>(); };
    Controls.GlobalActions.Minimap.performed += ( obj ) => { ToggleMinimap(); };
    // Controls.BipedActions.DEVBig.performed += context => ((PlayerBiped) PlayerController.pawn).ScaleChange( 2 );
    // Controls.BipedActions.DEVSmall.performed += context => ((PlayerBiped) PlayerController.pawn).ScaleChange( 0.5f );

    if( Application.isEditor )
    {
      InputAction DevCursorUnlock = new InputAction( "CursorUnlock", InputActionType.Button, "<Keyboard>/escape" );
      DevCursorUnlock.Enable();
      DevCursorUnlock.performed += context =>
      {
        if( Cursor.lockState != CursorLockMode.None )
        {
          Cursor.lockState = CursorLockMode.None;
          Cursor.visible = true;
        }
        else
        {
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;
        }
      };
      Controls.GlobalActions.Menu.AddBinding( "<Keyboard>/tab" );
    }
    else
    {
      Controls.GlobalActions.Menu.AddBinding( "<Keyboard>/escape" );
    }

  }

  public void LoadScene( SceneReference scene, bool waitForFadeIn = true, bool spawnPlayer = true, bool fadeOut = true, bool showLoadingScreen = true, System.Action onFail = null )
  {
    StartCoroutine( LoadSceneRoutine( scene.GetSceneName(), waitForFadeIn, spawnPlayer, fadeOut, showLoadingScreen, onFail ) );
  }

  public void LoadScene( string scene, bool waitForFadeIn = true, bool spawnPlayer = true, bool fadeOut = true, bool showLoadingScreen = true, System.Action onFail = null )
  {
    StartCoroutine( LoadSceneRoutine( scene, waitForFadeIn, spawnPlayer, fadeOut, showLoadingScreen, onFail ) );
  }

  IEnumerator LoadSceneRoutine( string scene, bool waitForFadeIn = true, bool spawnPlayer = true, bool fadeOut = true, bool showLoadingScreen = true, System.Action onFail = null )
  {
    Updating = false;
    if( fadeOut )
    {
      FadeBlack();
      while( fadeTimer.IsActive )
        yield return null;
    }
    else
    {
      fader.color = Color.black;
      fader.gameObject.SetActive( true );
      yield return null;
    }
    // music fade out
    Timer musicTimer = new Timer();
    TimerParams musicTimerParamsFadeOut = new TimerParams
    {
      unscaledTime = true,
      repeat = false,
      duration = MusicTransitionDuration,
      UpdateDelegate = delegate( Timer obj )
      {
        musicSource0.volume = 1 - obj.ProgressNormalized;
        musicSource1.volume = 1 - obj.ProgressNormalized;
      },
      CompleteDelegate = delegate
      {
        musicSource0.volume = 0;
        musicSource1.volume = 0;
        StopMusic();
      }
    };
    musicTimer.Start( musicTimerParamsFadeOut );
    Pause();
    HUD.SetActive( false );
    if( showLoadingScreen )
      yield return ShowLoadingScreenRoutine( "Loading... " + scene );
    if( CurrentPlayer != null )
    {
      CurrentPlayer.PreSceneTransition();
      SceneManager.MoveGameObjectToScene( CurrentPlayer.gameObject, gameObject.scene );
    }
    loadingScene = true;
    //progress.fillAmount = 0;
    //prog = 0;
    yield return new WaitForSecondsRealtime( 1 );
    AsyncOperation ao = SceneManager.LoadSceneAsync( scene, LoadSceneMode.Single );
    if( ao != null )
    {
      while( !ao.isDone )
      {
        //prog = Mathf.MoveTowards( prog, ao.progress, Time.unscaledDeltaTime * progressSpeed );
        //progress.fillAmount = prog;
        //progTarget = ao.progress;
        //print( ao.progress.ToString() );
        yield return null;
      }
      TimerParams musicTimerParamsFadeIn = new TimerParams
      {
        unscaledTime = true,
        repeat = false,
        duration = MusicTransitionDuration,
        UpdateDelegate = delegate( Timer obj )
        {
          musicSource0.volume = obj.ProgressNormalized;
          musicSource1.volume = obj.ProgressNormalized;
        },
        CompleteDelegate = delegate
        {
          musicSource0.volume = 1;
          musicSource1.volume = 1;
        }
      };
      musicTimer.Start( musicTimerParamsFadeIn );

      if( CurrentPlayer == null )
      {
        if( spawnPlayer )
          SpawnPlayer();
      }
      else
      {
        CurrentPlayer.transform.position = FindSpawnPosition();
        CurrentPlayer.PostSceneTransition();
      }
      
      sceneScript = FindObjectOfType<SceneScript>();
      if( sceneScript != null )
        sceneScript.StartScene();
      
      GameObject generatedMeshCollider = Util.GenerateNavMeshForEdgeColliders();
      foreach( var mesh in meshSurfaces )
        mesh.BuildNavMesh();
      Destroy( generatedMeshCollider );
      
      //Global.instance.MinimapRender( meshSurfaces[0].center );
    }
    else
    {
      Debug.LogError( "Scene failed to load: " + scene );
      onFail?.Invoke();
    }

    loadingScene = false;
    HUD.SetActive( true );
    Unpause();
    if( showLoadingScreen )
      HideLoadingScreen();
    FadeClear();
    if( waitForFadeIn )
      while( fadeTimer.IsActive )
        yield return null;
    Updating = true;
  }

  void Update()
  {
    frames++;
    Timer.UpdateTimers();
    debugText2.text = "Active Timers: " + Timer.ActiveTimers.Count;
    debugText3.text = "Remove Timers: " + Timer.RemoveTimers.Count;
    debugText4.text = "New Timers: " + Timer.NewTimers.Count;

    if( !Updating )
      return;
    
    for( int i = 0; i < Entity.Limit.All.Count; i++ )
      Entity.Limit.All[ i ].EntityUpdate();

    for( int i = 0; i < Controller.All.Count; i++ )
      Controller.All[i].Update();
    
    // pawns are not added to Limit
    for( int i = 0; i < Controller.All.Count; i++ )
      if( Controller.All[i].pawn != null )
        Controller.All[i].pawn.EntityUpdate();

    if( loadingScene )
    {
      //prog = Mathf.MoveTowards( prog, progTarget, Time.unscaledDeltaTime * progressSpeed );
      //progress.fillAmount = prog;
    }

    float H = 0;
    float S = 1;
    float V = 1;
    Color.RGBToHSV( shiftyColor, out H, out S, out V );
    H += colorShiftSpeed * Time.unscaledDeltaTime;
    shiftyColor = Color.HSVToRGB( H, 1, 1 );
    shifty.color = shiftyColor;

    // DEBUG ZOOM WITH MOUSE
    /*
    if( Camera.main.orthographic )
    {
      if( Mathf.Abs( zoomDelta ) > 0 )
      {
        CameraController.orthoTarget += zoomDelta * Time.deltaTime;
        CameraController.orthoTarget = Mathf.Clamp( CameraController.orthoTarget, 1, 10 );
        FloatSetting["Zoom"].Value = CameraController.orthoTarget;
      }
      */
      debugText.text = Camera.main.orthographicSize.ToString( "##.#" ); 
      /*
    }
    else
    {
      CameraController.zOffset += zoomDelta * Time.deltaTime;
      debugText.text = CameraController.zOffset.ToString( "##.#" );
    }
    zoomDelta = 0;
    */

    if( Minimap.activeInHierarchy )
    {
      MinimapScroller.transform.position += (Vector3) (-Controls.MenuActions.Move.ReadValue<Vector2>() * mmScrollSpeed * Time.unscaledDeltaTime);
      mmPlayer.anchoredPosition = 2 * MinimapCamera.worldToCameraMatrix.MultiplyPoint( PlayerController.pawn.transform.position );
    }
  }

  void OnApplicationFocus( bool hasFocus )
  {
    if( hasFocus )
      Cursor.lockState = ActiveDiegetic || Paused ? CursorLockMode.None : CursorLockMode.Locked;
    else
      Cursor.lockState = CursorLockMode.None;
    Cursor.visible = (Cursor.lockState == CursorLockMode.None);
  }

  void LateUpdate()
  {
    if( !Updating )
      return;

    Profiler.BeginSample( "EntityLateUpdate" );
    for( int i = 0; i < Entity.Limit.All.Count; i++ )
    {
      Entity.Limit.All[ i ].EntityLateUpdate();
    }
    Profiler.EndSample();
    
    CameraController.CameraLateUpdate();
  }

  public void SpawnPlayer()
  {
    GameObject go = Spawn( AvatarPrefab, FindSpawnPosition(), Quaternion.identity, null, false );
    Pawn pawn = go.GetComponent<Pawn>();
    CurrentPlayer = pawn;
    PlayerController.AssignPawn( pawn );
    //Global.instance.CameraController.orthoTarget = 3;
  }

  public Vector3 FindSpawnPosition()
  {
    // todo find more appropriate position based on some criteria
    GameObject go = null;
    go = GameObject.FindGameObjectWithTag( "FirstSpawn" );
    if( go == null )
      go = GameObject.FindGameObjectWithTag( "Respawn" );
    if( go != null )
      return go.transform.position;
    return Vector3.zero;
  }

  public Vector3 FindRandomSpawnPosition()
  {
    // cycle through random spawn points
    GameObject go = null;
    List<GameObject> gos = new List<GameObject>();
    gos.AddRange( GameObject.FindGameObjectsWithTag( "Respawn" ) );
    gos.AddRange( GameObject.FindGameObjectsWithTag( "FirstSpawn" ) );
    GameObject[] spawns = gos.ToArray();
    if( spawns.Length > 0 )
    {
      spawnCycleIndex %= spawns.Length;
      go = spawns[spawnCycleIndex++];
    }
    if( go != null )
    {
      Vector3 pos = go.transform.position;
      pos.z = 0;
      return pos;
    }
    return Vector3.zero;
  }

  public void Slow()
  {
    Slowed = true;
    Time.timeScale = slowtime;
    Time.fixedDeltaTime = 0.02f * Time.timeScale;
    mixer.TransitionToSnapshots( new UnityEngine.Audio.AudioMixerSnapshot[]
    {
      snapNormal,
      snapSlowmo
    }, new float[]
    {
      0,
      1
    }, AudioFadeDuration );
  }

  public void NoSlow()
  {
    Slowed = false;
    Time.timeScale = 1;
    Time.fixedDeltaTime = 0.02f * Time.timeScale;
    mixer.TransitionToSnapshots( new UnityEngine.Audio.AudioMixerSnapshot[]
    {
      snapNormal,
      snapSlowmo
    }, new float[]
    {
      1,
      0
    }, AudioFadeDuration );
  }

  public static void Pause()
  {
    Paused = true;
    Time.timeScale = 0;
  }

  public static void Unpause()
  {
    Paused = false;
    Time.timeScale = 1;
  }

  public void ShowHUD()
  {
    HUD.SetActive( true );
  }

  public void HideHUD()
  {
    HUD.SetActive( false );
  }

  void ShowPauseMenu()
  {
    if( ScreenSettingsCountdownTimer.IsActive )
      return;
    Pause();
    mixer.SetFloat( "MusicVolume", Util.DbFromNormalizedVolume( FloatSetting["MusicVolume"].Value * 0.8f ) );
    mixer.SetFloat( "SFXVolume", Util.DbFromNormalizedVolume( FloatSetting["SFXVolume"].Value * 0.8f ) );
    if( ActiveDiegetic != null )
    {
      ActiveDiegetic.InteractableOff();
    }
    else
    {
      PlayerController.OnPauseMenu();
      Controls.MenuActions.Enable();
    }
    PauseMenu.Select();
    HUD.SetActive( false );
    UnityEngine.Cursor.lockState = CursorLockMode.None;
    UnityEngine.Cursor.visible = true;
    EnableRaycaster( true );
    UIInputModule.enabled = true;
  }

  void HidePauseMenu()
  {
    if( ScreenSettingsCountdownTimer.IsActive )
      return;
    Unpause();
    mixer.SetFloat( "MusicVolume", Util.DbFromNormalizedVolume( FloatSetting["MusicVolume"].Value ) );
    mixer.SetFloat( "SFXVolume", Util.DbFromNormalizedVolume( FloatSetting["SFXVolume"].Value ) );
    PauseMenu.Unselect();
    HUD.SetActive( true );
    EnableRaycaster( false );
    if( ActiveDiegetic != null )
    {
      ActiveDiegetic.InteractableOn();
      UnityEngine.Cursor.lockState = CursorLockMode.None;
      UnityEngine.Cursor.visible = true;
    }
    else
    {
      UIInputModule.enabled = false;
      PlayerController.OnUnpause();
      Controls.MenuActions.Disable();
      UnityEngine.Cursor.lockState = CursorLockMode.Locked;
      UnityEngine.Cursor.visible = false;
    }
#if UNITY_WEBGL && !UNITY_EDITOR
      // cannot write settings on exit in webgl builds, so write them here
      WriteSettings();
#endif
  }

  void TogglePauseMenu()
  {
    if( MenuShowing )
      HidePauseMenu();
    else
      ShowPauseMenu();
  }

  public void DiegeticMenuOn( DiegeticUI dui )
  {
    ActiveDiegetic = dui;
    Controls.BipedActions.Disable();
    Controls.MenuActions.Enable();
    UIInputModule.enabled = true;
    OverrideCameraZone( dui.CameraZone );
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  public void DiegeticMenuOff()
  {
    ActiveDiegetic = null;
    Controls.MenuActions.Disable();
    Controls.BipedActions.Enable();
    UIInputModule.enabled = false;
    OverrideCameraZone( null );
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  public void ShowMinimap()
  {
    Minimap.SetActive( true );
    Controls.BipedActions.Disable();
    Controls.MenuActions.Enable();
    
    mmPlayer.anchoredPosition = 2 * MinimapCamera.worldToCameraMatrix.MultiplyPoint( PlayerController.pawn.transform.position );
    MinimapScroller.transform.localPosition = -mmPlayer.anchoredPosition * MinimapScroller.transform.localScale.x;
  }

  public void HideMinimap()
  {
    Minimap.SetActive( false );
    Controls.BipedActions.Enable();
    Controls.MenuActions.Disable();
  }

  [SerializeField] Shader grey;
  [SerializeField] Shader grey2;
  [SerializeField] RectTransform mmPlayer;
  
  public void MinimapRender( Vector2 position )
  {
    MinimapCamera.targetTexture.DiscardContents( true, true );
    
    MinimapCamera.transform.position = position;
    Shader cached = bigsheetMaterial.shader;
    Shader cached2 = backgroundMaterial.shader;
    bigsheetMaterial.shader = grey;
    backgroundMaterial.shader = grey2;
    MinimapCamera.Render();
    bigsheetMaterial.shader = cached;
    backgroundMaterial.shader = cached2;
  }

  public void ToggleMinimap()
  {
    if( Minimap.activeInHierarchy )
      HideMinimap();
    else
      ShowMinimap();
  }

  public void EnableRaycaster( bool enable = true )
  {
    UI.GetComponent<UnityEngine.UI.GraphicRaycaster>().enabled = enable;
  }

  void ShowLoadingScreen( string message = "Loading.." )
  {
    LoadingScreen.SetActive( true );
    Text txt = LoadingScreen.GetComponentInChildren<Text>();
    txt.text = message;
  }

  void HideLoadingScreen()
  {
    LoadingScreen.SetActive( false );
  }

  IEnumerator ShowLoadingScreenRoutine( string message = "Loading.." )
  {
    // This is a coroutine simply to wait a single frame after activating the loading screen.
    // Otherwise the screen will not show!
    ShowLoadingScreen( message );
    yield return null;
  }

  public GameObject Spawn( string resourceName, Vector3 position, Quaternion rotation, Transform parent = null, bool limit = true, bool initialize = true )
  {
    // allow the lookup to check the name replacement table
    GameObject prefab = null;
    if( ResourceLookup.ContainsKey( resourceName ) )
    {
      prefab = ResourceLookup[resourceName];
    }
    /*else if( replacements.ContainsKey( resourceName ) )
    {
      if( ResourceLookup.ContainsKey( replacements[resourceName] ) )
        prefab = ResourceLookup[replacements[resourceName]];
    }*/
    if( prefab != null )
      return Spawn( prefab, position, rotation, parent, limit, initialize );
    return null;
  }

  public GameObject Spawn( GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool limit = true, bool initialize = true )
  {
    if( limit )
    {
      ILimit[] limits = prefab.GetComponentsInChildren<ILimit>();
      foreach( var cmp in limits )
      {
        if( !cmp.IsUnderLimit() )
          return null;
      }
    }

    GameObject go = Instantiate( prefab, position, rotation, parent );
    go.name = prefab.name;

    if( initialize )
    {
      SerializedComponent[] scs = go.GetComponentsInChildren<SerializedComponent>();
      foreach( var sc in scs )
        sc.AfterDeserialize();
    }
    return go;
  }

  public void AudioOneShot( AudioClip clip, Vector3 position )
  {
    // independent, temporary positional sound object
    GameObject go = Instantiate( audioOneShotPrefab, position, Quaternion.identity );
    AudioSource source = go.GetComponent<AudioSource>();
    source.PlayOneShot( clip );
    new Timer( clip.length, null, delegate { Destroy( go ); } );
  }

  public void FadeBlack()
  {
    fader.color = new Color( fader.color.r, fader.color.g, fader.color.b, 0 );
    fadeTimer.Stop( true );
    // set gameObject active after Stop() because FadeClear() CompleteDelegate
    // makes the gameObject inactive
    fader.gameObject.SetActive( true );
    TimerParams tp = new TimerParams
    {
      unscaledTime = true,
      repeat = false,
      duration = 1,
      UpdateDelegate = delegate( Timer t )
      {
        Color fc = fader.color;
        fc.a = t.ProgressNormalized;
        fader.color = fc;
      },
      CompleteDelegate = delegate { fader.color = Color.black; }
    };
    fadeTimer.Start( tp );
  }

  public void FadeClear()
  {
    //fader.color = new Color( fader.color.r, fader.color.g, fader.color.b, 1 );
    //fader.gameObject.SetActive( true );
    fadeTimer.Stop( true );
    TimerParams tp = new TimerParams
    {
      unscaledTime = true,
      repeat = false,
      duration = 1,
      UpdateDelegate = delegate( Timer t )
      {
        Color fc = fader.color;
        fc.a = 1 - t.ProgressNormalized;
        fader.color = fc;
      },
      CompleteDelegate = delegate
      {
        fader.color = Color.clear;
        fader.gameObject.SetActive( false );
      }
    };
    fadeTimer.Start( tp );
  }

  public void Speak( Transform headTransform, CharacterIdentity character, string text, float timeout, int priority = 0 )
  {
    // priority 0 = offhand remarks
    // priority 1 = unsolicted chat
    // priority 2 = player-engaged speech
    // priority 3 = mandatory message
    //if( CurrentPlayer == null || character == null )
    //return;
    // equal priority overrides
    if( SpeechTimer.IsActive && priority < SpeechPriority )
      return;
    //float DistanceSqr = Vector3.SqrMagnitude( character.moveTransform.position - playerCharacter.moveTransform.position );
    //if( DistanceSqr > SpeechRange * SpeechRange )
    //return;

    SpeechCharacter = character;
    SpeechPriority = priority;
  
    //SpeechIcon.sprite = SpeechCharacter.Icon;
    
    /*string colorString = "#ffffff",
    Color color = Color.white;
    ColorUtility.TryParseHtmlString( colorString, out color );
    SpeechText.color = color;*/

    SpeechBubble.SetActive( true );
    SpeechText.text = text;
    //SpeechText.rectTransform.localScale = Vector3.one * (1f - 0.5f * Mathf.Clamp( Mathf.Sqrt( DistanceSqr ) / SpeechRange, 0, 1 ));

    SpeechTimer.Stop( false );
    SpeechTimer.Start( timeout, delegate( Timer timer )
    {
      SpeechIconCamera.transform.position = headTransform.GetComponent<SpriteRenderer>().bounds.center;
    }, delegate()
    {
      SpeechBubble.SetActive( false );
      SpeechCharacter = null;
    } );

    SpeechAnimator.runtimeAnimatorController = character.animationController;
    SpeechAnimator.Play( "talk" );
  }
  
  public void OverrideCameraZone( CameraZone zone )
  {
    CameraController.AssignOverrideCameraZone(zone);
  }

  void VerifyPersistentData()
  {
    // Ensure that all persistent data files exist. If not, unpack them from the archive within the build.
    bool unpack = false;
    foreach( var filename in persistentFilenames )
      if( !File.Exists( Application.persistentDataPath + "/" + filename ) )
        unpack = true;
    if( unpack )
    {
      string zipPath = Application.temporaryCachePath + "/persistent.zip";
      TextAsset zipfile = Resources.Load( "persistent" ) as TextAsset;
      if( zipfile != null )
      {
        File.WriteAllBytes( zipPath, zipfile.bytes );
        Debug.Log( "Unzipping persistent: " + zipPath );
        using( ZipFile zip = ZipFile.Read( zipPath ) )
        {
          zip.ExtractAll( Application.persistentDataPath, ExtractExistingFileAction.OverwriteSilently );
        }
      }
      else
      {
        Debug.LogWarning( "no level directory or zip file in build: " + name );
      }
    }
  }

#region Settings

  string[] resolutions = {"640x360", "640x400", "1024x512", "1024x1024", "1280x640", "1280x720", "1280x800" };
  int ResolutionWidth;
  int ResolutionHeight;

  void InitializeSettings()
  {
    SettingUI[] settings = PauseMenu.gameObject.GetComponentsInChildren<SettingUI>( true );
    // use existing UI objects, if they exist
    foreach( var s in settings )
    {
      if( s.isString )
      {
        s.stringValue.Init();
        StringSetting.Add( s.stringValue.name, s.stringValue );
      }
      if( s.isInteger )
      {
        s.intValue.Init();
        FloatSetting.Add( s.intValue.name, s.intValue );
      }
      if( s.isBool )
      {
        s.boolValue.Init();
        BoolSetting.Add( s.boolValue.name, s.boolValue );
      }
    }

    // screen settings are applied explicitly when user pushes button
    CreateBoolSetting( "Fullscreen", false, null );
    CreateStringSetting( "Resolution", "1280x800", null );
    CreateFloatSetting( "ResolutionSlider", 4, 0, resolutions.Length - 1, resolutions.Length - 1, delegate( float value )
    {
      string Resolution = resolutions[Mathf.FloorToInt( Mathf.Clamp( value, 0, resolutions.Length - 1 ) )];
      string[] tokens = Resolution.Split( new char[] {'x'} );
      ResolutionWidth = int.Parse( tokens[0].Trim() );
      ResolutionHeight = int.Parse( tokens[1].Trim() );
      StringSetting["Resolution"].Value = ResolutionWidth.ToString() + "x" + ResolutionHeight.ToString();
    } );
    CreateFloatSetting( "UIScale", 1, 0.1f, 4, 20, null );

    CreateFloatSetting( "MasterVolume", 0.8f, 0, 1, 20, delegate( float value ) { mixer.SetFloat( "MasterVolume", Util.DbFromNormalizedVolume( value ) ); } );
    CreateFloatSetting( "MusicVolume", 0.9f, 0, 1, 20, delegate( float value ) { mixer.SetFloat( "MusicVolume", Util.DbFromNormalizedVolume( value ) ); } );
    CreateFloatSetting( "SFXVolume", 1, 0, 1, 20, delegate( float value )
    {
      mixer.SetFloat( "SFXVolume", Util.DbFromNormalizedVolume( value ) );
      if( Updating )
        AudioOneShot( VolumeChangeNoise, Camera.main.transform.position );
    } );
    /*CreateFloatSetting( "MusicTrack", 0, 0, MusicLoops.Length - 1, 1.0f / (MusicLoops.Length - 1), delegate ( float value ){ PlayMusicLoop( MusicLoops[Mathf.FloorToInt( Mathf.Clamp( value, 0, MusicLoops.Length - 1 ) )] ); } );*/

    CreateBoolSetting( "ShowOnboardingControls", true, OnboardingControls.SetActive );
    CreateBoolSetting( "UseCameraVertical", true, delegate( bool value ) { CameraController.UseVerticalRange = value; } );
    CreateBoolSetting( "CursorInfluence", false, delegate( bool value )
    {
      if( CameraController != null ) 
        CameraController.CursorInfluence = value;
    } );
    CreateBoolSetting( "AimSnap", false, delegate( bool value ) { AimSnap = value; } );
    CreateBoolSetting( "AutoAim", false, delegate( bool value ) { AutoAim = value; } );
    CreateBoolSetting( "ShowAimPath", false, delegate( bool value ) { ShowAimPath = value; } );

    CreateFloatSetting( "CursorOuter", 1, 0, 1, 20, delegate( float value ) { CursorOuter = value; } );
    CreateFloatSetting( "CursorSensitivity", 1, 0.01f, 2, 100, delegate( float value ) { CursorSensitivity = value; } );
    CreateFloatSetting( "CameraLerpAlpha", 10, 0, 10, 100, delegate( float value ) { CameraController.lerpAlpha = value; } );
    CreateFloatSetting( "Zoom", 3, 1, 5, 20, delegate( float value ) { CameraController.orthoTarget = value; } );
    //CreateFloatSetting( "ThumbstickDeadzone", .3f, 0, .5f, 10, delegate ( float value ) { deadZone = value; } );
    CreateFloatSetting( "PlayerSpeedFactor", 0.3f, 0, 1, 10, delegate( float value )
    {
      if( PlayerController != null ) PlayerController.HACKSetSpeed( value );
    } );

    foreach( var scene in sceneRefs )
    {
      if( scene.GetSceneName() == "GLOBAL" )
        continue;
      GameObject go = Instantiate( SceneListElementTemplate, SceneListElementTemplate.transform.parent );
      go.GetComponentInChildren<Text>().text = scene.GetSceneName();
      go.GetComponentInChildren<Button>().onClick.AddListener( () =>
      {
        HidePauseMenu();
        LoadScene( scene.GetSceneName() );
      } );
      if( SceneList.InitiallySelected == null )
        SceneList.InitiallySelected = go;
    }
    Destroy( SceneListElementTemplate );
    
    foreach( var audioLoop in Music )
    {
      GameObject go = Instantiate( MusicListElementTemplate, MusicListElementTemplate.transform.parent );
      go.GetComponentInChildren<Text>().text = audioLoop.name;
      go.GetComponentInChildren<Button>().onClick.AddListener( () =>
      {
        PlayMusic( audioLoop );
        /*if( audioLoop.intro!=null )
          PlayMusic( audioLoop );
        else
          CrossFadeTo( audioLoop );*/
      } );
      if( MusicList.InitiallySelected == null )
        MusicList.InitiallySelected = go;
    }
    Destroy( MusicListElementTemplate );
  }

  // explicit UI navigation
  public void NextNav( Selectable selectable )
  {
    Navigation previousNav = previousNavSelectable.navigation;
    previousNav.selectOnDown = selectable;
    previousNavSelectable.navigation = previousNav;
    Navigation thisNav = selectable.navigation;
    thisNav.selectOnUp = previousNavSelectable;
    selectable.navigation = thisNav;
    previousNavSelectable = selectable;
  }

  void CreateBoolSetting( string key, bool value, System.Action<bool> onChange )
  {
    BoolValue bv;
    if( !BoolSetting.TryGetValue( key, out bv ) )
    {
      GameObject go = Instantiate( ToggleTemplate, SettingsParent.transform );
      SettingUI sss = go.GetComponent<SettingUI>();
      sss.isBool = true;
      bv = sss.boolValue;
      bv.name = key;
      bv.Init();
      BoolSetting.Add( key, bv );
    }
    bv.onValueChanged = onChange;
    bv.Value = value;
    NextNav( bv.toggle );
  }

  void CreateFloatSetting( string key, float value, float min, float max, int steps, System.Action<float> onChange )
  {
    FloatValue bv;
    if( !FloatSetting.TryGetValue( key, out bv ) )
    {
      GameObject go = Instantiate( SliderTemplate, SettingsParent.transform );
      SettingUI sss = go.GetComponent<SettingUI>();
      sss.isInteger = true;
      bv = sss.intValue;
      bv.name = key;
      bv.minValue = min;
      bv.maxValue = max;
      bv.steps = steps;
      bv.Init();
      FloatSetting.Add( key, bv );
    }
    bv.onValueChanged = onChange;
    bv.Value = value;
    NextNav( bv.slider );
  }

  void CreateStringSetting( string key, string value, System.Action<string> onChange )
  {
    StringValue val;
    if( !StringSetting.TryGetValue( key, out val ) )
    {
      GameObject go = Instantiate( StringTemplate, SettingsParent.transform );
      SettingUI sss = go.GetComponent<SettingUI>();
      sss.isString = true;
      val = sss.stringValue;
      val.name = key;
      val.Init();
      StringSetting.Add( key, val );
    }
    val.onValueChanged = onChange;
    val.Value = value;
    if( val.inputField != null )
      NextNav( val.inputField );
  }


  void ReadSettings()
  {
    JsonData json = new JsonData();
    if( File.Exists( settingsPath ) )
    {
      string gameJson = File.ReadAllText( settingsPath );
      if( gameJson.Length > 0 )
      {
        JsonReader reader = new JsonReader( gameJson );
        json = JsonMapper.ToObject( reader );
      }

      foreach( var pair in BoolSetting )
        pair.Value.Value = JsonUtil.Read( pair.Value.Value, json, "settings", pair.Key );

      foreach( var pair in FloatSetting )
        pair.Value.Value = JsonUtil.Read( pair.Value.Value, json, "settings", pair.Key );

      foreach( var pair in StringSetting )
        pair.Value.Value = JsonUtil.Read( pair.Value.Value, json, "settings", pair.Key );
    }
  }

  void WriteSettings()
  {
    JsonWriter writer = new JsonWriter();
    writer.PrettyPrint = true;
    writer.WriteObjectStart(); // root

    writer.WritePropertyName( "settings" );
    writer.WriteObjectStart();
    foreach( var pair in StringSetting )
    {
      writer.WritePropertyName( pair.Key );
      writer.Write( pair.Value.Value );
    }
    foreach( var pair in FloatSetting )
    {
      writer.WritePropertyName( pair.Key );
      writer.Write( pair.Value.Value, "0.##" );
    }
    foreach( var pair in BoolSetting )
    {
      writer.WritePropertyName( pair.Key );
      writer.Write( pair.Value.Value );
    }
    writer.WriteObjectEnd();

    writer.WriteObjectEnd(); // root end
    //print( settingsPath );
    File.WriteAllText( settingsPath, writer.ToString() );

#if UNITY_WEBGL && !UNITY_EDITOR
    FileSync();
#endif
  }

#if UNITY_WEBGL && !UNITY_EDITOR
  [DllImport( "__Internal" )]
  private static extern void FileSync();
#endif

  public void ApplyScreenSettings()
  {
    string[] tokens = StringSetting["Resolution"].Value.Split( new char[] {'x'} );
    ResolutionWidth = int.Parse( tokens[0].Trim() );
    ResolutionHeight = int.Parse( tokens[1].Trim() );
#if UNITY_WEBGL
    // let the web page determine the windowed size
    if( BoolSetting["Fullscreen"].Value )
      Screen.SetResolution( ResolutionWidth, ResolutionHeight, true );
    else
      Screen.fullScreen = false;
#else
    Screen.SetResolution( ResolutionWidth, ResolutionHeight, BoolSetting["Fullscreen"].Value );
#endif
    CanvasScaler.scaleFactor = FloatSetting["UIScale"].Value;
    ScreenSettingsCountdownTimer.Stop( false );
    ConfirmDialog.Unselect();
  }

  struct ScreenSettings
  {
    public bool Fullscreen;
    public string Resolution;
    public float UIScale;
  }

  ScreenSettings CachedScreenSettings = new ScreenSettings();

  public void ScreenChangePrompt()
  {
    CachedScreenSettings.Fullscreen = Screen.fullScreen;
    CachedScreenSettings.Resolution = Screen.width.ToString() + "x" + Screen.height.ToString();
    CachedScreenSettings.UIScale = CanvasScaler.scaleFactor;

    ConfirmDialog.Select();
    Screen.SetResolution( ResolutionWidth, ResolutionHeight, BoolSetting["Fullscreen"].Value );
    CanvasScaler.scaleFactor = FloatSetting["UIScale"].Value;

    TimerParams tp = new TimerParams
    {
      unscaledTime = true,
      repeat = false,
      duration = 10,
      UpdateDelegate = delegate( Timer obj ) { ScreenSettingsCountdown.text = "Accept changes or revert in <color=orange>" + (10 - obj.ProgressSeconds).ToString( "0" ) + "</color> seconds"; },
      CompleteDelegate = RevertScreenSettings
    };
    ScreenSettingsCountdownTimer.Start( tp );
  }

  public void RevertScreenSettings()
  {
    BoolSetting["Fullscreen"].Value = CachedScreenSettings.Fullscreen;
    StringSetting["Resolution"].Value = CachedScreenSettings.Resolution;
    FloatSetting["UIScale"].Value = CachedScreenSettings.UIScale;
    ApplyScreenSettings();
  }

#endregion

  public void StopMusic()
  {
    musicSource0.Stop();
    musicSource1.Stop();
  }

  public void PlayMusic( AudioLoop audioLoop )
  {
    audioLoop.Play( musicSource0, musicSource1 );
    activeMusicSource = musicSource1;
  }

  /*public void MusicTransition( AudioLoop loop )
  {
    // This will fade out entirely before fading into the given intro loop
    Timer t = new Timer();
    t.Start( MusicTransitionDuration, delegate( Timer obj )
    {
      musicSource0.volume = 1 - obj.ProgressNormalized;
      musicSource1.volume = 1 - obj.ProgressNormalized;
    }, delegate
    {
      loop.Play( musicSource0, musicSource1 );
      activeMusicSource = musicSource1;
      t.Start( MusicTransitionDuration, delegate( Timer obj )
      {
        musicSource0.volume = obj.ProgressNormalized;
        musicSource1.volume = obj.ProgressNormalized;
      }, null );
    } );
  }*/

  
  public void CrossFadeTo( AudioLoop loop )
  {
    AudioSource otherSource;
    if( activeMusicSource == musicSource0 )
    {
      activeMusicSource = musicSource1;
      otherSource = musicSource0;
    }
    else
    {
      activeMusicSource = musicSource0;
      otherSource = musicSource1;
    }

    activeMusicSource.clip = loop==null? null : loop.loop;
    activeMusicSource.loop = true;
    activeMusicSource.Play();

    Timer t = new Timer();
    t.unscaledTime = true;
    t.Start( MusicTransitionDuration, delegate( Timer obj )
    {
      activeMusicSource.volume = obj.ProgressNormalized;
      otherSource.volume = 1 - obj.ProgressNormalized;
    },null );
  }
  
  /*
 public void CrossFadeToClip( AudioClip clip )
 {
   AudioSource next = (activeMusicSource == musicSource0) ? musicSource1 : musicSource0;
   next.clip = clip;
   Timer t = new Timer( MusicTransitionDuration, delegate ( Timer obj )
   {
     activeMusicSource.volume = obj.ProgressNormalized;
     next.volume = 1 - obj.ProgressNormalized;
   }, null );
 }
 */
}