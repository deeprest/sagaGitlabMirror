using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Global : MonoBehaviour
{

  //  public void Log(string message, [System.Runtime.CompilerServices.CallerFilePath] string filePath = "", [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
  //  {
  //    Debug.Log( lineNumber + " " + message );
  //  }

  public static Global instance;
  public static bool Paused = false;
  public static bool IsQuiting = false;

  [Header( "References" )]
//  public Object InitialScene;
  public string InitialSceneName;
  public CameraController CameraController;

  [Header( "Prefabs" )]
  public GameObject AvatarPrefab;

  [Header( "Runtime Objects" )]
  public GameObject CurrentPlayer;
  public RectTransform cursor;

  void OnApplicationQuit()
  {
    if( !IsQuiting )
    {
      Application.CancelQuit();
      IsQuiting = true;
      // do pre-quit stuff here
      //
      Application.Quit();
    }
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
    SceneManager.sceneLoaded += delegate(Scene arg0, LoadSceneMode arg1 )
    {
      Debug.Log( "scene loaded" );
    };
    SceneManager.activeSceneChanged += delegate(Scene arg0, Scene arg1 )
    {
      Debug.Log( "active scene changed" );
    };

    InitializeControls();



    StartCoroutine( InitializeRoutine() );
  }

  IEnumerator InitializeRoutine()
  {
    SceneManager.LoadScene( InitialSceneName, LoadSceneMode.Single );
    yield return null;

    GameObject[] spawns = GameObject.FindGameObjectsWithTag( "Respawn" );
    if( spawns.Length > 0 )
    {
      CurrentPlayer = Spawn( AvatarPrefab, spawns[ Random.Range( 0, spawns.Length ) ].transform.position, Quaternion.identity, null, false );
      //CameraController.LockOn( CurrentPlayer );
      CameraController.LookTarget = CurrentPlayer;
    }
    else
    {
      CurrentPlayer = GameObject.Instantiate( AvatarPrefab, null );
      CameraController.LookTarget = CurrentPlayer;
//      CameraController.LockOn( CurrentPlayer );
    }
    yield return new WaitForSecondsRealtime( 0.1f );
    Unpause();
    yield return null;
  }

  public float cursorOuter = 100;
  public float cursorInner = 50;
  public Vector3 cursorDelta;
  public float cursorSensitivity = 1;

  void Update()
  {
    Timer.UpdateTimers();

    if( Input.GetKeyDown( KeyCode.P ) )
    {
      if( Paused )
        Unpause();
      else
        Pause();
    }
      
    cursorDelta += new Vector3( Input.GetAxis( "Cursor X" ) * cursorSensitivity, Input.GetAxis( "Cursor Y" ) * cursorSensitivity, 0 );
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
      Vector3 origin = Camera.main.WorldToScreenPoint( CurrentPlayer.transform.position );
      Vector3 delta = cursorDelta;
      float mag = Mathf.Max( Mathf.Min( delta.magnitude, cursorOuter ), cursorInner );
      delta = delta.normalized * mag;
      cursorDelta = delta;
      cursor.anchoredPosition = origin + cursorDelta;
    }
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

  public Sprite[] cursors;
  int cursorIndex = 0;

  void NextCursor()
  {
    cursorIndex = ++cursorIndex % ( cursors.Length - 1 );
    SetCursor( cursors[ cursorIndex ] );
  }

  void SetCursor( Sprite spr )
  {
    cursor.GetComponent<Image>().sprite = spr;
    cursor.sizeDelta = new Vector2( spr.rect.width, spr.rect.height ) * cursorScale; // * spr.pixelsPerUnit;
  }

  public float cursorScale = 4;

  [Header( "Control Remap" )]
  public Dictionary<string,KeyCode> keyMap = new Dictionary<string, KeyCode>();
  public Dictionary<string,string> axisMap = new Dictionary<string, string>();
  public GameObject controllerRemapScreen;
  public RectTransform controllerMapListParent;
  public GameObject controllerMapListItemTemplate;
  public Text remapInstruction;

  void InitializeControls()
  {
    for( int i = 0; i < 20; i++ )
      axes.Add( "Joy0Axis" + i );

    keyMap[ "MoveRight" ] = KeyCode.D;
    keyMap[ "MoveLeft" ] = KeyCode.A;
    keyMap[ "Jump" ] = KeyCode.W;
    keyMap[ "Dash" ] = KeyCode.Space;
    keyMap[ "Fire" ] = KeyCode.Mouse0;
    axisMap[ "ShootX" ] = "Joy0Axis2";
    axisMap[ "ShootY" ] = "Joy0Axis3";

    // UI
    controllerRemapScreen.SetActive( false );
    controllerMapListItemTemplate.SetActive( false );
    //populate controller remap list
    foreach( var pair in keyMap )
      AddControlListItem( pair.Key );
    foreach( var pair in axisMap )
      AddControlListItem( pair.Key );
  }

  void AddControlListItem( string key )
  {
    GameObject go = GameObject.Instantiate( controllerMapListItemTemplate, controllerMapListParent );
    go.SetActive( true );
    go.name = key;
    Text[] ts = go.GetComponentsInChildren<Text>();
    ts[ 0 ].text = key;
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
    bool done = false;

    KeyCode[] allKeys = (KeyCode[])System.Enum.GetValues( typeof(KeyCode) );
    // remap gamepad input
    Dictionary<string,KeyCode> changeKey = new Dictionary<string, KeyCode>();
    Dictionary<string,KeyCode>.Enumerator enu = keyMap.GetEnumerator();
    while( !done && enu.MoveNext() )
    {
      string key = enu.Current.Key;
      GameObject go = controllerMapListParent.Find( key ).gameObject;
      Text[] ts = go.GetComponentsInChildren<Text>();
      ts[ 0 ].color = Color.red;
      remapInstruction.text = "press desired button for action: " + key;

      bool hit = false;
      while( !hit )
      {
        if( Input.GetButtonDown( "Cancel" ) )
          break;
        for( int i = 0; i < allKeys.Length; i++ )
        {
          if( Input.GetKeyDown( allKeys[ i ] ) )
          {
            ts[ 1 ].text = i.ToString();
            hit = true;
            changeKey[ key ] = allKeys[ i ];
            break;
          }
        }
        yield return null;
      }

      if( hit )
      {
        ts[ 0 ].color = Color.white;
        yield return new WaitForSecondsRealtime( 0.1f );
      }
    }


    Dictionary<string,string> changeAxis = new Dictionary<string, string>();
    Dictionary<string,string>.Enumerator axisEnu = axisMap.GetEnumerator();
    while( !done && axisEnu.MoveNext() )
    {
      string key = axisEnu.Current.Key;
      //pointer
      GameObject go = controllerMapListParent.Find( key ).gameObject;
      Text[] ts = go.GetComponentsInChildren<Text>();
      ts[ 0 ].color = Color.red;
      remapInstruction.text = "press desired button for action: " + key;

      bool hit = false;
      while( !hit )
      {
        if( Input.GetButtonDown( "Cancel" ) )
          break;
        foreach( var ax in axes )
        {
          if( Mathf.Abs( Input.GetAxisRaw( ax ) ) > 0.5f )
          {
            changeAxis[ key ] = ax;
            hit = true;
            break;
          }
        }
        if( !hit && Input.anyKeyDown )
        {
          for( int i = 0; i < allKeys.Length; i++ )
          {
            if( Input.GetKeyDown( allKeys[ i ] ) )
            {
              ts[ 1 ].text = i.ToString();
              hit = true;
              changeKey[ key ] = allKeys[ i ];
              break;
            }
          }
        }
        yield return null;
      }

      if( hit )
      {
        ts[ 0 ].color = Color.white;
        yield return new WaitForSecondsRealtime( 0.1f );
      }
    }

    foreach( var pair in changeKey )
      keyMap[ pair.Key ] = pair.Value;
    foreach( var pair in changeAxis )
      axisMap[ pair.Key ] = pair.Value;
    
    controllerRemapScreen.SetActive( false );
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
}
