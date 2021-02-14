using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RainMaker : MonoBehaviour
{
  [SerializeField] ParticleSystem ps;
  [SerializeField] Mesh mesh;
  
  public float width;
  public float increment = 1;
  public float maxDistance = 100;
  public Vector2 direction = Vector2.down;

  public float threshold = 1;

#if UNITY_EDITOR
  public bool GenerateNow;
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

  Vector2 prevPoint;
  Vector2 prevOrigin;
  
  public void Generate()
  {
#if UNITY_EDITOR
    DestroyImmediate( mesh );
#else
    Destroy( mesh );
#endif
    int mask = LayerMask.GetMask( new string[] {"Default", "triggerAndCollision", "destructible"} );

    mesh = new Mesh();
    List<Vector3> vert = new List<Vector3>();
    List<int> indices = new List<int>();

    Vector2 pos = ps.transform.position;
    Vector2 origin;
    Vector2 min = pos + new Vector2( -width * 0.5f, 0 );

    int steps = Mathf.FloorToInt( width / increment );
    int a = 1;
    for( int i = 0; i < steps; i++ )
    {
      origin = min + Vector2.right * i * increment;
      Vector2 point;
      RaycastHit2D hit = Physics2D.Raycast( origin, direction, maxDistance, mask );
      if( hit.transform != null )
        point = hit.point;
      else
        point = origin + direction.normalized * maxDistance;

      if( i > 0 )
      {
        if( Mathf.Abs( point.y - prevPoint.y ) > threshold || i == steps-1)
        {
          indices.Add( a * 2 - 1 );
          indices.Add( a * 2 - 2 );
          indices.Add( a * 2 );

          indices.Add( a * 2 - 1 );
          indices.Add( a * 2 );
          indices.Add( a * 2 + 1 );
          
          vert.Add( prevOrigin - pos );
          vert.Add( prevPoint - pos );
          
          prevOrigin = origin;
          prevPoint = point;
          a++;
          
          indices.Add( a * 2 - 1 );
          indices.Add( a * 2 - 2 );
          indices.Add( a * 2 );

          indices.Add( a * 2 - 1 );
          indices.Add( a * 2 );
          indices.Add( a * 2 + 1 );
          
          vert.Add( origin - pos );
          vert.Add( point - pos );
          
          a++;
        }
        
        prevOrigin = origin;
        prevPoint = point;
      }
      else
      {
        vert.Add( origin - pos );
        vert.Add( point - pos );

        prevOrigin = origin;
        prevPoint = point;
      }
      
      
    }

    mesh.SetVertices( vert );
    mesh.SetIndices( indices.ToArray(), MeshTopology.Triangles, 0, true );

    ParticleSystem.ShapeModule sm = ps.shape;
    sm.mesh = mesh;
    ParticleSystem.MainModule main = ps.main;
    main.startRotation = Mathf.Atan2( direction.x, direction.y );

    
#if UNITY_EDITOR
    mf.mesh = mesh;
#endif
  }
}