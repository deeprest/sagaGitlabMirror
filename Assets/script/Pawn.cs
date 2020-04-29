using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InputState
{
  public bool MoveRight;
  public bool MoveLeft;
  public bool JumpStart;
  public bool JumpEnd;
  public bool DashStart;
  public bool DashEnd;
  public bool ChargeStart;
  public bool Charge;
  public bool ChargeEnd;
  public bool Graphook;
  public bool Shield;
  public bool Fire;
  public bool Interact;
  public Vector2 Aim;
  public bool NextWeapon;
  public bool Down;
}

public class Pawn : Entity
{
  public Controller controller;

  public InputState input = new InputState();

  public void ApplyInput( InputState state )
  {
    input = state;
  }

  protected void ResetInput()
  {
    input = default;
  }
}
