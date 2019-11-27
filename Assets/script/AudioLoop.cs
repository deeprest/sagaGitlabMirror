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
  public AudioClip intro;
  public AudioClip loop;
  public float introDelay = 0.2f;

  public void Play( AudioSource introSource, AudioSource source )
  {
    introSource.playOnAwake = false;
    introSource.priority = source.priority;
    introSource.volume = source.volume;

    introSource.loop = false;
    introSource.clip = intro;
    introSource.PlayScheduled( AudioSettings.dspTime + introDelay );

    source.loop = true;
    source.clip = loop;
    source.PlayScheduled( AudioSettings.dspTime + introDelay + intro.length );
  }
}