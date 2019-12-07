using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ControlBindingScreen : MonoBehaviour
{
  [SerializeField] Transform parent;
  [SerializeField] GameObject template;
  Controls c;
  List<ControlBindingItem> list = new List<ControlBindingItem>();
  ControlBindingItem rebinding;

  void CreateItem( InputAction action )
  {
    GameObject go = Instantiate( template, parent );
    ControlBindingItem cbi = go.GetComponentInChildren<ControlBindingItem>();
    cbi.action = action;
    cbi.txtAction.text = action.name;
    go.GetComponent<Button>().onClick.AddListener( () => StartRebind( cbi ) );
    list.Add( cbi );
  }

  void Start()
  {
    IEnumerator<InputAction> enumerator = Global.instance.Controls.GetEnumerator();
    while( enumerator.MoveNext() )
    {
      if( !Global.instance.Controls.MenuActions.Get().Contains( enumerator.Current ) )
        CreateItem( enumerator.Current );
    }
  }

  private void Update()
  {
    foreach( var item in list )
    {
      item.txtAction.text = item.action.name;
      item.txtControl.text = Global.instance.ReplaceWithControlNames( "[" + item.action.name + "]" );
    }
  }

  public void StartRebind( ControlBindingItem cbi )
  {
    cbi.button.interactable = false;
    InputActionRebindingExtensions.PerformInteractiveRebinding( cbi.action )
      //.OnCancel( OnCancel )
      .OnComplete( (x)=> {
        cbi.OnComplete( x );
        cbi.button.interactable = true;
        EventSystem.current.SetSelectedGameObject( cbi.button.gameObject );
      } )
      .OnMatchWaitForAnother( 0.1f )
      .Start();

  }

}
