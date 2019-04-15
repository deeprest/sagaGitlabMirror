using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
  private void Awake()
  {
    sac = GetComponentsInChildren<SpriteChunk>();
  }


  public bool facingRight;
  SpriteChunk[] sac;

  void LateUpdate()
  {
    foreach( var sa in sac )
    {
      if( sa.flipXRenderer )
      {
        sa.spriteRenderer.flipX = !facingRight;
        sa.spriteRenderer.material.SetInt( "_FlipX", !facingRight ? 1 : 0 );
      }
      if( !facingRight && sa.flipXPosition )
      {
        Vector3 pos = sa.transform.localPosition;
        pos.x = -pos.x;
        sa.transform.localPosition = pos;
      }
    }
  }
}
