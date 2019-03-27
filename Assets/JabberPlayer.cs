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


//[ExecuteInEditMode]
public class JabberPlayer : MonoBehaviour
{
  public AudioSource audioSource;
  public Jabber jabber;
  public string pattern = "this is the text that determines what sounds are made";
  public bool Loop = false;

  [Header("Debug")]
  public int index;
  public bool isPlaying = false;
  public PhonemeClip phoClip;

  float stamp;
  float wait;
  float pitchsine;
  float initialPitch;

  public void Play( string input )
  {
    initialPitch = audioSource.pitch;
    index = 0;
    pattern = input;
    audioSource.Stop();
    PlayNext();
  }

  public void Stop()
  {
    audioSource.pitch = initialPitch;
    audioSource.Stop();
    isPlaying = false;
  }

  void PlayNext()
  {
    if( index > pattern.Length - 1 )
    {
      if( Loop )
      {
        index = 0;
      }
      else
      {
        Stop();
        return;
      }
    }
    isPlaying = true;
    string letter = pattern[ index ].ToString().ToLower();

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
        pho = jabber.Phonemes[ Random.Range( 0, jabber.Phonemes.Count ) ];
      else
        pho = jabber.Phonemes.Find( x => x.PatternLetters.Contains( letter ) );
      if( pho == null )
        pho = jabber.Phonemes.Find( x => x.isDefault == true );
      if( pho == null )
      {
        Debug.Log( "Must have one phoneme marked as the default" );
        return;
      }
        
      phoClip = pho.phoclips[ Random.Range( 0, pho.phoclips.Count ) ];
      audioSource.clip = phoClip.clip;

      if( phoClip.clip!=null && phoClip.UseOffset && !phoClip.UseClipLength )
      {
        phoClip.Offset = Mathf.Max( 0, Mathf.Min( phoClip.Offset, phoClip.clip.length - jabber.PhonemeLength ) );
        audioSource.time = phoClip.Offset;
      }
      else
      {
        audioSource.time = 0;
      }

      if( jabber.PitchSine )
      {
        pitchsine += ( Time.time - stamp ) * jabber.PitchSinSpeed;
        audioSource.pitch = initialPitch + jabber.PitchRange * Mathf.Sin( pitchsine );
      }
      else
      if( jabber.PitchRandom )
      {
        audioSource.pitch = initialPitch + jabber.PitchRange * ( Random.value * 2 - 1 );
      }
      else
      {
        audioSource.pitch = initialPitch;
      }

      audioSource.Play();

      if( phoClip.UseClipLength )
        wait = phoClip.clip.length;
      else
        wait = jabber.PhonemeLength;

      // exclude punctuation
      int idx = pattern.IndexOfAny( new char[]{ ' ', '.', ',','!','?' }, index );
      if( idx == -1 || idx >= index + jabber.LettersPerPhoneme )
        index += jabber.LettersPerPhoneme;
      else
        index = idx+1;
    }
    stamp = Time.time;
  }


    
  void Update()
  {
    if( isPlaying )
    {
      if( Time.time - stamp >= wait )
        PlayNext();
    }
  }
}
