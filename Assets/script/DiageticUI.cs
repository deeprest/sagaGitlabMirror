using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class DiageticUI : WorldSelectable
{
  [SerializeField] GraphicRaycaster raycaster;
  [SerializeField] Animator animator;
  [SerializeField] Text instruction;
  [SerializeField] string inactiveInstruction;
  [SerializeField] string activeInstruction;
  [SerializeField] GameObject cameraTarget;
  public CameraZone CameraZone { get { return cameraTarget.GetComponent<CameraZone>(); } }
  public GameObject InitiallySelected;
  GameObject selectedObject;

  void Start()
  {
    animator.Play( "idle" );
    raycaster.enabled = false;
    InteractableOff();
    selectedObject = InitiallySelected;
    instruction.gameObject.SetActive( false );
  }

  public override void Highlight()
  {
    animator.Play( "highlight" );
    instruction.gameObject.SetActive( true );
    instruction.text = Global.instance.ReplaceWithControlNames( inactiveInstruction );
  }
  public override void Unhighlight()
  {
    animator.Play( "idle" );
    instruction.gameObject.SetActive( false );
  }

  public override void Select()
  {
    animator.Play( "idle" );
    instruction.text = Global.instance.ReplaceWithControlNames( activeInstruction );
    raycaster.enabled = true;
    Global.instance.DiegeticMenuOn( this );
    InteractableOn();
  }

  public override void Unselect()
  {
    animator.Play( "idle" );
    instruction.text = Global.instance.ReplaceWithControlNames( inactiveInstruction );
    raycaster.enabled = false;
    Global.instance.DiegeticMenuOff();
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
