using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Character
{
  [Header( "Gather" )]
  // Gather: carry objects to a destination
  public Destination GatherDestination;
  Transform GatherObject;
  Vector3 GatherObjectLastKnownPosition;
  float GatherUpdatePatience = 30f;
  float GatherStartTime;


  // gather, collect, harvest
  void ConsiderGather( Interest interest )
  {
    if( KnownDestinations.Count == 0 )
      return;
    if( !AllowedToHoldObject( interest.go.transform ) )
      return;
    // NOTE If there are too many objects to consider for gather, then this character may never see the objects in the list of interests.
    // So either have a large max active interest limit, or remove the need for line of sight.
    //if( CanSeeObject( interest.go, true ) )
    {
      // remove null destinations
      List<Destination> remove = new List<Destination>();
      foreach( var i in KnownDestinations )
        if( i == null )
          remove.Add( i );
      foreach( var i in remove )
        KnownDestinations.Remove( i );
      // find appropriate destinations for the gather object interest tag
      List<Destination> AppropriateDestinations = KnownDestinations.FindAll( x => x.GatherDestinationTags.Contains( interest.tag ) );
      Destination closest = null;
      float closestDistance = float.MaxValue;
      foreach( var dest in AppropriateDestinations )
      {
        CarryObjectPile cop = dest.GetComponent<CarryObjectPile>();
        if( cop != null && cop.count == cop.mount.Length )
          continue;
        float sqrDistance = Vector3.SqrMagnitude( dest.transform.position - moveTransform.position );
        if( sqrDistance < closestDistance )
        {
          closest = dest;
          closestDistance = sqrDistance;
        }
      }
      if( closest != null )
      {
        if( Vector3.SqrMagnitude( interest.objectPositionWhenSensed - closest.transform.position ) > closest.ArrivalRadius * closest.ArrivalRadius )
        {
          // HACK Transition to same state has a bug. Pop state here to avoid setting GatherObject to null just before BeginGather is called.
          if( CurrentState.Name == "Gather" )
            PopState();
          GatherDestination = closest;
          GatherObjectLastKnownPosition = interest.objectPositionWhenSensed;
          GatherObject = interest.go.transform;
          PushState( "Gather", interest );
        }
      }
    }
  }

  void PushGather()
  {
    if( GatherObject == null || GatherDestination == null )
    {
      PopState();
      return;
    }
    CurrentMoveSpeed = WalkSpeed;
    GatherStartTime = Time.time;

    if( !SetPath( GatherObject.position, delegate
    {
      if( GatherDestination!=null && GatherObject != null && AllowedToHoldObject( GatherObject ) && WithinHoldRange( GatherObject ) )
      {
        HoldObject( GatherObject );
        Vector3 DropoffPoint = GatherDestination.transform.position + RandomFlatVector() * GatherDestination.ArrivalRadius;
        DropoffPoint.y = Global.Instance.GlobalSpriteOnGroundY;
        if( SetPath( DropoffPoint, delegate
        {
          //Transform heldObject = HeldObject;
          DropObject();

          /*if( heldObject.GetComponent<Rigidbody>()==null )
            heldObject.transform.position = DropoffPoint;
          else
            heldObject.transform.position = DropoffPoint + Vector3.up * 0.2f;
            */

          /*Swag swag = heldObject.GetComponent<Swag>();
          if( swag != null )
          {
            LerpToTarget lerp = swag.lerp;
            lerp.localOffset = DropoffPoint;
            lerp.targetTransform = null;
            lerp.localOffset += Vector3.up * 0.2f;
            lerp.Continuous = false;
            lerp.Rigidbody = true;
            lerp.force = 20;
            lerp.UseMaxDistance = true;
            lerp.MaxDistance = 1f;
            lerp.duration = 0.5f; 
            lerp.Scale = false;
            lerp.lerpType = LerpToTarget.LerpType.Linear;
            lerp.enabled = true;
          }*/
          PopState();
        } ) )
        {
          DestinationRadius = GatherDestination.ArrivalRadius;
        }
        else
        {
          Debug.Log( "failed path to gather destination", GatherDestination );
          PopState();
        }
      }
      else
        PopState();
    } ) )
    {
      Debug.Log( "failed path to gather object", GatherObject );
      PopState();
    }
  }

  void UpdateGather()
  {
    if( Time.time - GatherStartTime > GatherUpdatePatience )
    {
      PopState();
      return;
    }

    if( GatherObject == null )
    {
      PopState();
      return;
    }

    if( GatherObject != HeldObject )
    {
      // if within view of where the object was last seen
      const float GatherSourceRadius = 2f;
      if( Vector3.SqrMagnitude( moveTransform.position - GatherObjectLastKnownPosition ) < GatherSourceRadius * GatherSourceRadius )
      {
        // if we can see the object is held by another character then ignore it
        if( CanSeeObject( GatherObject.gameObject, false ) )
        {
          CarryObject swag = GatherObject.GetComponent<CarryObject>();
          if( swag != null && swag.IsHeld )
          {
            PopState();
            return;
          }
        }
      }
    }
  }

  void PopGather()
  {
    ClearPath();
    DropObject();
    GatherObject = null;
  }
}

