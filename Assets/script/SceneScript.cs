using UnityEngine;
using System.Collections;


public class SceneScript : MonoBehaviour
{
  // temp
  public Level level;

  public PolygonCollider2D sb;
  public virtual void StartScene() { }

  public void PlayerInputOff()
  {
    Global.instance.Controls.BipedActions.Disable();
  }

  public void ReplaceCameraPoly( Collider2D collider )
  {
    Global.instance.AssignCameraPoly( collider );
    Global.instance.CameraController.EncompassBounds = false;
  }

  public void ReplaceCameraPolyEncompass( Collider2D collider )
  {
    Global.instance.AssignCameraPoly( collider );
    Global.instance.CameraController.EncompassBounds = true;
  }

  Timer runTimer = new Timer();
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
  }

}