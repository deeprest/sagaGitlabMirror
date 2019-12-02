using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour, ITrigger
{
  public bool once;
  bool triggered;
  public UnityEngine.Events.UnityEvent evt;

  public void Trigger( Transform instigator )
  {
    if( !once || (once &&!triggered) )
    {
      triggered = true;
      evt.Invoke();
    }
  }

  public void LoadScene( Object arg )
  {
    Global.instance.LoadScene( arg.name );
  }
}
