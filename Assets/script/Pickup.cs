using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupPart
{
  None,
  Head,
  ArmFront,
  ArmBack,
  Legs
}
[SelectionBase]
public class Pickup : WorldSelectable
{
  [SerializeField] Animator animator;
  public Weapon weapon;
  public Ability ability;
  public PickupPart PickupPart;

  Vector2 velocity;
  public BoxCollider2D box;
  public float friction = 0.05f;
  public float raylength = 0.01f;
  public float contactSeparation = 0.01f;
  protected bool collideRight = false;
  protected bool collideLeft = false;
  protected bool collideTop = false;
  protected bool collideBottom = false;
  // cache
  public RaycastHit2D[] RaycastHits = new RaycastHit2D[1];
  public int hitCount;
  public RaycastHit2D hit;
  protected Vector2 adjust;
  Vector2 boxOffset;

  public override void Highlight()
  {
    base.Highlight();
    if( animator != null )
      animator.Play( "highlight" );
  }
  public override void Unhighlight()
  {
    base.Unhighlight();
    if( animator != null )
      animator.Play( "idle" );
  }
  public override void Select()
  {
    //Destroy( gameObject );
    /*if( animator != null )
      animator.Play( "selected" );*/
  }
  public override void Unselect()
  {
  }

  void Start()
  {
    if( animator != null )
      animator.Play( "idle" );
  }

  /*
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
  */

  protected void BoxCollision()
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    const float corner = 0.707f;
    boxOffset.x = box.offset.x * Mathf.Sign( transform.localScale.x );
    boxOffset.y = box.offset.y;
    adjust = (Vector2)transform.position + boxOffset;

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.down, RaycastHits, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), Global.CharacterCollideLayers | Global.WorldSelectableLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation;
        break;
      }
    }
    /*
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.up, RaycastHits, Mathf.Max( raylength, velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        break;
      }
    }
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.left, RaycastHits, Mathf.Max( raylength, -velocity.x * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        //hitLeft = hit;
        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        break;
      }
    }

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.right, RaycastHits, Mathf.Max( raylength, velocity.x * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        //hitRight = hit;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        break;
      }
    }
    */
    transform.position = adjust - boxOffset;
  }

}
