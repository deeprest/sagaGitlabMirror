using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour
{
  [SerializeField] Text message;
  [SerializeField] bool Colorize = true;
  string initialText;

  private void OnDestroy()
  {
    Global.instance.OnGameControllerChanged -= UpdateText;
  }

  void UpdateText()
  {
     message.text = Global.instance.ReplaceWithControlNames( initialText, Colorize );
  }

  void Start()
  {
    initialText = message.text;
    Global.instance.OnGameControllerChanged += UpdateText;
    UpdateText();
  }
}
