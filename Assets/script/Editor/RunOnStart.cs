using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunOnStart : MonoBehaviour
{
  [RuntimeInitializeOnLoadMethod]
  static void OnStart()
  {
    for( int i = 0; i < SceneManager.sceneCount; i++ )
    {
      Scene scene = SceneManager.GetSceneAt( i );
      if( scene.name == "GLOBAL" )
        return;
    }
    Instantiate( Resources.Load<GameObject>( "GLOBAL" ) );
  }
}

