using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour
{
  [SerializeField] Text message;
  [SerializeField] bool Colorize = true;
  string initialText;

  private void Awake()
  {
    initialText = message.text;
  }

  void Start()
  {
    message.text = Global.instance.ReplaceWithControlNames( initialText, Colorize );
  }
}
