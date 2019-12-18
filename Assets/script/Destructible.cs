  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : Character
{
  [Header( "Destructible" )]
  public UnityEngine.Events.UnityEvent onDestruct;

  void Start()
  {
    CharacterStart();
    UpdateLogic = null;
    if( !UseGravity )
    {
      UpdateHit = null;
      UpdateCollision = null;
      UpdatePosition = null;
    }
  }

  protected override void Die()
  {
    base.Die();
    onDestruct.Invoke();
  }

}