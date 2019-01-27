using UnityEngine;
using System.Collections;

public partial class Character
{
  [Header( "Idle" )]
  public float IdlePatience = 10;
  float IdleStartTime = 0f;

  void PushIdle()
  {
    IdleStartTime = Time.time;
    CurrentMoveSpeed = 0f;
  }

  void UpdateIdle()
  {
    if( Time.time - IdleStartTime > IdlePatience )
    {
      PushState( "Wander" );
      return;
    }
  }
}
