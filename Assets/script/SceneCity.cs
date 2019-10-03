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
    new Timer( 5, delegate
    {
      Global.instance.Controls.BipedActions.Enable();
    }, delegate
    {
      Global.instance.Controls.BipedActions.Enable();
    } );
  }

}


