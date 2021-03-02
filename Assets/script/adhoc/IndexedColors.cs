using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( (typeof(IndexedColors)) )]
public class ICSEditor : Editor
{
  public override void OnInspectorGUI()
  {
    var ics = target as IndexedColors;
    ics.ExplicitUpdate();
    DrawDefaultInspector();
  }
}
#endif

public class IndexedColors : MonoBehaviour
{
  [SerializeField] SpriteRenderer[] srs;
  public Color[] colors = new Color[1];

  void Start()
  {
    ExplicitUpdate();
  }

  public void ExplicitUpdate()
  {
#if UNITY_EDITOR
    if( srs != null && srs.Length > 0 )
      foreach( var sr in srs )
        if( sr != null )
          sr.sharedMaterial.SetColorArray( "_IndexColors", colors );
#else
        for( int i = 0; i<srs.Length; i++ )
          srs[i].material.SetColorArray( "_IndexColors", colors );
#endif
  }
}