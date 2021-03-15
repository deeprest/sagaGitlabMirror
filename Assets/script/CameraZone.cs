﻿using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof(CameraZone) )]
public class CameraZoneEditor : Editor
{
  public override void OnInspectorGUI()
  {
    CameraZone cz = target as CameraZone;
    for( int i = 0; i < cz.colliders.Length; i++ )
      cz.colliders[i].isTrigger = true;
    DrawDefaultInspector();
  }
}
#endif

public class CameraZone : MonoBehaviour
{
  public static List<CameraZone> All = new List<CameraZone>();
  
  [Tooltip("Higher priority zones will take precedence over lesser priorities.")]
  public int priority;
  [Tooltip("The camera will increase its size to view all colliders. Useful for rooms.")]
  public bool EncompassBounds;
  [Tooltip("Ignore setting to set active camera zone to this when player enters zone.")]
  public bool IgnoreAutoSwitch;
  [Tooltip("Set the camera zoom when this zone is the active zone.")]
  public bool SetOrtho;
  [Tooltip("If SetOrtho is true, this is the camera zoom value.")]
  public float orthoTarget;
  [Tooltip( "The camera will stay within the shapes of these colliders when the zone is active." )]
  public Collider2D[] colliders;

  public Bounds CameraBounds;

  private void Awake()
  {
    All.Add( this );
    UpdateBounds();
  }

  private void OnDestroy()
  {
    All.Remove( this );
  }

  void UpdateBounds()
  {
    foreach( var cld in colliders )
    {
      #if UNITY_EDITOR || DEVELOPMENT_BUILD
      if( cld.isTrigger == false )
        Debug.LogWarning( cld.gameObject.name+" should be a trigger"  );
      cld.isTrigger = true;
      #endif
      
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

  public static bool DoesOverlapAnyZone( Vector2 point, ref CameraZone active )
  {
    CameraZone zone = null;
    for( int z = 0; z < All.Count; z++ )
    {
      for( int i = 0; i < All[z].colliders.Length; i++ )
      {
        if( All[z].colliders[i].OverlapPoint( point ) 
          && !All[z].IgnoreAutoSwitch 
          && (zone==null || All[z].priority > zone.priority) )
        {
          zone = All[z];
        }
      }
    }
    active = zone;
    return false;
  }
}
