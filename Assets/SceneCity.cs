using UnityEngine;
using System.Collections;

public class SceneScript : MonoBehaviour
{
  public virtual void StartScene() { }
}

public class SceneCity : SceneScript
{
  public override void StartScene()
  {
    if( Application.isEditor && !Global.instance.SimulatePlayer )
      return;

    Global.instance.ChopDrop();
    Global.instance.ready.SetActive( true );

    Global.instance.CurrentPlayer.playerInput = false;
    new Timer( 5, delegate
    {
      Global.instance.CurrentPlayer.inputRight = true;
    }, delegate
    {
      Global.instance.CurrentPlayer.playerInput = true;
    } );
  }

}


