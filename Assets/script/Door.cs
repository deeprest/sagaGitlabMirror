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
  public UnityEngine.Events.UnityEvent onEnterTrigger;
  public UnityEngine.Events.UnityEvent onExitTrigger;
  public UnityEngine.Events.UnityEvent onEnter;
  public UnityEngine.Events.UnityEvent onExit;

  void OnDestroy()
  {
    timer.Stop( false );
  }

  public void Trigger( Transform instigator )
  {
    if( transitioning )
      return;
    this.instigator = instigator;
    Entering = Vector3.Dot( (inside.position - transform.position).normalized, (instigator.position - transform.position) ) < 0;
    if( Entering )
      onEnterTrigger.Invoke();
    else
      onExitTrigger.Invoke();
    Open();
  }

  public void Open()
  {
    transitioning = true;
    animator.Play( "opening" );
    audio.PlayOneShot( soundOpen );
    timer.Start( (1.0f/animationRate) * openFrame, null, delegate
    {
      // if the door is toggled open/close, then set transitioning to false here
      // if it auto-closes on a timer, then transitioning should only end after closed. 
      //transitioning = false;
      isOpen = true;
      cd.enabled = false;
      obstacle.enabled = false;
      if( Entering )
        onEnter.Invoke();
      else
        onExit.Invoke();
      timer.Start( 2, null, delegate
      {
        Close();
      } );
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
