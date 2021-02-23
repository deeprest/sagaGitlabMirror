using UnityEngine;

//[CreateAssetMenu]
public class AirbotAbility : Ability
{
  public float accSpeed = 10; //20
  public float maxAcc = 5; //10
  public float rotateTarget = 50;
  public float rotSpeed = 180f; //360
  public float dec = 4; //10
  PlayerBiped biped;
  Animator animator;

  public override void Equip( Transform parentTransform )
  {
    base.Equip( parentTransform );
    biped = pawn as PlayerBiped;
    animator = go.GetComponent<Animator>();
    animator.Play( "idle" );
  }

  Vector2 vel = Vector2.zero;
  
  public override void UpdateAbility()
  {
    if( !biped.onGround && biped.dashStart )
    {
      IsActive = true;
      vel = pawn.velocity;
    }

    if( !pawn.input.Dash || biped.onGround )
      IsActive = false;

    if( IsActive )
    {
      animator.Play( "spin" );
      
      if( !pawn.collideTop && pawn.input.MoveUp ) vel += Vector2.up * accSpeed * Time.deltaTime;
      if( !pawn.collideRight && pawn.input.MoveRight ) vel += Vector2.right * accSpeed * Time.deltaTime;
      if( !pawn.collideLeft && pawn.input.MoveLeft ) vel += Vector2.left * accSpeed * Time.deltaTime;
      if( !pawn.collideBottom && pawn.input.MoveDown ) vel += Vector2.down * accSpeed * Time.deltaTime;
      if( !(pawn.input.MoveUp || pawn.input.MoveDown || pawn.input.MoveRight || pawn.input.MoveLeft) )
        vel = Vector2.MoveTowards( vel, Vector2.zero, dec * Time.deltaTime );
      vel.x = Mathf.Clamp( vel.x, -maxAcc, maxAcc );
      vel.y = Mathf.Clamp( vel.y, -maxAcc, maxAcc );
      pawn.velocity = vel;
  
      biped.rotating = true;
      biped.rotateSpeed = rotSpeed;
      biped.rotateTarget = Mathf.Clamp( (vel.x/maxAcc) * -rotateTarget, -rotateTarget, rotateTarget);
    }
    else
    {
      animator.Play( "idle" );
      biped.rotating = false;
      vel = Vector2.zero;
    }
  }

  public override void Deactivate()
  {
    IsActive = false;
  }

  public override void Unequip()
  {
    base.Unequip();
    //playerBiped.backMount.localRotation = Quaternion.identity; 
  }
}