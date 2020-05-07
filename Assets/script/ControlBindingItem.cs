using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControlBindingItem : MonoBehaviour
{
  public InputAction action;
  //public InputControl control;
  public Text txtAction;
  public Text txtControl;
  public Button button;


  public void OnComplete( InputActionRebindingExtensions.RebindingOperation operation )
  {
    Debug.Log( InputActionRebindingExtensions.GetBindingDisplayString( operation.action ) );
    //control = operation.selectedControl;
    txtControl.text = Global.instance.ReplaceWithControlNames( "[" + operation.action.name + "]" );
    //txtControl.text = InputControlPath.ToHumanReadableString( operation.selectedControl.path );
    //txtControl.text = operation.action.GetBindingDisplayString( InputBinding.DisplayStringOptions.DontUseShortDisplayNames );
  }
}
