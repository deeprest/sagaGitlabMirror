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
  public float sightRange = 6;
  public float flySpeed = 2;
  const float small = 0.1f;
  int pathIndex = 0;
  Vector2 origin;
  public Vector2[] path;

  void Start()
  {
    CharacterStart();
    UpdateLogic = UpdateAirbot;
    UpdateHit = null;
    UpdateCollision = BoxCollision;
    origin = transform.position;
  }

  void UpdateAirbot()
  {
    if( path.Length > 0 )
    {
      if( Vector3.Distance( transform.position, origin + path[pathIndex]) < small )
        pathIndex = ++pathIndex % path.Length;
      MoveDirection = origin + path[pathIndex] - (Vector2)transform.position;
      velocity = MoveDirection.normalized * flySpeed;
    }
    else
    {
      velocity = Vector2.zero;
    }
  }
#if KINEMATIC
  private void FixedUpdate()
  {
    body.MovePosition( ugh + velocity * Time.fixedDeltaTime );
  }
#endif

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
