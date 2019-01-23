using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct AnimFrame
{
  public Sprite sprite;

  public string[] mountName;
  public Vector2[] point;
}

[CreateAssetMenu]
public class AnimSequence : ScriptableObject
{
  public int fps = 20;
  public bool loop = true;
  public int loopStartIndex = 0;
  public Sprite[] sprites;

  public bool UseFrames = false;
  public AnimFrame[] frames;

  public float GetDuration()
  {
    if( UseFrames )
      return ( 1.0f / fps ) * frames.Length;
    else
      return ( 1.0f / fps ) * sprites.Length;
  }
}
