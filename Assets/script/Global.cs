#pragma warning disable 414
//#define DESTRUCTION_LIST

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Playables;
using LitJson;
using Ionic.Zip;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( Global ) )]
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



public class WorldSelectable : MonoBehaviour
{
  public virtual void Highlight() { }
  public virtual void Unhighlight() { }
  public virtual void Select() { }
  public virtual void Unselect() { }
}


public class Global : MonoBehaviour
{
  public static Global instance;
  public static bool Paused = false;
  public static bool Slowed = false;
  public static bool IsQuiting = false;

  [Header( "Global Settings" )]
  public static float Gravity = 16;
  public const float MaxVelocity = 60;
  [SerializeField] string InitialSceneName;
  public float deadZone = 0.1f;
  // screenshot timer
  public int screenshotInterval;
  public Timer ScreenshotTimer = new Timer();


  [Tooltip( "Pretend this is a build we're running" )]
  public bool SimulatePlayer = false;
  [SerializeField] float slowtime = 0.2f;
  Timer fadeTimer = new Timer();
  public float RepathInterval = 1;
  // sidestep
  public bool GlobalSidestepping = true;
  public float SidestepDistance = 1f;
  public float SidestepRaycastDistance = 1f;
  public float SidestepInterval = 1f;
  public float SidestepIgnoreWithinDistanceToGoal = 0.5f;

  public static string[] persistentFilenames = new string[] {
    /*"settings.json",
    "characters.json",
    "events.json"*/
  };

  // note: allowing characters to collide introduces potential for "pinch points"
  public static string[] CharacterCollideLayers = { "Default", "destructible", "triggerAndCollision" }; //, "character", "enemy" };
  public static string[] CharacterSidestepLayers = { "character", "enemy" };
  public static string[] CharacterDamageLayers = { "character" };
  public static string[] TriggerLayers = { "trigger", "triggerAndCollision" };
  public static string[] ProjectileNoShootLayers = { "Default" };
  public static string[] DefaultProjectileCollideLayers = { "Default", "character", "triggerAndCollision", "enemy", "destructible", "bouncyGrenade", "flameProjectile" };
  public static string[] FlameProjectileCollideLayers = { "Default", "character", "triggerAndCollision", "enemy", "destructible", "bouncyGrenade" };
  // check first before spawning to avoid colliding with these layers on the first frame
  public static string[] BouncyGrenadeCollideLayers = { "character", "triggerAndCollision", "enemy", "projectile", "destructible", "flameProjectile" };
  public static string[] TurretSightLayers = { "Default", "character", "triggerAndCollision", "destructible" };

  [Header( "Settings" )]
  public GameObject ToggleTemplate;
  public GameObject SliderTemplate;
  string settingsPath { get { return Application.persistentDataPath + "/" + "settings.json"; } }
  public Dictionary<string, FloatValue> FloatSetting = new Dictionary<string, FloatValue>();
  public Dictionary<string, BoolValue> BoolSetting = new Dictionary<string, BoolValue>();
  public Dictionary<string, StringValue> StringSetting = new Dictionary<string, StringValue>();
  // screen settings
  public GameObject ScreenSettingsPrompt;
  public Text ScreenSettingsCountdown;
  Timer ScreenSettingsCountdownTimer = new Timer();

  [Header( "References" )]
  public CameraController CameraController;
  [SerializeField] Animator animator;
  [SerializeField] AudioClip VolumeChangeNoise;

  [Header( "Prefabs" )]
  public GameObject audioOneShotPrefab;
  public GameObject AvatarPrefab;

  [Header( "Transient (Assigned at runtime)" )]
  public bool Updating = false;
  public Collider2D CameraPoly;
  public Bounds CameraBounds;
  SceneScript sceneScript;
  public PlayerController CurrentPlayer;
  public Dictionary<string, int> AgentType = new Dictionary<string, int>();

  [Header( "UI" )]
  public GameObject UI;
  CanvasScaler CanvasScaler;
  public UIScreen PauseMenu;
  public GameObject PauseMenuFirstSelected;
  [SerializeField] GameObject HUD;
  DiageticUI ActiveDiagetic;
  bool MenuShowing { get { return PauseMenu.gameObject.activeInHierarchy; } }
  public GameObject LoadingScreen;
  [SerializeField] Image fader;
  public GameObject ready;
  [SerializeField] GameObject OnboardingControls;
  // cursor 
  [SerializeField] Transform Cursor;
  Vector2 cursorDelta;
  Vector2 cursorOrigin;
  public float CursorOuter = 1;
  public Vector2 AimPosition;
  public Vector2 CursorWorldPosition;
  public float CursorSensitivity = 1;
  public float CursorScale = 2;
  public bool AimSnap;
  public float SnapAngleDivide = 8;
  public float SnapCursorDistance = 1;
  public Transform CursorSnapped;
  public bool AutoAim;
  public Transform CursorAutoAim;
  public float AutoAimCircleRadius = 1;
  public float AutoAimDistance = 5;
  // status 
  public Image weaponIcon;
  // settings
  public GameObject SettingsParent;
  public UIScreen ConfirmDialog;

  [Header( "Input" )]
  public Controls Controls;
  public bool UsingGamepad;
  public Color ControlNameColor = Color.red;

  [Header( "Debug" )]
  [SerializeField] Text debugFPS;
  [SerializeField] Text debugText;
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

  public GameData gameData;
  public Dictionary<string, GameObject> ResourceLookup = new Dictionary<string, GameObject>();

#if DESTRUCTION_LIST
  // This exists only because of a Unity crash bug when objects with active
  // Contacts are destroyed from within OnCollisionEnter2D()
  List<GameObject> DestructionList = new List<GameObject>();
#endif

  [Header( "Audio" )]
  public UnityEngine.Audio.AudioMixer mixer;
  public UnityEngine.Audio.AudioMixerSnapshot snapSlowmo;
  public UnityEngine.Audio.AudioMixerSnapshot snapNormal;
  public UnityEngine.Audio.AudioMixerSnapshot snapMusicSilence;
  public float MusicTransitionDuration = 0.5f;
  public float AudioFadeDuration = 0.1f;
  [SerializeField] AudioSource musicSource0;
  [SerializeField] AudioSource musicSource1;
  AudioSource activeMusicSource;
  [SerializeField] AudioLoop[] MusicLoops;


  [Header( "Speech" )]
  public GameObject SpeechBubble;
  public Text SpeechText;
  public Image SpeechIcon;
  public Animator SpeechAnimator;
  CharacterIdentity SpeechCharacter;
  int SpeechPriority = 0;
  public float SpeechRange = 8;
  Timer SpeechTimer = new Timer();


  [RuntimeInitializeOnLoadMethod]
  static void RunOnStart()
  {
    Application.wantsToQuit += WantsToQuit;
  }

  static bool WantsToQuit()
  {
    IsQuiting = true;
    // do pre-quit stuff here
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
    CanvasScaler = UI.GetComponent<CanvasScaler>();
    InitializeSettings();
    ReadSettings();
    ApplyScreenSettings();
    InitializeControls();

    SceneManager.sceneLoaded += delegate ( Scene arg0, LoadSceneMode arg1 )
    {
      //Debug.Log( "scene loaded: " + arg0.name );
    };
    SceneManager.activeSceneChanged += delegate ( Scene arg0, Scene arg1 )
    {
      //Debug.Log( "active scene changed from " + arg0.name + " to " + arg1.name );
    };
    InputSystem.onDeviceChange +=
    ( device, change ) => {
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

    GameObject[] res = Resources.LoadAll<GameObject>( "" );
    foreach( GameObject go in res )
      ResourceLookup.Add( go.name, go );

    NavMeshSurface[] meshSurfaces = FindObjectsOfType<NavMeshSurface>();
    foreach( var mesh in meshSurfaces )
      AgentType[NavMesh.GetSettingsNameFromID( mesh.agentTypeID )] = mesh.agentTypeID;

    /*fpsTimer = new Timer( int.MaxValue, 1, delegate ( Timer tmr )
    {
      debugFPS.text = frames.ToString();
      frames = 0;
    }, null );*/

    if( Camera.main.orthographic )
      debugText.text = Camera.main.orthographicSize.ToString( "##.#" );
    else
      debugText.text = CameraController.zOffset.ToString( "##.#" );

    if( Camera.main.orthographic )
      Camera.main.orthographicSize = 2;
    else
      Camera.main.fieldOfView = 20;

    HideHUD();
    HidePauseMenu();
    HideLoadingScreen();
    SpeechBubble.SetActive( false );

    if( Application.isEditor && !SimulatePlayer )
    {
      LoadingScreen.SetActive( false );
      fader.color = Color.clear;
      Updating = true;
      sceneScript = FindObjectOfType<SceneScript>();
      if( sceneScript != null )
        sceneScript.StartScene();
      foreach( var mesh in meshSurfaces )
        mesh.BuildNavMesh();
    }
    else
    {
      //StartCoroutine( LoadSceneRoutine( InitialSceneName, false, false, true, false ) );
      //StartCoroutine( LoadSceneRoutine( "intro", false, false, true, false ) );
      StartCoroutine( LoadSceneRoutine( "home", true, true, true, false ) );
    }

  }

  void Start()
  {
#if UNITY_EDITOR
    // workaround for Unity Editor bug where AudioMixer.SetFloat() does not work in Awake()
    mixer.SetFloat( "MasterVolume", Util.DbFromNormalizedVolume( FloatSetting["MasterVolume"].Value ) );
    mixer.SetFloat( "MusicVolume", Util.DbFromNormalizedVolume( FloatSetting["MusicVolume"].Value ) );
    mixer.SetFloat( "SFXVolume", Util.DbFromNormalizedVolume( FloatSetting["SFXVolume"].Value ) );
#endif
  }

  public string ReplaceWithControlNames( string source )
  {
    // todo support composites
    string outstr = "";
    string[] tokens = source.Split( new char[] { '[' } );
    foreach( var tok in tokens )
    {
      if( tok.Contains( "]" ) )
      {
        string[] ugh = tok.Split( new char[] { ']' } );
        if( ugh.Length > 2 )
          return "BAD FORMAT";
        int inputTypeIndex = UsingGamepad ? 1 : 0;
        // warning!! controls must be in correct order per-action: keyboard+mouse first, then gamepad
        InputAction ia = Controls.BipedActions.Get().FindAction( ugh[0] );
        if( ia == null ) ia = Controls.MenuActions.Get().FindAction( ugh[0] );
        if( ia == null ) ia = Controls.GlobalActions.Get().FindAction( ugh[0] );
        if( ia == null ) return "ACTION NOT FOUND: " + ugh[0];
        if( ia.controls.Count <= inputTypeIndex )
          return "BAD CONTROL INDEX";
        InputControl ic = ia.controls[inputTypeIndex];
        string controlName = "BAD NAME";
        if( ic.shortDisplayName != null )
          controlName = ic.shortDisplayName;
        else
          controlName = ic.name.ToUpper();
        outstr += "<color=#" + ColorUtility.ToHtmlStringRGB( ControlNameColor ) + ">" + controlName + "</color>";
        outstr += ugh[1];
      }
      else
        outstr += tok;
    }
    return outstr;
  }

  void InitializeControls()
  {
    Controls = new Controls();
    Controls.Enable();
    Controls.MenuActions.Disable();

    Controls.GlobalActions.Any.performed += ( obj ) => {
      bool newvalue = obj.control.path.Contains( "Gamepad" );
      if( newvalue != UsingGamepad )
        print( "UsingGamepad new value: " + newvalue.ToString() + " " + obj.control.path );
      UsingGamepad = newvalue;
    };

    Controls.GlobalActions.Menu.performed += ( obj ) => TogglePauseMenu();
    Controls.GlobalActions.Pause.performed += ( obj ) => {
      if( Paused )
        Unpause();
      else
        Pause();
    };
    Controls.GlobalActions.DevSlowmo.performed += ( obj ) => {
      if( Slowed )
        NoSlow();
      else
        Slow();
    };
    Controls.GlobalActions.Screenshot.performed += ( obj ) => Util.Screenshot();
#if !UNITY_EDITOR
    Controls.GlobalActions.CursorLockToggle.performed += (obj) => {
      if( UnityEngine.Cursor.lockState == CursorLockMode.Locked )
        UnityEngine.Cursor.lockState = CursorLockMode.None;
      else
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    };
#endif
    Controls.GlobalActions.DEVRespawn.performed += ( obj ) => {
      Chopper chopper = FindObjectOfType<Chopper>();
      if( (!Application.isEditor || SimulatePlayer) && chopper != null )
        chopper.StartDrop( CurrentPlayer );
      else
      {
        CurrentPlayer.transform.position = FindSpawnPosition();
        CurrentPlayer.velocity = Vector2.zero;
      }
    };

    Controls.MenuActions.Back.performed += ( obj ) => {
      // todo back out to previous UI screen...

      // ...or if there is none, close diagetic UI
      if( ActiveDiagetic != null && !MenuShowing && CurrentPlayer != null )
        CurrentPlayer.UnselectWorldSelection();
    };
    // DEVELOPMENT
    Controls.BipedActions.DEVZoom.started += ( obj ) => {
      zoomDelta += obj.ReadValue<float>();
    };
  }

  public void LoadScene( string sceneName, bool waitForFadeIn = true, bool spawnPlayer = true, bool fadeOut = true, bool showLoadingScreen = true )
  {
    StartCoroutine( LoadSceneRoutine( sceneName, waitForFadeIn, spawnPlayer, fadeOut, showLoadingScreen ) );
  }

  IEnumerator LoadSceneRoutine( string sceneName, bool waitForFadeIn = true, bool spawnPlayer = true, bool fadeOut = true, bool showLoadingScreen = true )
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
      UpdateDelegate = delegate ( Timer obj )
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
      yield return ShowLoadingScreenRoutine( "Loading... " + sceneName );
    if( CurrentPlayer != null )
    {
      CurrentPlayer.PreSceneTransition();
      SceneManager.MoveGameObjectToScene( CurrentPlayer.gameObject, gameObject.scene );
    }
    loadingScene = true;
    //progress.fillAmount = 0;
    //prog = 0;
    yield return new WaitForSecondsRealtime( 1 );
    AsyncOperation ao = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Single );
    while( !ao.isDone )
    {
      //prog = Mathf.MoveTowards( prog, ao.progress, Time.unscaledDeltaTime * progressSpeed );
      //progress.fillAmount = prog;
      //progTarget = ao.progress;
      //print( ao.progress.ToString() );
      yield return null;
    }
    loadingScene = false;

    NavMeshSurface[] meshSurfaces = FindObjectsOfType<NavMeshSurface>();
    foreach( var mesh in meshSurfaces )
      mesh.BuildNavMesh();

    if( CurrentPlayer == null )
    {
      if( spawnPlayer )
        SpawnPlayer();
    }
    else
    {
      CurrentPlayer.PostSceneTransition();
    }

    TimerParams musicTimerParamsFadeIn = new TimerParams
    {
      unscaledTime = true,
      repeat = false,
      duration = MusicTransitionDuration,
      UpdateDelegate = delegate ( Timer obj )
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

    HUD.SetActive( true );

    sceneScript = FindObjectOfType<SceneScript>();
    if( sceneScript != null )
      sceneScript.StartScene();

    Unpause();
    if( showLoadingScreen )
      HideLoadingScreen();
    FadeClear();
    if( waitForFadeIn )
      while( fadeTimer.IsActive )
        yield return null;
    Updating = true;
  }

  float lastTime;
  void Update()
  {
#if DESTRUCTION_LIST
    for( int i = 0; i < DestructionList.Count; i++ )
      Destroy( DestructionList[i] );
    DestructionList.Clear();
#endif

    frames++;
    if( Time.unscaledTime - lastTime >= 1 )
    {
      lastTime = Time.unscaledTime;
      debugFPS.text = frames.ToString();
      frames = 0;
    }

    Timer.UpdateTimers();

    if( !Updating )
      return;

    if( loadingScene )
    {
      //prog = Mathf.MoveTowards( prog, progTarget, Time.unscaledDeltaTime * progressSpeed );
      //progress.fillAmount = prog;
    }

#if UNITY_EDITOR_LINUX
    if( Input.GetKeyDown(KeyCode.Escape) )
    Cursor.lockState = CursorLockMode.None;
#else
#endif

    float H = 0;
    float S = 1;
    float V = 1;
    Color.RGBToHSV( shiftyColor, out H, out S, out V );
    H += colorShiftSpeed * Time.unscaledDeltaTime;
    shiftyColor = Color.HSVToRGB( H, 1, 1 );
    shifty.color = shiftyColor;

    if( Camera.main.orthographic )
    {
      if( Mathf.Abs( zoomDelta ) > 0 )
      {
        CameraController.orthoTarget += zoomDelta * Time.deltaTime;
        CameraController.orthoTarget = Mathf.Clamp( CameraController.orthoTarget, 1, 10 );
        FloatSetting["Zoom"].Value = CameraController.orthoTarget;
      }
      debugText.text = Camera.main.orthographicSize.ToString( "##.#" );
    }
    else
    {
      CameraController.zOffset += zoomDelta * Time.deltaTime;
      debugText.text = CameraController.zOffset.ToString( "##.#" );
    }
    zoomDelta = 0;

  }

  void OnApplicationFocus( bool hasFocus )
  {
    UnityEngine.Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
    UnityEngine.Cursor.visible = !hasFocus;
  }

  void LateUpdate()
  {
    if( !Updating )
      return;

    if( CurrentPlayer != null )
    {
      if( ActiveDiagetic != null )
      {
        /*
        CursorWorldPosition = Camera.main.ScreenToWorldPoint( UnityEngine.Input.mousePosition );
        CursorDelta = (CursorWorldPosition - (Vector2)CurrentPlayer.arm.position) / CursorFactor;
        AimPosition = CursorWorldPosition;
        Cursor.position = CursorWorldPosition;
        */
      }
      else
      {
#if UNITY_WEBGL && !UNITY_EDITOR
        Vector2 delta = Controls.BipedActions.Aim.ReadValue<Vector2>() * CursorSensitivity;
        delta.y = -delta.y;
        cursorDelta += delta;
#else
        /*currentDelta = UnityEngine.InputSystem.Mouse.current.delta.ReadValue();
        if( currentDelta.magnitude > 0 )
          Debug.LogFormat( "current {0} {1}", currentDelta.x, currentDelta.y );
        Vector2 aim = Controls.BipedActions.Aim.ReadValue<Vector2>();
        if( aim.magnitude > 0 )
          Debug.LogFormat( "aim {0} {1}", aim.x, aim.y );*/

        cursorDelta += Controls.BipedActions.Aim.ReadValue<Vector2>() * CursorSensitivity;
#endif
        cursorDelta = cursorDelta.normalized * Mathf.Max( Mathf.Min( cursorDelta.magnitude, Camera.main.orthographicSize * Camera.main.aspect * CursorOuter ), 0.1f );
        cursorOrigin = CurrentPlayer.arm.position;

        if( AimSnap )
        {
          // set cursor
          Cursor.gameObject.SetActive( true );
          CursorSnapped.gameObject.SetActive( true );

          float angle = Mathf.Atan2( cursorDelta.x, cursorDelta.y ) / Mathf.PI;
          float snap = Mathf.Round( angle * SnapAngleDivide ) / SnapAngleDivide;
          Vector2 snapped = new Vector2( Mathf.Sin( snap * Mathf.PI ), Mathf.Cos( snap * Mathf.PI ) );
          AimPosition = cursorOrigin + snapped * SnapCursorDistance;
          CursorSnapped.position = AimPosition;
          CursorWorldPosition = cursorOrigin + cursorDelta;
          Cursor.position = CursorWorldPosition;
        }
        else
        {
          // set cursor
          Cursor.gameObject.SetActive( true );
          CursorSnapped.gameObject.SetActive( false );

          AimPosition = cursorOrigin + cursorDelta;
          CursorWorldPosition = AimPosition;
          //Cursor.anchoredPosition = Camera.main.WorldToScreenPoint( CursorWorldPosition );
          Cursor.position = CursorWorldPosition;
        }

        if( AutoAim )
        {
          CursorWorldPosition = cursorOrigin + cursorDelta;
          RaycastHit2D[] hits = Physics2D.CircleCastAll( CurrentPlayer.transform.position, AutoAimCircleRadius, cursorDelta, AutoAimDistance, LayerMask.GetMask( new string[] { "enemy" } ) );
          float distance = Mathf.Infinity;
          Transform closest = null;
          foreach( var hit in hits )
          {
            float dist = Vector2.Distance( CursorWorldPosition, hit.transform.position );
            if( dist < distance )
            {
              closest = hit.transform;
              distance = dist;
            }
          }

          if( closest == null )
          {
            CursorAutoAim.gameObject.SetActive( false );
          }
          else
          {
            CursorAutoAim.gameObject.SetActive( true );
            // todo adjust for flight path 
            //Rigidbody2D body = CurrentPlayer.weapon.ProjectilePrefab.GetComponent<Rigidbody2D>();
            AimPosition = closest.position;
            CursorAutoAim.position = AimPosition;
          }
          Cursor.position = CursorWorldPosition;

        }
        else
        {
          CursorAutoAim.gameObject.SetActive( false );
        }
      }
    }
    else
    {
      Cursor.gameObject.SetActive( false );
    }

    CameraController.CameraLateUpdate();
  }

  public void SpawnPlayer()
  {
    GameObject go = Spawn( AvatarPrefab, FindSpawnPosition(), Quaternion.identity, null, false );
    CurrentPlayer = go.GetComponent<PlayerController>();
    CameraController.LookTarget = CurrentPlayer.gameObject;
    CameraController.transform.position = CurrentPlayer.transform.position;

    // settings are read before player is created, so set player settings here.
    CurrentPlayer.SpeedFactorNormalized = FloatSetting["PlayerSpeedFactor"].Value;
  }

  GameObject FindSpawnPoint()
  {
    GameObject[] spawns = GameObject.FindGameObjectsWithTag( "Respawn" );
    if( spawns.Length > 0 )
      return spawns[Random.Range( 0, spawns.Length )];
    return null;
  }

  public Vector3 FindSpawnPosition()
  {
    GameObject go = FindSpawnPoint();
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
    Time.fixedDeltaTime = 0.01f * Time.timeScale;
    mixer.TransitionToSnapshots( new UnityEngine.Audio.AudioMixerSnapshot[] {
      snapNormal,
      snapSlowmo
    }, new float[] {
      0,
      1
    }, AudioFadeDuration );
  }

  public void NoSlow()
  {
    Slowed = false;
    Time.timeScale = 1;
    Time.fixedDeltaTime = 0.01f * Time.timeScale;
    mixer.TransitionToSnapshots( new UnityEngine.Audio.AudioMixerSnapshot[] {
      snapNormal,
      snapSlowmo
    }, new float[] {
      1,
      0
    }, AudioFadeDuration );
  }

  public static void Pause()
  {
    Paused = true;
    Time.timeScale = 0;
    Time.fixedDeltaTime = 0;
    instance.mixer.SetFloat( "MusicVolume", Util.DbFromNormalizedVolume( instance.FloatSetting["MusicVolume"].Value * 0.8f ) );
    instance.mixer.SetFloat( "SFXVolume", Util.DbFromNormalizedVolume( instance.FloatSetting["SFXVolume"].Value * 0.8f ) );
  }

  public static void Unpause()
  {
    Paused = false;
    Time.timeScale = 1;
    Time.fixedDeltaTime = 0.01f * Time.timeScale;
    instance.mixer.SetFloat( "MusicVolume", Util.DbFromNormalizedVolume( instance.FloatSetting["MusicVolume"].Value ) );
    instance.mixer.SetFloat( "SFXVolume", Util.DbFromNormalizedVolume( instance.FloatSetting["SFXVolume"].Value ) );
  }

  public void SetCursor( Sprite spr )
  {
    Cursor.GetComponent<SpriteRenderer>().sprite = spr;
    Cursor.localScale = Vector3.one * CursorScale;
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
    if( ActiveDiagetic != null )
    {
      ActiveDiagetic.InteractableOff();
    }
    else
    {
      Controls.MenuActions.Enable();
      Controls.BipedActions.Disable();
    }
    PauseMenu.Select();
    HUD.SetActive( false );
    //UnityEngine.Cursor.lockState = CursorLockMode.None;
    UnityEngine.Cursor.visible = true;
    EnableRaycaster( true );
  }

  void HidePauseMenu()
  {
    if( ScreenSettingsCountdownTimer.IsActive )
      return;
    Unpause();
    PauseMenu.Unselect();
    HUD.SetActive( true );
    //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    UnityEngine.Cursor.visible = false;
    EnableRaycaster( false );
    if( ActiveDiagetic != null )
    {
      ActiveDiagetic.InteractableOn();
    }
    else
    {
      Controls.BipedActions.Enable();
      Controls.MenuActions.Disable();
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

  public void DiageticMenuOn( DiageticUI dui )
  {
    ActiveDiagetic = dui;
    Controls.MenuActions.Enable();
    Controls.BipedActions.Disable();
    AssignCameraPoly( dui.CameraPoly );
    CameraController.EncompassBounds = true;
    //UnityEngine.Cursor.lockState = CursorLockMode.None;
    Cursor.gameObject.SetActive( false );
  }

  public void DiageticMenuOff()
  {
    ActiveDiagetic = null;
    Controls.MenuActions.Disable();
    Controls.BipedActions.Enable();
    SceneScript ss = FindObjectOfType<SceneScript>();
    if( ss != null )
      AssignCameraPoly( ss.sb );
    CameraController.EncompassBounds = false;
    //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    Cursor.gameObject.SetActive( true );
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

#if DESTRUCTION_LIST
  // This exists only because of a Unity crash bug when objects with active
  // Contacts are destroyed from within OnCollisionEnter2D()
  public void Destroy( GameObject go )
  {
    go.SetActive( false );
    DestructionList.Add( go );
  }
#endif

  public GameObject Spawn( string resourceName, Vector3 position, Quaternion rotation, Transform parent = null, bool limit = true, bool initialize = true )
  {
    // allow the lookup to check the name replacement table
    GameObject prefab = null;
    if( ResourceLookup.ContainsKey( resourceName ) )
    {
      prefab = ResourceLookup[resourceName];
    }
    else
    if( gameData.replacements.ContainsKey( resourceName ) )
    {
      if( ResourceLookup.ContainsKey( gameData.replacements[resourceName] ) )
        prefab = ResourceLookup[gameData.replacements[resourceName]];
    }
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
    new Timer( clip.length, null, delegate
    {
      Destroy( go );
    } );
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
      UpdateDelegate = delegate ( Timer t )
      {
        Color fc = fader.color;
        fc.a = t.ProgressNormalized;
        fader.color = fc;
      },
      CompleteDelegate = delegate
      {
        fader.color = Color.black;
      }
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
      UpdateDelegate = delegate ( Timer t )
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

  public void Speak( CharacterIdentity character, string text, float timeout, int priority = 0 )
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

    SpeechIcon.sprite = SpeechCharacter.Icon;
    /*string colorString = "#ffffff", 
    Color color = Color.white;
    ColorUtility.TryParseHtmlString( colorString, out color );
    SpeechText.color = color;*/

    SpeechBubble.SetActive( true );
    SpeechText.text = text;
    //SpeechText.rectTransform.localScale = Vector3.one * (1f - 0.5f * Mathf.Clamp( Mathf.Sqrt( DistanceSqr ) / SpeechRange, 0, 1 ));

    SpeechTimer.Stop( false );
    SpeechTimer.Start( timeout, null, delegate ()
    {
      SpeechBubble.SetActive( false );
      SpeechCharacter = null;
    } );

    SpeechAnimator.runtimeAnimatorController = character.animationController;
    SpeechAnimator.Play( "talk" );
  }

  public void AssignCameraPoly( Collider2D collider )
  {
    CameraPoly = collider;
    if( collider is PolygonCollider2D )
    {
      PolygonCollider2D poly = collider as PolygonCollider2D;
      // camera poly bounds points are local to polygon
      CameraBounds = new Bounds();
      foreach( var p in poly.points )
        CameraBounds.Encapsulate( p );
    }
    else if( collider is BoxCollider2D )
    {
      BoxCollider2D box = collider as BoxCollider2D;
      CameraBounds = box.bounds;
    }
  }

  void VerifyPersistentData()
  {
    // ensure that all persistent data files exist. IF not, unpack from zip in build.
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

  string[] resolutions = { "640x360", "640x400", "1024x512", "1280x720", "1280x800", "1920x1080" };
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
      }S:
      if( s.isBool )
      {
        s.boolValue.Init();
        BoolSetting.Add( s.boolValue.name, s.boolValue );
      }
    }
    // "Apply Screen Settings" is first button 
    previousSelectable = PauseMenuFirstSelected.GetComponent<Selectable>();
    // screen settings are applied explicitly when user pushes button
    CreateBoolSetting( "Fullscreen", false, null );
    CreateFloatSetting( "ResolutionSlider", 4, 0, resolutions.Length - 1, 1.0f / (resolutions.Length - 1), delegate ( float value )
    {
      string Resolution = resolutions[Mathf.FloorToInt( Mathf.Clamp( value, 0, resolutions.Length - 1 ) )];
      string[] tokens = Resolution.Split( new char[] { 'x' } );
      ResolutionWidth = int.Parse( tokens[0].Trim() );
      ResolutionHeight = int.Parse( tokens[1].Trim() );
      StringSetting["Resolution"].Value = ResolutionWidth.ToString() + "x" + ResolutionHeight.ToString();
    } );
    CreateStringSetting( "Resolution", "1280x800", null );
    CreateFloatSetting( "UIScale", 1, 0.1f, 4, 0.05f, null );

    CreateFloatSetting( "MasterVolume", 0.8f, 0, 1, 0.05f, delegate ( float value ){ mixer.SetFloat( "MasterVolume", Util.DbFromNormalizedVolume( value ) ); } );
    CreateFloatSetting( "MusicVolume", 0.9f, 0, 1, 0.05f, delegate ( float value ){ mixer.SetFloat( "MusicVolume", Util.DbFromNormalizedVolume( value ) ); } );
    CreateFloatSetting( "SFXVolume", 1, 0, 1, 0.05f, delegate ( float value )
    {
      mixer.SetFloat( "SFXVolume", Util.DbFromNormalizedVolume( value ) );
      AudioOneShot( VolumeChangeNoise, Camera.main.transform.position );
    } );
    /*CreateFloatSetting( "MusicTrack", 0, 0, MusicLoops.Length - 1, 1.0f / (MusicLoops.Length - 1), delegate ( float value ){ PlayMusicLoop( MusicLoops[Mathf.FloorToInt( Mathf.Clamp( value, 0, MusicLoops.Length - 1 ) )] ); } );*/

    CreateBoolSetting( "ShowOnboardingControls", true, OnboardingControls.SetActive );
    CreateBoolSetting( "UseCameraVertical", true, delegate ( bool value ) { CameraController.UseVerticalRange = value; } );
    CreateBoolSetting( "CursorInfluence", false, delegate ( bool value ) { CameraController.CursorInfluence = value; } );
    CreateBoolSetting( "AimSnap", false, delegate ( bool value ) { AimSnap = value; } );
    CreateBoolSetting( "AutoAim", false, delegate ( bool value ) { AutoAim = value; } );


    CreateFloatSetting( "CursorOuter", 1, 0, 1, 0.05f, delegate ( float value ) { CursorOuter = value; } );
    CreateFloatSetting( "CursorSensitivity", 0.1f, 0, 1, 0.05f, delegate ( float value ) { CursorSensitivity = value; } );
    CreateFloatSetting( "CameraLerpAlpha", 20, 0, 50, 0.01f, delegate ( float value ) { CameraController.lerpAlpha = value; } );
    CreateFloatSetting( "Zoom", 3, 1, 5, 0.05f, delegate ( float value ) { CameraController.orthoTarget = value; } );
    //CreateFloatSetting( "ThumbstickDeadzone", .3f, 0, .5f, 0.1f, delegate ( float value ) { deadZone = value; } );
    CreateFloatSetting( "PlayerSpeedFactor", 0, 0, 1, 0.1f, delegate ( float value ) { if( CurrentPlayer != null ) CurrentPlayer.SpeedFactorNormalized = value; } );
  }

  Selectable previousSelectable;
  // explicit UI navigation
  void NextNav( Selectable selectable )
  {
    Navigation previousNav = previousSelectable.navigation;
    previousNav.selectOnDown = selectable;
    previousSelectable.navigation = previousNav;
    Navigation thisNav = selectable.navigation;
    thisNav.selectOnUp = previousSelectable;
    selectable.navigation = thisNav;
    previousSelectable = selectable;
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

  void CreateFloatSetting( string key, float value, float min, float max, float normalizedStep, System.Action<float> onChange )
  {
    FloatValue bv;
    if( !FloatSetting.TryGetValue( key, out bv ) )
    {
      GameObject go = Instantiate( SliderTemplate, SettingsParent.transform );
      SettingUI sss = go.GetComponent<SettingUI>();
      sss.isInteger = true;
      bv = sss.intValue;
      bv.name = key;
      bv.Init();
      FloatSetting.Add( key, bv );
    }
    bv.slider.normalizedStep = normalizedStep;
    bv.slider.minValue = min;
    bv.slider.maxValue = max;
    bv.onValueChanged = onChange;
    bv.Value = value;
    NextNav( bv.slider );
  }

  void CreateStringSetting( string key, string value, System.Action<string> onChange )
  {
    StringValue val;
    if( !StringSetting.TryGetValue( key, out val ) )
    {
      GameObject go = Instantiate( SliderTemplate, SettingsParent.transform );
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
    string[] tokens = StringSetting["Resolution"].Value.Split( new char[] { 'x' } );
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
      UpdateDelegate = delegate ( Timer obj )
      {
        ScreenSettingsCountdown.text = "Accept changes or revert in <color=orange>" + (10 - obj.ProgressSeconds).ToString( "0" ) + "</color> seconds";
      },
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

  static GameObject FindFirstEnabledSelectable( GameObject gameObject )
  {
    GameObject go = null;
    var selectables = gameObject.GetComponentsInChildren<Selectable>( true );
    foreach( var selectable in selectables )
    {
      if( selectable.IsActive() && selectable.IsInteractable() )
      {
        go = selectable.gameObject;
        break;
      }
    }
    return go;
  }

  public void StopMusic()
  {
    musicSource0.Stop();
    musicSource1.Stop();
  }

  public void PlayMusic( AudioLoop audioLoop )
  {
    audioLoop.Play( musicSource0, musicSource1 );
  }
  /*
 public void MusicTransition( AudioLoop loop )
 {
   Timer t = new Timer();
   t.Start( MusicTransitionDuration, 
   delegate ( Timer obj )
   {
     musicSource0.volume = 1 - obj.ProgressNormalized;
     musicSource1.volume = 1 - obj.ProgressNormalized;
   },
   delegate
   {
     loop.Play( musicSource0, musicSource1 );
     t.Start( MusicTransitionDuration, 
     delegate ( Timer obj )
     {
       musicSource0.volume = obj.ProgressNormalized;
       musicSource1.volume = obj.ProgressNormalized;
     }, null );
   }
   );
 }

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