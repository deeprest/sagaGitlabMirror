using UnityEngine;

public class Wheelbot : Entity
{
  public Transform rotator;
  [SerializeField] float wheelAnimRate = 3;
  public float wheelVelocity = 2;
  float wheelTime;

  protected override void Start()
  {
    base.Start();
    UpdateLogic = UpdateWheel;
    UpdateHit = CircleHit;
    UpdateCollision = BoxCollisionSingle;
    velocity.x = wheelVelocity;
  }

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