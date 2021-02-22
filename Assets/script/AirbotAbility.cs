using UnityEngine;

//[CreateAssetMenu]
public class AirbotAbility : Ability
{
  public float speed = 1;

  public float accSpeed = 1;
  Vector2 acc;
  public float maxAcc = 10;
  
  public override void UpdateAbility()
  {
    PlayerBiped playerBiped = pawn as PlayerBiped;
    if( !playerBiped.onGround && playerBiped.dashStart )
      IsActive = true;

    if( !pawn.input.Dash || playerBiped.onGround )
      IsActive = false;

    if( IsActive )
    {
      /*Vector2 dir = Vector2.zero;
      if( pawn.input.MoveUp ) dir += Vector2.up;
      if( pawn.input.MoveRight ) dir += Vector2.right;
      if( pawn.input.MoveLeft ) dir += Vector2.left;
      if( pawn.input.MoveDown ) dir += Vector2.down;
      pawn.velocity = dir * speed;*/

      Vector2 vel = new Vector2( pawn.velocity.x, 0 );
      if( pawn.input.MoveUp ) vel += Vector2.up * accSpeed * Time.deltaTime;
      if( pawn.input.MoveRight ) vel += Vector2.right * accSpeed * Time.deltaTime;
      if( pawn.input.MoveLeft ) vel += Vector2.left * accSpeed * Time.deltaTime;
      if( pawn.input.MoveDown ) vel += Vector2.down * accSpeed * Time.deltaTime;
      vel.x = Mathf.Clamp( vel.x, -maxAcc, maxAcc );
      vel.y = Mathf.Clamp( vel.y, -maxAcc, maxAcc );
      pawn.velocity = vel;
    }
  }
}