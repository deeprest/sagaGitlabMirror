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
  public bool Ability;
  public bool Shield;
  public bool Fire;
  public bool Interact;
  public bool NextWeapon;
  public bool NextAbility;
  // 8 byte
  public Vector2 Aim;
}

public class Pawn : Entity
{
  public Controller controller;
  public InputState input;

  public Vector2 CursorWorldPosition;
  public GameObject InteractIndicator;
  
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
