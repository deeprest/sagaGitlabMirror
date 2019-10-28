using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DiageticUI : WorldSelectable
{
  [SerializeField] GraphicRaycaster raycaster;
  [SerializeField] Animator animator;
  [SerializeField] GameObject indicator;
  [SerializeField] GameObject cameraTarget;
  public PolygonCollider2D CameraPoly { get { return cameraTarget.GetComponent<PolygonCollider2D>(); } }
  public GameObject InitiallySelected;
  GameObject selectedObject;

  void Start()
  {
    animator.Play( "idle" );
    raycaster.enabled = false;
    InteractableOff();
    selectedObject = InitiallySelected;
  }

  public override void Highlight()
  {
    animator.Play( "highlight" );
  }
  public override void Unhighlight()
  {
    animator.Play( "idle" );
  }

  public override void Select()
  {
    animator.Play( "idle" );
    indicator.SetActive( false );
    raycaster.enabled = true;
    Global.instance.DiageticMenuOn( this );
    InteractableOn();
  }

  public override void Unselect()
  {
    animator.Play( "idle" );
    indicator.SetActive( true );
    raycaster.enabled = false;
    Global.instance.DiageticMenuOff();
    InteractableOff();
  }

  public void InteractableOn()
  {
    Selectable[] selectables = GetComponentsInChildren<Selectable>();
    foreach( var sel in selectables )
      sel.interactable = true;
    EventSystem.current.SetSelectedGameObject( selectedObject );
  }

  public void InteractableOff()
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
