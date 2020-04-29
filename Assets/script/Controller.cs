using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : ScriptableObject
{
  public static List<Controller> All = new List<Controller>();

  protected InputState input = new InputState();
  protected Pawn pawn;

  public virtual void Awake()
  {
    All.Add( this );
  }
  private void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    All.Remove( this );
  }

  public virtual void Update() { }

  public virtual void AssignPawn( Pawn pawn )
  {
    this.pawn = pawn;
  }
}

