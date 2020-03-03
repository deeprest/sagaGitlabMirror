using UnityEngine;
using System.Collections;


public class SceneScript : MonoBehaviour
{
  public AudioLoop music;
  // generation
  public bool GenerateLevelOnStart = true;
  public Level level;
  // the camera collider
  public Collider2D sb;
  public BoxCollider NavmeshBox;

  public virtual void StartScene() 
  {
    if( Application.isEditor && !Global.instance.SimulatePlayer && Global.instance.CurrentPlayer == null )
    {
      Global.instance.SpawnPlayer();
      //return;
    }
    // level generation
    if( GenerateLevelOnStart && level != null )
      level.Generate();

    Global.instance.AssignCameraPoly( sb );

    // music fade in
    if( music != null )
      Global.instance.PlayMusic( music );
      
  }

  public void PlayerInputOff()
  {
    Global.instance.Controls.BipedActions.Disable();
  }

  public void CameraZoom( float value )
  {
    Global.instance.CameraController.orthoTarget = value;
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