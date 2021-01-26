using UnityEngine;

public class PhysicsPawn : Pawn
{
  public float moveSpeed = 3;
  public float jumpSpeed = 5;
  const float corner = 0.707f;
  private float Scale = 1;
  public float airFriction = 0;
  

  void FixedUpdate() 
  {
    Vector2 inputVelocity = Vector2.zero;
    // INPUT
    if( input.MoveRight )
      inputVelocity.x = moveSpeed;
    else if( input.MoveLeft )
      inputVelocity.x = -moveSpeed;
    else
      inputVelocity.x = 0;
    if( collideBottom && (input.Jump&&!pinput.Jump) )
      inputVelocity.y = jumpSpeed;
    else if( (!input.Jump&&pinput.Jump) )
      inputVelocity.y = 0;

    Vector2 acc = Vector2.zero;
    if( overrideVelocityTimer.IsActive )
      acc += overrideVelocity;
    acc += Vector2.down * Global.Gravity;
    velocity += acc * Time.fixedDeltaTime;
    
    velocity.x += -Mathf.Clamp( airFriction, 0, velocity.x );
    velocity += inputVelocity;
      

    if( collideTop )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
    if( collideBottom )
    {
      //velocity.x -= Mathf.Clamp( friction * Time.fixedDeltaTime, 0, velocity.x );
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
    
    Vector2 baseVelocity = Vector2.zero;
    if( carryCharacter != null )
      baseVelocity = carryCharacter.velocity;

    body.MovePosition( body.position + (baseVelocity + velocity) * Time.fixedDeltaTime );
      
    adjust = body.position + velocity * Time.fixedDeltaTime;
    
    carryCharacter = null;
    // COLLISION / ADJUST
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    float down = DownOffset + contactSeparation;
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size * Scale, 0, Vector2.down, RaycastHits, Mathf.Max( down, -velocity.y * Time.fixedDeltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f * Scale + DownOffset;
        //hitBottomNormal = hit.normal;
        // moving platforms
        Entity cha = hit.transform.GetComponent<Entity>();
        if( cha != null )
        {
#if UNITY_EDITOR
          if( cha.GetInstanceID() == GetInstanceID() )
            Debug.LogError( "character set itself as carry character", gameObject );
#endif
          carryCharacter = cha;
        }
        break;
      }
    }

    //body.MovePosition( adjust );
  }
}