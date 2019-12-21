using UnityEngine;
using System.Collections;

public partial class Character
{
  // "Investigate" means "Go to the area and then wander around"
  [Header( "Investigate" )]
  public float InvestigateRadius = 8f;
  public float InvestigateArrivalDistance = 3f;
  float InvestigateStartTime = 0f;
  public float InvestigatePatience = 10f;
  public float InvestigateAdjustSpeed = 0.1f;
  Vector3 InvestigatePosition;
  Vector3 FinalInvestigateDirection;


  void Investigate( Vector3 position, Interest sourceInterest = null )
  {
    if( isPlayerControlled )
      return;
    InvestigatePosition = position + Random.insideUnitSphere * InvestigateRadius;  
    InvestigatePosition.y = transform.position.y;
    FinalInvestigateDirection = Random.insideUnitSphere;
    FinalInvestigateDirection.y = 0f;
    PushState( "Investigate", sourceInterest );
  }

  void ConsiderInvestigate( Interest interest )
  {
    // NOTE this will ignore a higher priority investigation if already investigating.
    if( CurrentState.Name == "Investigate" )
      return;
    // move in the direction of suspected hostility
    if( Vector3.SqrMagnitude( interest.go.transform.position - moveTransform.position ) > 1f * 1f )
      Investigate( interest.go.transform.position, interest );
  }

  void PushInvestigate()
  {
    CurrentMoveSpeed = WalkSpeed;
    InvestigateStartTime = Time.time;
    SidestepAvoidance = true;

    SetPath( InvestigatePosition, delegate
    {
      CurrentMoveSpeed = WalkSpeed * 0.5f;
      MoveDirection = FinalInvestigateDirection;
    } );
  }

  void UpdateInvestigate()
  {
    if( Time.time - InvestigateStartTime > InvestigatePatience )
    {
      PopState();
      return;
    } 
  }

}

