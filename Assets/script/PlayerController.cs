using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu]
public class PlayerController : Controller
{
  public float DirectionalCursorDistance = 3;

  // init bindings to modify input struct
  public override void Awake()
  {
    base.Awake();
    BindControls();
  }

  void BindControls()
  {
    Global.instance.Controls.BipedActions.Fire.started += ( obj ) => input.Fire = true;
    Global.instance.Controls.BipedActions.Fire.canceled += ( obj ) => input.Fire = false;
    Global.instance.Controls.BipedActions.Jump.started += ( obj ) => input.JumpStart = true;
    Global.instance.Controls.BipedActions.Jump.canceled += ( obj ) => input.JumpEnd = true;
    Global.instance.Controls.BipedActions.Dash.started += ( obj ) => input.DashStart = true;
    Global.instance.Controls.BipedActions.Dash.canceled += ( obj ) => input.DashEnd = true;
    //Global.instance.Controls.BipedActions.Shield.started += ( obj ) => input.Shield = true;
    //Global.instance.Controls.BipedActions.Shield.canceled += ( obj ) => input.Shield = false;
    Global.instance.Controls.BipedActions.Graphook.performed += ( obj ) => input.Graphook = true;
    Global.instance.Controls.BipedActions.NextWeapon.performed += ( obj ) => input.NextWeapon = true;
    Global.instance.Controls.BipedActions.Charge.started += ( obj ) => input.ChargeStart = true;
    Global.instance.Controls.BipedActions.Charge.canceled += ( obj ) => input.ChargeEnd = true;
    Global.instance.Controls.BipedActions.Down.performed += ( obj ) => input.Down = true;
    Global.instance.Controls.BipedActions.Interact.performed += ( obj ) => { input.Interact = true; };

    Global.instance.Controls.BipedActions.Aim.performed += ( obj ) => { aimDeltaSinceLastFrame += obj.ReadValue<Vector2>(); };
  }


  // any persistent input variables
  Vector2 cursorDelta = Vector2.zero;
  bool fire;
  public Vector2 aimDeltaSinceLastFrame;

  // apply input to pawn
  public override void Update()
  {
#if UNITY_WEBGL && !UNITY_EDITOR
      Vector2 delta = Global.instance.Controls.BipedActions.Aim.ReadValue<Vector2>() * Global.instance.CursorSensitivity;
      delta.y = -delta.y;
      if( Global.instance.UsingGamepad )
        cursorDelta = delta * DirectionalCursorDistance;
      else
        cursorDelta += delta;
#else

    if( Global.instance.UsingGamepad )
    {
      cursorDelta = Global.instance.Controls.BipedActions.Aim.ReadValue<Vector2>() * DirectionalCursorDistance;
    }
    else
    {
      cursorDelta += aimDeltaSinceLastFrame * Global.instance.CursorSensitivity;// * Time.deltaTime;
      //cursorDelta += Global.instance.Controls.BipedActions.Aim.ReadValue<Vector2>() * Global.instance.CursorSensitivity;
      aimDeltaSinceLastFrame = Vector2.zero;
      cursorDelta = cursorDelta.normalized * Mathf.Max( Mathf.Min( cursorDelta.magnitude, Camera.main.orthographicSize * Camera.main.aspect * Global.instance.CursorOuter ), 0.1f );
    }
#endif
    input.Aim = cursorDelta;

    if( Global.instance.Controls.BipedActions.MoveRight.ReadValue<float>() > 0.5f )
      input.MoveRight = true;
    if( Global.instance.Controls.BipedActions.MoveLeft.ReadValue<float>() > 0.5f )
      input.MoveLeft = true;


    if( pawn != null )
      pawn.ApplyInput( input );
    input = default;
  }
}

