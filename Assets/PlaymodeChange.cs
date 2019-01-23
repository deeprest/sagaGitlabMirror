#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class PlayModeStateChangedExample
{
  // register an event handler when the class is initialized
  static PlayModeStateChangedExample()
  {
    EditorApplication.playModeStateChanged += LogPlayModeState;
  }

  static SpriteAnimator[] sas;

  private static void LogPlayModeState(PlayModeStateChange state)
  {
    Debug.Log(state);
    if( state == PlayModeStateChange.ExitingEditMode )
    {
      sas = GameObject.FindObjectsOfType<SpriteAnimator>();
      foreach( var sa in sas )
        if( sa.gameObject.scene.isLoaded )
        {
          //Debug.Log( sa.gameObject.name, sa );
          if( !sa.UseSpriteRenderer )
            sa.Stop();
        }
    }
    else
    if( state == PlayModeStateChange.EnteredEditMode )
    {
/*        foreach( var sa in sas )
          if( sa.gameObject.scene.isLoaded )
          {
          if( !sa.UseSpriteRenderer )
            sa.Play( sa.CurrentSequence );
          }

*/
    }
  }
}

#endif