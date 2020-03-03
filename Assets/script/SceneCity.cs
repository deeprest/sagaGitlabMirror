using UnityEngine;
using System.Collections;

public class SceneCity : SceneScript
{
  [SerializeField] Chopper chopper;
  [SerializeField] float runRightDuration = 3;

  public override void StartScene()
  {
    base.StartScene();

    chopper.StartDrop( Global.instance.CurrentPlayer );
    //Global.instance.ready.SetActive( true );
    Global.instance.Controls.BipedActions.Disable();
    Global.instance.Controls.BipedActions.Aim.Enable();
    Global.instance.Controls.BipedActions.Fire.Enable();
    new Timer( runRightDuration, delegate
    {
      Global.instance.CurrentPlayer.inputRight = true;
    }, delegate
    {
      Global.instance.Controls.BipedActions.Enable();
    } );
  }

}


