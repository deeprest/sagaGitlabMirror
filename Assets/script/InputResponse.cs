using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputResponse : MonoBehaviour, Controls.IBipedActionsActions
{
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

  public void OnMoveRight( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnMoveLeft( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnMove( InputAction.CallbackContext context )
  {
    // if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    // if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnJump( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnDash( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnAim( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnFire( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnAbility( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnInteract( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnNextWeapon( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnNextAbility( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnCharge( InputAction.CallbackContext context )
  {
    // if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    // if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }

  public void OnDown( InputAction.CallbackContext context )
  {
    if( context.started ) anim[lookup[context.action.name]].Play( "on" );
    if( context.canceled ) anim[lookup[context.action.name]].Play( "off" );
  }
}