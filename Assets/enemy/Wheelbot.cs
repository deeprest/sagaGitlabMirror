using UnityEngine;
using System.Collections;


public class Wheelbot : Enemy
{
  const float wheelAnimRate = 0.0167f;
  public float wheelVelocity = 1;
  float wheelTime = 0;

  void Start()
  {
    EnemyStart();
    UpdateEnemy = UpdateWheel;
    velocity.x = wheelVelocity;
    animator.enabled = false;
  }

  void UpdateWheel()
  {
    if( collideLeft )
      velocity.x = wheelVelocity;
    if( collideRight )
      velocity.x = -wheelVelocity;
    wheelVelocity = Mathf.Abs( velocity.x );
    animator.flipX = velocity.x > 0;
    wheelTime += Mathf.Abs( velocity.x ) * wheelAnimRate * Time.timeScale;
    animator.AdvanceFrame( wheelTime );
    animator.UpdateFrame();
  }
}