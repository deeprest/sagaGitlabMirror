using UnityEngine;

[System.Serializable]
public struct InputState
{
  // 1 byte
  public bool MoveLeft;
  public bool MoveRight;
  public bool MoveUp;
  public bool MoveDown;
  public bool Jump;
  public bool Dash;
  public bool Fire;
  public bool Charge;
  // 1 byte
  public bool Ability;
  public bool Interact;
  public bool NextWeapon;
  public bool NextAbility;
  public bool NOTUSED0;
  public bool NOTUSED1;
  public bool NOTUSED2;
  public bool NOTUSED3;
  // 8 byte
  public Vector2 Aim;
}

public class Pawn : Entity
{
  public Controller controller;
  public InputState input;
  public InputState pinput;

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
