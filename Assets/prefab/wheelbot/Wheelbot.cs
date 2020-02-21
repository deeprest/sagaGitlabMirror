using UnityEngine;
using System.Collections;


public class Wheelbot : Character, IDamage
{
  public Transform rotator;
  [SerializeField] float wheelAnimRate = 0.0167f;
  public float wheelVelocity = 1;
  float wheelTime = 0;

  void Start()
  {
    CharacterStart();
    UpdateHit = null;
    //UpdateCollision = null;
    UpdateLogic = UpdateWheel;
    velocity.x = wheelVelocity;
  }

  void UpdateWheel()
  {
    if( collideLeft )
      velocity.x = wheelVelocity;
    if( collideRight )
      velocity.x = -wheelVelocity;
    wheelVelocity = Mathf.Abs( velocity.x );
    wheelTime += velocity.x * wheelAnimRate * Time.timeScale;
    rotator.rotation = Quaternion.Euler( new Vector3( 0, 0, wheelTime ) );
  }

}