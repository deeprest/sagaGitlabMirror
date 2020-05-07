using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderPawn : Pawn
{
  [Header( "Setting" )]
  public float moveSpeed = 3;
  public float jumpSpeed = 5;

#if UNITY_EDITOR
  public List<LineSegment> debugPath = new List<LineSegment>();
#endif

  protected override void Start()
  {
    base.Start();
    UpdateCollision = null;
    UpdatePosition = null;
    UpdateLogic = UpdateSpider;
    UpdateHit = CircleHit;
  }

  public override void OnControllerAssigned()
  {
    controller.CursorInfluence = false;
  }

  public float drtm = 1;

  void UpdateSpider()
  {
    if( Physics2D.CircleCastNonAlloc( transform.position, circle.radius, Vector2.down, RaycastHits, Time.deltaTime * drtm, Global.CharacterCollideLayers ) > 0 )
      collideBottom = true;

    if( Physics2D.CircleCastNonAlloc( transform.position, circle.radius, Vector2.up, RaycastHits, Time.deltaTime * drtm, Global.CharacterCollideLayers ) > 0 )
      collideTop = true;

    if( Physics2D.CircleCastNonAlloc( transform.position, circle.radius, Vector2.left, RaycastHits, Time.deltaTime * drtm, Global.CharacterCollideLayers ) > 0 )
      collideLeft = true;

    if( Physics2D.CircleCastNonAlloc( transform.position, circle.radius, Vector2.right, RaycastHits, Time.deltaTime * drtm, Global.CharacterCollideLayers ) > 0 )
      collideRight = true;

    if( body.bodyType == RigidbodyType2D.Dynamic )
    {
      if( input.MoveRight )
        body.AddForce( (moveSpeed - body.velocity.x) * Vector2.right * body.mass, ForceMode2D.Impulse );
      if( input.MoveLeft )
        body.AddForce( (-moveSpeed - body.velocity.x) * Vector2.right * body.mass, ForceMode2D.Impulse );
      if( !input.MoveRight && !input.MoveLeft )
        body.AddForce( -body.velocity.x * Vector2.right * body.mass, ForceMode2D.Impulse );

      if( collideLeft || collideRight )
      {
        body.gravityScale = 0;
        if( input.MoveUp )
          body.AddForce( (moveSpeed - body.velocity.y) * Vector2.up * body.mass, ForceMode2D.Impulse );
        if( input.MoveDown )
          body.AddForce( (-moveSpeed - body.velocity.y) * Vector2.up * body.mass, ForceMode2D.Impulse );
        if( !input.MoveUp && !input.MoveDown )
          body.AddForce( -body.velocity.y * Vector2.up * body.mass, ForceMode2D.Impulse );
      }
      else if( collideTop )
      {
        body.gravityScale = 0;
      }
      else
      {
        body.gravityScale = 1;
        if( input.JumpEnd )
          body.AddForce( Mathf.Min( 0, -body.velocity.y ) * Vector2.up * body.mass, ForceMode2D.Impulse );
        else if( collideBottom && input.JumpStart )
          body.AddForce( jumpSpeed * Vector2.up * body.mass, ForceMode2D.Impulse );
      }

    }
    else if( body.bodyType == RigidbodyType2D.Kinematic )
    {
      velocity.y += -Global.Gravity * Time.deltaTime;
      if( input.MoveRight ) velocity.x = moveSpeed;
      if( input.MoveLeft ) velocity.x = -moveSpeed;
      if( !input.MoveRight && !input.MoveLeft ) velocity.x = 0;
      if( collideBottom && input.JumpStart ) velocity.y = jumpSpeed;
      else if( input.JumpEnd ) velocity.y = 0;
    }

    // You want to reset the collision flags after each time you "process" the inputs.
    // Otherwise if you reset them in FixedUpdate() you might have what feels like "collision lag"
    collideRight = false;
    collideTop = false;
    collideLeft = false;
    collideBottom = false;
    //collideBodyBottom = null;


#if UNITY_EDITOR
    // draw path
    if( debugPath.Count > 0 )
      foreach( var ls in debugPath )
        Debug.DrawLine( ls.a, ls.b, Color.magenta );
#endif
  }

  private void FixedUpdate()
  {
    if( body.bodyType == RigidbodyType2D.Kinematic )
    {
      if( collideRight ) velocity.x = Mathf.Min( velocity.x, 0 );
      if( collideLeft ) velocity.x = Mathf.Max( velocity.x, 0 );
      if( collideTop ) velocity.y = Mathf.Min( velocity.y, 0 );
      if( collideBottom )
      {
        //if( collideBodyBottom != null ) velocity.y = Mathf.Max( velocity.y, collideBodyBottom.velocity.y );
        //else velocity.y = Mathf.Max( velocity.y, 0 );
      }
      body.MovePosition( body.position + (velocity * Time.fixedDeltaTime) );
    }
#if UNITY_EDITOR
    debugPath.Clear();
#endif
  }

  private void OnCollisionStay2D( Collision2D collision )
  {
    for( int i = 0; i < collision.contactCount; i++ )
    {
      if( -collision.contacts[i].normal.x > 0.5f ) collideRight = true;
      if( -collision.contacts[i].normal.x < 0.5f ) collideLeft = true;
      if( -collision.contacts[i].normal.y > 0.5f ) collideTop = true;
      if( -collision.contacts[i].normal.y < 0.5f ) collideBottom = true;
#if UNITY_EDITOR
      debugPath.Add( new LineSegment { a = collision.contacts[i].point, b = collision.contacts[i].point + collision.contacts[i].normal } );
#endif
    }
  }

  void CircleHit()
  {
    if( ContactDamage != null )
    {
      hitCount = Physics2D.CircleCastNonAlloc( body.position, circle.radius, body.velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
      for( int i = 0; i < hitCount; i++ )
      {
        if( RaycastHits[i].transform.GetInstanceID() == transform.GetInstanceID() )
          continue;
        hit = RaycastHits[i];
        IDamage dam = hit.transform.GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = this;
          dmg.damageSource = transform;
          dmg.point = hit.point;
          dam.TakeDamage( dmg );
        }

      }
    }
  }


}
