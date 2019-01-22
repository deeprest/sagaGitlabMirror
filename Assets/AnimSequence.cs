using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimPoint
{
  public string name;
  public Vector2Int pos;
}

[System.Serializable]
public class AnimFrame
{
  public Sprite sprite;
  public AnimPoint[] point;
}

[CreateAssetMenu]
public class AnimSequence : ScriptableObject
{
  public int fps = 20;
  public bool loop = true;
  public int loopStartIndex = 0;
  public Sprite[] sprites;

  public AnimFrame[] frames;
}
