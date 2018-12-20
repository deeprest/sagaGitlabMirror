using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  new public SpriteRenderer renderer;
  public SpriteAnimator animator;
  public ParticleSystem dashSmoke;
  // keys
  public KeyCode DashKey = KeyCode.Space;
  public KeyCode RightKey = KeyCode.F;
  public KeyCode LeftKey = KeyCode.S;
  public KeyCode JumpKey = KeyCode.E;
  // settings
  public float raydowndist = 0.3f;
  public float floorOffset = 0.27f;
  public float rayside = 0.3f;
  public float boxHalfWidth = 0.1f;
  public float boxHalfHeightHi = 0.1f;
  public float boxHalfHelghtLo = 0.15f;
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

  void Update()
  {
    bool collideRight = false;
    bool collideLeft = false;
    bool collideHead = false;
    bool collideFeet = false;

    if( facingRight )
    {
      RaycastHit2D hi = Physics2D.Raycast( transform.position + Vector3.up * boxHalfHeightHi, Vector2.right, rayside );
      RaycastHit2D lo = Physics2D.Raycast( transform.position + Vector3.down * boxHalfHelghtLo, Vector2.right, rayside );
      if( hi.transform != null || lo.transform != null )
        collideRight = true;
    }
    else
    {
      RaycastHit2D hi = Physics2D.Raycast( transform.position + Vector3.up * boxHalfHeightHi, Vector2.left, rayside );
      RaycastHit2D lo = Physics2D.Raycast( transform.position + Vector3.down * boxHalfHelghtLo, Vector2.left, rayside );
      if( hi.transform != null || lo.transform != null )
        collideLeft = true;
    }
    // head raycast
    RaycastHit2D hitLeftHead = Physics2D.Raycast( transform.position + Vector3.left * boxHalfWidth, Vector2.up, raydowndist );
    RaycastHit2D hitRightHead = Physics2D.Raycast( transform.position + Vector3.right * boxHalfWidth, Vector2.up, raydowndist );
    if( ( hitLeftHead.transform != null || hitRightHead.transform != null ) )
      collideHead = true;
    // ground raycast
    RaycastHit2D hitLeftFoot = Physics2D.Raycast( transform.position + Vector3.left * boxHalfWidth, Vector2.down, raydowndist );
    RaycastHit2D hitRightFoot = Physics2D.Raycast( transform.position + Vector3.right * boxHalfWidth, Vector2.down, raydowndist );
    if( !jumping && ( hitLeftFoot.transform != null || hitRightFoot.transform != null ) )
    {
      collideFeet = true;
    }


    string anim = "idle";

    velocity.x = 0;
    velocity.y += -gravity * Time.smoothDeltaTime;

    if( facingRight )
      renderer.flipX = false;
    else
      renderer.flipX = true;
      
    if( Input.GetKey( RightKey ) )
    {
      inputRight = true;
      velocity += Vector3.right * moveVel;
      facingRight = true;
      if( !facingRight )
        dashing = false;
    }
    else
    {
      inputRight = false;
    }

    if( Input.GetKey( LeftKey ) )
    {
      inputLeft = true;
      velocity += Vector3.left * moveVel;
      facingRight = false;
      if( facingRight )
        dashing = false;
    }
    else
    {
      inputLeft = false;
    }

    if( Input.GetKeyDown( JumpKey ) )
    {
      if( onGround || collideRight || collideLeft )
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


    /*

    if( Input.GetKeyDown( DashKey ) )
    {
      if( onGround )
      {
        dashing = true;
        dashStart = Time.time;
        anim = "dash";
      }
    }
    else
    if( Input.GetKeyUp( DashKey ) )
    {
      dashing = false;
    }

    if( dashing )
    {
      if( facingRight )
      {
        velocity += Vector3.right * dashVel;
        dashSmoke.transform.localPosition = new Vector3( -0.36f, -0.2f, 0 );
      }
      else
      {
        velocity += Vector3.left * dashVel;
        dashSmoke.transform.localPosition = new Vector3( 0.36f, -0.2f, 0 );
      }
      if( onGround && !dashSmoke.isPlaying )
        dashSmoke.Play();
      if( !onGround && dashSmoke.isPlaying )
        dashSmoke.Stop();
      
      if( Time.time - dashStart >= dashDuration )
      {
        dashing = false;
        dashSmoke.Stop();
      }
    }
*/



    if( velocity.y < 0 )
      jumping = false;

    if( jumping && ( Time.time - jumpStart >= jumpDuration ) )
      jumping = false;

    if( landing && ( Time.time - landStart >= landDuration ) )
      landing = false;

    if( jumping )
      anim = "jump";
    
    if( onGround && ( inputRight || inputLeft ) )
      anim = "run";

    if( !onGround && !jumping )
      anim = "fall";
    
    

    if( collideFeet && !onGround )
    {
      landing = true;
      landStart = Time.time;
      AnimSequence seq = animator.animLookup[ "land" ];
      landDuration = ( 1.0f / seq.fps ) * seq.sprites.Length;
      anim = "land";
    }

    if( collideFeet )
    {
      onGround = true;
      RaycastHit2D hitFoot = ( hitLeftFoot.transform != null ) ? hitLeftFoot : hitRightFoot;
      Vector3 adjust = transform.position;
      if( transform.position.y - raydowndist < hitFoot.point.y )
        adjust.y = hitFoot.point.y + floorOffset;
      transform.position = adjust;
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
    if( anim == "wallslide" )
      dashSmoke.Play();
    else
      dashSmoke.Stop();
    animator.Play( anim );
    transform.position += velocity * Time.smoothDeltaTime;
  }
}
