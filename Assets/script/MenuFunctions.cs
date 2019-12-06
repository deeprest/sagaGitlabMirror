using UnityEngine;
using System.Collections;

public class MenuFunctions : MonoBehaviour
{
  public void StopMusic()
  {
    Global.instance.StopMusic();
  }

  public void PlayMusic( AudioLoop al )
  {
    Global.instance.PlayMusic( al );
  }

}
