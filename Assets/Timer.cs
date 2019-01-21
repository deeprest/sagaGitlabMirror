using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timer
{
  // New timers are held until the next frame, so that adding timers within the callback
  // of another timer does not modify the ActiveTimers collection while iterating.
  public static List<Timer> NewTimers = new List<Timer>();
  //  public static List<Timer> InactiveTimers = new List<Timer>();
  public static List<Timer> ActiveTimers = new List<Timer>();
  public static List<Timer> RemoveTimers = new List<Timer>();
    

  public static void UpdateTimers()
  {
    ActiveTimers.AddRange( NewTimers );
    NewTimers.Clear();

    foreach( var timer in ActiveTimers )
    {
      if( timer.IsActive )
        timer.Update();
      else
        RemoveTimers.Add( timer );

    }
    foreach( var timer in RemoveTimers )
      ActiveTimers.Remove( timer );
    RemoveTimers.Clear();
  }


  public Timer()
  {
    active = false;
    repeat = false;
//    InactiveTimers.Add( this );
  }

  public Timer( float duration, System.Action<Timer> UpdateDelegate, System.Action CompleteDelegate )
  {
    active = true;
    NewTimers.Add( this );
    StartTime = Time.time;
    Duration = duration;
    repeat = false;
    OnUpdate = UpdateDelegate;
    OnComplete = CompleteDelegate;
  }

  public Timer( int loops, float interval, System.Action<Timer> IntervalDelegate, System.Action CompleteDelegate )
  {
    Start( loops, interval, IntervalDelegate, CompleteDelegate );
  }

  public void Start( float duration, System.Action<Timer> UpdateDelegate, System.Action CompleteDelegate )
  {
    active = true;
    NewTimers.Add( this );
    StartTime = Time.time;
    Duration = duration;
    repeat = false;
    OnUpdate = UpdateDelegate;
    OnComplete = CompleteDelegate;
  }

  public void Start( int loops, float interval, System.Action<Timer> IntervalDelegate, System.Action CompleteDelegate )
  {
    active = true;
    NewTimers.Add( this );
    StartTime = Time.time;
    Interval = interval;
    IntervalStartTime = StartTime;
    Duration = interval * loops;
    repeat = true;
    OnInterval = IntervalDelegate;
    // OnUpdate
    OnComplete = CompleteDelegate;
  }

  public void Stop( bool callOnComplete )
  {
    if( callOnComplete && OnComplete != null )
      OnComplete();
    active = false;
  }

  public void Update()
  {
    if( active )
    {

      if( Time.time - StartTime > Duration )
      {
        active = false;
        if( OnComplete != null )
          OnComplete();
      }
      else if( repeat )
      {
        if( Time.time - IntervalStartTime > Interval )
        {
          IntervalStartTime = Time.time;
          if( OnInterval != null )
            OnInterval( this );
        }
        if( OnUpdate != null )
          OnUpdate( this );
      }
      else
      {
        if( OnUpdate != null )
          OnUpdate( this );
      }
    }
  }

  public void SetProgress( float progressSeconds )
  {
    StartTime = Time.time - progressSeconds;
  }

  public bool IsActive{ get { return active; } }

  public float ProgressSeconds{ get { return Time.time - StartTime; } }

  public float ProgressNormalized{ get { return Mathf.Clamp( ( Time.time - StartTime ) / Duration, 0, 1 ); } }

  bool active;
  float StartTime;
  float Duration;
  float Interval;
  float IntervalStartTime;
  bool repeat;
  System.Action<Timer> OnUpdate;
  System.Action<Timer> OnInterval;
  System.Action OnComplete;
}