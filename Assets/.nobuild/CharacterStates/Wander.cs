using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Character
{
  [Header( "Wander" )]
  public float WanderSpeed = 10f;
  float WanderStartTime = 0f;
  public float WanderPatience = 30f;
  //float WanderDirectionChangeStartTime = 0f;
  public float WanderDirectionChangeInterval = 4f;
  public float WanderDistance = 2f;
  public Destination WanderDestination;
  public int WanderDestinationIndex;

  void PushWander()
  {
    CurrentMoveSpeed = WanderSpeed;
    WanderStartTime = Time.time;
    //WanderDirectionChangeStartTime = WanderStartTime;
    SidestepAvoidance = true;

    System.Action[] actions = new System.Action[] {
      WanderHome,
      WanderRandom,
      WanderRandomDestination/*, WanderFollow */
    };
    actions[ Random.Range( 0, actions.Length ) ].Invoke();
  }

  void PopWander()
  {
    ClearPath();
  }


  void WanderFollow()
  {
    Character[] chas = GetCharactersWithinRadius( 6 );
    if( chas.Length == 0 )
    {
      PopState();
      return;
    }
    Character followee = chas[ Random.Range( 0, chas.Length - 1 ) ];
    if( ShouldFollow( followee ) )
      Follow( followee, null );
  }

  void WanderHome()
  {
    if( Home != null && Vector3.SqrMagnitude( moveTransform.position - Home.transform.position ) > Home.ArrivalRadius * Home.ArrivalRadius )
    {
      // go home
      Vector3 randomVector = Random.insideUnitSphere;
      randomVector.y = 0;
      randomVector *= Home.ArrivalRadius;
      if( SetPath( Home.transform.position + randomVector, delegate
      {
        CurrentMoveSpeed = 0f;
        PopState();
      } ) )
      {
        CurrentMoveSpeed = WalkSpeed;
      }
    }
  }

  void WanderRandom()
  {
    // random direction
    float r = Random.value;
    Vector3 dir = ( moveTransform.forward * r ) + ( moveTransform.right * ( r * 2 - 1 ) );
    SetPath( moveTransform.position + ( dir * WanderDistance * Time.timeScale ), delegate
    {
      // until character is tired of wandering aimlessly
      WanderRandom();
    } );
  }

  void WanderRandomDestination()
  {
    CleanKnownDestinations();
    // go to random destination
    if( KnownDestinations.Count == 0 )
    {
      WanderRandom();
      return;
    }
    // random known destination
    List<Destination> des = new List<Destination>( KnownDestinations.FindAll( x => x.CommunalDestination == true ) );
    if( Home != null )
      des.Remove( Home );
    if( des.Count == 0 )
    {
      WanderRandom();
      return;
    }
    for( int i = 0; i < 5; i++ )
    {
      WanderDestination = des[ ++WanderDestinationIndex % des.Count ];
      if( WanderDestination != null )
      {
        Vector3 randomVector = Random.insideUnitSphere;
        randomVector.y = 0;
        randomVector *= WanderDestination.ArrivalRadius;
        SetPath( WanderDestination.transform.position + randomVector, delegate()
        {
          CurrentMoveSpeed = 0; 
          PopState();
        } );
        return;
      }
    }
    //default
    WanderRandom();
  }

  void UpdateWander()
  {
    if( Time.time - WanderStartTime > WanderPatience )
    {
      PopState();
      return;
    }
    //    if( Time.time - WanderDirectionChangeStartTime > WanderDirectionChangeInterval )
    //      WanderChangeDirection();
  }

}

