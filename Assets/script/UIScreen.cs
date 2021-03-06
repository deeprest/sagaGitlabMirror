﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIScreen : MonoBehaviour
{
  public static bool HasActiveScreen { get { return stack.Count > 0; } }
  static List<UIScreen> stack = new List<UIScreen>();
  // the object to select when this UIScreen is selected
  GameObject SelectedObject;
  public GameObject InitiallySelected;

  void Awake()
  {
    if( InitiallySelected == null )
      InitiallySelected = GetComponentInChildren<Selectable>().gameObject;
  }
  public void Back()
  {
    if( stack.Count > 1 )
      stack[stack.Count - 1].Unselect();
  }

  public virtual void Highlight() { }
  public virtual void Unhighlight() { }

  public void Select()
  {
    if( stack.Count > 0 )
      stack[stack.Count - 1].InteractableOff();
    stack.Add( this );
    gameObject.SetActive( true );
    InteractableOn();
  }

  public virtual void Unselect()
  {
    if( stack.Count > 0 )
    {
      UIScreen top = stack[stack.Count - 1];
      while( stack.Count > 0 )
      {
        if( top == this )
          break;
        top.Unselect();
      }
    }
    InteractableOff();
    gameObject.SetActive( false );
    stack.Remove( this );
    if( stack.Count > 0 )
      stack[stack.Count - 1].InteractableOn();
  }

  public virtual void InteractableOn()
  {
    if( SelectedObject == null )
      SelectedObject = InitiallySelected;
    Selectable[] selectables = GetComponentsInChildren<Selectable>();
    foreach( var sel in selectables )
      sel.interactable = true;
    EventSystem.current.SetSelectedGameObject( SelectedObject );
  }

  public virtual void InteractableOff()
  {
    // warning: get the currentSelectedGameObject before setting interactable to false!
    SelectedObject = EventSystem.current.currentSelectedGameObject;
    if( SelectedObject == null || !SelectedObject.transform.IsChildOf( transform ) )
      SelectedObject = InitiallySelected;
    Selectable[] selectables = GetComponentsInChildren<Selectable>();
    foreach( var sel in selectables )
      sel.interactable = false;
  }

}