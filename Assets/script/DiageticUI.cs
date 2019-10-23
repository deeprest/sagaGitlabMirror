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
    raycaster.enabled = true;
    //Cursor.lockState = CursorLockMode.None;
    //Cursor.visible = true;
    Global.instance.DiageticMenuOn( cameraTarget.GetComponent<PolygonCollider2D>(), InitiallySelected  );
  }

  public override void Unselect()
  {
    animator.Play( "idle" );
    indicator.SetActive( true );
    raycaster.enabled = false;
    //Cursor.lockState = CursorLockMode.Locked;
    //Cursor.visible = false;
    Global.instance.DiageticMenuOff();
  }

  void Start()
  {
    animator.Play( "idle" );
  }

}
