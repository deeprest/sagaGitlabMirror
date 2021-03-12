using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( (typeof(WorldText)) )]
public class WorldTextEditor : Editor
{
  public override void OnInspectorGUI()
  {
    if( !Application.isPlaying )
    {
      var ics = target as WorldText;
      ics.ExplicitUpdate();
    }
    DrawDefaultInspector();
  }
}
#endif

public class WorldText : MonoBehaviour
{
  public bool IsBreakable;
  public GameObject prefab;
  public string text = "asdf";
  string cachedText;
  public float x;
  public float width = .1f;
  float cachedWidth;
  public FontSpriteReference font;

  public void ExplicitUpdate()
  {
    if( font == null || (cachedText == text && Mathf.Approximately( cachedWidth, width )) )
      return;
    x = 0;
    cachedText = text;
    cachedWidth = width;
    for( int i = transform.childCount - 1; i >= 0; i-- )
      Util.Destroy( transform.GetChild( i ).gameObject );
    for( int i = 0; i < text.Length; i++ )
    {
      int index = FontSpriteReference.map.IndexOf( text[i] );
      if( index > 0 )
      {
        GameObject go;
#if UNITY_EDITOR
        if( Application.isEditor && !Application.isPlaying )
          go = (GameObject) PrefabUtility.InstantiatePrefab( prefab, transform );
        else
          go = Instantiate( prefab, transform, false );
#else
          go = Instantiate( prefab, transform, false );
#endif

        go.transform.localPosition = Vector3.right * (width * (text.Length - 1) * -0.5f + x);

        if( IsBreakable )
        {
          // background, optional
          go.transform.GetComponent<SpriteRenderer>().sprite = font.spritesBackground[index];
          // glyph in front is first child
          if( go.transform.childCount > 0 )
            go.transform.GetChild( 0 ).GetComponent<SpriteRenderer>().sprite = font.sprites[index];
        }
        else
        {
          go.transform.GetComponent<SpriteRenderer>().sprite = font.sprites[index];
        }
      }
      x += width;
    }
  }
}