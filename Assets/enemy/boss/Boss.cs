using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Character
{
  public bool facingRight;
  SpriteChunk[] sac;
  Vector3 pos;
  public BoxCollider2D fist;
  public BoxCollider2D torso;

  private void Awake()
  {
    sac = GetComponentsInChildren<SpriteChunk>();
    Physics2D.IgnoreCollision( box, fist );
    Physics2D.IgnoreCollision( box, torso );
    Physics2D.IgnoreCollision( torso, fist );
  }

  void Start()
  {
    UpdateHit = BoxHit;
    UpdateCollision = BoxCollision;
    UpdatePosition = BasicPosition;
    //UpdateEnemy = UpdateWheel;
  }

  void LateUpdate()
  {
    //GetComponent<Animator>().SetBool( "facingRight", facingRight );
    //GetComponent<Animator>().GetCurrentAnimatorClipInfo( 0 )[0].clip.SampleAnimation( gameObject, GetComponent<Animator>().playbackTime );

    transform.localScale = new Vector3( facingRight ? 1 : -1, 1, 1 );

    foreach( var sa in sac )
    {
      if( sa.flipXRenderer )
      {
       //sa.spriteRenderer.flipX = !facingRight;
        //sa.spriteRenderer.material.SetInt( "_FlipX", !facingRight ? 1 : 0 );
        //sa.spriteRenderer.sortingOrder = (facingRight? 1 : -1) * sa.spriteOrder;
      }
      //if( sa.flipXPosition )
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
