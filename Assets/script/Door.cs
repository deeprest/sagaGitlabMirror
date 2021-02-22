using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Door : MonoBehaviour, ITrigger
{
  [SerializeField] Animator animator;
  [SerializeField] Collider2D cd;
  [SerializeField] AudioSource audio;
  [SerializeField] NavMeshObstacle obstacle;
  public bool isOpen;
  bool transitioning;
  bool deniedDelay;
  [SerializeField] float animationRate = 16;
  public int openFrame = 19;
  public int closeFrame = 3;
  Timer timer = new Timer();
  public AudioClip soundOpen;
  public AudioClip soundClose;
  public AudioClip soundDenied;

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

  [FormerlySerializedAs( "OpenForTeams" )]
  public Team[] OpenOnlyForTeams;

  void OnDestroy()
  {
    timer.Stop( false );
  }

  void OnTriggerEnter2D( Collider2D collider )
  {
    Trigger( collider.transform );
  }

  public void Trigger( Transform instigator )
  {
    if( transitioning || deniedDelay )
      return;

    Entity check = instigator.GetComponent<Entity>();
    if( check != null && OpenOnlyForTeams.Length > 0 )
    {
      bool OpenForThisCharacter = false;
      for( int i = 0; i < OpenOnlyForTeams.Length; i++ )
      {
        if( OpenOnlyForTeams[i] == check.Team )
        {
          OpenForThisCharacter = true;
          break;
        }
      }
      if( !OpenForThisCharacter )
      {
        Global.instance.AudioOneShot( soundDenied, transform.position );
        deniedDelay = true;
        timer.Start(3,null, delegate { deniedDelay = false; });
        animator.Play( "denied" );
        return;
      }
    }
    this.instigator = instigator;

    SceneScript sceneScript = Global.instance.sceneScript;
    PlayerBiped player = instigator.GetComponentInParent<PlayerBiped>();
    if( player == null )
    {
      OpenAndClose( openDuration, null, null );
    }
    else
    {
      bool right = Vector3.Dot( (transform.position - instigator.position).normalized, inside.right ) > 0;
      bool Entering = Vector3.Dot( (inside.position - transform.position).normalized, (instigator.position - transform.position) ) < 0;
      if( MMXStyleDoorTransition )
      {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        Global.Pause();
        Global.instance.Controls.BipedActions.Disable();
        

        OpenAndClose( openDuration, delegate
        {
          // door is now open
          if( sceneScript != null )
          {
            if( Entering )
            {
              if( CameraIn != null ) 
                Global.instance.OverrideCameraZone( CameraIn );
              else 
                Global.instance.OverrideCameraZone( null );
              Global.instance.CrossFadeTo( Music );
            }
            else
            {
              if( CameraOut != null ) 
                Global.instance.OverrideCameraZone( CameraOut );
              else 
                Global.instance.OverrideCameraZone( null );
              Global.instance.CrossFadeTo( sceneScript.music );
            }
          }
          // todo make door less awful
          player.DoorTransition( right, openDuration, doorRunDistance );
        },
        delegate
        {
          // door is now closed
          Global.instance.Controls.BipedActions.Enable();
          Global.Unpause();
        } );
      }
      else
      {
        if( ((Entering && CameraIn != null) || (!Entering && CameraOut)) )
          Global.instance.Controls.BipedActions.Disable();
        OpenAndClose( openDuration, delegate
        {
          if( Entering )
          {
            if( CameraIn != null )
            {
              Global.instance.OverrideCameraZone( CameraIn );
              runTimer.Start( runDuration, delegate
              {
                if( right )
                  player.ApplyInput( new InputState { MoveRight = true } );
                else
                  player.ApplyInput( new InputState { MoveLeft = true } );
              }, delegate
              {
                Global.instance.Controls.BipedActions.Enable();
              } );
            }
            else
            {
              if( sceneScript != null )
                Global.instance.OverrideCameraZone( null );
            }
          }
          else
          {
            if( CameraOut != null )
            {
              Global.instance.OverrideCameraZone( CameraOut );
              runTimer.Start( runDuration, delegate
              {
                if( right )
                  player.ApplyInput( new InputState { MoveRight = true } );
                else
                  player.ApplyInput( new InputState { MoveLeft = true } );
              }, delegate
              {
                Global.instance.Controls.BipedActions.Enable();
              } );
            }
            else
            {
              if( sceneScript != null )
                Global.instance.OverrideCameraZone( null );
            }
          }
        }, null );
      }
    }
  }

  public void OpenAndClose( float duration, System.Action onOpen, System.Action onClose )
  {
    Open( delegate
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
