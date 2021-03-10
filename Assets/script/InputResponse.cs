using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputResponse : MonoBehaviour, Controls.IBipedActionsActions
{
  [SerializeField] WorldText nameTxt;
  [SerializeField] Animator[] anim;
  Dictionary<string, int> lookup = new Dictionary<string, int>();


  void Start()
  {
    Enable();
  }

  void OnDestroy()
  {
    Disable();
  }

  void Enable()
  {
    Global.instance.Controls.BipedActions.SetCallbacks( this );
    int i = 0;
    foreach( var animator in anim )
      lookup.Add( animator.name, i++ );
  }

  void Disable()
  {
    Global.instance.Controls.BipedActions.SetCallbacks( null );
  }

  void DoTheThing( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
    nameTxt.text = context.action.name;
    nameTxt.ExplicitUpdate();
  }

  public void OnMoveRight( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }

  public void OnMoveLeft( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }

  public void OnMove( InputAction.CallbackContext context ) { }

  public void OnJump( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }

  public void OnDash( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }

  public void OnAim( InputAction.CallbackContext context ) { }

  public void OnFire( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }

  public void OnAbility( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }

  public void OnInteract( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }

  public void OnNextWeapon( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }

  public void OnNextAbility( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }

  public void OnCharge( InputAction.CallbackContext context ) { }

  public void OnDown( InputAction.CallbackContext context )
  {
    DoTheThing( context );
  }
}