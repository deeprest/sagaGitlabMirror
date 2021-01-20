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

public class Liftbot : Entity, IWorldSelectable
{
  public float flySpeed = 2;
  const float slowSpeed = 200;
  public float waitDuration = 1;
  const float closeEnough = 0.25f;
  //protected float closeEnough { get { return flySpeed * Application.targetFrameRate * Time.timeScale; } }
  int pathIndex = 0;
  public Vector2 origin;
  public Vector2[] path;
  Timer timeout = new Timer();
  public bool pingpong;
  int indexIncrement = 1;
  public bool UseWaitDuration = true;
  public bool IsTriggeredByPlayer;

  protected override void Start()
  {
    base.Start();
    UpdateLogic = UpdateLiftbot;
    UpdateHit = null;
    UpdateCollision = null;
    UpdatePosition = BasicPosition;
    origin = transform.position;
    if( !IsTriggeredByPlayer )
      timeout.Start( waitDuration, null, NextWaypoint );
  }

  protected override void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    base.OnDestroy();
    timeout.Stop( false );
  }

  protected void NextWaypoint()
  {
    int next = pathIndex + indexIncrement;
    if( pingpong && (next >= path.Length || next < 0) )
      indexIncrement = -indexIncrement;
    pathIndex = (pathIndex + indexIncrement) % path.Length;
    //if( flySpeed > 0 )
    //  timeout.Start( DistanceToWaypoint() / flySpeed, null, NextWaypoint );
  }
  
  void UpdateLiftbot()
  {
    /*Vector2 line = path[pathIndex] - path[(pathIndex-1+path.Length) % path.Length];
    Debug.DrawLine( origin+path[pathIndex], origin +path[(pathIndex-1+path.Length) % path.Length], Color.green );*/
    
    if( path.Length > 0 )
    {
      Vector2 delta = origin + path[pathIndex] - (Vector2)transform.position;
      if( delta.sqrMagnitude < closeEnough*closeEnough )
      {
        // WARNING
        // This does not update player position, which creates an immediate an noticeable offset
        // between the player's feet and the liftbot when reaching a waypoint. 
        /*transform.position = origin + path[pathIndex];*/
        
        // Smooth arrival
        velocity = delta * Time.smoothDeltaTime * slowSpeed;
        if( !IsTriggeredByPlayer )
        {
          if( UseWaitDuration )
          {
            if( !timeout.IsActive )
              timeout.Start( waitDuration, null, NextWaypoint );
          }
          else 
            NextWaypoint();
        }
      }
      else
      {
        velocity = delta.normalized * flySpeed;
      }
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
  }

  public void Highlight()
  {
    if( Global.instance.CurrentPlayer != null )
    {
      Global.instance.CurrentPlayer.InteractIndicator.SetActive( true );
      Global.instance.CurrentPlayer.InteractIndicator.transform.position = transform.position;
    }
  }
  public void Unhighlight()
  {
    if( Global.instance.CurrentPlayer != null )
      Global.instance.CurrentPlayer.InteractIndicator.SetActive( false );
  }
  public void Select()
  {
    UseWaitDuration = true;
    if( IsTriggeredByPlayer && (origin + path[pathIndex] - (Vector2)transform.position).sqrMagnitude < closeEnough*closeEnough )
      NextWaypoint();
  }
  public void Unselect()
  {
    UseWaitDuration = false;
  }

  public Vector2 GetPosition()
  {
    return transform.position;
  }
}
