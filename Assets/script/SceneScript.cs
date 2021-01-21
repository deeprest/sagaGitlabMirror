using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;


public class SceneScript : MonoBehaviour
{
  public AudioLoop music;
  public CameraZone CameraZone;
  public BoxCollider NavmeshBox;
  public Light2D ambientLight;

  public virtual void StartScene()
  {
    if( Application.isEditor && !Global.instance.SimulatePlayer )
    {
      if( Global.instance.CurrentPlayer == null )
        Global.instance.SpawnPlayer();
      else
      {
        Global.instance.CurrentPlayer.transform.position = Global.instance.FindRandomSpawnPosition();
        Global.instance.CurrentPlayer.velocity = Vector2.zero;
      }
    }

    Global.instance.AssignCameraZone( CameraZone );

    if( music != null )
      Global.instance.PlayMusic( music );

    if( ambientLight != null )
    {
      float targetIntensity = ambientLight.intensity;
      new Timer( 3, delegate( Timer timer ) { ambientLight.intensity = timer.ProgressNormalized * targetIntensity; }, null );
    }
  }

  public void PlayerInputOff()
  {
    Global.instance.Controls.BipedActions.Disable();
  }

  public void CameraZoom( float value )
  {
    Global.instance.CameraController.orthoTarget = value;
  }

  public void AssignCameraZone( CameraZone  zone )
  {
    Global.instance.AssignCameraZone( zone );
  }

  /*Timer runTimer = new Timer();
  public void RunLeft( float duration )
  {
    Global.instance.Controls.BipedActions.Disable();
    TimerParams tp = new TimerParams
    {
      unscaledTime = true,
      repeat = false,
      duration = duration,
      UpdateDelegate = delegate ( Timer t )
      {
        Global.instance.CurrentPlayer.inputLeft = true;
      },
      CompleteDelegate = delegate
      {
        Global.instance.Controls.BipedActions.Enable();
      }
    };
    runTimer.Start( tp );
  }

  public void RunRight( float duration )
  {
    Global.instance.Controls.BipedActions.Disable();
    TimerParams tp = new TimerParams
    {
      unscaledTime = true,
      repeat = false,
      duration = duration,
      UpdateDelegate = delegate ( Timer t )
      {
        Global.instance.CurrentPlayer.inputRight = true;
      },
      CompleteDelegate = delegate
      {
        Global.instance.Controls.BipedActions.Enable();
      }
    };
    runTimer.Start( tp );
  }*/

  

}