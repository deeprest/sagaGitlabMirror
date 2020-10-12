using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// gibberish like in these games: Okami, Undertale, Animal Crossing

// per-clip offset
[System.Serializable]
public class PhonemeClip
{
  public bool UseClipLength = false;
  public bool UseOffset = true;
  public float Offset;
  public AudioClip clip;
}

[System.Serializable]
public class Phoneme
{
  public string PatternLetters;
  public bool isDefault;
  public List<PhonemeClip> phoclips;
}
  
[CreateAssetMenu()]
public class Jabber : ScriptableObject
{
  public int LettersPerPhoneme = 3;
  public float PhonemeLength = 0.1f;
  public float SilenceLength = 0.1f;

  public bool EnablePitchVariation = false;
  public float PitchOffset = 0;
  public bool PitchSine = false;
  public float PitchRange = 0.1f;
  public float PitchSinSpeed = 20;
  public bool PitchRandom = true;

  public bool RandomPhoneme = false;
  public List<Phoneme> Phonemes;
}
