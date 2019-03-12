using UnityEngine;
using System.Collections;

public class Hornet : Enemy
{
  public float sightRange = 6;
  public float flySpeed = 2;
  public float up = 3;
  public float over = 2;
  public float small = 0.1f;
  public float rot = 15;
  public float rotspeed = 20;
  public float topspeedrot = 1;
  public float acc = 2;
  Vector3 tvel;
  Timer wheelDrop = new Timer();
  public float wheelDropInterval = 3;
  public Transform drop;
  public GameObject dropPrefab;

  void Start()
  {
    EnemyStart();
    UpdateEnemy = UpdateHornet;
  }

  void UpdateHornet()
  {
    if( Global.instance.CurrentPlayer != null )
    {
      Vector3 player = Global.instance.CurrentPlayer.transform.position;
      Vector3 tpos = player + Vector3.up * up + Vector3.right * over;
      Vector3 delta = tpos - transform.position;
      if( delta.sqrMagnitude < small * small )
        tvel = Vector3.zero;
      else if( delta.sqrMagnitude < sightRange * sightRange )
      {
        tvel = (tpos - transform.position).normalized * flySpeed;
        // todo guns
        // todo missles
        // todo drop wheels
        if( tvel.x > 0 && !wheelDrop.IsActive)
        {
          wheelDrop.Start( wheelDropInterval, null, null );
          GameObject go = Global.instance.Spawn( dropPrefab, drop.position, Quaternion.identity );
          Physics2D.IgnoreCollision( go.GetComponent<Collider2D>(), GetComponent<Collider2D>() );
          Wheelbot wheelbot = go.GetComponent<Wheelbot>();
          wheelbot.wheelVelocity = Mathf.Sign( player.x - transform.position.x );
        }
      }
      velocity += (tvel-velocity) * acc * Time.deltaTime;
      transform.rotation = Quaternion.RotateTowards( transform.rotation, Quaternion.Euler( 0, 0, Mathf.Clamp( velocity.x, -topspeedrot, topspeedrot ) * -rot ), rotspeed * Time.deltaTime );
    }
  }

}
