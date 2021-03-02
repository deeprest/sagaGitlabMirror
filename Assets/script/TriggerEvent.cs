using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour, ITrigger
{
  public bool once;
  bool triggered;
  public UnityEvent evt;

  public void Trigger( Transform instigator )
  {
    if( instigator.root != Global.instance.PlayerController.pawn.transform.root )
      return;
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

  public void DebugLog( string str ) { Debug.Log(str  );}

  public void AssignOverrideCameraZone( CameraZone zone )
  {
    Global.instance.CameraController.AssignOverrideCameraZone( zone );
  }
  
  public void CameraOverride( bool on )
  {
    Global.instance.CameraController.CameraZoneOverride = on;
  }
}
