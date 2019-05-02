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
    //animator.enabled = false;
    animator.StartPlayback();
  }

  void UpdateWheel()
  {
    if( collideLeft )
      velocity.x = wheelVelocity;
    if( collideRight )
      velocity.x = -wheelVelocity;
    wheelVelocity = Mathf.Abs( velocity.x );
    //animator.enabled = (wheelVelocity > 0.1f);

    /*renderer.flipX = velocity.x > 0;
    renderer.material.SetInt( "_FlipX", velocity.x > 0 ? 1 : 0 );
    wheelTime += Mathf.Abs( velocity.x ) * wheelAnimRate * Time.timeScale;*/

    animator.speed = -velocity.x;

    //animator.playbackTime = wheelTime;

    //animator.AdvanceFrame( wheelTime );
    //animator.UpdateFrame();
  }
}