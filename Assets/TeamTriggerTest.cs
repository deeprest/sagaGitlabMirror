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
  void OnTriggerEnter2D( Collider2D collider )
  {
    Trigger( collider.transform );
  }
}
