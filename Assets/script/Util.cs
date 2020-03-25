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

  public static bool DoLinesIntersect( float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3 )
  {
    float d = (y3 - y2) * (x1 - x0) - (x3 - x2) * (y1 - y0);
    if( Mathf.Abs( d ) > Mathf.Epsilon )
    {
      float ua = ((x3 - x2) * (y0 - y2) - (y3 - y2) * (x0 - x2)) / d;
      float ub = ((x1 - x0) * (y0 - y2) - (y1 - y0) * (x0 - x2)) / d;
      if( ua < 0.0 || ua > 1.0 || ub < 0.0 || ub > 1.0 ) ua = 0;
      return ua > 0;
    }
    return false;
  }

  // Thank you cecarlsen, Minassian, and CgPanda!
  // https://forum.unity.com/threads/line-intersection.17384/
  public static bool LineSegmentsIntersectionWithPrecisonControl( Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection, float fSelfDefinedEpsilon = 0.0001f )
  {
    UnityEngine.Assertions.Assert.IsTrue( fSelfDefinedEpsilon > 0 );
    float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num;
    float x1lo, x1hi, y1lo, y1hi;
    Ax = p2.x - p1.x;
    Bx = p3.x - p4.x;
    // X bound box test
    if( Ax < 0 )
    {
      x1lo = p2.x;
      x1hi = p1.x;
    }
    else
    {
      x1hi = p2.x;
      x1lo = p1.x;
    }
    if( Bx > 0 )
    {
      if( (x1hi < p4.x && Mathf.Abs( x1hi - p4.x ) > fSelfDefinedEpsilon) || (p3.x < x1lo && Mathf.Abs( p3.x - x1lo ) > fSelfDefinedEpsilon) )
        return false;
    }
    else
    {
      if( (x1hi < p3.x && Mathf.Abs( x1hi - p3.x ) > fSelfDefinedEpsilon) || (p4.x < x1lo && Mathf.Abs( p4.x - x1lo ) > fSelfDefinedEpsilon) )
        return false;
    }
    Ay = p2.y - p1.y;
    By = p3.y - p4.y;
    // Y bound box test
    if( Ay < 0 )
    {
      y1lo = p2.y;
      y1hi = p1.y;
    }
    else
    {
      y1hi = p2.y;
      y1lo = p1.y;
    }
    if( By > 0 )
    {
      if( (y1hi < p4.y && Mathf.Abs( y1hi - p4.y ) > fSelfDefinedEpsilon) || (p3.y < y1lo && Mathf.Abs( p3.y - y1lo ) > fSelfDefinedEpsilon) )
        return false;
    }
    else
    {
      if( (y1hi < p3.y && Mathf.Abs( y1hi - p3.y ) > fSelfDefinedEpsilon) || (p4.y < y1lo && Mathf.Abs( p4.y - y1lo ) > fSelfDefinedEpsilon) )
        return false;
    }
    Cx = p1.x - p3.x;
    Cy = p1.y - p3.y;
    d = By * Cx - Bx * Cy;  // alpha numerator
    f = Ay * Bx - Ax * By;  // both denominator
    // alpha tests
    if( f > 0 )
    {
      if( (d < 0 && Mathf.Abs( d ) > fSelfDefinedEpsilon) || (d > f && Mathf.Abs( d - f ) > fSelfDefinedEpsilon) )
        return false;
    }
    else
    {
      if( (d > 0 && Mathf.Abs( d ) > fSelfDefinedEpsilon) || (d < f && Mathf.Abs( d - f ) > fSelfDefinedEpsilon) )
        return false;
    }
    e = Ax * Cy - Ay * Cx;  // beta numerator
    // beta tests
    if( f > 0 )
    {
      if( (e < 0 && Mathf.Abs( e ) > fSelfDefinedEpsilon) || (e > f) && Mathf.Abs( e - f ) > fSelfDefinedEpsilon )
        return false;
    }
    else
    {
      if( (e > 0 && Mathf.Abs( e ) > fSelfDefinedEpsilon) || (e < f && Mathf.Abs( e - f ) > fSelfDefinedEpsilon) )
        return false;
    }
    // check if they are parallel
    if( Mathf.Abs( f ) < fSelfDefinedEpsilon )
      return false;
    // compute intersection coordinates
    num = d * Ax; // numerator
    intersection.x = p1.x + num / f;
    num = d * Ay;
    intersection.y = p1.y + num / f;
    return true;
  }

  public static float Cross( Vector2 a, Vector2 v )
  {
    return a.x * v.y - a.y * v.x;
  }
  static bool IsZero( float value )
  {
    return Mathf.Abs( value ) < Mathf.Epsilon;
  }

  // https://www.codeproject.com/tips/862988/find-the-intersection-point-of-two-line-segments
  /// <see cref="http://stackoverflow.com/a/14143738/292237"/>
  public static bool LineSegementsIntersect( Vector2 p, Vector2 p2, Vector2 q, Vector2 q2, ref Vector2 intersection, bool considerCollinearOverlapAsIntersect = false )
  {
    Vector2 r = p2 - p;
    Vector2 s = q2 - q;
    float rxs = Cross( r, s );
    float qpxr = Cross( (q - p), r );

    // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
    if( IsZero( rxs ) && IsZero( qpxr ) )
    {
      // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
      // then the two lines are overlapping,
      if( considerCollinearOverlapAsIntersect )
        if( (0 <= Cross( q - p, r ) && Cross( q - p, r ) <= Cross( r, r )) || (0 <= Cross( p - q, s ) && Cross( p - q, s ) <= Cross( s, s )) )
          return true;

      // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
      // then the two lines are collinear but disjoint.
      // No need to implement this expression, as it follows from the expression above.
      return false;
    }

    // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
    if( IsZero( rxs ) && !IsZero( qpxr ) )
      return false;

    // t = (q - p) x s / (r x s)
    var t = Cross( q - p, s ) / rxs;

    // u = (q - p) x r / (r x s)

    var u = Cross( q - p, r ) / rxs;

    // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
    // the two line segments meet at the point p + t r = q + u s.
    if( !IsZero( rxs ) && (0 <= t && t <= 1) && (0 <= u && u <= 1) )
    {
      // We can calculate the intersection point using either t or u.
      intersection = p + t * r;

      // An intersection was found.
      return true;
    }

    // 5. Otherwise, the two line segments are not parallel but do not intersect.
    return false;
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


  public static GameObject GenerateNavMeshForEdgeColliders()
  {
    GameObject go = new GameObject( "EdgeCollider2D Generated Mesh Collider" );
    go.transform.position = Vector3.back;
    go.layer = LayerMask.NameToLayer( "Ignore Raycast" );

    List<CombineInstance> combine = new List<CombineInstance>();
    EdgeCollider2D[] edges = Object.FindObjectsOfType<EdgeCollider2D>();
    for( int e = 0; e < edges.Length; e++ )
    {
      EdgeCollider2D edge = edges[e];
      CombineInstance ci = new CombineInstance();
      ci.transform = edge.transform.localToWorldMatrix;
      ci.mesh = edge.CreateMesh( false, false );
      combine.Add( ci );
    }

    Mesh mesh = new Mesh();
    if( combine.Count == 1 )
      mesh = combine[0].mesh;
    else
      mesh.CombineMeshes( combine.ToArray(), false, false );

    MeshCollider mc = go.AddComponent<MeshCollider>();
    mc.sharedMesh = mesh;
    return go;
  }
}

[System.Serializable]
public struct LineSegment
{
  public Vector3 a;
  public Vector3 b;
}
