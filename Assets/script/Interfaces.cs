using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ActionData
{
  public Character instigator;
  public List<string> actions;
  public Transform indicator;
}

public interface IAction
{
  void OnAction( Character instigator, string action = "default" );
  void GetActionContext( ref ActionData actionData );
}

public interface IDamage
{
  bool TakeDamage( Damage d );
}

/*public interface ITeam
{
  void SetTeam( Team team );
}*/

public interface IOwnable
{
  bool IsOwned();
  void ClearOwner();
  void AssignOwner( Character owner );
}

public interface ITrigger
{
  void Trigger( Transform instigator );
}