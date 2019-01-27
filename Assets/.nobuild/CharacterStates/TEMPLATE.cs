/*
using UnityEngine;
using System.Collections;

// this goes in Awake()
// AddState( new CharacterState( "STATE_NAME", 0, ConsiderSTATE_NAME, PushSTATE_NAME, UpdateSTATE_NAME, PopSTATE_NAME, ResumeSTATE_NAME, SuspendSTATE_NAME ) );

// template STATE_NAME
partial class Character
{
  [Header("STATE_NAME")]
  public bool CanSTATE_NAME = false;
  public float STATE_NAMESpeed = 10f;
  float STATE_NAMEStartTime = 0f;
  public float STATE_NAMEPatience = 30f;

 void ConsiderSTATE_NAME( Interest interest )
  {
    if( !CanSTATE_NAME )
      return;
      PushState( "STATE_NAME" );
  }
  
  void PushSTATE_NAME()
  {
    CurrentMoveSpeed = STATE_NAMESpeed;
    STATE_NAMEStartTime = Time.time;
  }

  void ResumeSTATE_NAME()
  {

  }

  void SuspendSTATE_NAME()
  {

  }

  void PopSTATE_NAME()
  {

  }

  void UpdateSTATE_NAME()
  {
    if( Time.time - STATE_NAMEStartTime > STATE_NAMEPatience )
    {
      PopState();
      return;
    }
  }
}
*/