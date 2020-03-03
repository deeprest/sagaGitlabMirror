using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour, ITrigger
{
  [SerializeField] Animator animator;
  [SerializeField] Collider2D cd;
  [SerializeField] AudioSource audio;
  [SerializeField] NavMeshObstacle obstacle;
  public bool isOpen = false;
  bool transitioning = false;
  [SerializeField] float animationRate = 16;
  public int openFrame = 19;
  public int closeFrame = 3;
  Timer timer = new Timer();
  public AudioClip soundOpen;
  public AudioClip soundClose;

  bool Entering;
  public Transform inside;
  Transform instigator;
  //public UnityEngine.Events.UnityEvent onEnterTrigger;
  //public UnityEngine.Events.UnityEvent onExitTrigger;
  //public UnityEngine.Events.UnityEvent onEnter;
  //public UnityEngine.Events.UnityEvent onExit;

  [SerializeField] Collider2D CameraIn;
  [SerializeField] Collider2D CameraOut;

  Timer runTimer = new Timer();
  Timer cameraDelay = new Timer();
  [SerializeField] float openDuration = 1;

  void OnDestroy()
  {
    timer.Stop( false );
  }

  public void Trigger( Transform instigator )
  {
    if( transitioning )
      return;
    this.instigator = instigator;
    SceneScript sceneScript = FindObjectOfType<SceneScript>();
    if( Global.instance.CurrentPlayer != null && instigator.IsChildOf( Global.instance.CurrentPlayer.transform ) )
      sceneScript.PlayerInputOff();
    Entering = Vector3.Dot( (inside.position - transform.position).normalized, (instigator.position - transform.position) ) < 0;
    /*if( Entering )
      onEnterTrigger.Invoke();
    else
      onExitTrigger.Invoke();*/
    //if( Entering )
    Open();

    cameraDelay.Start( 1, null, delegate
    {
      if( Entering )
      {
        if( CameraIn != null )
          sceneScript.ReplaceCameraPolyEncompass( CameraIn );
        else
          sceneScript.ReplaceCameraPoly( sceneScript.sb );
      }
      else
      {
        if( CameraOut != null )
          sceneScript.ReplaceCameraPolyEncompass( CameraOut );
        else
          sceneScript.ReplaceCameraPoly( sceneScript.sb );
      }

    } );

  }

  public void Open()
  {
    transitioning = true;
    animator.Play( "opening" );
    audio.PlayOneShot( soundOpen );
    timer.Start( (1.0f / animationRate) * openFrame, null, delegate
      {
        // if the door is toggled open/close, then set transitioning to false here
        // if it auto-closes on a timer, then transitioning should only end after closed. 
        //transitioning = false;
        isOpen = true;
        cd.enabled = false;
        obstacle.enabled = false;

        if( Global.instance.CurrentPlayer != null && instigator.IsChildOf( Global.instance.CurrentPlayer.transform ) )
        {
          //Time.timeScale = 0;
          bool right = Vector3.Dot( (transform.position - instigator.position).normalized, inside.right ) > 0;
          Global.instance.Controls.BipedActions.Disable();
          TimerParams tp = new TimerParams
          {
            unscaledTime = true,
            repeat = false,
            duration = 1,
            UpdateDelegate = delegate ( Timer t )
            {
              if( right )
                Global.instance.CurrentPlayer.inputRight = true;
              else
                Global.instance.CurrentPlayer.inputLeft = true;
            },
            CompleteDelegate = delegate
            {
              Global.instance.Controls.BipedActions.Enable();
              //Time.timeScale = 1;
            }
          };
          runTimer.Start( tp );

          /*if( Entering )
          onEnter.Invoke();
        else
          onExit.Invoke();*/
          timer.Start( openDuration, null, delegate
          {
            Close();
          } );
        }
      } );


  }

  public void Close()
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
      } );
    } );
  }
}
