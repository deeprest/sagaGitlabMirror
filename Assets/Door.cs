using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, ITrigger
{
  [SerializeField] SpriteAnimator sa;
  [SerializeField] Collider2D cd;
  [SerializeField] new AudioSource audio;
  public bool isOpen = false;
  bool transitioning = false;
  public AnimSequence open;
  public AnimSequence close;
  public int openFrame = 19;
  public int closeFrame = 3;
  Timer timer = new Timer();
  public AudioClip soundOpen;
  public AudioClip soundClose;

  public void Trigger( Transform instigator )
  {
    if( transitioning )
      return;
    Open();
  } 

  public void Open()
  {
    transitioning = true;
    sa.Play( open );
    audio.PlayOneShot( soundOpen );
    timer.Start( (1.0f / open.fps) * openFrame, null, delegate
    {
      // if the door is toggled open/close, then set transitioning to false here
      // if it auto-closes on a timer, then transitioning should only end after closed. 
      //transitioning = false;
      isOpen = true;
      cd.enabled = false;
      timer.Start( 2, null, delegate
      {
        Close();
      } );
    } );
  }

  public void Close()
  {
    transitioning = true;
    sa.Play( close );
    audio.PlayOneShot( soundClose );
    timer.Start( (1.0f / close.fps) * closeFrame, null, delegate
    {
      isOpen = false;
      cd.enabled = true;
      timer.Start( 2, null, delegate
      {
        transitioning = false;
      } );
    } );
  }
}
