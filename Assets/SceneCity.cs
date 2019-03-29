using UnityEngine;
using System.Collections;

public class SceneCity : MonoBehaviour
{
  // Use this for initialization
  void Start()
  {
    if( !Application.isEditor || Global.instance.SimulatePlayer )
    {
      Global.instance.SpawnPlayer();
      Global.instance.ChopDrop();
    }
  }

}
