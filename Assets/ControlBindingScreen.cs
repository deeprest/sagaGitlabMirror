using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControlBindingScreen : MonoBehaviour
{
  [SerializeField] Transform parent;
  [SerializeField] GameObject template;
  Controls c;
  Timer updateTextTimer = new Timer();
  // action, text
  Dictionary<System.Action<InputAction.CallbackContext>, ControlCounter> map = new Dictionary<System.Action<InputAction.CallbackContext>, ControlCounter>();
  class ControlCounter
  {
    public ControlCounter( string txt, InputAction atn, System.Action<InputAction.CallbackContext> cc )
    {
      text = txt;
      count = 3;
      action = atn;
      iacc = cc;
      action.performed += iacc;
    }
    public string text;
    public int count = 5;
    public InputAction action;
    public System.Action<InputAction.CallbackContext> iacc;
  }
  List<System.Action<InputAction.CallbackContext>> Removal = new List<System.Action<InputAction.CallbackContext>>();

  void UpdateText()
  {
    for( int i = 0; i < parent.childCount; i++ )
      Destroy( parent.GetChild( i ).gameObject );
    foreach( var pair in map )
    {
      if( pair.Value.count <= 0 )
      {
        Removal.Add( pair.Key );
      }
      else
      {
        GameObject go = Instantiate( template, parent );
        Text txt = go.GetComponentInChildren<Text>();
        txt.text = Global.instance.ReplaceWithControlNames( pair.Value.text );
        txt.color = Color.Lerp( Color.white, Color.grey, 1.0f / pair.Value.count );
      }
    }
    foreach( var obj in Removal )
    {
      map[obj].action.performed -= obj;
      map.Remove( obj );
    }
    Removal.Clear();
  }


  void Start()
  {
    c = Global.instance.Controls;
    // snippet dc1
    map.Add( Move, new ControlCounter( "[Move] Move", c.BipedActions.Move, Move ) );
    map.Add( Menu, new ControlCounter( "[Menu] Menu", c.GlobalActions.Menu, Menu ) );
    map.Add( Fire, new ControlCounter( "[Fire] Shoot", c.BipedActions.Fire, Fire ) );
    map.Add( Jump, new ControlCounter( "[Jump] Jump", c.BipedActions.Jump, Jump ) );
    map.Add( Dash, new ControlCounter( "[Dash] Dash", c.BipedActions.Dash, Dash ) );
    map.Add( Shield, new ControlCounter( "[Shield] Shield", c.BipedActions.Shield, Shield ) );
    map.Add( Graphook, new ControlCounter( "[Graphook] Graphook", c.BipedActions.Graphook, Graphook ) );
    map.Add( NextWeapon, new ControlCounter( "[NextWeapon] NextWeapon", c.BipedActions.NextWeapon, NextWeapon ) );
    map.Add( Charge, new ControlCounter( "[Charge] Charge", c.BipedActions.Charge, Charge ) );
    map.Add( WorldSelect, new ControlCounter( "[WorldSelect] WorldSelect", c.BipedActions.WorldSelect, WorldSelect ) );
    UpdateText();
    updateTextTimer.Start( int.MaxValue, 1, delegate ( Timer obj ) { UpdateText(); }, null );
  }

  // snippet dc2
  void Menu( InputAction.CallbackContext obj ) { map[Menu].count--; UpdateText(); }
  void Move( InputAction.CallbackContext obj ) { map[Move].count--; UpdateText(); }
  void Fire( InputAction.CallbackContext obj ) { map[Fire].count--; UpdateText(); }
  void Jump( InputAction.CallbackContext obj ) { map[Jump].count--; UpdateText(); }
  void Dash( InputAction.CallbackContext obj ) { map[Dash].count--; UpdateText(); }
  void Shield( InputAction.CallbackContext obj ) { map[Shield].count--; UpdateText(); }
  void Graphook( InputAction.CallbackContext obj ) { map[Graphook].count--; UpdateText(); }
  void NextWeapon( InputAction.CallbackContext obj ) { map[NextWeapon].count--; UpdateText(); }
  void Charge( InputAction.CallbackContext obj ) { map[Charge].count--; UpdateText(); }
  void WorldSelect( InputAction.CallbackContext obj ) { map[WorldSelect].count--; UpdateText(); }

}
