using UnityEngine;

[System.Serializable]
public struct InputState
{
  // 1 byte
  public bool MoveLeft;
  public bool MoveRight;
  public bool MoveUp;
  public bool MoveDown;
  // todo remove start/end
  public bool JumpStart;
  public bool JumpEnd;
  // todo just have dash, forget start/end
  public bool DashStart;
  public bool DashEnd;
  // 1 byte
  // todo charge state only needs one bool
  public bool ChargeStart;
  public bool ChargeEnd;
  public bool Graphook;
  public bool Shield;
  public bool Fire;
  public bool Interact;
  public bool NextWeapon;
  // ---
  // 8 byte
  public Vector2 Aim;
}

public class Pawn : Entity
{
  public Controller controller;
  public InputState input = new InputState();

  public Vector2 CursorWorldPosition;
  public GameObject InteractIndicator;
  //public bool CursorInfluence;

  public void ApplyInput( InputState state )
  {
    input = state;
  }

  protected void ResetInput()
  {
    input = default;
  }

  public virtual void OnControllerAssigned(){}
  public virtual void OnControllerUnassigned(){}

  public virtual void UnselectWorldSelection(){}

}
