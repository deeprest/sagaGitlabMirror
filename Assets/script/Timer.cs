using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct TimerParams
{
  public bool unscaledTime;
  public float duration;
  
  public bool repeat;
  public int loops;
  public float interval;

  public System.Action<Timer> UpdateDelegate;
  public System.Action CompleteDelegate;
}

public class Timer
{
  // New timers are held until the next frame, so that adding timers within the callback
  // of another timer does not modify the ActiveTimers collection while iterating.
  public static List<Timer> NewTimers = new List<Timer>();
  public static List<Timer> ActiveTimers = new List<Timer>();
  public static List<Timer> RemoveTimers = new List<Timer>();


  public static void UpdateTimers()
  {
    // remove first so that timers in both new and remove lists still get added.
    foreach( var timer in RemoveTimers )
      ActiveTimers.Remove( timer );
    RemoveTimers.Clear();
    ActiveTimers.AddRange( NewTimers );
    NewTimers.Clear();
    foreach( var timer in ActiveTimers )
    {
      if( timer.IsActive )
        timer.Update();
      else
        RemoveTimers.Add( timer );
    }
  }


  public Timer()
  {
    active = false;
    repeat = false;
  }

  public Timer( float duration, System.Action<Timer> UpdateDelegate, System.Action CompleteDelegate )
  {
    Start( duration, UpdateDelegate, CompleteDelegate );
  }

  public Timer( int loops, float interval, System.Action<Timer> IntervalDelegate, System.Action CompleteDelegate )
  {
    Start( loops, interval, IntervalDelegate, CompleteDelegate );
  }

  public void Start( TimerParams param )
  {
    if( !active )
      NewTimers.Add( this );
    active = true;
    unscaledTime = param.unscaledTime;
    StartTime = time;
    if( param.repeat )
    {
      Interval = param.interval;
      IntervalStartTime = StartTime;
      Duration = param.interval * param.loops;
    }
    else
    {
      Duration = param.duration;
    }
    OnUpdate = param.UpdateDelegate;
    OnComplete = param.CompleteDelegate;
  }

  public void Start( float duration )
  {
    if( !active )
      NewTimers.Add( this );
    active = true;
    StartTime = time;
    Duration = duration;
    repeat = false;
  }

  public void Start( float duration, System.Action<Timer> UpdateDelegate, System.Action CompleteDelegate )
  {
    if( !active )
      NewTimers.Add( this );
    active = true;
    StartTime = time;
    Duration = duration;
    repeat = false;
    OnUpdate = UpdateDelegate;
    OnComplete = CompleteDelegate;
  }

  public void Start( int loops, float interval, System.Action<Timer> IntervalDelegate, System.Action CompleteDelegate )
  {
    if( !active )
      NewTimers.Add( this );
    active = true;
    StartTime = time;
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
    active = false;
    RemoveTimers.Add( this );
    if( callOnComplete && OnComplete != null )
      OnComplete();
  }

  public void Update()
  {
    if( active )
    {
      if( time - StartTime > Duration )
      {
        Stop( true );
      }
      else if( repeat )
      {
        if( time - IntervalStartTime > Interval )
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
    StartTime = time - progressSeconds;
  }

  public bool IsActive { get { return active; } }

  public float ProgressSeconds { get { return time - StartTime; } }

  public float ProgressNormalized{ get{ return Mathf.Clamp( (time - StartTime) / Duration, 0, 1 ); }}

  float time { get { return unscaledTime ? Time.unscaledTime : Time.time; } }
  bool active = false;
  public bool unscaledTime;
  float StartTime;
  float Duration;
  float Interval;
  float IntervalStartTime;
  bool repeat;
  System.Action<Timer> OnUpdate;
  System.Action<Timer> OnInterval;
  System.Action OnComplete;
}