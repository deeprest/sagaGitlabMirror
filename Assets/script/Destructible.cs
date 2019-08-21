using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : Character
{
  void Start()
  {
    CharacterStart();
    UpdateLogic = null;
    UpdateHit = null;
    UpdateCollision = null;
    UpdatePosition = null;
  }

}