using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNode : MonoBehaviour
{
  public int openBitmask;
  public int yindex;

  public void Initialize()
  {
    TextMesh label = GetComponent<TextMesh>();
    if( label != null )
      label.text = name;
  }
} 

