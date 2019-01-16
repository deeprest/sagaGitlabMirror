using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testbuttons : MonoBehaviour
{
  [SerializeField]
  Text t;
  // Update is called once per frame
  void Update()
  {
    t.text = "";
    for (int i = 0; i < 20; i++)
    {
      if( Input.GetKey("joystick button " + i) )
      t.text += "Button " + i + "=" + Input.GetKey("joystick button " + i) + "| ";
    }
  }
}