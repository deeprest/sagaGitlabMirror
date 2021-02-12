using UnityEngine;

[SelectionBase]
public class WorldSelectable : MonoBehaviour, IWorldSelectable
{
  public virtual void Highlight()
  {
    if( Global.instance.CurrentPlayer != null )
    {
      Global.instance.CurrentPlayer.InteractIndicator.SetActive( true );
      Global.instance.CurrentPlayer.InteractIndicator.transform.position = GetPosition();
    }
  }
  public virtual void Unhighlight()
  {
    if( Global.instance.CurrentPlayer != null )
      Global.instance.CurrentPlayer.InteractIndicator.SetActive( false );
  }
  public virtual void Select() { }
  public virtual void Unselect() { }

  public virtual Vector2 GetPosition()
  {
    return transform.position; 
  }
}
