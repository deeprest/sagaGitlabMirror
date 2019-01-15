using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  //  public CircleCollider2D circle;
  //public BoxCollider2D box;

  new public SpriteRenderer renderer;
  public SpriteAnimator animator;
  public ParticleSystem dashSmoke;
  // keys
  public KeyCode DashKey = KeyCode.Space;
  public KeyCode RightKey = KeyCode.F;
  public KeyCode LeftKey = KeyCode.S;
  public KeyCode JumpKey = KeyCode.E;
  // settings
  public float raylength = 0.1f;
  public Vector2 box = new Vector2( 0.3f, 0.3f );
  public Vector2 hitOffset = new Vector2( 0.3f, 0.3f );

  // velocities
  public float gravity = 16;
  public float moveVel = 2;
  public float jumpVel = 5;
  public float dashVel = 5;
  // durations
  public float jumpDuration = 0.4f;
  public float dashDuration = 1;
  public float landDuration = 0.1f;
  public float wallSlideFactor = 0.5f;
  // state
  public bool facingRight = true;
  public bool inputRight = false;
  public bool inputLeft = false;
  public bool onGround = false;
  public bool jumping = false;
  public bool landing = false;
  public bool dashing = false;

  public Vector3 velocity = Vector3.zero;
  float dashStart;
  float jumpStart;
  float landStart;

  public bool collideRight = false;
  public bool collideLeft = false;
  public bool collideHead = false;
  public bool collideFeet = false;

  void Update()
  {
    if( Global.Paused )
      return;
    
    collideRight = false;
    collideLeft = false;
    collideHead = false;
    collideFeet = false;

    /*RaycastHit2D[] hits = Physics2D.CircleCastAll( transform.position, radius, Vector2.down, castDistance, Physics2D.AllLayers );
    //RaycastHit2D[] hits = Physics2D.BoxCastAll( transform.position, box, 0, velocity, velocity.magnitude * Time.smoothDeltaTime, Physics2D.AllLayers );
    foreach( var hit in hits )
    {
      if( Mathf.Abs( hit.normal.y ) < wallY )
      {
        if( hit.normal.x > 0 )
          collideLeft = true;
        else
          collideRight = true;
      }
      else
      {
        if( hit.normal.y > 0 )
        {
          collideFeet = true;
          hitFoot = hit;
        }
        else
          collideHead = true;
      }
    }*/
   

    {
      Vector3 p = transform.position + ( Vector3.right * box.x );
      RaycastHit2D hitRight = Physics2D.Raycast( p, Vector2.right, raylength );
      if( hitRight.transform != null )
      {
        if( hitRight.normal.x < -0.5f )
        {
          dashing = false;
          collideRight = true;
          Vector3 adjust = transform.position;
          if( transform.position.x + box.x > hitRight.point.x )
            adjust.x = hitRight.point.x - box.x;
          transform.position = adjust;
        }
      }
    }
    {
      Vector3 p = transform.position + ( Vector3.left * box.x );
      RaycastHit2D hitLeft = Physics2D.Raycast( p, Vector2.left, raylength );
      if( hitLeft.transform != null )
      {
        if( hitLeft.normal.x > 0.5f )
        {
          dashing = false;
          collideLeft = true;
          Vector3 adjust = transform.position;
          if( transform.position.x - box.x < hitLeft.point.x )
            adjust.x = hitLeft.point.x + box.x;
          transform.position = adjust;
        }
      }
    }
      
    // ground raycast
    RaycastHit2D hitLeftFoot = Physics2D.Raycast( transform.position + Vector3.left * hitOffset.x, Vector2.down, box.y );
    RaycastHit2D hitRightFoot = Physics2D.Raycast( transform.position + Vector3.right * hitOffset.x, Vector2.down, box.y );
    if( !jumping && ( hitLeftFoot.transform != null || hitRightFoot.transform != null ) )
    {
      RaycastHit2D hitFoot = ( hitLeftFoot.transform != null ) ? hitLeftFoot : hitRightFoot;
      if( hitFoot.normal.y > 0.5f )
      {
        collideFeet = true;
        Vector3 adjust = transform.position;
        if( transform.position.y - box.y < hitFoot.point.y )
          adjust.y = hitFoot.point.y + hitOffset.y;
        transform.position = adjust;
      }
    }

    // head raycast 
    RaycastHit2D hitLeftHead = Physics2D.Raycast( transform.position + Vector3.left * hitOffset.x, Vector2.up, box.y );
    RaycastHit2D hitRightHead = Physics2D.Raycast( transform.position + Vector3.right * hitOffset.x, Vector2.up, box.y );
    if( ( hitLeftHead.transform != null || hitRightHead.transform != null ) )
    {
      collideHead = true;
      RaycastHit2D hitHead = ( hitLeftHead.transform != null ) ? hitLeftHead : hitRightHead;
      Vector3 adjust = transform.position;
      if( transform.position.y + box.y > hitHead.point.y )
        adjust.y = hitHead.point.y - hitOffset.y;
      transform.position = adjust;
    }

    string anim = "idle";

    velocity.x = 0;
    velocity.y += -gravity * Time.deltaTime;

    if( facingRight )
      renderer.flipX = false;
    else
      renderer.flipX = true;
      
    if( Input.GetKey( RightKey ) )
    {
      inputRight = true;
      velocity.x = moveVel;
      if( !facingRight && onGround )
        dashing = false;
      facingRight = true;
    }
    else
    {
      inputRight = false;
    }

    if( Input.GetKey( LeftKey ) )
    {
      inputLeft = true;
      velocity.x = -moveVel;
      if( facingRight && onGround )
        dashing = false;
      facingRight = false;
    }
    else
    {
      inputLeft = false;
    }

    if( Input.GetKeyDown( JumpKey ) )
    {
      if( onGround || ( inputRight && collideRight ) || ( inputLeft && collideLeft ) )
      {
        jumping = true;
        jumpStart = Time.time;
        velocity.y = jumpVel;
      }
    }
    else
    if( Input.GetKeyUp( JumpKey ) )
    {
      jumping = false;
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
        
    if( Input.GetKeyDown( DashKey ) )
    {
      if( onGround || collideLeft || collideRight )
      {
        dashing = true;
        dashStart = Time.time;
      }
    }
    else
    if( Input.GetKeyUp( DashKey ) )
    {
      if( !jumping )
        dashing = false;
    }

    if( velocity.y < 0 )
      jumping = false;

    if( jumping && ( Time.time - jumpStart >= jumpDuration ) )
      jumping = false;

    if( landing && ( Time.time - landStart >= landDuration ) )
      landing = false;

    if( collideFeet && !onGround )
    {
      dashing = false;
      landing = true;
      landStart = Time.time;
      //AnimSequence seq = animator.animLookup[ "land" ];
      //landDuration = ( 1.0f / seq.fps ) * seq.sprites.Length;
    }

    if( onGround && ( inputRight || inputLeft ) )
      anim = "run";
    
    if( dashing )
    {
      if( onGround )
      {
        anim = "dash";
        if( Time.time - dashStart >= dashDuration )
        {
          dashing = false;
          dashSmoke.Stop();
        }
      }
      if( facingRight )
      {
        velocity.x = dashVel;
        dashSmoke.transform.localPosition = new Vector3( -0.36f, -0.2f, 0 );
      }
      else
      {
        velocity.x = -dashVel;
        dashSmoke.transform.localPosition = new Vector3( 0.36f, -0.2f, 0 );
      }
    }

    if( jumping )
      anim = "jump";
    
    if( !onGround && !jumping )
      anim = "fall";
    
    if( landing )
      anim = "land";

    if( collideFeet )
    {
      onGround = true;
      velocity.y = Mathf.Max( velocity.y, 0 );
    }
    else
    {
      onGround = false;
    }

    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
      if( jumping )
      {
        anim = "walljump";
      }
      else
      if( inputRight && !onGround && velocity.y < 0 )
      {
        velocity.y *= wallSlideFactor;
        anim = "wallslide";
        dashSmoke.transform.localPosition = new Vector3( 0.2f, -0.2f, 0 );
      }

    }

    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
      if( jumping )
      {
        anim = "walljump";
      }
      else
      if( inputLeft && !onGround && velocity.y < 0 )
      {
        velocity.y *= wallSlideFactor;
        anim = "wallslide";
        dashSmoke.transform.localPosition = new Vector3( -0.2f, -0.2f, 0 );
      }
    }

    if( collideHead )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }

    if( anim == "wallslide" || anim == "dash" )
      dashSmoke.Play();
    else
      dashSmoke.Stop();
    
    animator.Play( anim );
    transform.position += velocity * Time.smoothDeltaTime;
  }
}
