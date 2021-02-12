using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RainMaker : MonoBehaviour
{
  [SerializeField] ParticleSystem ps;
  
  
    [SerializeField] Mesh mesh;
    public float width;
  public float maxDistance = 100;
  public Vector2 direction = Vector2.down;

#if UNITY_EDITOR 
  public bool GenerateNow;
  [SerializeField] MeshFilter mf;
#endif
  
    void Update()
    {
        if( GenerateNow )
        {
          GenerateNow = false;
          Generate();
        }
    }

  public void Generate() 
  { 
#if UNITY_EDITOR
    DestroyImmediate( mesh );
#endif
    int mask = LayerMask.GetMask( new string[] {"Default","triggerAndCollision","destructible"} );
          
    mesh = new Mesh();
    List<Vector3> vert = new List<Vector3>();
    List<int> indices = new List<int>();

    Vector2 pos = transform.position;
    Vector2 origin;
    Vector2 min = (Vector2) transform.position + new Vector2(-width*0.5f,0);
          
    const float increment = 1;
    int steps = Mathf.FloorToInt( width/ increment );
    for( int i = 0; i < steps-1; i++ )
    {
      if( i > 0 ) 
      {
        indices.Add( i*2-1 );
        indices.Add( i*2-2 );
        indices.Add( i*2 );
              
        indices.Add( i*2-1 );
        indices.Add( i*2 );
        indices.Add( i*2+1 );
      }

      origin = min + Vector2.right * i * increment;
      vert.Add( origin - pos );

      Vector2 point;
      RaycastHit2D hit = Physics2D.Raycast( origin, direction, maxDistance, mask );
      if( hit.transform != null )
        point = hit.point;
      else
        point = origin + direction.normalized * maxDistance;
      vert.Add( point - pos );
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
