﻿using UnityEngine;
using System.Collections;


public class SceneScript : MonoBehaviour
{
  // temp
  public Level level;

  public PolygonCollider2D sb;
  public virtual void StartScene() { }

  public void PlayerInputOff()
  {
    Global.instance.CurrentPlayer.playerInput = false;
  }

  public void ReplaceCameraPoly( PolygonCollider2D poly )
  {
    Global.instance.AssignCameraPoly( poly );
    Global.instance.CameraController.EncompassBounds = false;
  }

  public void ReplaceCameraPolyEncompass( PolygonCollider2D poly )
  {
    Global.instance.AssignCameraPoly( poly );
    Global.instance.CameraController.EncompassBounds = true;
  }

  Timer runTimer = new Timer();
  public void RunLeft( float duration )
  {
    Global.instance.CurrentPlayer.playerInput = false;
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
        Global.instance.CurrentPlayer.playerInput = true;
      }
    };
    runTimer.Start( tp );
  }

  public void RunRight( float duration )
  {
    Global.instance.CurrentPlayer.playerInput = false;
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
        Global.instance.CurrentPlayer.playerInput = true;
      }
    };
    runTimer.Start( tp );
  }

}