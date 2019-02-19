using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof( AnimSequence ) )]
public class AnimSequenceEditor : Editor
{
  public override void OnInspectorGUI()
  {
    AnimSequence seq = target as AnimSequence;
    if( GUI.Button( EditorGUILayout.GetControlRect(), "Sprites to Frames" ) )
      AnimSequence.SpritesToFrames( seq );
    if( GUI.Button( EditorGUILayout.GetControlRect(), "Mark Keyframes" ) )
    {
      AnimFrame lastGood = seq.frames[0];
      lastGood.keyFrame = null;
      for( int i = 1; i < seq.frames.Length; i++ )
      {
        if( seq.frames[i].point.Count == 0 )
        {
          seq.frames[i].keyFrame = lastGood;
        }
        else
        {
          lastGood = seq.frames[i];
          lastGood.keyFrame = null;
        }
      }
    }

    DrawDefaultInspector();
  }
}
#endif

[System.Serializable]
public class AnimFramePoint
{
  public string name;
  public Vector2 point;
}

[System.Serializable]
public class AnimFrame
{
  // if this reference is null, then this is a keyframe
  public AnimFrame keyFrame;
  public Sprite sprite;
  public List<AnimFramePoint> point;
}

[CreateAssetMenu]
public class AnimSequence : ScriptableObject
{
  //public string ID;
  public bool UseFrames = false;
  public int fps = 20;
  public bool loop = true;
  public int loopStartIndex = 0;
  public Sprite[] sprites;

  public AnimFrame[] frames;

  public float GetDuration()
  {
    if( UseFrames )
      return (1.0f / fps) * frames.Length;
    else
      return (1.0f / fps) * sprites.Length;
  }

  public static void SpritesToFrames( AnimSequence seq )
  {
    seq.frames = new AnimFrame[seq.sprites.Length];
    for( int i = 0; i < seq.sprites.Length; i++ )
    {
      seq.frames[i] = new AnimFrame();
      seq.frames[i].sprite = seq.sprites[i];
    }
  }

  public AnimFrame GetKeyFrame( int index )
  {
    if( frames[index].keyFrame != null )
      return frames[index].keyFrame;
    return frames[index];
  }

  public void DeleteFrame( int index )
  {
    frames[index].point.Clear();
    int previous = Mathf.Max( 0, index - 1 );
    if( frames[previous].keyFrame == null )
      frames[index].keyFrame = frames[previous];
    else
      frames[index].keyFrame = frames[previous].keyFrame;
  }
}
