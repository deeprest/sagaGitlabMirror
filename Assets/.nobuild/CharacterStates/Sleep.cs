using UnityEngine;
using System.Collections;

partial class Character
{
  enum SleepState
  {
    None,
    MovingToBed,
    GettingIntoBed,
    Sleeping,
    WakingUp
  }

  [Header( "Sleep" )]
  public bool CanSleep = false;
  [Tooltip("travel to bed first, or sleep on ground")]

  public bool InSleepCycle = false;
  public bool SleepInterrupt = false;
  float SleepStartTime = 0f;
  const float SleepDuration = 360f;
  SleepState sleepState = SleepState.None;
  float wakeStart;
  const float wakeDuration = 3;
  LerpToTarget spriteLerp;
  LerpToTarget moveTransformLerp;
  const float MaxBedLerpDistance = 1f;

  public float LerpDuration = 1;
  public float sleepCameraLerpDurationIn = 5;
  public float sleepCameraLerpDurationOut = 1;


  public float sleepTimescale = 10;
  public float sleepTODDuration = 600;
  public float sleepTODWait = 8;
  public AudioClip sleepyMusic;
  AudioSource musicSource;
  CameraController cam;
  float start;
  float todStart;


  Bed TargetBed;

  public void GoToBed( Bed bed)
  {
    TargetBed = bed;
    PushState( "Sleep" );
  }

  public void SleepOnGround()
  {
    PushState( "Sleep" );
  }

  void ConsiderSleep( Interest interest )
  {
    if( !CanSleep )
      return;
    SleepOnGround();
  }


  void PushSleep()
  {
    InSleepCycle = true;

    CurrentMoveSpeed = SprintSpeed;
    SleepInterrupt = false;

    spriteLerp = billboard.gameObject.GetComponent<LerpToTarget>();
    if( spriteLerp == null )
      spriteLerp = billboard.gameObject.AddComponent<LerpToTarget>();

    moveTransformLerp = moveTransform.gameObject.GetComponent<LerpToTarget>();
    if( moveTransformLerp == null )
      moveTransformLerp = moveTransform.gameObject.AddComponent<LerpToTarget>();


    if( !isPlayerControlled && TargetBed != null && Vector3.Distance(moveTransform.position,TargetBed.transform.position) > MaxBedLerpDistance )
    {
      sleepState = SleepState.MovingToBed;
      CurrentMoveSpeed = WalkSpeed;
      if( !SetPath( TargetBed.destination.transform.position, delegate
      {
        StartSleep();
      } ) )
      {
        Debug.Log( "path failed" );
      }
    }
    else
    {
      sleepState = SleepState.GettingIntoBed;
      StartSleep();
    }
  }

  void StartSleep()
  {
    sleepState = SleepState.GettingIntoBed;
    CurrentMoveSpeed = 0;
    body.isKinematic = true;
    ball.LockToGround = false;
    FaceDirectionOfMovement = false;
    billboard.faceCamera = false;
    walk.enabled = false;

    spriteLerp.moveTransform = billboard.transform;
    if( TargetBed != null && Vector3.Distance(moveTransform.position,TargetBed.transform.position) < MaxBedLerpDistance )
    {
      Physics.IgnoreCollision( ball.Sphere, TargetBed.GetComponent<Collider>(), true );
      moveTransform.parent = TargetBed.sleepTarget;
      moveTransformLerp.moveTransform = moveTransform;
      moveTransformLerp.targetTransform = TargetBed.sleepTarget;
      moveTransformLerp.lerpType = LerpToTarget.LerpType.Curve;
      moveTransformLerp.duration = LerpDuration;
      moveTransformLerp.LerpRotation = false;
      moveTransformLerp.OnLerpEnd = null;
      moveTransformLerp.enabled = true;
      spriteLerp.targetTransform = TargetBed.sleepTarget;
      spriteLerp.WorldTarget = false;
    }
    else
    {
      spriteLerp.WorldTarget = true;
      spriteLerp.targetPositionWorld = new Vector3( moveTransform.position.x, Global.Instance.GlobalSpriteOnGroundY, moveTransform.position.z );
      spriteLerp.targetRotationForward = Vector3.up;
      spriteLerp.targetRotationUp = moveTransform.forward;
    }
    spriteLerp.lerpType = LerpToTarget.LerpType.Curve;
    spriteLerp.duration = LerpDuration;
    spriteLerp.LerpRotation = true;
    spriteLerp.enabled = true;
    spriteLerp.OnLerpEnd = delegate
    {
      if( !SleepInterrupt )
      {
        sleepState = SleepState.Sleeping;
        SleepStartTime = Time.time;
        PlayFaceAnimation( "sleep" );
        if( isPlayerControlled )
        {
          Time.timeScale = sleepTimescale;
          start = Time.unscaledTime;
        }
      }
    };



    if( isPlayerControlled )
    {
      Zone zone = Global.Instance.GetZone( gameObject );
      musicSource = zone.GetComponent<AudioSource>();
      musicSource.clip = sleepyMusic;
      musicSource.Play();
      Global.Instance.AudioGoSleep( 1 );
      cam = Global.Instance.cameraController;
      cam.LookControlEnabled = false;
      start = Time.unscaledTime;
      todStart = Global.Instance.CurrentTimeOfDay;
    }
  }

  void PopSleep()
  {
    InSleepCycle = false;
    sleepState = SleepState.None;
    body.isKinematic = false;
    if( TargetBed!=null )
      Physics.IgnoreCollision( ball.Sphere, TargetBed.GetComponent<Collider>(), false );
    ball.LockToGround = true;
    FaceDirectionOfMovement = true;
    moveTransform.parent = null;
    billboard.faceCamera = true;
    walk.enabled = true;
    PlayFaceAnimation( "idle" );
  }


  void UpdateSleep()
  {
    if( !InSleepCycle )
      return;
    
    if( isPlayerControlled )
    {
      if( Input.GetButtonDown( "Interact" ) )
      {
        SleepInterrupt = true;
      }
    }

    if( SleepInterrupt && sleepState != SleepState.WakingUp )
    {
      sleepState = SleepState.WakingUp;
      wakeStart = Time.time;

      if( isPlayerControlled )
      {
        Time.timeScale = 1;
        musicSource.Stop();
        Global.Instance.AudioWakeUp( 1 );
        cam.LookControlEnabled = true;
      }

      if( TargetBed != null )
      {
        moveTransformLerp.moveTransform = moveTransform;
        moveTransformLerp.targetTransform = TargetBed.wakeTarget;
        moveTransformLerp.lerpType = LerpToTarget.LerpType.Curve;
        moveTransformLerp.duration = spriteLerp.duration;
        moveTransformLerp.LerpRotation = false;
        moveTransformLerp.enabled = true;
        moveTransformLerp.OnLerpEnd = delegate
        {
          PopState();
        };
      }
      else
      {
        PopState();
      }
    }

    switch( sleepState )
    {
      case SleepState.MovingToBed:
        break;

      case SleepState.GettingIntoBed:
        if( isPlayerControlled )
        {
          if( Time.unscaledTime - start < sleepCameraLerpDurationIn )
          {
            float alpha = ( Time.unscaledTime - start ) / sleepCameraLerpDurationIn;
            cam.lookInput.y = Mathf.Lerp( cam.lookInput.y, 90, alpha );
          }
        }
        break;

      case SleepState.Sleeping:
        Wake = Mathf.Clamp( Wake + WakeGainRate * Time.deltaTime, 0, WakeFull );
        if( Time.time - SleepStartTime > SleepDuration )
        {
          SleepInterrupt = true; 
        }
        if( isPlayerControlled )
        {
          if( Time.unscaledTime - start < sleepTODWait )
          {
            float alpha = ( Time.unscaledTime - start ) / sleepTODWait;
            Global.Instance.CurrentTimeOfDay = todStart + sleepTODDuration * alpha;
            Global.Instance.UpdateTimeOfDay();
          }
        }
        break;

      case SleepState.WakingUp:
        if( isPlayerControlled )
        {
          if( Time.unscaledTime - wakeStart < sleepCameraLerpDurationOut )
          {
            float alpha = ( Time.unscaledTime - wakeStart ) / sleepCameraLerpDurationOut;
            cam.lookInput.y = Mathf.Lerp( cam.lookInput.y, 15, alpha );
          }
        }
        break;
      default:
        break;
    }
  }

}

