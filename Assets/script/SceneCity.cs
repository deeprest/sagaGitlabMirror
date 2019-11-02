using UnityEngine;
using System.Collections;

public class SceneCity : SceneScript
{

  public override void StartScene()
  {
    if( Application.isEditor && !Global.instance.SimulatePlayer && Global.instance.CurrentPlayer == null )
    {
      //Global.instance.SpawnPlayer();
      return;
    }

    Global.instance.ChopDrop();
    //Global.instance.ready.SetActive( true );
    Global.instance.Controls.BipedActions.Disable();
    Global.instance.Controls.BipedActions.Aim.Enable();
    Global.instance.Controls.BipedActions.Fire.Enable();
    new Timer( 5, delegate
    {
      Global.instance.CurrentPlayer.inputRight = true;
    }, delegate
    {
      Global.instance.Controls.BipedActions.Enable();
    } );
  }

}


