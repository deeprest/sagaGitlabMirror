using UnityEngine;
using System.Collections;

public class SceneHome : SceneScript
{
  public override void StartScene()
  {
    base.StartScene();
    // for return from other level
    if( Global.instance.CurrentPlayer != null )
      Global.instance.CurrentPlayer.transform.position = Global.instance.FindSpawnPosition();
  }

}
