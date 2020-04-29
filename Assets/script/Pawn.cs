using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

[System.Serializable]
public struct InputState
{
  // 1 byte
  public bool MoveRight;
  public bool MoveLeft;
  public bool JumpStart;
  public bool JumpEnd;
  public bool DashStart;
  public bool DashEnd;
  public bool ChargeStart;
  public bool Charge;
  // 1 byte
  public bool ChargeEnd;
  public bool Graphook;
  public bool Shield;
  public bool Fire;
  public bool Interact;
  public bool NextWeapon;
  public bool Down;
  public bool padding;
  // 8 byte 
  public Vector2 Aim;
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
