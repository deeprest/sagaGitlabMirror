using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
  public bool facingRight;
  SpriteChunk[] sac;
  Vector3 pos;

  private void Awake()
  {
    sac = GetComponentsInChildren<SpriteChunk>();
  }

  void LateUpdate()
  {
    GetComponent<Animator>().SetBool( "facingRight", facingRight );


    //GetComponent<Animator>().GetCurrentAnimatorClipInfo( 0 )[0].clip.SampleAnimation( gameObject, GetComponent<Animator>().playbackTime );

    foreach( var sa in sac )
    {
      if( sa.flipXRenderer )
      {
        sa.spriteRenderer.flipX = !facingRight;
        sa.spriteRenderer.material.SetInt( "_FlipX", !facingRight ? 1 : 0 );
        sa.spriteRenderer.sortingOrder = (facingRight? 1 : -1) * sa.spriteOrder;
      }
      if( sa.flipXPosition )
      {
        /*if( !facingRight )
        {
          pos = sa.transform.localPosition;
          pos.x = -pos.x;
          sa.transform.localPosition = pos;
        }*/
      }
    }
  }
}
