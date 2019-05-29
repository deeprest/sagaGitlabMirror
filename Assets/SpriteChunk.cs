using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent( typeof( SpriteRenderer ) )]
public class SpriteChunk : MonoBehaviour
{
  public SpriteRenderer spriteRenderer;
  public bool flipXRenderer = true;
  public bool flipXPosition = true;
  public int spriteOrder;

  void Awake()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    spriteOrder = spriteRenderer.sortingOrder;
  }

}