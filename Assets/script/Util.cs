using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Util
{
  public static float DbFromNormalizedVolume( float normalizedVolume )
  {
    return (1 - Mathf.Sqrt( normalizedVolume )) * -80f;
  }

  public static float NormalizedVolumeFromDb( float db )
  {
    return Mathf.Pow( -((db / -80f) - 1f), 2f );
  }

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

  public static void Screenshot()
  {
    string now = System.DateTime.Now.Year.ToString() +
                   System.DateTime.Now.Month.ToString( "D2" ) +
                   System.DateTime.Now.Day.ToString( "D2" ) + "." +
                   System.DateTime.Now.Hour.ToString( "D2" ) +
                   System.DateTime.Now.Minute.ToString( "D2" ) +
                   System.DateTime.Now.Second.ToString( "D2" );
    ScreenCapture.CaptureScreenshot( Application.persistentDataPath + "/" + now + ".png" );
  }

  public static Vector2 Project2D( Vector2 a, Vector2 b )
  {
    return new Vector2( (Vector2.Dot( a, b ) / (b.x * b.x + b.y * b.y)) * b.x, (Vector2.Dot( a, b ) / (b.x * b.x + b.y * b.y)) * b.y );
  }

  public static void DirectoryCopy( string sourceDirName, string destDirName, bool copySubDirs = true )
  {
    // if you want to copy the source directory it self (similar that you have 
    // right clicked on source folder and clicked copy then in the destination 
    // folder you clicked paste) you should use like this:
    // DirectoryCopy(source, Path.Combine(dest, new DirectoryInfo(source).Name));

    // Get the subdirectories for the specified directory.
    DirectoryInfo dir = new DirectoryInfo( sourceDirName );

    if( !dir.Exists )
      throw new DirectoryNotFoundException( "Source directory does not exist or could not be found: " + sourceDirName );

    DirectoryInfo[] dirs = dir.GetDirectories();
    // If the destination directory doesn't exist, create it.
    if( !Directory.Exists( destDirName ) )
    {
      Directory.CreateDirectory( destDirName );
    }

    // Get the files in the directory and copy them to the new location.
    FileInfo[] files = dir.GetFiles();
    foreach( FileInfo file in files )
    {
      string temppath = Path.Combine( destDirName, file.Name );
      file.CopyTo( temppath, false );
    }

    // If copying subdirectories, copy them and their contents to new location.
    if( copySubDirs )
    {
      foreach( DirectoryInfo subdir in dirs )
      {
        string temppath = Path.Combine( destDirName, subdir.Name );
        DirectoryCopy( subdir.FullName, temppath, copySubDirs );
      }
    }
  }

  public class Command
  {
    public string cmd;
    public string args;
    public string dir;
  }
  public class CommandResult
  {
    public string stdout;
    public string stderr;
    public int returnCode;
  }

  public static void ExecuteCommands( Command[] cmds, bool shell = false )
  {
    foreach( Command c in cmds )
      Execute( c, shell );
  }

  public static CommandResult Execute( Command c, bool shell = false )
  {
    Debug.Log( "Running... " + c.cmd + " " + c.args + " in " + c.dir );
    System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
    start.UseShellExecute = shell;
    start.RedirectStandardOutput = !shell;
    start.RedirectStandardError = !shell;
    start.FileName = c.cmd; //"/usr/bin/python";
    start.Arguments = c.args;
    start.WorkingDirectory = c.dir;

    CommandResult res = new CommandResult();
    System.Diagnostics.Process process = System.Diagnostics.Process.Start( start );
    if( shell )
    {
      res.stdout = process.StandardOutput.ReadToEnd();
      res.stderr = process.StandardError.ReadToEnd();
      process.WaitForExit();
      Debug.Log( res.stdout );
      if( res.stderr.Length > 0 )
        Debug.LogError( res.stderr );

      res.returnCode = process.ExitCode;
    }
    return res;
  }
}

[System.Serializable]
public struct LineSegment
{
  public Vector3 a;
  public Vector3 b;
}