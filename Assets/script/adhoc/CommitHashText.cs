using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CommitHashText : MonoBehaviour
{
  public Text text;
  [FormerlySerializedAs( "BreakableText" )]
  public WorldText worldText;

  void Start()
  {
    if( text != null )
    {
      text.text = Commit.Hash.ToUpper();
    }
    if( worldText != null )
    {
      worldText.text = Commit.Hash.ToUpper();
      worldText.ExplicitUpdate();
    }
  }
}