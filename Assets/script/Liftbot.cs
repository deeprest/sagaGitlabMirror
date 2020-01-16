using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( Liftbot ) )]
public class LiftbotEdtitor : Editor
{
  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    float length = 0;
    Liftbot bot = target as Liftbot;
    if( bot.path != null && bot.path.Length > 0 )
    {
      Vector2[] points = bot.path;
      for( int i = 0; i < points.Length; i++ )
      {
        if( bot.pingpong && i + 1 == points.Length )
        {
          length *= 2;
          break;
        }
        int next = (i + 1) % points.Length;
        Vector2 segment = points[next] - points[i];
        length += segment.magnitude;
      }
    }
    EditorGUILayout.LabelField( "path length", length.ToString() );
    float duration = (length / bot.flySpeed) + (bot.pingpong ? bot.path.Length * 2 - 2 : bot.path.Length) * bot.waitDuration;
    EditorGUILayout.LabelField( "return time", duration.ToString() );
  }

  void OnSceneGUI()
  {
    Liftbot bot = target as Liftbot;
    if( bot.path != null && bot.path.Length > 0 )
    {
      Vector2[] points = bot.path;
      for( int i = 0; i < points.Length; i++ )
      {
        if( bot.pingpong && i + 1 == points.Length )
          break;
        int next = (i + 1) % points.Length;

        Vector2 origin = bot.transform.position;
        if( Application.isPlaying )
          origin = bot.origin;
        Handles.DrawLine( origin + points[i], origin + points[next] );
      }
    }
  }
}
#endif

public class Liftbot : Character
{
  public float flySpeed = 2;
  public float waitDuration = 2;
  int pathIndex = 0;
  public Vector2 origin;
  public Vector2[] path;
  Timer timeout = new Timer();
  bool waiting;
  public bool pingpong = false;
  int indexIncrement = 1;

  private void OnDestroy()
  {
    timeout.Stop( false );
  }

  void Start()
  {
    CharacterStart();
    UpdateLogic = UpdateAirbot;
    UpdateHit = null;
    UpdateCollision = null;
    UpdatePosition = BasicPosition;
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
    int next = pathIndex + indexIncrement;
    if( pingpong && (next >= path.Length || next < 0) )
      indexIncrement = -indexIncrement;
    pathIndex = (pathIndex + indexIncrement) % path.Length;
    timeout.Stop( false );
    //if( flySpeed > 0 )
    //  timeout.Start( DistanceToWaypoint() / flySpeed, null, NextWaypoint );
  }

  float closeEnough { get { return flySpeed * Time.maximumDeltaTime * Time.timeScale; } }

  void UpdateAirbot()
  {
    if( !waiting && path.Length > 0 )
    {
      Vector2 delta = origin + path[pathIndex] - (Vector2)transform.position;
      float dot = Vector2.Dot( velocity, delta );
      if( DistanceToWaypoint() < closeEnough || dot < 0 )
      {
        velocity = Vector2.zero;
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
