using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

  void OnTriggerEnter2D( Collider2D collider )
  {
    Trigger( collider.transform );
  }

  public void LoadScene( string arg )
  {
    Global.instance.LoadScene( arg );
  }

  public void LoadScene( SceneAssetObject arg )
  {
    Global.instance.LoadScene( arg.scene.GetSceneName() );
  }

}
