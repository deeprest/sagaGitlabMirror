﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour, ITrigger
{
  [SerializeField] Animator animator;
  [SerializeField] Collider2D cd;
  [SerializeField] new AudioSource audio;
  [SerializeField] NavMeshObstacle obstacle;
  public bool isOpen = false;
  bool transitioning = false;
  [SerializeField] float animationRate = 16;
  public int openFrame = 19;
  public int closeFrame = 3;
  Timer timer = new Timer();
  public AudioClip soundOpen;
  public AudioClip soundClose;

  public Transform inside;
  Transform instigator;

  [SerializeField] CameraZone CameraIn;
  [SerializeField] CameraZone CameraOut;
  [SerializeField] AudioLoop Music;

  Timer doorTimer = new Timer();
  Timer runTimer = new Timer();
  [SerializeField] float openDuration = 3;
  [SerializeField] bool MMXStyleDoorTransition = true;
  [SerializeField] float doorRunDistance = 1;
  [SerializeField] float runDuration = 0.5f;

  public Team[] OpenForTeams;

  void OnDestroy()
  {
    timer.Stop( false );
  }

  public void Trigger( Transform instigator )
  {
    if( transitioning )
      return;

    Character check = instigator.GetComponent<Character>();
    if( check != null )
    {
      bool OpenForThisCharacter = false;
      for( int i = 0; i < OpenForTeams.Length; i++ )
      {
        if( OpenForTeams[i] == check.Team )
        {
          OpenForThisCharacter = true;
          break;
        }
      }
      if( !OpenForThisCharacter )
        return;
    }
    this.instigator = instigator;

    // is player?
    if( Global.instance.CurrentPlayer != null && instigator.IsChildOf( Global.instance.CurrentPlayer.transform ) )
    {
      bool right = Vector3.Dot( (transform.position - instigator.position).normalized, inside.right ) > 0;
      if( MMXStyleDoorTransition )
      {
        DoMMXStyleDoorTransition();
      }
      else
      {
        bool Entering = Vector3.Dot( (inside.position - transform.position).normalized, (instigator.position - transform.position) ) < 0;
        if( ((Entering && CameraIn != null) || (!Entering && CameraOut)) )
          Global.instance.Controls.BipedActions.Disable();
        OpenAndClose( openDuration, delegate
        {
          if( Entering )
          {
            if( CameraIn != null )
            {
              Global.instance.AssignCameraZone( CameraIn );
              runTimer.Start( runDuration, delegate
              {
                if( right ) Global.instance.CurrentPlayer.inputRight = true;
                else Global.instance.CurrentPlayer.inputLeft = true;
              }, delegate
              {
                Global.instance.Controls.BipedActions.Enable();
              } );
            }
            else
            {
              SceneScript sceneScript = FindObjectOfType<SceneScript>();
              if( sceneScript != null )
                Global.instance.AssignCameraZone( sceneScript.CameraZone );
            }
          }
          else
          {
            if( CameraOut != null )
            {
              Global.instance.AssignCameraZone( CameraOut );
              runTimer.Start( runDuration, delegate
              {
                if( right ) Global.instance.CurrentPlayer.inputRight = true;
                else Global.instance.CurrentPlayer.inputLeft = true;
              }, delegate
              {
                Global.instance.Controls.BipedActions.Enable();
              } );
            }
            else
            {
              SceneScript sceneScript = FindObjectOfType<SceneScript>();
              if( sceneScript != null )
                Global.instance.AssignCameraZone( sceneScript.CameraZone );
            }
          }
        }, null );
      }
    }
    else
    {
      OpenAndClose( openDuration, null, null );
    }
  }

  void DoMMXStyleDoorTransition()
  {
    bool right = Vector3.Dot( (transform.position - instigator.position).normalized, inside.right ) > 0;
    animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    Global.Pause();
    Global.instance.Controls.BipedActions.Disable();
    OpenAndClose( openDuration, delegate
    {
      // open
      SceneScript sceneScript = FindObjectOfType<SceneScript>();
      if( sceneScript != null )
      {
        bool Entering = Vector3.Dot( (inside.position - transform.position).normalized, (instigator.position - transform.position) ) < 0;
        if( Entering )
        {
          if( CameraIn != null ) Global.instance.AssignCameraZone( CameraIn );
          else Global.instance.AssignCameraZone( sceneScript.CameraZone );
          if( Music != null )
            Global.instance.MusicTransition( Music );
        }
        else
        {
          if( CameraOut != null ) Global.instance.AssignCameraZone( CameraOut );
          else Global.instance.AssignCameraZone( sceneScript.CameraZone );
          if( Music != null )
            Global.instance.MusicTransition( sceneScript.music );
        }
      }
      // todo make door less awful
      Global.instance.CurrentPlayer.DoorRun( right, openDuration, doorRunDistance );
    },
    delegate
    {
      // close
      Global.instance.Controls.BipedActions.Enable();
      Global.Unpause();
    } );
  }

  public void OpenAndClose( float duration, System.Action onOpen, System.Action onClose )
  {
    Open( delegate
    {
      if( Global.instance.CurrentPlayer != null && instigator.IsChildOf( Global.instance.CurrentPlayer.transform ) )
      {
        if( onOpen != null )
          onOpen();
        TimerParams tp = new TimerParams
        {
          unscaledTime = Global.Paused,
          repeat = false,
          duration = duration,
          CompleteDelegate = delegate
          {
            Close( null );
            if( onClose != null )
              onClose();
          }
        };
        doorTimer.Start( tp );
      }
    } );
  }

  public void Open( System.Action onOpen = null )
  {
    transitioning = true;
    animator.Play( "opening" );
    audio.PlayOneShot( soundOpen );
    TimerParams timerParams = new TimerParams
    {
      unscaledTime = Global.Paused,
      repeat = false,
      duration = (1.0f / animationRate) * openFrame,
      UpdateDelegate = null,
      CompleteDelegate = delegate
      {
        // if the door is toggled open/close, then set transitioning to false here
        // if it auto-closes on a timer, then transitioning should only end after closed. 
        //transitioning = false;
        isOpen = true;
        cd.enabled = false;
        obstacle.enabled = false;
        if( onOpen != null )
          onOpen();
      }
    };
    timer.Start( timerParams );
  }

  public void Close( System.Action onClosed = null )
  {
    transitioning = true;
    animator.Play( "closing" );
    audio.PlayOneShot( soundClose );
    timer.Start( (1.0f / animationRate) * closeFrame, null, delegate
    {
      isOpen = false;
      cd.enabled = true;
      obstacle.enabled = true;
      timer.Start( 2, null, delegate
      {
        transitioning = false;
        if( onClosed != null )
          onClosed();
      } );
    } );
  }
}
