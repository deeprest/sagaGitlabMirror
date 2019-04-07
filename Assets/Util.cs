using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Util
{
/*  public static void Log(string message, [System.Runtime.CompilerServices.CallerFilePath] string filePath = "", [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
  {
    Debug.Log( lineNumber + " " + message );
  }*/

  public static float NormalizeAngle( float angle )
  {
    // to range [-180,180]
    while( angle > 180 )
      angle -= 360;
    while( angle < -180 )
      angle += 360;
    return angle;
  }

  public static string Timestamp()
  {
    return System.DateTime.Now.Year.ToString() +
      System.DateTime.Now.Month.ToString( "D2" ) +
      System.DateTime.Now.Day.ToString( "D2" ) + "." +
      System.DateTime.Now.Hour.ToString( "D2" ) +
      System.DateTime.Now.Minute.ToString( "D2" );
  }
}