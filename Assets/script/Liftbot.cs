using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( Liftbot ) )]
public class LiftbotEdtitor : Editor
{
  void OnSceneGUI()
  {
    Liftbot bot = target as Liftbot;
    if( bot.path != null && bot.path.Length > 0 )
    {
      Vector2[] points = bot.path;
      for( int i = 0; i < points.Length; i++ )
      {
        int next = (i + 1) % points.Length;
        Vector2 segment = points[next] - points[i];
        Handles.DrawLine( (Vector2)bot.transform.position + points[i], (Vector2)bot.transform.position + points[next] );
      }
    }
  }
}
#endif

public class Liftbot : Character
{
  public float flySpeed = 2;
  public float waitDuration = 2;
  public float small = 0.1f;
  int pathIndex = 0;
  Vector2 origin;
  public Vector2[] path;
  Timer timeout = new Timer();
  bool waiting;

  private void OnDestroy()
  {
    timeout.Stop( false );
  }

  void Start()
  {
    CharacterStart();
    UpdateLogic = UpdateAirbot;
    UpdateHit = null;
    UpdateCollision = null;  //BoxCollision;
    UpdatePosition = null;
    origin = transform.position;
    timeout.Start( waitDuration, null, NextWaypoint );
  }

  float DistanceToWaypoint()
  {
    return Vector3.Distance( transform.position, origin + path[pathIndex] );
  }

  void NextWaypoint()
  {
    waiting = false;
    pathIndex = ++pathIndex % path.Length;
    timeout.Stop( false );
    if( flySpeed > 0 )
      timeout.Start( DistanceToWaypoint() / flySpeed, null, NextWaypoint );
  }

  void UpdateAirbot()
  {
    if( path.Length > 0 )
    {
      if( DistanceToWaypoint() < flySpeed * Time.maximumDeltaTime )
      {
        transform.position = origin + path[pathIndex];
        if( !waiting )
        {
          waiting = true;
          timeout.Start( waitDuration, null, NextWaypoint );
        }
      }
      else
      {
        MoveDirection = origin + path[pathIndex] - (Vector2)transform.position;
        velocity = MoveDirection.normalized * flySpeed;
      }
    }
    else
    {
      velocity = Vector2.zero;
    }
  }

  private void FixedUpdate()
  {
    if( pushTimer.IsActive )
      velocity = pushVelocity;

    if( UseGravity )
      velocity.y += -Global.Gravity * Time.fixedDeltaTime;

    if( collideTop )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
    if( collideBottom )
    {
      velocity.x -= (velocity.x * friction) * Time.fixedDeltaTime;
      velocity.y = Mathf.Max( velocity.y, 0 );
    }
    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
    }
    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
    }

    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
    velocity -= (velocity * airFriction) * Time.fixedDeltaTime;
    transform.position += (Vector3)velocity * Time.fixedDeltaTime;

  }

  public override bool TakeDamage( Damage d )
  {
    // absorb hits, but do not take damage
    return true;
  }

  protected override void Die()
  {
    base.Die();
    // todo
  }
}
