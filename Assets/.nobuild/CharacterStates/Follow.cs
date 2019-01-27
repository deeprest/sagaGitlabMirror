using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Character
{
  [Header( "Follow" )]
  public bool CanFollow = true;
  //const int MaxFollowers = 9;
  float FollowThrottle;
  [Serialize] public Character FollowCharacter;
  Vector3 lastPositionForFootprint;
  float HealthRegenAccumulator = 0f;
  public List<Character> Followers = new List<Character>();

  public bool ShouldFollow( Character target )
  {
    /*if( target.Rank <= Rank )
        return;
      if( FollowCharacter != null )
      {
        if( target.Rank <= FollowCharacter.Rank )
          return;
      }*/
    AffinityData aff = GetAffinity( target.id );
    if( aff.Value > AffFollow )
    {
      Speak( "ok" );
      return true;
    }
    else
    {
      // rejection
      string say = "...";
      if( aff.Value > AffTalk )
      {
        string[] exp = new string[] {
          "No.",
          "No, thanks.",
          "Nope!",
          "Hmm...",
          "I'm busy.",
          "Nah.",
          "Pass.",
          "Hard pass.",
          "I don't think so.",
          "I can't right now.",
          "Do you even know where you're going?",
          "Try me again later.",
          "Yeah, right.",
          "Do I know you?",
          "You're probably looking for someone else.",
          "Not in the mood.",
          "I don't feel like it."
        };
        say = exp[ Random.Range( 0, exp.Length ) ];
      }
      Speak( say, 2 );
    }
    return false;
  }

  public void Follow( Character target , Interest sourceInterest = null )
  {
    if( !CanFollow || target==null || target == this || target.Followers.Contains(this) )
      return;
    if( FollowCharacter != null )
    {
      FollowCharacter.Followers.Remove( this );
    }
    FollowCharacter = target;
    FollowCharacter.Followers.Add( this );
    PushState( "Follow", sourceInterest );
  }

  // When there are tiers of many Followers, it's easy to get trapped in a room because they do not move out of the way; 
  // because they are not following the player, they are following a follower.
  public Character FindFollowerRecursive()
  {
    if( Followers.Count < Global.Instance.MaxFollowers )
      return this;
    foreach( var c in Followers )
    {
      if( c.Followers.Count < Global.Instance.MaxFollowers )
        return c;
    }
    foreach( var c in Followers )
    {
      Character cha = c.FindFollowerRecursive();
      if( cha != null )
        return cha;
    }
    return null;
  }

  Character[] GetCharactersWithinRadius( float radius )
  {
    List<Character> list = new List<Character>();
    int layermask = LayerMask.GetMask( new string[]{ "Character" } );
    if( Physics.CheckSphere( moveTransform.position, radius, layermask, QueryTriggerInteraction.Ignore ) )
    {
      int count = Physics.OverlapSphereNonAlloc( moveTransform.position, radius, SensorColliders, layermask, QueryTriggerInteraction.Ignore );
      for( int i = 0; i < count; i++ )
      {
        Collider collider = SensorColliders[ i ];
        Character cha = collider.GetComponent<Character>();
        if( cha != this )
          list.Add( cha );
      }
    }
    return list.ToArray();
  }

  void FollowAll( float radius = 8f )
  {
    Speak( "followme" );
    Character[] chas = GetCharactersWithinRadius( radius );
    foreach( var cha in chas )
    {
      if( cha != this )
        cha.Follow( this, null );
    }
  }

  void UnfollowAll()
  {
    Character[] fol = Followers.ToArray();
    foreach( var c in fol )
    {
      if( c == null )
        continue;
      // leave the follow state on the stack, because it might not be the current state, but make the follow character null, which
      // will pop the follow state when it becomes current. 
      c.FollowCharacter = null;
      c.PushState( "Wander" );
      Followers.Remove( c );
    }
  }

  /*public Vector3 GetFollowPosition( Vector3 closestToMe )
  {
    Vector3 closer = moveTransform.position;

    #if ENDLESS_WORLD
    if( World.Instance.EndlessWorld )
    {
      // get closest position, whether regular transform or surrogate.
      // this is so follwing works across world seams
      float closerDistance = Vector3.Distance( moveTransform.position, closestToMe );
      for( int i = 0; i < 8; i++ )
      {
        Vector3 check = moveTransform.position + World.Instance.cameraController.offset[ i ];
        float distance = Vector3.Distance( check, closestToMe );
        if( distance < closerDistance )
        {
          closer = check;
          closerDistance = distance;
        }
      }
    }
    #endif
    return closer;
  }*/

  void ConsiderFollow( Interest interest )
  {
    Character c = interest.go.GetComponent<Character>();
    if( c != null )
      Follow( c, interest );
  }

  void PushFollow()
  {
    //SidestepAvoidance = false;
  }

  void PopFollow()
  {
    //SidestepAvoidance = DefaultSidestepAvoidance;
  }

  void UpdateFollow()
  {
    if( FollowCharacter == null )
    {
      PopState();
      return;
    }
    else
    {
      Vector3 delta = FollowCharacter.moveTransform.position - moveTransform.position;
      delta.y = 0;
      Vector3 NewMoveDirection = delta;
      if( delta.magnitude > Global.Instance.FollowDistanceRun )
        CurrentMoveSpeed = SprintSpeed;
      else
        if( delta.magnitude > Global.Instance.FollowDistanceWalk )
          CurrentMoveSpeed = WalkSpeed;
        else
          if( delta.magnitude < Global.Instance.FollowDistanceTooClose )
          {
            CurrentMoveSpeed = WalkSpeed;
            // move out of the way
            if( Vector3.Dot( -delta.normalized, FollowCharacter.MoveDirection.normalized ) > 0 )
              NewMoveDirection = Vector3.Project( -delta, Vector3.Cross( FollowCharacter.MoveDirection.normalized, Vector3.up ) ).normalized;
            else
              NewMoveDirection = -delta;
          }
          else
            CurrentMoveSpeed = 0f;

      if( !SetPath( moveTransform.position + NewMoveDirection ) )
      {
        Debug.Log( "Follower failed to set path", gameObject );
        Debug.DrawLine( moveTransform.position, moveTransform.position + NewMoveDirection, Color.red, 10f );
        MoveDirection = NewMoveDirection.normalized;
      }
    }
  }

}
