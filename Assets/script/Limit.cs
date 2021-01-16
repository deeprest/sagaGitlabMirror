using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ILimit
{
  bool IsUnderLimit();
}

/*
// Put this in Awake()
if( !Limit.OnCreate( this ) )     
  return;
*/

public class Limit<T> where T : MonoBehaviour
{
  public List<T> All = new List<T>();
  public bool IsUnderLimit(){ return All.Count < UpperLimit; }
  public int UpperLimit = 10;
  public bool EnforceUpper = true;
  public bool AlwaysCreate = false;

  public bool OnCreate( T obj )
  {
    if( EnforceUpper && All.Count >= UpperLimit )
    {
      Object.Destroy( obj.gameObject );
      return false;
    }
    else
    {
      All.Add( obj );
      return true;
    }
  }

  public void OnDestroy( T obj )
  {
    if( !Global.IsQuiting )
      All.Remove( obj );
  }
}

public static class LimitTypes
{
  public static void RegisterType( System.Type type )
  {
    types.Add( type );
  }
  public static List<System.Type> types = new List<System.Type>();
}
