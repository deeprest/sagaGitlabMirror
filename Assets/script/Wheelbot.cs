using UnityEngine;
using System.Collections;


public class Wheelbot : Entity, IDamage
{
  public Transform rotator;
  [SerializeField] float wheelAnimRate = 3;
  public float wheelVelocity = 2;
  float wheelTime = 0;

  protected override void Start()
  {
    base.Start();
    UpdateHit = CircleHit;
    UpdateCollision = CircleCollisionVelocity;
    UpdateLogic = UpdateWheel;
    velocity.x = wheelVelocity;
  }
  /*
  void WheelCollision()
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    const float corner = 0.707f;
    boxOffset.x = box.offset.x * Mathf.Sign( transform.localScale.x );
    boxOffset.y = box.offset.y;
    adjust = (Vector2)transform.position + boxOffset;

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.down, RaycastHits, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.y > corner )
      {
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation;
        // moving platforms
        Character cha = hit.transform.GetComponent<Character>();
        if( cha != null )
          carryCharacter = cha;
        break;
      }
    }
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.up, RaycastHits, Mathf.Max( raylength, velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.y < -corner )
      {
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
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
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
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
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
        collideRight = true;
        //hitRight = hit;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        break;
      }
    }

    transform.position = adjust - boxOffset;
  }
  */

  void UpdateWheel()
  {
    if( collideLeft )
      velocity.x = wheelVelocity;
    if( collideRight )
      velocity.x = -wheelVelocity;

    wheelTime += velocity.x * -wheelAnimRate * Time.timeScale;
    rotator.rotation = Quaternion.Euler( new Vector3( 0, 0, wheelTime ) );
  }

}