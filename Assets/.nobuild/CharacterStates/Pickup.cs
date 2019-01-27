using UnityEngine;
using System.Collections;

public partial class Character
{
  [Header( "Pickup" )]
  // Pickup: move to object and acquire (put in inventory)
  GameObject ObjectToPickup;
  Vector3 PickupObjectLastKnownPosition;
  float PickupStartTime;
  float PickupPatience = 20;


  // physically move to an object and then acquire it
  void PickupObject( GameObject go )
  {
    ObjectToPickup = go;
    PickupObjectLastKnownPosition = go.transform.position;
    PushState( "Pickup" );
  }

  void ConsiderPickup( Interest interest )
  {
    if( !CanPickupItems )
      return;
    ObjectToPickup = interest.go;
    PickupObjectLastKnownPosition = interest.go.transform.position;
    PushState( "Pickup", interest );
  }

  void PushPickup()
  {
    CurrentMoveSpeed = WalkSpeed;
    PickupStartTime = Time.time;
    SidestepAvoidance = true;

    if( !SetPath( ObjectToPickup.transform.position, delegate
    {
      if( ObjectToPickup == null )
      {
        PopState();
        return;
      }
      AcquireObject( ObjectToPickup.gameObject );
      PopState();
    } ) )
    {
      Debug.Log( "failed path to pickup object", GatherObject );
      PopState();
    }
  }

  void UpdatePickup()
  {
    // TODO patience or forward progress check
    // similar to gather:
    if( Time.time - PickupStartTime > PickupPatience )
    {
      PopState();
      return;
    }

    if( ObjectToPickup == null )
    {
      PopState();
      return;
    }

    if( ObjectToPickup.transform != HeldObject )
    {
      // if within view of where the object was last seen
      const float PickupSourceRadius = 2f;
      if( Vector3.SqrMagnitude( moveTransform.position - PickupObjectLastKnownPosition ) < PickupSourceRadius * PickupSourceRadius )
      {
        // if we can see the object is held by another character then ignore it
        if( CanSeeObject( ObjectToPickup, false ) )
        {
          CarryObject swag = ObjectToPickup.GetComponent<CarryObject>();
          if( swag != null && swag.IsHeld )
          {
            PopState();
            return;
          }
        }
      }
    }
  }

  void PopPickup()
  {
    ClearPath();
    SidestepAvoidance = DefaultSidestepAvoidance;
  }
}

