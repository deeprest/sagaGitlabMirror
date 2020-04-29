using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTriggerTest : MonoBehaviour, ITrigger
{
  public Team SetToTeam;
  public void Trigger( Transform instigator )
  {
    instigator.GetComponent<Entity>().Team = SetToTeam;
  }
}
