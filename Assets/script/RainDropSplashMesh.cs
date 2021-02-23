using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class RainDropSplashMesh : MonoBehaviour
{
  [SerializeField] ParticleSystem ps;
  [SerializeField] Mesh mesh;

  public float width = 50;
  public float increment = 1;
  public float maxDistance = 20;
  public Vector2 direction = Vector2.down;
  
  public float breakThreshold = 0.1f;
  public float levelAngleThreshold = 2;
  [FormerlySerializedAs( "offset" )]
  public float verticalOffset = 1;

  RaycastHit2D[] hits = new RaycastHit2D[8];
  Vector2 prevLow;
  Vector2 prevHigh;

#if UNITY_EDITOR
  public bool GenerateNow = false;
  [SerializeField] MeshFilter mf;

  void Update()
  {
    if( GenerateNow )
    {
      GenerateNow = false;
      Generate();
    }
  }
#endif

  public void Generate()
  {
    if( Application.isEditor && !Application.isPlaying )
      DestroyImmediate( mesh );
    else
      Destroy( mesh );

    int mask = LayerMask.GetMask( new string[] {"Default", "triggerAndCollision"} );

    mesh = new Mesh();
    List<Vector3> vert = new List<Vector3>();
    List<int> indices = new List<int>();

    Vector2 pos = ps.transform.position;
    Vector2 pointHigh;
    Vector2 min = pos + new Vector2( -width * 0.5f, 0 );
    Vector2 prevDelta = Vector2.right;

    int steps = Mathf.FloorToInt( width / increment );
    int a = 1;
    bool firstHit = true;
    for( int i = 0; i < steps; i++ )
    {
      pointHigh = min + Vector2.right * i * increment;
      Vector2 pointLow = Vector2.zero;
      int hitCount = Physics2D.RaycastNonAlloc( pointHigh, direction, hits, maxDistance, mask );
      bool hit = false;
      for( int j = 0; j < hitCount; j++ )
      {
        if( hits[j].transform != null && hits[j].rigidbody == null )
        {
          hit = true;
          pointLow = hits[j].point;
          break;
        }
      }
      if( !hit && i < steps - 1 )
        continue;
      //pointLow = pointHigh + direction.normalized * maxDistance;

      // edge thickness 
      pointHigh = pointLow + Vector2.up * verticalOffset;

      float newAngle = Vector2.Angle( pointLow - prevLow, prevDelta );

      if( firstHit )
      {
        firstHit = false;
        vert.Add( pointHigh - pos );
        vert.Add( pointLow - pos );
      }
      else if( Mathf.Abs( pointLow.y - prevLow.y ) > breakThreshold || i == steps - 1 )
      {
        indices.Add( a * 2 - 1 );
        indices.Add( a * 2 - 2 );
        indices.Add( a * 2 );

        indices.Add( a * 2 - 1 );
        indices.Add( a * 2 );
        indices.Add( a * 2 + 1 );

        vert.Add( prevHigh - pos );
        vert.Add( prevLow - pos );
        a++;

        prevHigh = pointHigh;
        prevLow = pointLow;

        // bridge the gaps
        /*
          indices.Add( a * 2 - 1 );
          indices.Add( a * 2 - 2 );
          indices.Add( a * 2 );

          indices.Add( a * 2 - 1 );
          indices.Add( a * 2 );
          indices.Add( a * 2 + 1 );
        */
        vert.Add( pointHigh - pos );
        vert.Add( pointLow - pos );
        a++;
      }
      else if( newAngle > levelAngleThreshold )
      { 
        indices.Add( a * 2 - 1 );
        indices.Add( a * 2 - 2 );
        indices.Add( a * 2 );

        indices.Add( a * 2 - 1 );
        indices.Add( a * 2 );
        indices.Add( a * 2 + 1 );

        vert.Add( prevHigh - pos );
        vert.Add( prevLow - pos );
        a++;
      }
      
      prevDelta = pointLow - prevLow;
      prevHigh = pointHigh;
      prevLow = pointLow;
    }

    mesh.SetVertices( vert );
    mesh.SetIndices( indices.ToArray(), MeshTopology.Triangles, 0, true );

    ParticleSystem.ShapeModule sm = ps.shape;
    sm.mesh = mesh;

#if UNITY_EDITOR
    mf.mesh = mesh;
#endif
  }
}