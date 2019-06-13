using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

public class Attack
{
  public Transform instigator;
}

// TODO upgrade to new input system

public class Global : MonoBehaviour
{

  public static Global instance;
  public static bool Paused = false;
  public static bool Slowed = false;
  public static bool IsQuiting = false;

  // settings
  public static float Gravity = 16;
  public const float MaxVelocity = 60;
  public float deadZone = 0.3f;
  public bool SimulatePlayer = false;

  [Header( "References" )]
  public string InitialSceneName;
  public CameraController CameraController;
  public Image fader;
  public GameObject ready;
  public AudioSource music;

  [Header( "Prefabs" )]
  public GameObject audioOneShotPrefab;
  public GameObject AvatarPrefab;

  [Header( "Runtime Objects" )]
  public PlayerController CurrentPlayer;
  public RectTransform cursor;
  public Chopper chopper;


  public float slowtime = 0.2f;
  [SerializeField] Text debugButtons;
  [SerializeField] Text debugText;

  Timer fadeTimer = new Timer();
  public float introdelay = 13;

  [Header( "UI" )]
  public float cursorOuter = 100;
  public float cursorInner = 50;
  public Vector3 cursorDelta;
  public float cursorSensitivity = 1;
  public bool CursorPlayerRelative = true;
  public Sprite[] cursors;
  int cursorIndex = 0;
  Vector3 aimRaw;
  public float cursorLerp = 10;
  public bool positionalCursor = true;
  public float cursorSpeed = 30;
  public float cursorScale = 4;


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
      Debug.Log( "scene loaded: "+arg0.name );
    };
    SceneManager.activeSceneChanged += delegate ( Scene arg0, Scene arg1 )
    {
      Debug.Log( "active scene changed from "+arg0.name+" to "+arg1.name );
    };

    // UI
    //Cursor.visible = false;
    if( Camera.main.orthographic )
      debugText.text = Camera.main.orthographicSize.ToString( "##.#" );
    else
      debugText.text = CameraController.zOffset.ToString( "##.#" );
    InitializeControls();
    StartCoroutine( InitializeRoutine() );
  }

  public void SpawnPlayer()
  {
    GameObject go = Spawn( AvatarPrefab, FindSpawnPosition(), Quaternion.identity, null, false );
    CurrentPlayer = go.GetComponent<PlayerController>();
    CameraController.LookTarget = CurrentPlayer.gameObject;
    CameraController.transform.position = CurrentPlayer.transform.position;
  }

  IEnumerator InitializeRoutine()
  {
    Pause();
    if( Application.isEditor && !SimulatePlayer )
    {
      music.Play();
      SpawnPlayer();
      yield return new WaitForSecondsRealtime( 1 );
      Unpause();
    }
    else
    {
      if( Camera.main.orthographic )
        Camera.main.orthographicSize = 2;
      else
        Camera.main.fieldOfView = 20;
      music.Play();
      //yield return LoadSceneRoutine( "home" );
      fader.color = Color.black;
      AsyncOperation ao = SceneManager.LoadSceneAsync( "home", LoadSceneMode.Single );
      while( !ao.isDone )
        yield return null;
      SpawnPlayer();
      Unpause();
      FadeClear();
      while( fadeTimer.IsActive )
        yield return null;
    }
    yield return null;
  }

  public void LoadScene( string sceneName, bool waitForFadeIn = true )
  {
    StartCoroutine( LoadSceneRoutine( sceneName, waitForFadeIn ) );
  }

  IEnumerator LoadSceneRoutine( string sceneName, bool waitForFadeIn = true )
  {
    FadeBlack();
    while( fadeTimer.IsActive )
      yield return null;
    if( CurrentPlayer != null )
    {
      CurrentPlayer.PreSceneTransition();
      SceneManager.MoveGameObjectToScene( CurrentPlayer.gameObject, gameObject.scene );
    }
    AsyncOperation ao = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Single );
    while( !ao.isDone )
      yield return null;
    Scene scene = SceneManager.GetSceneByName( sceneName );
    if( CurrentPlayer != null )
    {
      CurrentPlayer.PostSceneTransition();
      SceneManager.MoveGameObjectToScene( CurrentPlayer.gameObject, scene );
    }
    FadeClear();
    if( waitForFadeIn )
      while( fadeTimer.IsActive )
        yield return null;
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

  void Update()
  {
    //Timer.UpdateTimers();

    debugButtons.text = "";
    for( int i = 0; i < 20; i++ )
    {
      if( Input.GetKey( "joystick button " + i ) )
        debugButtons.text += "Button " + i + "=" + Input.GetKey( "joystick button " + i ) + "| ";
    }

    if( !remappingControls )
    {
      if( UsingKeyboard )
      {
        for( int i = 0; i < 20; i++ )
        {
          if( Input.GetKey( "joystick button " + i ) )
          {
            UseGamepad();
          }
        }
      }
      else
      {
        for( int i = 0; i < activateKeyboardKeys.Length; i++ )
        {
          if( Input.GetKeyDown( activateKeyboardKeys[i] ) )
          {
            UseKeyboard();
          }
        }
      }
    }

    if( Mathf.Abs( Input.GetAxis( "Zoom" ) ) > 0 )
    {
      if( Camera.main.orthographic )
      {
        Camera.main.orthographicSize += Input.GetAxis( "Zoom" );
        debugText.text = Camera.main.orthographicSize.ToString("##.#");
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
#endif
    if( Input.GetKeyDown( KeyCode.Return ) )
    {
      Chopper chopper = FindObjectOfType<Chopper>();
      if( (!Application.isEditor || Global.instance.SimulatePlayer) && chopper != null )
        ChopDrop();
      else
        CurrentPlayer.transform.position = FindSpawnPosition();
    }

    if( UsingKeyboard )
    {
      cursorDelta += new Vector3( Input.GetAxis( "Cursor X" ), Input.GetAxis( "Cursor Y" ), 0 ) * cursorSensitivity;
    }
    else
    {
      Vector3 raw = new Vector3( Input.GetAxisRaw( icsCurrent.axisMap["ShootX"] ), -Input.GetAxisRaw( icsCurrent.axisMap["ShootY"] ), 0 );
      if( raw.sqrMagnitude > deadZone * deadZone )
        aimRaw = raw;
      else
        aimRaw = Vector3.zero;
    }

    if( Input.GetKeyDown( KeyCode.R ) )
      NextCursor();
  }

  void OnApplicationFocus( bool hasFocus )
  {
    Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = !hasFocus;
  }



  void LateUpdate()
  {
    Timer.UpdateTimers();

    if( CurrentPlayer != null )
    {
      if( UsingKeyboard )
      {
        cursorDelta = cursorDelta.normalized * Mathf.Max( Mathf.Min( cursorDelta.magnitude, cursorOuter ), cursorInner );
      }
      else if( positionalCursor )
      {
        cursorDelta += aimRaw * cursorSpeed;
        cursorDelta = cursorDelta.normalized * Mathf.Max( Mathf.Min( cursorDelta.magnitude, cursorOuter ), cursorInner );
      }
      else
      {
        cursorDelta = Vector3.Lerp( cursorDelta, aimRaw * cursorOuter, Mathf.Clamp01( cursorLerp * Time.deltaTime ) );
      }

      cursor.gameObject.SetActive( cursorDelta.sqrMagnitude > cursorInner * cursorInner );

      Vector3 origin;
      if( CursorPlayerRelative )
        origin = CurrentPlayer.arm.position;
      else
        origin = Camera.main.transform.position;
      origin.z = 0;
      cursor.anchoredPosition = Camera.main.WorldToScreenPoint( origin ) + cursorDelta;
    }

    CameraController.CameraLateUpdate();
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
    Time.timeScale = 0;
    Paused = true;
  }

  public static void Unpause()
  {
    Time.timeScale = 1;
    Paused = false;
  }



  void NextCursor()
  {
    cursorIndex = ++cursorIndex % (cursors.Length - 1);
    SetCursor( cursors[cursorIndex] );
  }

  void SetCursor( Sprite spr )
  {
    cursor.GetComponent<Image>().sprite = spr;
    cursor.sizeDelta = new Vector2( spr.rect.width, spr.rect.height ) * cursorScale; // * spr.pixelsPerUnit;
  }



  public class InputControlScheme
  {
    public Dictionary<string, KeyCode> keyMap = new Dictionary<string, KeyCode>();
    public Dictionary<string, string> axisMap = new Dictionary<string, string>();
  }

  public InputControlScheme icsCurrent { get; set; }

  InputControlScheme icsKeyboard = new InputControlScheme();
  InputControlScheme icsGamepad = new InputControlScheme();

  public bool UsingKeyboard { get { return icsCurrent == icsKeyboard; } }

  KeyCode[] allKeys;
  KeyCode[] activateKeyboardKeys;
  bool remappingControls = false;

  [Header( "Control Remap" )]
  public GameObject controllerRemapScreen;
  public RectTransform controllerMapListParent;
  public GameObject controllerMapListItemTemplate;
  public Text remapInstruction;
  public Sprite keyboardSprite;
  public Sprite gamepadSprite;
  public Image controllerIndicator;

  void UseKeyboard()
  {
    print( "using keyboard" );
    icsCurrent = icsKeyboard;
    RepopulateControlBindingList();
    controllerIndicator.sprite = keyboardSprite;
    CursorPlayerRelative = false;
  }

  void UseGamepad()
  {
    print( "using gamepad" );
    icsCurrent = icsGamepad;
    RepopulateControlBindingList();
    controllerIndicator.sprite = gamepadSprite;
    CursorPlayerRelative = true;
  }

  void RepopulateControlBindingList()
  {
    // repopulate controller remap list
    for( int i = 0; i < controllerMapListParent.childCount; i++ )
    {
      Transform tf = controllerMapListParent.GetChild( i );
      if( tf.gameObject == controllerMapListItemTemplate )
        continue;
      Destroy( tf.gameObject );
    }
    foreach( var pair in icsCurrent.keyMap )
      AddControlListItem( pair.Key );
    foreach( var pair in icsCurrent.axisMap )
      AddControlListItem( pair.Key );
  }

  string[] GetAllActions()
  {
    List<string> str = new List<string>();
    foreach( var pair in icsCurrent.keyMap )
    {
      if( !str.Contains( pair.Key ) )
        str.Add( pair.Key );
    }
    foreach( var pair in icsCurrent.axisMap )
    {
      if( !str.Contains( pair.Key ) )
        str.Add( pair.Key );
    }
    return str.ToArray();
  }

  void SetDefaultControls()
  {
    // keyboard
    icsKeyboard.keyMap["MoveRight"] = KeyCode.D;
    icsKeyboard.keyMap["MoveLeft"] = KeyCode.A;
    icsKeyboard.keyMap["Jump"] = KeyCode.W;
    icsKeyboard.keyMap["Down"] = KeyCode.S;
    icsKeyboard.keyMap["Dash"] = KeyCode.Space;
    icsKeyboard.keyMap["Fire"] = KeyCode.Mouse0;
    icsKeyboard.keyMap["graphook"] = KeyCode.Mouse1;

    icsKeyboard.keyMap["Charge"] = KeyCode.Mouse0;

    // gamepad
    icsGamepad.keyMap["MoveRight"] = KeyCode.JoystickButton8;
    icsGamepad.keyMap["MoveLeft"] = KeyCode.JoystickButton7;
    icsGamepad.keyMap["Jump"] = KeyCode.JoystickButton14;
    icsGamepad.keyMap["Down"] = KeyCode.JoystickButton0;
    icsGamepad.keyMap["Dash"] = KeyCode.JoystickButton13;
    icsGamepad.keyMap["Fire"] = KeyCode.None;
    icsGamepad.keyMap["graphook"] = KeyCode.None;
    icsGamepad.keyMap["Charge"] = KeyCode.None;
    icsGamepad.axisMap["ShootX"] = "Joy0Axis2";
    icsGamepad.axisMap["ShootY"] = "Joy0Axis3";

  }


  void InitializeControls()
  {
    allKeys = (KeyCode[])System.Enum.GetValues( typeof( KeyCode ) );
    List<KeyCode> temp = new List<KeyCode>( allKeys );
    temp.RemoveAll( x => x >= KeyCode.JoystickButton0 );
    // Escape is used to access keybind menu. 
    // This is intended to avoid changing current input scheme before remapping controls
    temp.RemoveAll( x => x == KeyCode.Escape || (x >= KeyCode.Mouse0 && x <= KeyCode.Mouse6) );
    activateKeyboardKeys = temp.ToArray();

    for( int i = 0; i < 20; i++ )
      axes.Add( "Joy0Axis" + i );

    SetDefaultControls();
    UseKeyboard();

    // UI
    controllerRemapScreen.SetActive( false );
    controllerMapListItemTemplate.SetActive( false );


  }

  void AddControlListItem( string key )
  {
    GameObject go = Instantiate( controllerMapListItemTemplate, controllerMapListParent );
    go.SetActive( true );
    go.name = key;
    Text[] ts = go.GetComponentsInChildren<Text>();
    ts[0].text = key;
  }

  public void InitiateRemap()
  {
    controllerRemapScreen.SetActive( true );
    remapInstruction.text = "press desired button for each action";
    StartCoroutine( RemapRoutine() );
  }

  List<string> axes = new List<string>();

  IEnumerator RemapRoutine()
  {
    remappingControls = true;
    bool done = false;

    Dictionary<string, KeyCode> changeKey = new Dictionary<string, KeyCode>();
    Dictionary<string, string> changeAxis = new Dictionary<string, string>();

    string[] actions = GetAllActions();
    for( int a = 0; !done && a < actions.Length; a++ )
    {
      string key = actions[a];
      GameObject go = controllerMapListParent.Find( key ).gameObject;
      Text[] ts = go.GetComponentsInChildren<Text>();
      ts[0].color = Color.red;
      remapInstruction.text = "press desired button for action: " + key;

      bool hit = false;
      while( !hit )
      {
        yield return null;
        if( Input.GetButtonDown( "Cancel" ) )
          break;
        for( int i = 0; i < allKeys.Length; i++ )
        {
          if( Input.GetKeyDown( allKeys[i] ) )
          {
            ts[1].text = i.ToString();
            hit = true;
            changeKey[key] = allKeys[i];
            break;
          }
        }
        if( !hit )
        {
          foreach( var ax in axes )
          {
            if( Mathf.Abs( Input.GetAxisRaw( ax ) ) > 0.5f )
            {
              changeAxis[key] = ax;
              hit = true;
              break;
            }
          }
        }
      }

      if( hit )
      {
        ts[0].color = Color.white;
        Input.ResetInputAxes();
        yield return new WaitForSecondsRealtime( 0.2f );
      }
    }

    foreach( var pair in changeKey )
      icsCurrent.keyMap[pair.Key] = pair.Value;
    foreach( var pair in changeAxis )
      icsCurrent.axisMap[pair.Key] = pair.Value;

    controllerRemapScreen.SetActive( false );
    Input.ResetInputAxes();
    yield return null;
    remappingControls = false;
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
        fc.a = t.ProgressNormalized;
        fader.color = fc;
      }
    };
    fadeTimer.Start( tp );
  }

  public void FadeClear()
  {
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
        //fader.gameObject.SetActive( false );
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