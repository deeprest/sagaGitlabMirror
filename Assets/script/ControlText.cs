using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlText : MonoBehaviour
{
  [SerializeField] Text[] text;

  void Start()
  {
    foreach( var txt in text )
      txt.text = Global.instance.ReplaceWithControlNames( txt.text );
  }


}
