using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIScreen : MonoBehaviour
{
  public static bool HasActiveScreen { get { return stack.Count > 0; } }
  static List<UIScreen> stack = new List<UIScreen>();
  public GameObject InitiallySelected;
  GameObject selectedObject;

  public virtual void Highlight() { }
  public virtual void Unhighlight() { }

  public virtual void Select()
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
      while( stack.Count> 0 )
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
    Selectable[] selectables = GetComponentsInChildren<Selectable>();
    foreach( var sel in selectables )
      sel.interactable = true;
    EventSystem.current.SetSelectedGameObject( selectedObject );
  }

  public virtual void InteractableOff()
  {
    // warning: get the currentSelectedGameObject before setting interactable to false!
    selectedObject = EventSystem.current.currentSelectedGameObject;
    if( selectedObject == null || !selectedObject.transform.IsChildOf( transform ) )
      selectedObject = InitiallySelected;
    Selectable[] selectables = GetComponentsInChildren<Selectable>();
    foreach( var sel in selectables )
      sel.interactable = false;
  }
}