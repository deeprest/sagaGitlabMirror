using UnityEngine;
using System.Collections;

public class SceneHome : SceneScript
{
  public override void StartScene()
  {
    if( Application.isEditor && !Global.instance.SimulatePlayer && Global.instance.CurrentPlayer == null )
    {
      Global.instance.SpawnPlayer();
      return;
    }

    // for return from other level
    if( Global.instance.CurrentPlayer != null )
      Global.instance.CurrentPlayer.transform.position = Global.instance.FindSpawnPosition();
  }

}
