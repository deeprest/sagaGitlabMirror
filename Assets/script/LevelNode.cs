using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNode : MonoBehaviour
{
  public int openBitmask;
  public int yindex;
  [SerializeField] TextMesh label;

  public void Initialize()
  {
    if( label != null )
      label.text = name;
  }
} 

