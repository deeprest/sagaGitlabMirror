using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

// thank you Lassade: https://answers.unity.com/users/86543/lassade.html

[ExecuteInEditMode]
public class PrefabAABB : MonoBehaviour
{
  /// <summary>
  /// local space Axis Aligned Bounding Box
  /// </summary>
  public Bounds bounds;
  Transform _transform;

  void OnDrawGizmos()
  {
    Gizmos.DrawWireCube( /*transform.position +*/ bounds.center, bounds.size );
  }

  void Reset()
  {
    RecalculateBounds();
  }

  public Bounds WorldBounds()
  {
    if( _transform == null )
      _transform = transform;

    Bounds b = bounds;
    b.center += _transform.position;

    Vector3 size = b.size;
    Vector3 tsize = _transform.lossyScale;
    size.x *= tsize.x;
    size.y *= tsize.y;
    size.z *= tsize.z;
    b.size = size;

    return b;
  }

  [ContextMenu( "Recalculate Bounds" )]
  public void RecalculateBounds()
  {
    BoxCollider2D[] boxColliders = gameObject.GetComponentsInChildren<BoxCollider2D>();
    bounds = new Bounds( Vector3.zero, Vector3.zero );
    foreach( BoxCollider2D boxCollider in boxColliders )
    {
      if( bounds.extents == Vector3.zero )
        bounds = boxCollider.bounds;
      bounds.Encapsulate( boxCollider.bounds );
    }
    /*
       MeshFilter this_mf = GetComponent<MeshFilter>();
       if( this_mf == null )
       {
         bounds = new Bounds( Vector3.zero, Vector3.zero );
       }
       else
       {
         bounds = this_mf.sharedMesh.bounds;
       }

       MeshFilter[] mfs = GetComponentsInChildren<MeshFilter>();
       foreach( MeshFilter mf in mfs )
       {
         Vector3 pos = mf.transform.localPosition;
         Bounds child_bounds = mf.sharedMesh.bounds;
         child_bounds.center += pos;
         bounds.Encapsulate( child_bounds );
       }
       */
  }

#if UNITY_EDITOR
  void Update()
  {
    if( Application.isPlaying ) return;
    RecalculateBounds();
  }
#endif
}