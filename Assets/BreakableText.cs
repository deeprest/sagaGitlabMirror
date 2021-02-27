using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( (typeof(BreakableText)) )]
public class BreakableLettersEditor : Editor
{
  public override void OnInspectorGUI()
  {
    if( !Application.isPlaying )
    {
      var ics = target as BreakableText;
      ics.ExplicitUpdate();
    }
    DrawDefaultInspector();
  }
}
#endif

public class BreakableText : MonoBehaviour
{
  [Tooltip("Prefab requires a BreakableJunk component")]
  public GameObject prefab;
  //public GameObject prefabJunk;
  public string text = "asdf";
  string cachedText;
  public float x;
  public float width = .1f;
  public FontSpriteReference font;

  void Start()
  {
    //ExplicitUpdate();
  }

  public void ExplicitUpdate()
  {
    if( font == null || cachedText == text )
      return;
    x = 0;
    cachedText = text;
    for( int i = transform.childCount - 1; i >= 0; i-- )
      Util.Destroy( transform.GetChild( i ).gameObject );
    for( int i = 0; i < text.Length; i++ )
    {
      int index = FontSpriteReference.map.IndexOf( text[i] );
      if( index > 0 )
      {
        #if UNITY_EDITOR
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab( prefab, transform );
        
        #else
GameObject go = Instantiate( prefab, transform, false );
#endif
        go.transform.localPosition = Vector3.right * ((width*text.Length) * -0.5f + x);
        // background, optional
        go.transform.GetComponent<SpriteRenderer>().sprite = font.spritesBackground[index];
        // glyph in front is first child
        go.transform.GetChild( 0 ).GetComponent<SpriteRenderer>().sprite = font.sprites[index];
      }
      x += width;
    }
  }
}