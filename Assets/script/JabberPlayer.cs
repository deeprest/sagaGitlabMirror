using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof(JabberPlayer) )]
public class JabberPlayerEditor : Editor
{
  public override void OnInspectorGUI()
  {
    JabberPlayer jp = target as JabberPlayer;
    if( jp.isPlaying )
    {
      if( GUI.Button( EditorGUILayout.GetControlRect(), "Stop" ) )
        jp.Stop();
    }
    else
    {
      if( GUI.Button( EditorGUILayout.GetControlRect(), "Play" ) )
        jp.Play( jp.pattern );
    }
    DrawDefaultInspector();
  }
}
#endif


[ExecuteInEditMode]
public class JabberPlayer : MonoBehaviour
{
  public AudioSource audioSource;
  public Jabber jabber;
  public string pattern = "this is the text that determines what sounds are made";
#if UNITY_EDITOR
  public bool Loop = false;
#endif

  [Header( "Debug" )]
  public int index;
  public bool isPlaying = false;
  public PhonemeClip phoClip;

  float stamp;
  float wait;
  float pitchsine;

#if UNITY_EDITOR
  float time
  {
    get
    {
      if( Application.isEditor && !Application.isPlaying ) return Time.realtimeSinceStartup;
      else return Time.time;
    }
  }
#else
  float time { get { return Time.time; } }
#endif

  public void Play( string input )
  {
    index = 0;
    pattern = input;
    audioSource.Stop();
    PlayNext();
  }

  public void Stop()
  {
    audioSource.pitch = 1;
    audioSource.Stop();
    isPlaying = false;
  }

  void PlayNext()
  {
    if( index > pattern.Length - 1 )
    {
#if UNITY_EDITOR
      if( Application.isPlaying )
      {
        Stop();
        return;
      }
      else if( Loop )
      {
        index = 0;
      }
      else
      {
        Stop();
        return;
      }

#else
      Stop();
      return;
#endif
    }
    isPlaying = true;
    string letter = pattern[index].ToString().ToLower();

    if( letter == " " )
    {
      audioSource.Stop();
      wait = jabber.SilenceLength;
      index++;
    }
    else
    {
      Phoneme pho = null;
      if( jabber.RandomPhoneme )
        pho = jabber.Phonemes[Random.Range( 0, jabber.Phonemes.Count )];
      else
        pho = jabber.Phonemes.Find( x => x.PatternLetters.Contains( letter ) );
      if( pho == null )
        pho = jabber.Phonemes.Find( x => x.isDefault == true );
      if( pho == null )
      {
        Debug.LogWarning( "Must have one phoneme marked as the default" );
        return;
      }

      phoClip = pho.phoclips[Random.Range( 0, pho.phoclips.Count )];
      audioSource.clip = phoClip.clip;

      if( phoClip.clip != null && phoClip.UseOffset && !phoClip.UseClipLength )
      {
        phoClip.Offset = Mathf.Max( 0, Mathf.Min( phoClip.Offset, phoClip.clip.length - jabber.PhonemeLength ) );
        audioSource.time = phoClip.Offset;
      }
      else
      {
        audioSource.time = 0;
      }

      if( jabber.EnablePitchVariation )
      {
        if( jabber.PitchSine )
        {
          pitchsine += (time - stamp) * jabber.PitchSinSpeed;
          audioSource.pitch = 1 + jabber.PitchRange * Mathf.Sin( pitchsine );
        }
        else if( jabber.PitchRandom )
        {
          audioSource.pitch = 1 + jabber.PitchOffset + jabber.PitchRange * (Random.value * 2 - 1);
        }
        else
        {
          audioSource.pitch = 1 + jabber.PitchOffset;
        }
      }

      audioSource.Play();

      if( phoClip.UseClipLength )
        wait = phoClip.clip.length;
      else
        wait = jabber.PhonemeLength;

      // exclude punctuation
      int idx = pattern.IndexOfAny( new char[] {' ', '.', ',', '!', '?'}, index );
      if( idx == -1 || idx >= index + jabber.LettersPerPhoneme )
        index += jabber.LettersPerPhoneme;
      else
        index = idx + 1;
    }
    stamp = time;
  }


  void Update()
  {
    if( isPlaying )
    {
      if( time - stamp >= wait )
        PlayNext();
    }
  }


  private void OnValidate()
  {
    Stop();
  }

  // DidReloadScripts
  /*
#if UNITY_EDITOR
  private void OnDestroy()
  {
    // when using ExecuteInEditMode, need to stop manually
    Stop();
  }
#endif

#if UNITY_EDITOR
  private void Awake()
  {
    EditorApplication.playModeStateChanged += EditorApplication_PlayModeStateChanged;
  }

  void EditorApplication_PlayModeStateChanged( PlayModeStateChange obj )
  {
    if( obj == PlayModeStateChange.ExitingEditMode )
    {
      Debug.Log( "exiting edit mode." );
    }
    if( obj == PlayModeStateChange.ExitingPlayMode )
    {
      Stop();
      Debug.Log( "exiting play mode" );
    }
  }
#endif
*/
}