#pragma warning disable 414


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( Global ) )]
public class GlobalEditor : Editor
{
  Global obj;
  int screenshotInterval;
  Timer ScreenshotTimer = new Timer();

  public override void OnInspectorGUI()
  {
    screenshotInterval = EditorGUILayout.IntField( "Screenshot Interval", screenshotInterval );
    obj = target as Global;
    if( ScreenshotTimer.IsActive )
    {
      if( GUI.Button( EditorGUILayout.GetControlRect(), "Stop Screenshot Timer" ) )
        ScreenshotTimer.Stop( false );
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
    ScreenshotTimer.Start( screenshotInterval, null, delegate
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
  void TakeDamage( Damage d );
}


// TODO upgrade to 2019 and use new input system, or find something on asset store.

public class Global : MonoBehaviour
{
  public static Global instance;
  public static bool Paused = false;
  public static bool Slowed = false;
  public static bool IsQuiting = false;

  // settings
  public static float Gravity = 16;
  public const float MaxVelocity = 60;
  public float deadZone = 0.1f;
  public bool SimulatePlayer = false;
  [SerializeField] float slowtime = 0.2f;
  Timer fadeTimer = new Timer();
  public float RepathInterval = 1;

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
  public RectTransform cursor;
  public Image cursorImage;
  public float cursorOuter = 100;
  public float cursorInner = 50;
  public Vector3 cursorDelta;
  public float cursorSensitivity = 1;
  public bool CursorPlayerRelative = true;
  Vector3 aimRaw;
  public float gamepadCursorLerp = 10;
  public bool positionalCursor = true;
  public float cursorSpeed = 30;
  public float cursorScale = 4;
  // status 
  public Image weaponIcon;

  [Header( "Debug" )]
  [SerializeField] Text debugButtons;
  [SerializeField] Text debugText;
  // loading screen
  bool loadingScene;
  float prog = 0;
  [SerializeField] Image progress;
  [SerializeField] float progressSpeed = 0.5f;

  // color shift
  Color shiftyColor = Color.red;
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
      foreach( var mesh in meshSurfaces )
        mesh.BuildNavMesh();
      SpawnPlayer();
      fader.color = Color.clear;
      //yield return new WaitForSecondsRealtime( 1 );
      Updating = true;
      ss = FindObjectOfType<SceneScript>();
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
      //yield return new WaitForEndOfFrame();
      yield return new WaitForSecondsRealtime( 1 );
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
        Camera.main.orthographicSize += Input.GetAxis( "Zoom" );
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
      if( Cursor.lockState == CursorLockMode.Locked )
        Cursor.lockState = CursorLockMode.None;
      else
        Cursor.lockState = CursorLockMode.Locked;
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
      cursorDelta += new Vector3( Input.GetAxis( "Cursor X" ), Input.GetAxis( "Cursor Y" ), 0 ) * cursorSensitivity;
    }
    else
    {
      Vector3 raw = new Vector3( Input.GetAxisRaw( GameInput.icsCurrent.axisMap["ShootX"] ), -Input.GetAxisRaw( GameInput.icsCurrent.axisMap["ShootY"] ), 0 );
      if( raw.sqrMagnitude > deadZone * deadZone )
        aimRaw = raw;
      else
        aimRaw = Vector3.zero;
    }

    if( Input.GetButtonUp( "Menu" ) )
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
    Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = !hasFocus;
  }

  void LateUpdate()
  {
    if( !Updating )
      return;

    if( CurrentPlayer != null )
    {
      if( GameInput.UsingKeyboard )
      {
        cursorDelta = cursorDelta.normalized * Mathf.Max( Mathf.Min( cursorDelta.magnitude, cursorOuter ), cursorInner );
      }
      else
      {
        if( positionalCursor )
        {
          cursorDelta += aimRaw * cursorSpeed;
          cursorDelta = cursorDelta.normalized * Mathf.Max( Mathf.Min( cursorDelta.magnitude, cursorOuter ), cursorInner );
        }
        else
        {
          cursorDelta = Vector3.Lerp( cursorDelta, aimRaw * cursorOuter, Mathf.Clamp01( gamepadCursorLerp * Time.deltaTime ) );
        }
      }

      cursor.gameObject.SetActive( cursorDelta.sqrMagnitude > cursorInner * cursorInner );

      Vector3 origin;
      if( CursorPlayerRelative )
        origin = CurrentPlayer.transform.position; //.arm.position;
      else
        origin = CameraController.transform.position;
      origin.z = 0;
      cursor.anchoredPosition = Camera.main.WorldToScreenPoint( origin ) + cursorDelta;
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
    cursorImage.sprite = spr;
    cursor.sizeDelta = new Vector2( spr.rect.width, spr.rect.height ) * cursorScale; // * spr.pixelsPerUnit;
  }

  void ShowMenu( bool show )
  {
    if( show )
    {
      Pause();
      MainMenu.SetActive( true );
      HUD.SetActive( false );
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      EnableRaycaster( true );
      GameInputRemapper.OnShow();
    }
    else
    {
      Unpause();
      MainMenu.SetActive( false );
      HUD.SetActive( true );
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
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
      //      SerializedComponent[] scs = go.GetComponentsInChildren<SerializedComponent>();
      //      foreach( var sc in scs )
      //        sc.AfterDeserialize();
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




}