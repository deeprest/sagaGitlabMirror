#pragma warning disable 414


using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;
using LitJson;
using Ionic.Zip;

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
      obj.Screenshot();
      StartTimer();
    } );
  }
}
#endif

public interface ITrigger
{
  void Trigger( Transform instigator );
}

public interface IDamage
{
  bool TakeDamage( Damage d );
}

public interface ISelect
{
  void Highlight();
  void Unhighlight();
  void Selected();
}


// TODO upgrade to 2019 and use new input system, or find something on asset store.

public class Global : MonoBehaviour
{
  public static Global instance;
  public static bool Paused = false;
  public static bool Slowed = false;
  public static bool IsQuiting = false;

  [Header( "Global Settings" )]
  public static float Gravity = 16;
  public const float MaxVelocity = 60;
  public float deadZone = 0.1f;

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

  // allowing characters to collide introduces potential for "pinch points"
  public static string[] CharacterCollideLayers = { "Default", "destructible", "triggerAndCollision" }; //, "character", "enemy" };
  public static string[] CharacterSidestepLayers = { "character", "enemy" };
  public static string[] CharacterDamageLayers = { "character" };
  public static string[] TriggerLayers = { "trigger", "triggerAndCollision" };
  public static string[] DefaultProjectileCollideLayers = { "Default", "character", "triggerAndCollision", "enemy", "destructible" };
  // check first before spawning to avoid colliding with these layers on the first frame
  public static string[] ProjectileNoShootLayers = { "Default" };
  public static string[] BouncyGrenadeCollideLayers = { "character", "triggerAndCollision", "enemy", "projectile", "destructible" };

  [Header( "References" )]
  [SerializeField] GameInputRemapper GameInputRemapper;
  [SerializeField] string InitialSceneName;
  public CameraController CameraController;
  [SerializeField] AudioSource music;

  [Header( "Prefabs" )]
  public GameObject audioOneShotPrefab;
  public GameObject AvatarPrefab;

  [Header( "Transient (Assigned at runtime)" )]
  public bool Updating = false;
  public PolygonCollider2D CameraPoly;
  public UnityEngine.Bounds CameraPolyBounds;
  [System.NonSerialized] public SceneScript ss;
  public PlayerController CurrentPlayer;
  [SerializeField] Chopper chopper;
  public Dictionary<string, int> AgentType = new Dictionary<string, int>();

  [Header( "UI" )]
  public GameObject UI;
  public GameObject MainMenu;
  [SerializeField] GameObject HUD;
  bool MenuShowing { get { return MainMenu.activeInHierarchy; } }
  public GameObject LoadingScreen;
  [SerializeField] Image fader;
  public GameObject ready;
  // cursor
  public Transform Cursor;
  public float cursorOuter = 100;
  public float cursorInner = 50;
  public Vector3 CursorDelta;
  public float cursorSensitivity = 1;
  public bool CursorPlayerRelative = true;
  Vector3 aimRaw;
  public float gamepadCursorLerp = 10;
  public bool positionalCursor = true;
  public float positionalCursorSpeed = 30;
  public float cursorScale = 4;
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

  [Header( "Screen Settings" )]
  bool Fullscreen;
  int ScreenWidth;
  int ScreenHeight;
  float UIScale = 1;
  public GameObject ScreenSettingsPrompt;
  public Text ScreenSettingsCountdown;
  Timer ScreenSettingsCountdownTimer = new Timer();

  [Header( "Debug" )]
  [SerializeField] Text debugButtons;
  [SerializeField] Text debugText;
  // loading screen
  bool loadingScene;
  float prog = 0;
  [SerializeField] Image progress;
  [SerializeField] float progressSpeed = 0.5f;

  // color shift
  public Color shiftyColor = Color.red;
  [SerializeField] float colorShiftSpeed = 1;
  [SerializeField] Image shifty;
  // This exists only because of a Unity crash bug when objects with active
  // Contacts are destroyed from within OnCollisionEnter2D()
  List<GameObject> DestructionList = new List<GameObject>();


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

    SceneManager.sceneLoaded += delegate ( Scene arg0, LoadSceneMode arg1 )
    {
      Debug.Log( "scene loaded: " + arg0.name );
    };
    SceneManager.activeSceneChanged += delegate ( Scene arg0, Scene arg1 )
    {
      Debug.Log( "active scene changed from " + arg0.name + " to " + arg1.name );
    };

    InitializeSettings();
    ReadSettings();
    ApplyScreenSettings();

    GameObject[] res = Resources.LoadAll<GameObject>( "" );
    foreach( GameObject go in res )
      ResourceLookup.Add( go.name, go );

    StartCoroutine( InitializeRoutine() );
  }

  IEnumerator InitializeRoutine()
  {
    ShowMenu( false );

    NavMeshSurface[] meshSurfaces = FindObjectsOfType<NavMeshSurface>();
    foreach( var mesh in meshSurfaces )
      AgentType[NavMesh.GetSettingsNameFromID( mesh.agentTypeID )] = mesh.agentTypeID;


    if( Camera.main.orthographic )
      debugText.text = Camera.main.orthographicSize.ToString( "##.#" );
    else
      debugText.text = CameraController.zOffset.ToString( "##.#" );

    GameInputRemapper.InitializeControls();

    if( Camera.main.orthographic )
      Camera.main.orthographicSize = 2;
    else
      Camera.main.fieldOfView = 20;

    if( Application.isEditor && !SimulatePlayer )
    {
      LoadingScreen.SetActive( false );
      music.Play();
      fader.color = Color.clear;
      //yield return new WaitForSecondsRealtime( 1 );
      Updating = true;
      ss = FindObjectOfType<SceneScript>();
      if( ss != null )
      {
        AssignCameraPoly( ss.sb );
        if( ss.level != null )
          ss.level.Generate();
      }
      foreach( var mesh in meshSurfaces )
        mesh.BuildNavMesh();

      if( CurrentPlayer == null )
        SpawnPlayer();
    }
    else
    {
      music.Play();
      yield return LoadSceneRoutine( "home", true, true, false );
    }
  }

  public void LoadScene( string sceneName, bool waitForFadeIn = true, bool spawnPlayer = true, bool fadeOut = true )
  {
    StartCoroutine( LoadSceneRoutine( sceneName, waitForFadeIn, spawnPlayer, fadeOut ) );
  }

  IEnumerator LoadSceneRoutine( string sceneName, bool waitForFadeIn = true, bool spawnPlayer = true, bool fadeOut = true )
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
    Pause();
    HUD.SetActive( false );
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
      //yield return new WaitForSecondsRealtime( 1 );
    }
    loadingScene = false;

    NavMeshSurface[] meshSurfaces = FindObjectsOfType<NavMeshSurface>();
    foreach( var mesh in meshSurfaces )
      mesh.BuildNavMesh();

    Scene scene = SceneManager.GetSceneByName( sceneName );
    if( CurrentPlayer == null )
    {
      if( spawnPlayer )
        SpawnPlayer();
    }
    else
    {
      CurrentPlayer.PostSceneTransition();
    }

    ss = FindObjectOfType<SceneScript>();
    if( ss != null )
    {
      if( ss.level != null )
        ss.level.Generate();
      AssignCameraPoly( ss.sb );
      ss.StartScene();
    }

    HUD.SetActive( true );
    Unpause();
    yield return HideLoadingScreenRoutine();
    FadeClear();
    if( waitForFadeIn )
      while( fadeTimer.IsActive )
        yield return null;
    Updating = true;
  }

  public void Screenshot()
  {
    string now = System.DateTime.Now.Year.ToString() +
                   System.DateTime.Now.Month.ToString( "D2" ) +
                   System.DateTime.Now.Day.ToString( "D2" ) + "." +
                   System.DateTime.Now.Minute.ToString( "D2" ) +
                   System.DateTime.Now.Second.ToString( "D2" );
    ScreenCapture.CaptureScreenshot( Application.persistentDataPath + "/" + now + ".png" );
  }

  float progTarget = 0;
  [SerializeField] GameObject spinner;
  [SerializeField] float spinnerMoveSpeed = 1;

  void Update()
  {
    for( int i = 0; i < DestructionList.Count; i++ )
      GameObject.Destroy( DestructionList[i] );
    DestructionList.Clear();

    Timer.UpdateTimers();

    if( !Updating )
      return;

    if( loadingScene )
    {
      //prog = Mathf.MoveTowards( prog, progTarget, Time.unscaledDeltaTime * progressSpeed );
      //progress.fillAmount = prog;

      if( GameInput.GetKey( "MoveRight" ) )
        spinner.transform.localPosition += Vector3.right * spinnerMoveSpeed * Time.unscaledDeltaTime;
      //inputLeft = Input.GetKey( Global.instance.icsCurrent.keyMap["MoveLeft"] );
    }

    debugButtons.text = "";
    for( int i = 0; i < 20; i++ )
    {
      //if( Input.GetKey( "joystick button " + i ) )
      debugButtons.text += "Button " + i + "=" + Input.GetKey( "joystick button " + i ) + "\n";
    }
    for( int i = 0; i < 6; i++ )
    {
      debugButtons.text += "Axis " + i + "=" + Input.GetAxis( "Joy0Axis" + i ) + "\n";
    }



    if( Mathf.Abs( Input.GetAxis( "Zoom" ) ) > 0 )
    {
      if( Camera.main.orthographic )
      {
        CameraController.orthoTarget += Input.GetAxis( "Zoom" );
        //Camera.main.orthographicSize += Input.GetAxis( "Zoom" );
        debugText.text = Camera.main.orthographicSize.ToString( "##.#" );
      }
      else
      {
        CameraController.zOffset += Input.GetAxis( "Zoom" );
        debugText.text = CameraController.zOffset.ToString( "##.#" );
      }
    }

    if( Input.GetKeyDown( KeyCode.O ) )
    {
      if( Slowed )
        Global.instance.NoSlow();
      else
        Global.instance.Slow();
    }

    if( Input.GetButtonDown( "Screenshot" ) )
    {
      Screenshot();
    }
#if !UNITY_EDITOR
    if( Input.GetKeyDown( KeyCode.P ) )
    {
      if( Paused )
        Unpause();
      else
        Pause();
    }
    if( Input.GetKeyDown( KeyCode.Escape ) )
    {
      if( UnityEngine.Cursor.lockState == CursorLockMode.Locked )
        UnityEngine.Cursor.lockState = CursorLockMode.None;
      else
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
#endif
    if( Input.GetKeyDown( KeyCode.Return ) )
    {
      Chopper chopper = FindObjectOfType<Chopper>();
      if( (!Application.isEditor || Global.instance.SimulatePlayer) && chopper != null )
        ChopDrop();
      else
        CurrentPlayer.transform.position = FindSpawnPosition();
    }

    if( GameInput.UsingKeyboard )
    {
      CursorDelta += new Vector3( Input.GetAxis( "Cursor X" ), Input.GetAxis( "Cursor Y" ), 0 ) * cursorSensitivity;
    }
    else
    {
      Vector3 raw = new Vector3( GameInput.GetAxisRaw( "AimX" ), -GameInput.GetAxisRaw( "AimY" ), 0 );
      if( raw.sqrMagnitude > deadZone * deadZone )
        aimRaw = raw;
      else
        aimRaw = Vector3.zero;
    }

    if( GameInput.GetKeyUp( "Menu" ) )
    {
      ShowMenu( !MenuShowing );
#if UNITY_STANDALONE_LINUX
      // hack: there is a weird bug where the Canvas will not update when an
      // object with a canvas renderer is disabled
      Canvas canvas = FindObjectOfType<Canvas>();
      canvas.gameObject.SetActive( false );
      canvas.gameObject.SetActive( true );
#endif
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
  }


  void OnApplicationFocus( bool hasFocus )
  {
    UnityEngine.Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
    UnityEngine.Cursor.visible = !hasFocus;
  }

  public Vector3 origin;
  public float CursorFactor = 0.02f;
  public Vector2 AimPosition;
  public Vector2 CursorWorldPosition;

  void LateUpdate()
  {
    if( !Updating )
      return;

    if( CurrentPlayer != null )
    {
      if( GameInput.UsingKeyboard )
      {
        CursorDelta = CursorDelta.normalized * Mathf.Max( Mathf.Min( CursorDelta.magnitude, cursorOuter ), cursorInner );
      }
      else
      {
        if( positionalCursor )
        {
          CursorDelta += aimRaw * positionalCursorSpeed;
          CursorDelta = CursorDelta.normalized * Mathf.Max( Mathf.Min( CursorDelta.magnitude, cursorOuter ), cursorInner );
        }
        else
          CursorDelta = Vector3.Lerp( CursorDelta, aimRaw * cursorOuter, Mathf.Clamp01( gamepadCursorLerp * Time.deltaTime ) );
      }

      Cursor.gameObject.SetActive( CursorDelta.sqrMagnitude > cursorInner * cursorInner );


      if( CursorPlayerRelative )
        origin = CurrentPlayer.arm.position;
      else
        origin = CameraController.transform.position;
      origin.z = 0;

      if( AimSnap )
      {
        // set cursor
        Cursor.gameObject.SetActive( true );
        CursorSnapped.gameObject.SetActive( true );

        float angle = Mathf.Atan2( CursorDelta.x, CursorDelta.y ) / Mathf.PI;
        float snap = Mathf.Round( angle * SnapAngleDivide ) / SnapAngleDivide;
        Vector3 snapped = new Vector3( Mathf.Sin( snap * Mathf.PI ), Mathf.Cos( snap * Mathf.PI ), 0 );
        AimPosition = origin + snapped * SnapCursorDistance;
        CursorSnapped.position = AimPosition;
        CursorWorldPosition = origin + CursorDelta * CursorFactor;
        Cursor.position = CursorWorldPosition;
      }
      else
      {
        // set cursor
        Cursor.gameObject.SetActive( true );
        CursorSnapped.gameObject.SetActive( false );

        AimPosition = origin + CursorDelta * CursorFactor;
        CursorWorldPosition = AimPosition;
        //Cursor.anchoredPosition = Camera.main.WorldToScreenPoint( CursorWorldPosition );
        Cursor.position = CursorWorldPosition;
      }


      if( AutoAim )
      {
        CursorWorldPosition = origin + CursorDelta * CursorFactor;
        RaycastHit2D[] hits = Physics2D.CircleCastAll( CurrentPlayer.transform.position, AutoAimCircleRadius, CursorDelta, AutoAimDistance, LayerMask.GetMask( new string[] { "enemy" } ) );
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

    CameraController.CameraLateUpdate();
  }



  public void SpawnPlayer()
  {
    GameObject go = Spawn( AvatarPrefab, FindSpawnPosition(), Quaternion.identity, null, false );
    CurrentPlayer = go.GetComponent<PlayerController>();
    CameraController.LookTarget = CurrentPlayer.gameObject;
    CameraController.transform.position = CurrentPlayer.transform.position;
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


  [Header( "Audio" )]
  public UnityEngine.Audio.AudioMixer mixer;
  public UnityEngine.Audio.AudioMixerSnapshot snapSlowmo;
  public UnityEngine.Audio.AudioMixerSnapshot snapNormal;
  public float AudioFadeDuration = 0.1f;

  public void Slow()
  {
    Time.timeScale = Global.instance.slowtime;
    Time.fixedDeltaTime = 0.01f * Time.timeScale;
    Slowed = true;
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
    Time.timeScale = 1;
    Time.fixedDeltaTime = 0.01f * Time.timeScale;
    Slowed = false;
    Global.instance.mixer.TransitionToSnapshots( new UnityEngine.Audio.AudioMixerSnapshot[] {
      Global.instance.snapNormal,
      Global.instance.snapSlowmo
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
  }

  public static void Unpause()
  {
    Paused = false;
    Time.timeScale = 1;
    Time.fixedDeltaTime = 0.01f * Time.timeScale;
  }

  public void SetCursor( Sprite spr )
  {
    Cursor.GetComponent<SpriteRenderer>().sprite = spr;
    Cursor.localScale = Vector3.one * cursorScale;
  }

  void ShowMenu( bool show )
  {
    if( show )
    {
      Pause();
      MainMenu.SetActive( true );
      HUD.SetActive( false );
      UnityEngine.Cursor.lockState = CursorLockMode.None;
      UnityEngine.Cursor.visible = true;
      EnableRaycaster( true );
      GameInputRemapper.OnShow();
    }
    else
    {
      Unpause();
      MainMenu.SetActive( false );
      HUD.SetActive( true );
      UnityEngine.Cursor.lockState = CursorLockMode.Locked;
      UnityEngine.Cursor.visible = false;
      EnableRaycaster( false );
      Input.ResetInputAxes();
    }
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

  IEnumerator HideLoadingScreenRoutine()
  {
    HideLoadingScreen();
    yield return null;
  }

  // This exists only because of a Unity crash bug when objects with active
  // Contacts are destroyed from within OnCollisionEnter2D()
  public void Destroy( GameObject go )
  {
    go.SetActive( false );
    DestructionList.Add( go );
  }

  public GameData gameData;
  public Dictionary<string, GameObject> ResourceLookup = new Dictionary<string, GameObject>();

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
    GameObject go = GameObject.Instantiate( audioOneShotPrefab, position, Quaternion.identity );
    AudioSource source = go.GetComponent<AudioSource>();
    source.PlayOneShot( clip );
    new Timer( clip.length, null, delegate
    {
      Destroy( go );
    } );
  }

  public void ChopDrop()
  {
    //Camera.main.fieldOfView = 25;
    chopper = FindObjectOfType<Chopper>();
    if( chopper != null )
    {
      chopper.character = CurrentPlayer;
      chopper.StartDrop();
    }
  }

  public void FadeBlack()
  {
    fader.color = new Color( fader.color.r, fader.color.g, fader.color.b, 0 );
    fadeTimer.Stop( true );
    // set gameObject active after Stop() because FadeClear() CompleteDelegate
    // makes the gameObject insactive
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

  [Header( "Speech" )]
  public GameObject SpeechBubble;
  public Text SpeechText;
  public Image SpeechIcon;
  CharacterIdentity SpeechCharacter;
  int SpeechPriority = 0;
  public float SpeechRange = 8;
  Timer SpeechTimer = new Timer();



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
  }



  public void AssignCameraPoly( PolygonCollider2D poly )
  {
    CameraPoly = poly;
    // camera poly bounds points are local to polygon
    CameraPolyBounds = new Bounds();
    foreach( var p in CameraPoly.points )
      CameraPolyBounds.Encapsulate( p );
  }



  public static string[] persistentFilenames = new string[] {
    /*"settings.json",
    "characters.json",
    "events.json"*/
  };
  string settingsPath { get { return Application.persistentDataPath + "/" + "settings.json"; } }
  public Dictionary<string, FloatValue> FloatSetting = new Dictionary<string, FloatValue>();
  public Dictionary<string, BoolValue> BoolSetting = new Dictionary<string, BoolValue>();

  public GameObject ToggleTemplate;
  public GameObject SliderTemplate;

  void CreateBoolSetting( string key, bool value, System.Action<bool> onChange )
  {
    BoolValue bv;
    if( !BoolSetting.TryGetValue( key, out bv ) )
    {
      GameObject go = Instantiate( ToggleTemplate, ToggleTemplate.transform.parent );
      SettingUI sss = go.GetComponent<SettingUI>();
      sss.isBool = true;
      bv = sss.boolValue;
      bv.name = key;
      bv.Init();
      BoolSetting.Add( key, bv );
    }
    bv.onValueChanged = onChange;
    bv.Value = value;
  }

  void CreateFloatSetting( string key, float value, System.Action<float> onChange )
  {
    FloatValue bv;
    if( !FloatSetting.TryGetValue( key, out bv ) )
    {
      GameObject go = Instantiate( SliderTemplate, SliderTemplate.transform.parent );
      SettingUI sss = go.GetComponent<SettingUI>();
      sss.isInteger = true;
      bv = sss.intValue;
      bv.name = key;
      bv.Init();
      FloatSetting.Add( key, bv );
    }
    bv.onValueChanged = onChange;
    bv.Value = value;
  }

  void InitializeSettings()
  {
    SettingUI[] settings = MainMenu.GetComponentsInChildren<SettingUI>( true );
    // use existing UI objects, if they exist
    foreach( var s in settings )
    {
      if( s.gameObject.GetInstanceID() == ToggleTemplate.GetInstanceID() || s.gameObject.GetInstanceID() == SliderTemplate.GetInstanceID() )
        continue;
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

    CreateBoolSetting( "Fullscreen", false, null );  //delegate ( bool value ) { Fullscreen = value; } );
    CreateFloatSetting( "ScreenWidth", 1024, null );  //delegate ( float value ) { ScreenWidth = Mathf.FloorToInt( value ); } );
    CreateFloatSetting( "ScreenHeight", 1024, null );  //delegate ( float value ) { ScreenHeight = Mathf.FloorToInt( value ); } );
    CreateFloatSetting( "UIScale", 1, null );// delegate ( float value ) { UI.GetComponent<CanvasScaler>().scaleFactor = value; } );

    CreateBoolSetting( "UseCameraVertical", true, delegate ( bool value ) { CameraController.UseVerticalRange = value; } );
    CreateBoolSetting( "CursorInfluence", true, delegate ( bool value ) { CameraController.CursorInfluence = value; } );
    CreateBoolSetting( "AimSnap", true, delegate ( bool value ) { AimSnap = value; } );
    CreateBoolSetting( "AutoAim", true, delegate ( bool value ) { AutoAim = value; } );

    CreateFloatSetting( "CursorOuterRadius", 150, delegate ( float value ) { cursorOuter = value; } );
    CreateFloatSetting( "CameraLerpAlpha", 50, delegate ( float value ) { CameraController.lerpAlpha = value; } );
    CreateFloatSetting( "ThumbstickDeadzone", 10, delegate ( float value ) { deadZone = value; } );

    ToggleTemplate.gameObject.SetActive( false );
    SliderTemplate.gameObject.SetActive( false );
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
        pair.Value.Value = JsonUtil.Read<bool>( pair.Value.Value, json, "settings", pair.Key );

      foreach( var pair in FloatSetting )
        pair.Value.Value = JsonUtil.Read<float>( pair.Value.Value, json, "settings", pair.Key );
    }
  }

  void WriteSettings()
  {
    JsonWriter writer = new JsonWriter();
    writer.PrettyPrint = true;
    writer.WriteObjectStart(); // root

    writer.WritePropertyName( "settings" );
    writer.WriteObjectStart();
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
    print( settingsPath );
    File.WriteAllText( settingsPath, writer.ToString() );
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



  public void ApplyScreenSettings()
  {
    Fullscreen = BoolSetting["Fullscreen"].Value;
    ScreenWidth = (int)FloatSetting["ScreenWidth"].Value;
    ScreenHeight = (int)FloatSetting["ScreenHeight"].Value;
    UIScale = FloatSetting["UIScale"].Value;
    Screen.SetResolution( ScreenWidth, ScreenHeight, Fullscreen );
    UI.GetComponent<CanvasScaler>().scaleFactor = UIScale;

    ScreenSettingsPrompt.SetActive( false );
    ScreenSettingsCountdownTimer.Stop( false );
  }

  public void ScreenChangePrompt()
  {
    Fullscreen = Screen.fullScreen;
    ScreenWidth = Screen.width;
    ScreenHeight = Screen.height;
    // UIScale ?

    ScreenSettingsPrompt.SetActive( true );
    Screen.SetResolution( (int)FloatSetting["ScreenWidth"].Value, (int)FloatSetting["ScreenHeight"].Value, BoolSetting["Fullscreen"].Value );
    UI.GetComponent<CanvasScaler>().scaleFactor = FloatSetting["UIScale"].Value;

    TimerParams tp = new TimerParams
    {
      unscaledTime = true,
      repeat = false,
      duration = 10,
      UpdateDelegate = delegate ( Timer obj )
      {
        ScreenSettingsCountdown.text = "Accept changes or revert in " + (10 - obj.ProgressSeconds).ToString( "0" ) + " seconds";
      },
      CompleteDelegate = RevertScreenSettings
    };
    ScreenSettingsCountdownTimer.Start( tp );
  }

  public void RevertScreenSettings()
  {
    BoolSetting["Fullscreen"].Value = Fullscreen;
    FloatSetting["ScreenWidth"].Value = ScreenWidth;
    FloatSetting["ScreenHeight"].Value = ScreenHeight;
    FloatSetting["UIScale"].Value = UIScale;

    ApplyScreenSettings();
  }

}