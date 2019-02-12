using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof( AudioLoop ) )]
public class AudioLoopEditor : Editor
{
  public override void OnInspectorGUI()
  {
    AudioLoop al = target as AudioLoop;
    AudioClip clip = al.clip;
    DrawDefaultInspector();
    if( clip != al.clip )
      al.loopFadeOut = al.clip.length;
    al.loopStart = Mathf.Clamp( al.loopStart, 0, al.clip.length );
    al.loopFadeOut = Mathf.Clamp( al.loopFadeOut, 0, al.clip.length );

  }
}
#endif
*/

[CreateAssetMenu]
public class AudioLoop : ScriptableObject
{
  public AudioClip clip;
  public float loopStart;
  public float loopFadeOut;
}