using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, ITrigger
{
  [SerializeField] SpriteAnimator sa;
  [SerializeField] Collider2D cd;
  public bool isOpen = false;
  bool transitioning = false;
  public AnimSequence open;
  public AnimSequence close;
  public int openFrame = 19;
  public int closeFrame = 3;
  Timer timer = new Timer();

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
    timer.Start( (1.0f/open.fps)*openFrame, null, delegate
    {
      transitioning = false;
      isOpen = true;
      cd.enabled = false;
      timer.Start( 2, null, delegate{ Close(); });
    } );
  }

  public void Close()
  {
    transitioning = true;
    sa.Play( close );
    timer.Start( (1.0f/close.fps)*closeFrame, null, delegate
    {
      transitioning = false;
      isOpen = false;
      cd.enabled = true;
    } );
  }
}
