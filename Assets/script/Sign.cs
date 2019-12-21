using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour
{
  [SerializeField] Text message;
  string initialText;

  private void Awake()
  {
    initialText = message.text;
  }

  void Update()
  {
    message.text = Global.instance.ReplaceWithControlNames( initialText );
  }
}
