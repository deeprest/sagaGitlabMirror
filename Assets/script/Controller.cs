using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : ScriptableObject
{
  public static List<Controller> All = new List<Controller>();

  // input state is modified and passed to pawn
  protected InputState input = new InputState();

  protected Pawn pawn;
  //public List<Pawn> minions = new List<Pawn>();

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

  public Pawn GetPawn()
  {
    return pawn;
  }

  //public void AddMinion( Pawn pawn )
  //{
  //  minions.Add( pawn );
  //}
}

