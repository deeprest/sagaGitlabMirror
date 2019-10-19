using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunOnStart : MonoBehaviour
{
  [RuntimeInitializeOnLoadMethod]
  static void OnStart()
  {
    /*for( int i = 0; i < SceneManager.sceneCount; i++ )
    {
      Scene scene = SceneManager.GetSceneAt( i );
      GameObject[] gos = scene.GetRootGameObjects();
      foreach( var go in gos )
        if( go.GetComponent<Global>() != null )
          return;
    }*/
    Global global = FindObjectOfType<Global>();
    if( global == null )
      Instantiate( Resources.Load<GameObject>( "GLOBAL" ) );

  }
}
