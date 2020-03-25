using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
  
  public int priority;
  [Tooltip("The camera will increase its size to view all colliders. Useful for rooms.")]
  public bool EncompassBounds;
  [Tooltip( "The camera will stay within the shapes of these colliders when the zone is active." )]
  public Collider2D[] colliders;

  public Bounds CameraBounds;

  private void Awake()
  {
    UpdateBounds();
  }

  void UpdateBounds()
  {
    foreach( var cld in colliders )
    {
      if( cld is PolygonCollider2D )
      {
        PolygonCollider2D poly = cld as PolygonCollider2D;
        // camera poly bounds points are local to polygon
        CameraBounds = new Bounds();
        foreach( var p in poly.points )
          CameraBounds.Encapsulate( p );
      }
      else if( cld is BoxCollider2D )
      {
        CameraBounds = (cld as BoxCollider2D).bounds;
      }
    }
  }
}
