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

  [Header( "References" )]
  public Object InitialScene;
  public string InitialSceneName;
  public CameraController CameraController;

  [Header( "Prefabs" )]
  public GameObject AvatarPrefab;

  [Header( "Runtime Objects" )]
  public GameObject CurrentPlayer;
  public RectTransform cursor;


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
      Debug.Log("scene loaded");
    };
    SceneManager.activeSceneChanged += delegate(Scene arg0, Scene arg1 )
    {
      Debug.Log("active scene changed");
    };
    StartCoroutine( InitializeRoutine() );
  }

  IEnumerator InitializeRoutine()
  {
    SceneManager.LoadScene( InitialSceneName, LoadSceneMode.Single );
    yield return null;

    GameObject[] spawns = GameObject.FindGameObjectsWithTag( "Respawn" );
    if( spawns.Length > 0 )
    {
      CurrentPlayer = GameObject.Instantiate( AvatarPrefab, spawns[ Random.Range( 0, spawns.Length ) ].transform.position, Quaternion.identity, null );
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
  Vector3 cursorDelta;

  void Update()
  {
    if( Input.GetKeyDown( KeyCode.P ) )
    {
      if( Paused )
        Unpause();
      else
        Pause();
    }
      
    cursorDelta += new Vector3( Input.GetAxis( "Mouse X" ), Input.GetAxis( "Mouse Y" ), 0 );
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
    cursorIndex = ++cursorIndex % (cursors.Length - 1);
    SetCursor( cursors[ cursorIndex ] );
  }

  void SetCursor( Sprite spr )
  {
    cursor.GetComponent<Image>().sprite = spr;
    cursor.sizeDelta = new Vector2( spr.rect.width, spr.rect.height ) * cursorScale; // * spr.pixelsPerUnit;
  }
  public float cursorScale = 4;
}
