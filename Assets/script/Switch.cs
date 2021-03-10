using UnityEngine;
using UnityEngine.Events;

public class Switch : WorldSelectable
{
  [SerializeField] bool InvokeOnStart;
  [SerializeField] bool on;
  [SerializeField] Animator animator;
  public UnityEvent onActivate;
  public UnityEvent onDeactivate;
#if false
  public override void Highlight()
  {
    base.Highlight();
    //animator.Play( "highlight" );
  }
  public override void Unhighlight()
  {
    //animator.Play( "idle" );
  }
#endif
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
    animator.Play( on? "on":"off" );
    if( InvokeOnStart )
    {
      if( on )
        onActivate.Invoke();
      else
        onDeactivate.Invoke();
    }
  }

}