using UnityEngine;

public class Pawn : Entity
{
  public Controller controller;
  public InputState input;
  public InputState pinput;

  public Vector2 CursorWorldPosition;
  public GameObject InteractIndicator;

  // protected override void OnDestroy()
  // {
  //   base.OnDestroy();
  //   controller.OnPawnDestroy();
  // }

  protected override void Awake()
  {
    // do not call base.Awake() to avoid being added to Limit
    EntityAwake();
  }

  public void ApplyInput( InputState state )
  {
    input = state;
  }

  protected void ResetInput()
  {
    pinput = input;
    input = default;
  }

  public virtual void OnControllerAssigned(){}
  public virtual void OnControllerUnassigned(){}

  public virtual void UnselectWorldSelection(){}

  
  public virtual  Vector2 GetShotOriginPosition()
  {
    return transform.position;
  }

  public virtual Vector2 GetAimVector()
  {
    return Vector2.up;
  }
  
}
