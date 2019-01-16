using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ILimit
{
  bool IsUnderLimit();
}

public class Limit<T> where T : MonoBehaviour
{
  public List<T> All = new List<T>();
  public bool IsUnderLimit(){ return All.Count < Upper; }
  public int Upper = 10;
  public bool EnforceUpper = true;

  public bool OnCreate( T obj )
  {
    if( EnforceUpper && All.Count >= Upper )
    {
      GameObject.Destroy( obj.gameObject );
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
