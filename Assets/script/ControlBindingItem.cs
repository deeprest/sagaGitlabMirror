using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControlBindingItem : MonoBehaviour
{
  public InputAction action;
  public Text txtAction;
  public Text txtControl;
  public Button button;

  public void OnComplete( InputActionRebindingExtensions.RebindingOperation operation )
  {
    txtControl.text = InputControlPath.ToHumanReadableString( operation.selectedControl.path );
  }
}
