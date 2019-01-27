using UnityEngine;
using System.Collections;

public partial class Character
{
  [Header( "Flee" )]
  public Transform FleeFrom;
  Vector3 FleeFromPosition;
  public float FleePatience = 5f;
  float FleeStartTime;

  void ConsiderFlee( Interest interest )
  {
    if( CanSeeObject( interest.go, true ) )
    {
      FleeFrom = interest.go.transform;
      FleeFromPosition = interest.objectPositionWhenSensed;
      PushState( "Flee", interest );
    }
  }

  void PushFlee()
  {
    FleeStartTime = Time.time;
    Vector3 fleeToPosition = moveTransform.position + ( moveTransform.position - FleeFromPosition ).normalized * 10f;
    // if fleeing away from home?
    //if( Home != null )
    //  fleeToPosition = Home.transform.position + Random.insideUnitSphere * Home.ArrivalRadius;
    fleeToPosition.y = 0f;
    if( SetPath( fleeToPosition, delegate
    {
      FleeFrom = null;
    } ) )
    {
      CurrentMoveSpeed = SprintSpeed;
    }

  }

  void UpdateFlee()
  {
    Vector3 fleeFrom = FleeFromPosition;
    if( FleeFrom != null && CanSeeObject( FleeFrom.gameObject, true ) )
    {
      FleeStartTime = Time.time;
      fleeFrom = FleeFrom.position;
      FleeFromPosition = FleeFrom.position;
    }

    if( Vector3.SqrMagnitude( moveTransform.position - fleeFrom ) < 1f * 1f )
    {
      CurrentMoveSpeed = SprintSpeed;
      MoveDirection = Vector3.Cross( ( moveTransform.position - fleeFrom ).normalized, Vector3.up );
    }
    else
    {
      MoveDirection = moveTransform.position - fleeFrom;
    }

    if( Time.time - FleeStartTime > FleePatience )
    {
      PopState();
      return;
    }
  }

}
