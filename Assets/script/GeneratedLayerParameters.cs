using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct LayerParam
{
  public float perlinLow;
  public float perlinHigh;
  public Gradient gradient;
  public bool randomOffset;
  public GameObject[] obj;
}

[System.Serializable]
public class PerlinPoint
{
  public float threshold;
  public float range;
  public Color lineColor = Color.red;
  public GameObject obj = null;
}


public class PointOfInterest
{
  public PerlinPoint def;
  public Vector3 point;
}

[CreateAssetMenu()]
public class GeneratedLayerParameters : ScriptableObject
{
  public float PerlinScale = 20;
  public LayerParam[] Layers;
  public List<PerlinPoint> PerlinPoints = new List<PerlinPoint>();
  public List<PointOfInterest> points = new List<PointOfInterest>();
  public GameObject blueBaseCamp;
  public GameObject redBaseCamp;

#if false
  public void Generate()
  {
    Generate( Random.Range( 0, 256 ) );
  }

  public void Generate( int seed )
  {
    Vector2 perlinOrigin = Vector2.one * 64f + Random.insideUnitCircle * 64f;
    points.Clear();
    foreach( var p in GenerationParams.Layers )
      GenerateLayer( perlinOrigin, PerlinScale, p.perlinLow, p.perlinHigh, p.gradient, p.obj, p.randomOffset );

    float minDistance = 10f;
    // filter points of interest
    List<PointOfInterest> good = new List<PointOfInterest>();
    foreach( var a in points )
    {
      bool okay = true;
      if( Mathf.Abs( (float)Zone.BlockSize - a.point.x ) < minDistance || Mathf.Abs( -a.point.x ) < minDistance ||
        Mathf.Abs( (float)Zone.BlockSize - a.point.z ) < minDistance || Mathf.Abs( -a.point.z ) < minDistance )
      {
        okay = false;
      }
      else
      {
        foreach( var b in good )
        {
          if( a == b )
            continue;
          if( Vector3.Distance( a.point, b.point ) < minDistance )
          {
            okay = false;
            break;
          }
        }
      }
      if( okay )
        good.Add( a );
    }
    points.Clear();
    points = good;

    Vector3 pointBlue;
    Vector3 pointRed;
    if( points.Count < 2 )
    {
      pointBlue = new Vector3( minDistance, 0, minDistance );
      pointRed = new Vector3( (float)Zone.BlockSize - minDistance, 0, (float)Zone.BlockSize - minDistance );
    }
    else
    {
      pointBlue = points[ 0 ].point;
      points.RemoveAt( 0 );
      pointRed = points[ points.Count - 1 ].point;
      points.RemoveAt( points.Count - 1 );
    }

    BuildBasecamp( blueBaseCamp, Tag.Blue, pointBlue, Quaternion.identity );
    BuildBasecamp( redBaseCamp, Tag.Red, pointRed, Quaternion.identity );

    // spawn objects of interest
    foreach( var p in points )
    {
      if( p.def.obj != null )
        Global.Instance.Spawn( p.def.obj, p.point, Quaternion.identity );
    }
  }

  void BuildBasecamp( GameObject prefab, Tag team, Vector3 position, Quaternion rotation )
  {
    int layerMask = ~LayerMask.GetMask( new string[]{ "Ground" } );

    /*Bounds bounds = new Bounds( Vector3.zero, Vector3.one );
      Collider[] mrs = blueBaseCamp.GetComponentsInChildren<Collider>();
      Vector3 prefabOffset = blueBaseCamp.transform.position;
      foreach( Collider mr in mrs )
      {
        Bounds b = new Bounds( mr.bounds.center - prefabOffset, mr.bounds.size );
        bounds.Encapsulate( b );
      }
      */

    Collider[] cls = Physics.OverlapBox( position, basecampSafeHalfExtents, rotation, layerMask, QueryTriggerInteraction.Collide );
    foreach( var cld in cls )
    {
      if( Application.isEditor && !Application.isPlaying )
        GameObject.DestroyImmediate( cld.gameObject );
      else
        GameObject.Destroy( cld.gameObject );
    }
    GameObject teambase = Global.Instance.Spawn( blueBaseCamp, position, rotation, null, false, true );
    Global.Instance.AssignTeam( teambase, team );

    // DEBUG
    /*GameObject box = new GameObject( "basecamp debug box" );
      box.transform.position = position;
      BoxCollider bc = box.AddComponent<BoxCollider>();
      bc.extents = basecampSafeHalfExtents;
      */

    {
      GameObject go = Global.Instance.Spawn( Global.Instance.avatarPrefab, position, rotation, null, false, false );
      Character male = go.GetComponent<Character>();
      male.IdentityName = "stan";
      SerializedComponent[] scs = go.GetComponentsInChildren<SerializedComponent>();
      foreach( var sc in scs )
        sc.AfterDeserialize();
      Global.Instance.AssignTeam( go, team );
    }
    {
      GameObject go = Global.Instance.Spawn( Global.Instance.avatarPrefab, position, rotation, null, false, false );
      Character female = go.GetComponent<Character>();
      female.IdentityName = "francine";
      SerializedComponent[] scs = go.GetComponentsInChildren<SerializedComponent>();
      foreach( var sc in scs )
        sc.AfterDeserialize();
      Global.Instance.AssignTeam( go, team );
    }
  }

  public void GenerateLayer( Vector2 perlinOrigin, float perlinScale, float perlinLow, float perlinHigh, 
    Gradient gradient, GameObject[] obj, bool randomOffset = false )
  {
    // ground color
    FloatMap cwy = new FloatMap( diffuseTexture );
    cwy.ColorA = gradient.Evaluate( 0 );
    cwy.ColorARangeEnd = gradient.Evaluate( 1 );
    cwy.FillPerlin( perlinOrigin, perlinScale );
    cwy.Render( perlinLow, perlinHigh );

    if( obj.Length > 0 )
    {
      // spawn objects
      FloatMap cwy2 = new FloatMap( Zone.BlockSize, Zone.BlockSize );
      cwy2.FillPerlin( perlinOrigin, perlinScale );
      float rot = 0;
      float value = 0;
      for( int x = 0; x < cwy2.width; x++ )
      {
        for( int y = 0; y < cwy2.height; y++ )
        {
          value = cwy2.buffer[ x + cwy2.width * y ];
          if( value > perlinLow && value < perlinHigh )
          {
            Vector3 pos = new Vector3( 0.5f + x, 0, 0.5f + y );
            if( Global.Instance.SpawnCheck( pos, 0.4f ) )
            {
              if( randomOffset )
              {
                Vector2 offset = Random.insideUnitCircle * 0.5f;
                pos += new Vector3( offset.x, 0, offset.y );
              }
              GameObject prefab = obj[ Random.Range( 0, obj.Length ) ];
              if( prefab == null )
              {
                continue;
              }
              Global.Instance.Spawn( prefab, pos, Quaternion.Euler(0,rot,0), null, false, true );
              rot += 90f; 
            }
          }
          foreach( var pp in PerlinPoints )
          {
            if( value > pp.threshold - pp.range * 0.5f && value < pp.threshold + pp.range * 0.5f )
            {
              PointOfInterest p = new PointOfInterest();
              p.def = pp;
              p.point = new Vector3( 0.5f + x, 0, 0.5f + y );
              points.Add( p );
            }
          }
        }
      }
    }
  }

  #endif
}


