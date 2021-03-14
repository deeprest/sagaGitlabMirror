using UnityEngine;
using UnityEngine.Events;

public class Switch : WorldSelectable
{
  [SerializeField] bool IsToggle;
  [SerializeField] bool InvokeOnStart;
  [SerializeField] bool on;
  [SerializeField] Animator animator;
  public UnityEvent onActivate;
  public UnityEvent onDeactivate;
  public Switch[] syncSwitches;

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

  public void AssignState( bool ison )
  {
    on = ison;
    if( ison )
    {
      animator.Play( "on" );
      onActivate.Invoke();
    }
    else
    {
      animator.Play( "off" );
      onDeactivate.Invoke();
    }
    for( int i = 0; i < syncSwitches.Length; i++ )
      syncSwitches[i].AssignState( IsToggle? on : false );
  }

  public override void Select()
  {
    if( IsToggle )
      AssignState( !on );
    else
      AssignState( true );
  }

  public override void Unselect() { }

  void Start()
  {
    if( InvokeOnStart )
      AssignState( on );
    else
      animator.Play( on ? "on" : "off" );
  }
}