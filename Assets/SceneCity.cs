using UnityEngine;
using System.Collections;

public class SceneCity : MonoBehaviour
{
  void Start()
  {
    //if( Application.isEditor && !Global.instance.SimulatePlayer )
      //return;
    //Global.instance.SpawnPlayer();
    Global.instance.ChopDrop();
    Global.instance.ready.SetActive( true );
  }

}
