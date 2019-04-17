using UnityEngine;
using System.Collections;

public class SceneCity : MonoBehaviour
{
  void Start()
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
