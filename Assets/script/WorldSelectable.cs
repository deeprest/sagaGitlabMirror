using UnityEngine;
using System.Collections;

public class WorldSelectable : MonoBehaviour
{
  public virtual void Highlight()
  {
    if( Global.instance.CurrentPlayer != null )
    {
      Global.instance.CurrentPlayer.InteractIndicator.SetActive( true );
      Global.instance.CurrentPlayer.InteractIndicator.transform.position = transform.position;
    }
  }
  public virtual void Unhighlight()
  {
    if( Global.instance.CurrentPlayer != null )
      Global.instance.CurrentPlayer.InteractIndicator.SetActive( false );
  }
  public virtual void Select() { }
  public virtual void Unselect() { }
}
