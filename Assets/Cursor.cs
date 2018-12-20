using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {

  public RectTransform rt;

  void Update () 
  {
    rt.anchoredPosition = new Vector2( Input.mousePosition.x, Input.mousePosition.y ); 
	}
}
