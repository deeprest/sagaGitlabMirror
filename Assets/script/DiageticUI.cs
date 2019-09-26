using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiageticUI : WorldSelectable
{
  [SerializeField] GraphicRaycaster raycaster;
  [SerializeField] GameObject InitiallySelected;
  [SerializeField] Animator animator;
  [SerializeField] GameObject indicator;
  [SerializeField] GameObject cameraTarget;

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
    //Global.instance.CameraController.LerpTo( cameraTarget );
    Global.instance.AssignCameraPoly( cameraTarget.GetComponent<PolygonCollider2D>() );
    Global.instance.CameraController.EncompassBounds = true;

    Global.instance.UI.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject( InitiallySelected );
    Global.instance.EnableRaycaster( false );
    raycaster.enabled = true;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  public override void Unselect()
  {
    animator.Play( "idle" );
    indicator.SetActive( true );
    SceneScript ss = Global.instance.GetSceneScript();
    if( ss != null )
      Global.instance.AssignCameraPoly( ss.sb );
    Global.instance.CameraController.EncompassBounds = false;

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    // avoid selecting things after we've left the menu
    Global.instance.UI.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject( null );
    Global.instance.EnableRaycaster( true );
    raycaster.enabled = false;
  }

  void Start()
  {
    animator.Play( "idle" );
  }






}
