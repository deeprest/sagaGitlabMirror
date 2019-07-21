//#define REFERENCE_IMPLEMENTATION
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System.Text;
using System.Reflection;


public static class JsonUtil
{
  public static void SerializeValue( JsonWriter writer, System.Type type, object value )
  {
    if( type == typeof(System.String) )
      writer.Write( (string)value );
    else
    if( type == typeof(System.Int32) )
      writer.Write( (System.Int32)value );
    else
    if( type == typeof(System.Boolean) )
      writer.Write( (System.Boolean)value );
    else
    if( type == typeof(System.Single) )
      writer.Write( System.Math.Round( (System.Single)value, 3 ) );
    else
    if( type == typeof(Vector3) )
      JsonUtil.WriteVector( writer, (Vector3)value );
    else
      writer.Write( null );
  }

  public static void WriteVector( JsonWriter writer, Vector3 vec )
  {
    writer.WriteArrayStart();
    writer.Write( System.Math.Round( vec.x, 3 ) );
    writer.Write( System.Math.Round( vec.y, 3 ) );
    writer.Write( System.Math.Round( vec.z, 3 ) );
    writer.WriteArrayEnd();
  }

  public static Vector3 ReadVector( JsonData data )
  {
    return new Vector3( (float)data[ 0 ].GetReal(), (float)data[ 1 ].GetReal(), (float)data[ 2 ].GetReal() );
  }

  public static Vector3 ReadVector( Vector3 defaultValue, JsonData data, string key )
  {
    if( data.Keys.Contains( key ) )
      return new Vector3( (float)data[ key ][ 0 ].GetReal(), (float)data[ key ][ 1 ].GetReal(), (float)data[ key ][ 2 ].GetReal() );
    return defaultValue;
  }

  public static T Read<T>( T defaultValue, JsonData data, params string[] list )
  {
    if( data==null )
      return defaultValue;
    JsonData obj = data;
    for( int i = 0; i < list.Length; i++ )
    {
      string key = list[ i ];
      if( obj.Keys.Contains( key ) )
      {
        if( i == list.Length - 1 )
        {
          JsonData value = obj[ key ];
          if( value == null )
            return (T)System.Convert.ChangeType( null, typeof(T) );
          ;
          JsonType type = value.GetJsonType();
          switch( type )
          {
            case JsonType.None:
              Debug.LogError( "should be value" );
              break;
            case JsonType.Object:
              return (T)System.Convert.ChangeType( value, typeof(T) );
            case JsonType.Array:
              Debug.LogError( "should be value" );
              break;
            case JsonType.String:
              return (T)System.Convert.ChangeType( value.GetString(), typeof(T) );
            case JsonType.Natural:
              return (T)System.Convert.ChangeType( value.GetNatural(), typeof(T) );
            case JsonType.Real:
              return (T)System.Convert.ChangeType( value.GetReal(), typeof(T) );
            case JsonType.Boolean:
              return (T)System.Convert.ChangeType( value.GetBoolean(), typeof(T) );
          }
        }
        else
          obj = obj[ key ];
      }
    }
    return defaultValue;
  }
}

public class SerializedComponent : MonoBehaviour
{
  public int id;

  public virtual void BeforeSerialize()
  {
  }

  public virtual void AfterDeserialize()
  {
  }
}

public class SerializeAttribute : PropertyAttribute
{
  // this attribute marks which fields to serialize
}

// Do not subclass this. Subclass SerializedComponent instead.
//[ExecuteInEditMode]
[SelectionBase]
public sealed class SerializedObject : MonoBehaviour
{
  public static Dictionary<int,SerializedObject> serializedObjects = new Dictionary<int, SerializedObject>();
  public static Dictionary<int,SerializedComponent> serializedComponents = new Dictionary<int, SerializedComponent>();
  // stored id, runtime id
  //public static Dictionary<int,int> idMap = new Dictionary<int, int>();

  public static int GlobalInstanceID = 0;
  public bool serialize = true;
  public int id = 0;
  List<SerializedComponent> components = new List<SerializedComponent>();
  Dictionary<string,int> refs = new Dictionary<string, int>();

  #if REFERENCE_IMPLEMENTATION
  public static void WriteScene( string sceneName )
  {
    foreach( var so in serializedObjects )
    {
      so.Value.BeforeSerialize();
    }

    JsonWriter writer = new JsonWriter();
    writer.PrettyPrint = true;

    writer.WriteObjectStart();
    writer.WritePropertyName( "objects" );
    writer.WriteArrayStart();
    foreach( var pair in serializedObjects )  
    {
      if( pair.Value.serialize )
      {
        writer.WriteObjectStart();
        pair.Value.Serialize( writer );
        writer.WriteObjectEnd(); 
      }
    }
    writer.WriteArrayEnd();
    writer.WriteObjectEnd();

    string dataFilePath = Application.persistentDataPath + "/" + sceneName + "/" + "scene.json";
    File.WriteAllText( dataFilePath, writer.ToString() );
  }

  public static void ReadScene( string sceneName )
  {
    idMap.Clear(); 
    serializedObjects.Clear();
    serializedComponents.Clear();

    string dataFilePath = Application.persistentDataPath + "/" + sceneName + "/" + "scene.json";
    File.Copy( dataFilePath, dataFilePath + "-backup", true );

    string json = "";
    try
    {
      json = File.ReadAllText( dataFilePath );
    }
    catch(FileNotFoundException)
    {
      Debug.Log( "file not found: " + dataFilePath );
    }
    if( json.Length > 0 )
    {
      JsonReader reader = new JsonReader( json );
      JsonData data = JsonMapper.ToObject( reader );
      JsonData obs = data["objects"];
      foreach( JsonData c in obs )
      {
        Deserialize( c );
      }
    }

    // serialized objects may be created in this step, so create a copy of the list to iterate over.
    SerializedObject[] sobs = new SerializedObject[serializedObjects.Values.Count];
    serializedObjects.Values.CopyTo(sobs,0);
    foreach( var so in sobs )
    {
      so.AfterDeserialize();
    }
  }
#endif

  public static SerializedObject ResolveObjectFromId( int id )
  {
    if( serializedObjects.ContainsKey( id ) )
      return serializedObjects[ id ];
//    if( idMap.ContainsKey( id ) && serializedObjects.ContainsKey( idMap[ id ] ) )
//      return serializedObjects[ idMap[ id ] ];
    Debug.LogWarning( "failed to resolve object: " + id );
    return null;
  }

  public static SerializedComponent ResolveComponentFromId( int id )
  {
    if( serializedComponents.ContainsKey( id ) )
      return serializedComponents[ id ];
//    if( idMap.ContainsKey( id ) && serializedComponents.ContainsKey( idMap[ id ] ) )
//      return serializedComponents[ idMap[ id ] ];
    Debug.LogWarning( "failed to resolve component: " + id );
    return null;
  }

  public static T ResolveComponentFromId<T>( int id ) where T : SerializedComponent
  {
    if( serializedComponents.ContainsKey( id ) )
    {
      SerializedComponent sc = serializedComponents[ id ];
      if( sc.GetType() != typeof(T) )
      {
        Debug.LogWarning( "resolvecomponent type mismatch: " + sc.GetType().ToString() + " " + typeof(T).ToString() );
        return default(T);
      }
      return (T)sc;
    }
    //    if( idMap.ContainsKey( id ) && serializedComponents.ContainsKey( idMap[ id ] ) )
    //      return serializedComponents[ idMap[ id ] ];
    Debug.LogWarning( "failed to resolve component: " + id );
    return null;
  }


  public void OnValidate()
  {
    gameObject.name = NormalizeName( gameObject.name );
  }

  public static string NormalizeName( string na )
  {
    int index = na.LastIndexOf( '(' );
    if( index == -1 )
      return na;
    return na.Substring( 0, index ).TrimEnd( new char[]{ ' ' } );
  }

  void Awake()
  {
    gameObject.name = NormalizeName( gameObject.name );
//    id = gameObject.GetInstanceID() ;
    id = ++GlobalInstanceID;

    if( Application.isPlaying )
    {
      if( serializedObjects.ContainsKey( id ) )
      {
        Debug.LogError( "SerializedObject key already exists! " + name + " " + id );
        GameObject.Destroy( gameObject );
      }
      else
      {
        AddSerializedObject( this );
      }
    }
  }

  public static void RemoveSerializedObject( SerializedObject so )
  {
    serializedObjects.Remove( so.id );
    //so.gameObject.GetComponentsInChildren<SerializedComponent>( so.components );
    foreach( var cmp in so.components )
    {
      serializedComponents.Remove( cmp.id );
    }
  }

  public static void AddSerializedObject( SerializedObject so )
  {
    serializedObjects.Add( so.id, so );
    so.gameObject.GetComponentsInChildren<SerializedComponent>( so.components );
    foreach( var cmp in so.components )
    {
      cmp.id = ++GlobalInstanceID;
      serializedComponents.Add( cmp.id, cmp );
    }
  }

  void OnDestroy()
  {
    if( !Global.IsQuiting )
    {
      RemoveSerializedObject( this );
    }
  }

  public static float RoundFloat( float value )
  {
    return Mathf.Round( value * 1000 ) / 1000;
  }

  public void Serialize( JsonWriter writer )
  {
    writer.WritePropertyName( "id" );
    writer.Write( id );
    writer.WritePropertyName( "resource" );
    writer.Write( gameObject.name );
    writer.WritePropertyName( "position" );
    JsonUtil.WriteVector( writer, transform.position );
    if( transform.rotation != Quaternion.identity )
    {
      writer.WritePropertyName( "rotation" );
      JsonUtil.WriteVector( writer, transform.rotation.eulerAngles );
    }
      
    // NOTE only one SerializedComponent of a given type per GameObject is supported.
    // TODO check for multiple components of a given type and print warning

    GetComponentsInChildren<SerializedComponent>( components );
    if( components.Count > 0 )
    {
      writer.WritePropertyName( "components" );
      writer.WriteArrayStart();
      foreach( var cmp in components )
      {
        System.Type cmpType = cmp.GetType();
        writer.WriteObjectStart();
        writer.WritePropertyName( "type" );
        writer.Write( cmpType.Name );
        writer.WritePropertyName( "id" );
        writer.Write( cmp.id );

        System.Reflection.FieldInfo[] fis = cmpType.GetFields();
        foreach( var fi in fis )
        {
          if( fi.IsDefined( typeof(SerializeAttribute), true ) )
          {
            object value = fi.GetValue( cmp );
            if( value == null )
              continue;
            if( fi.FieldType.IsSubclassOf( typeof(SerializedComponent) ) )
            {
              SerializedComponent sc = (SerializedComponent)value;
              if( sc == null )
                continue;
              SerializedObject so = sc.GetComponentInParent<SerializedObject>();
              if( so != null )
              {
                writer.WritePropertyName( "@" + fi.Name );
                writer.Write( sc.id );
              }
            }
            else
            if( value is IList )
            {
              IList list = value as IList;
              if( list.Count > 0 )
              {
                bool isListOfReferences = false;
                string propName = fi.Name;
                System.Type type = value.GetType();
                System.Type elementType = null;

                if( fi.FieldType.IsGenericType )
                {
                  elementType = type.GetGenericArguments()[ 0 ];
                  if( elementType.IsSubclassOf( typeof(SerializedComponent) ) )
                  {
                    // collection of serialized components
                    propName = "@" + propName;
                    isListOfReferences = true;
                  }
                }
                if( fi.FieldType.IsArray )
                {
                  elementType = list.GetType().GetElementType();
                }

                if( isListOfReferences )
                {
                  Debug.LogError( "Serializing an array of references is not implemented." );
                  /*writer.WritePropertyName( propName );
                  writer.WriteArrayStart();
                  foreach( SerializedComponent obj in value as IEnumerable )
                    writer.Write( obj.GetInstanceID() );
                  writer.WriteArrayEnd();*/
                }
                else
                {
                  writer.WritePropertyName( propName );
                  writer.WriteArrayStart();
                  foreach( var obj in list )
                    JsonUtil.SerializeValue( writer, elementType, obj );
                  writer.WriteArrayEnd();
                }
              }
            }
            else
            {
              writer.WritePropertyName( fi.Name );
              JsonUtil.SerializeValue( writer, fi.FieldType, value );
            }
          }
        }
        writer.WriteObjectEnd();
      }
      writer.WriteArrayEnd();
    }
  }

  public static GameObject Deserialize( JsonData data )
  {
    string resourceName = data[ "resource" ].GetString();
    Vector3 position = JsonUtil.ReadVector( data[ "position" ] );
    Vector3 rotation = Vector3.zero;
    if( data.Keys.Contains( "rotation" ) )
      rotation = JsonUtil.ReadVector( data[ "rotation" ] );
      
    GameObject go;
    if( Application.isEditor && !Application.isPlaying )
      go = GameObject.Instantiate( Resources.Load<GameObject>( "serialized/" + resourceName ), position, Quaternion.Euler( rotation ), null );
    else
      go = Global.instance.Spawn( resourceName, position, Quaternion.Euler( rotation ), null, false, false );
    if( go != null )
    {
      SerializedObject so = go.GetComponent<SerializedObject>();
      if( so == null )
      {
        // This can happen after name changes or removing the SerializedObject component from a prefab.
        Destroy( go );
        return null;
      }
        
      serializedObjects.Remove( so.id );
      so.id = (int)data[ "id" ].GetNatural();
      serializedObjects.Add( so.id, so );

      if( data.Keys.Contains( "components" ) )
      {
        JsonData cmps = data[ "components" ];
        foreach( JsonData cmpData in cmps )
        {
          string t = cmpData[ "type" ].ToString();
          System.Type stringType = System.Type.GetType( t );
          if( stringType == null )
            continue;
          Component component = go.GetComponentInChildren( stringType );
          if( component == null )
          {
            Debug.LogWarning( "Cannot find component of type: " + stringType, go );
            continue;
          }

          System.Type compType = component.GetType();
          if( !typeof(SerializedComponent).IsAssignableFrom( compType ) )
          {
            // if component types change (different parent class, or field type changes) 
            Debug.Log( "component is not serializedcomponent: " + stringType, go );
            continue;
          }
          SerializedComponent comp = (SerializedComponent)component;
          if( comp == null )
            continue;

          // this is hacky, but prevents the waste of many ids.
          serializedComponents.Remove( comp.id );
          comp.id = (int)cmpData[ "id" ].GetNatural();
          serializedComponents.Add( comp.id, comp );

          foreach( var key in cmpData.Keys )
          {
            if( key == "type" || key == "id" || key == "position" || key == "rotation" )
              continue;
            if( key.StartsWith( "@" ) )
            {
              if( cmpData[ key ].IsArray )
              {
                Debug.LogWarning( "Cannot deserialize an array of references. Not implemented." );
              }
              else
              {
                int refId = (int)cmpData[ key ].GetNatural();
                if( so.refs.ContainsKey( key ) )
                  Debug.LogWarning( "Duplicate serialized field: " + key +" (Click for object)", go );
                else
                  so.refs.Add( key, refId );
              }
            }
            else
            {
              System.Reflection.FieldInfo fi = compType.GetField( key );
              if( fi == null )
                continue;
              if( cmpData[ key ] == null )
                continue;
              if( cmpData[ key ].IsNatural )
                fi.SetValue( comp, (int)cmpData[ key ].GetNatural() );
              if( cmpData[ key ].IsReal )
                fi.SetValue( comp, (float)cmpData[ key ].GetReal() );
              if( cmpData[ key ].IsBoolean )
                fi.SetValue( comp, cmpData[ key ].GetBoolean() );
              if( cmpData[ key ].IsString )
                fi.SetValue( comp, cmpData[ key ].GetString() );
              if( cmpData[ key ].IsArray && cmpData[ key ].Count > 0 )
              {
                if( cmpData[ key ][ 0 ] == null )
                  continue;
                object obj = fi.GetValue( comp );
                JsonType jsonType = cmpData[ key ][ 0 ].GetJsonType();
                switch( jsonType )
                {
                  case JsonType.String:
                    foreach( JsonData element in cmpData[key] )
                      ( obj as List<string> ).Add( element.GetString() );
                    break;
                  case JsonType.Natural:
                    foreach( JsonData element in cmpData[key] )
                      ( obj as List<int> ).Add( (int)element.GetNatural() );
                    break;
                }
              }
            }
          }
        }
      }
      return go;
    }
    return null;
  }

  public void AfterDeserialize()
  {
    // resolve references
    foreach( var cmp in components )
    {
      System.Type type = cmp.GetType();
      foreach( var pair in refs )
      {
        string fieldName = pair.Key.Substring( 1 );
        System.Reflection.FieldInfo fi = type.GetField( fieldName );
        if( fi == null )
          continue;
        SerializedComponent resolvedComponent = ResolveComponentFromId( pair.Value );
        if( resolvedComponent != null )
        {
          if( fi.FieldType != resolvedComponent.GetType() )
          {
            Debug.LogWarning( "deserialize type mismatch", cmp );
          }
          else
            fi.SetValue( cmp, resolvedComponent );
        }
        else
        {
          // TODO resolve gameobject references
          // this is probably not needed when there are component references
        }
      }
      cmp.AfterDeserialize();
    }
  }

  public void BeforeSerialize()
  {
    foreach( var cmp in components )
    {
      cmp.BeforeSerialize();
    }
  }



}


