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
  //public AnimFrame keyFrame;
  public bool isKeyframe { get { return point.Count > 0; } }
  public Sprite sprite;
  public List<AnimFramePoint> point = new List<AnimFramePoint>();
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
    int i = index;
    while( i > 0 )
    {
      if( frames[i].isKeyframe )
        break;
      i--;
    }
    return frames[i];
  }

  public void DeleteFrame( int index )
  {
    frames[index].point.Clear();
  }
  public void NewFrame()
  {
    ArrayUtility.Add<AnimFrame>( ref frames, new AnimFrame() );
  }
}
