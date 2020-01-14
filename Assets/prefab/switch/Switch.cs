using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : WorldSelectable
{
  [SerializeField] bool InvokeOnStart;
  [SerializeField] bool on;
  [SerializeField] Animator animator;
  public UnityEngine.Events.UnityEvent onActivate;
  public UnityEngine.Events.UnityEvent onDeactivate;

  public override void Highlight()
  {
    //animator.Play( "highlight" );
  }
  public override void Unhighlight()
  {
    //animator.Play( "idle" );
  }
  public override void Select()
  {
    // toggle
    on = !on;
    if( on ) 
    {
      animator.Play( "on" );
      onActivate.Invoke();
    }
    else
    {
      animator.Play( "off" );
      onDeactivate.Invoke();
    }
  }
  public override void Unselect()
  { }

  void Start()
  {
    if( InvokeOnStart )
    {
      if( on )
      {
        animator.Play( "on" );
        onActivate.Invoke();
      }
      else
      {
        animator.Play( "off" );
        onDeactivate.Invoke();
      }
    }
  }

}