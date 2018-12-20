using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{
  public static Global instance;

  [Header( "References" )]
  public Object InitialScene;
  public CameraController CameraController;

  [Header( "Prefabs" )]
  public GameObject AvatarPrefab;

  [Header( "Runtime Objects" )]
  public GameObject CurrentPlayer;

  void Awake()
  {
    if( instance != null )
    {
      Destroy( gameObject );
      return;
    }
    instance = this;
    DontDestroyOnLoad( gameObject );
    Time.timeScale = 0;

    StartCoroutine( InitializeRoutine() );
  }

  IEnumerator InitializeRoutine()
  {
    if( InitialScene != null )
    {
      SceneManager.LoadScene( InitialScene.name, LoadSceneMode.Single );
      yield return null;
    }

    GameObject[] spawns = GameObject.FindGameObjectsWithTag( "Respawn" );
    if( spawns.Length > 0 )
    {
      CurrentPlayer = GameObject.Instantiate( AvatarPrefab, spawns[ Random.Range( 0, spawns.Length ) ].transform.position, Quaternion.identity, null );
      CameraController.LookTarget = CurrentPlayer;
    }
    else
    {
      CurrentPlayer = GameObject.Instantiate( AvatarPrefab, null );
      CameraController.LookTarget = CurrentPlayer;
    }

    Time.timeScale = 1;
  }


}
