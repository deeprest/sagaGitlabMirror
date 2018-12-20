using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AnimSequence : ScriptableObject
{
  //new public string name;
  public int fps = 16;
  public bool holdLastFrame = false;
  public int loopStartIndex = 0;
  public Sprite[] sprites;
}
