using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelNode : MonoBehaviour
{
  [SerializeField] TextMesh label;
  [SerializeField] BreakableText btext;

  void Awake()
  {
    if( Application.isPlaying && label != null )
      Destroy( label.gameObject );
  }
#if UNITY_EDITOR
  private void OnValidate()
  {
    if( label != null )
      label.text = name;
    if( btext != null )
    {
      btext.text = name;
      //btext.ExplicitUpdate();
    }
  }
#endif
} 

