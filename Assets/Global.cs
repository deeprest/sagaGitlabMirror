using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface ITrigger
{
  void Trigger( Transform instigator );
}
public interface IDamage
{
  void TakeDamage( Damage d );
}

public class Character : MonoBehaviour
{
  new public BoxCollider2D collider;

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
  public const float deadZone = 0.3f;

  [Header( "References" )]
  public string InitialSceneName;
  public CameraController CameraController;

  [Header( "Prefabs" )]
  public GameObject audioOneShotPrefab;
  public GameObject AvatarPrefab;

  [Header( "Runtime Objects" )]
  public PlayerController CurrentPlayer;
  public RectTransform cursor;
  public Chopper chopper;

  public float cursorOuter = 100;
  public float cursorInner = 50;
  public Vector3 cursorDelta;
  public float cursorSensitivity = 1;
  public bool CursorPlayerRelative = true;
  public Sprite[] cursors;
  int cursorIndex = 0;

  public float slowtime = 0.2f;
  [SerializeField] Text debugButtons;


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
    Pause();
    SceneManager.sceneLoaded += delegate ( Scene arg0, LoadSceneMode arg1 )
    {
      //Debug.Log( "scene loaded" );
    };
    SceneManager.activeSceneChanged += delegate ( Scene arg0, Scene arg1 )
    {
      //Debug.Log( "active scene changed" );
    };

    InitializeControls();

    StartCoroutine( InitializeRoutine() );
  }

  IEnumerator InitializeRoutine()
  {
    if( !Application.isEditor )
    {
      SceneManager.LoadScene( InitialSceneName, LoadSceneMode.Single );
      yield return null;
    }
    GameObject go = Spawn( AvatarPrefab, FindSpawnPosition(), Quaternion.identity, null, false );
    CurrentPlayer = go.GetComponent<PlayerController>();
    CameraController.LookTarget = CurrentPlayer.gameObject;
    CameraController.transform.position = CurrentPlayer.transform.position;

    yield return new WaitForSecondsRealtime( 1f );
    Unpause();
    yield return null;
  }

  void Update()
  {
    Timer.UpdateTimers();

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
      Camera.main.orthographicSize += Input.GetAxis( "Zoom" );
    }

    if( Input.GetKeyDown( KeyCode.O ) )
    {
      if( Slowed )
        Global.instance.NoSlow();
      else
        Global.instance.Slow();
    }

    /*if( Input.GetKeyDown( KeyCode.P ) )
    {
      if( Paused )
        Unpause();
      else
        Pause();
    }*/
    if( Input.GetKeyDown( KeyCode.Return ) )
    {
      GameObject go = FindSpawnPoint();
      if( go != null )
      {
        ChopDrop();
      }
    }

    if( UsingKeyboard )
      cursorDelta += new Vector3( Input.GetAxis( "Cursor X" ) * cursorSensitivity, Input.GetAxis( "Cursor Y" ) * cursorSensitivity, 0 );
    else
      cursorDelta = new Vector3( Input.GetAxisRaw( icsCurrent.axisMap["ShootX"] ), -Input.GetAxisRaw( icsCurrent.axisMap["ShootY"] ), 0 );

    if( Input.GetKeyDown( KeyCode.R ) )
      NextCursor();
  }

  void OnApplicationFocus( bool hasFocus )
  {
    if( hasFocus )
      Cursor.lockState = CursorLockMode.Locked;
    else
      Cursor.lockState = CursorLockMode.None;
  }

  void LateUpdate()
  {
    if( CurrentPlayer != null )
    {
      if( UsingKeyboard )
      {
        cursorDelta = cursorDelta.normalized * Mathf.Max( Mathf.Min( cursorDelta.magnitude, cursorOuter ), cursorInner );
      }
      else
      {
        if( cursorDelta.sqrMagnitude > deadZone * deadZone )
        {
          cursor.gameObject.SetActive( true );
          cursorDelta = cursorDelta.normalized * cursorOuter;
        }
        else
        { 
          cursor.gameObject.SetActive( false );
        }
      }

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

  Vector3 FindSpawnPosition()
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

  public float cursorScale = 4;

  public class InputControlScheme
  {
    public Dictionary<string, KeyCode> keyMap = new Dictionary<string, KeyCode>();
    public Dictionary<string, string> axisMap = new Dictionary<string, string>();
  }

  public InputControlScheme icsCurrent { get; set; }

  InputControlScheme icsKeyboard = new InputControlScheme();
  InputControlScheme icsGamepad = new InputControlScheme();

  public bool UsingKeyboard { get { return icsCurrent == icsKeyboard; } }

  [Header( "Control Remap" )]
  KeyCode[] allKeys;
  KeyCode[] activateKeyboardKeys;
  bool remappingControls = false;
  // UI
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

    icsKeyboard.keyMap["Charge"] = KeyCode.Mouse0;

    // gamepad
    icsGamepad.keyMap["MoveRight"] = KeyCode.JoystickButton8;
    icsGamepad.keyMap["MoveLeft"] = KeyCode.JoystickButton7;
    icsGamepad.keyMap["Jump"] = KeyCode.JoystickButton14;
    icsGamepad.keyMap["Down"] = KeyCode.JoystickButton0;
    icsGamepad.keyMap["Dash"] = KeyCode.JoystickButton13;
    icsGamepad.keyMap["Fire"] = KeyCode.None;
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
    GameObject go = GameObject.Instantiate( controllerMapListItemTemplate, controllerMapListParent );
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

    GameObject go = GameObject.Instantiate( prefab, position, rotation, parent );
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


  void ChopDrop()
  {
    chopper = FindObjectOfType<Chopper>();
    if( chopper != null )
    {
      chopper.character = CurrentPlayer;
      chopper.StartDrop();
    }
  }

}
