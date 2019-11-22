using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : WorldSelectable
{
  [SerializeField] Animator animator;
  public Weapon weapon;
  public Ability ability;

  Vector2 velocity;
  public BoxCollider2D box;
  public float friction = 0.05f;
  public float raylength = 0.01f;
  public float contactSeparation = 0.01f;
  public List<Transform> IgnoreCollideObjects;
  protected bool collideRight = false;
  protected bool collideLeft = false;
  protected bool collideTop = false;
  protected bool collideBottom = false;
  protected RaycastHit2D[] hits;

  public override void Highlight()
  {
    animator.Play( "highlight" );
  }
  public override void Unhighlight()
  {
    animator.Play( "idle" );
  }
  public override void Select()
  {
    animator.Play( "selected" );
  }
  public override void Unselect()
  { }

  void Start()
  {
    animator.Play( "idle" );
  }

  void Update()
  {
    BoxCollision();

    velocity.y -= Global.Gravity * Time.deltaTime;

    if( collideTop )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
    if( collideBottom )
    {
      velocity.x -= (velocity.x * friction) * Time.deltaTime;
      velocity.y = Mathf.Max( velocity.y, 0 );
    }
    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
    }
    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
    }

    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
    transform.position += (Vector3)velocity * Time.deltaTime;
  }

  void BoxCollision()
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;

    const float corner = 0.707f;

    Vector2 boxOffset = box.offset;
    boxOffset.x *= Mathf.Sign( transform.localScale.x );
    Vector2 adjust = (Vector2)transform.position + boxOffset;

    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.down, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.transform ) )
        continue;
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation;
        break;
      }
    }
    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.up, Mathf.Max( raylength, velocity.y * Time.deltaTime ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.transform ) )
        continue;
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        break;
      }
    }
    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.left, Mathf.Max( raylength, -velocity.x * Time.deltaTime ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.transform ) )
        continue;
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        break;
      }
    }

    hits = Physics2D.BoxCastAll( adjust, box.size, 0, Vector2.right, Mathf.Max( raylength, velocity.x * Time.deltaTime ), LayerMask.GetMask( Global.CharacterCollideLayers ) );
    foreach( var hit in hits )
    {
      if( IgnoreCollideObjects.Contains( hit.transform ) )
        continue;
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        break;
      }
    }
    transform.position = (Vector3)(adjust - boxOffset);
  }
}
