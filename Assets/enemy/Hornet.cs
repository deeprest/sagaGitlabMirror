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
  public float rotspeed = 5;

  void Start()
  {
    EnemyStart();
    UpdateEnemy = UpdateHornet;
  }

  // Update is called once per frame
  void UpdateHornet()
  {
    if( Global.instance.CurrentPlayer != null )
    {
      Vector3 player = Global.instance.CurrentPlayer.transform.position;
      Vector3 tpos = player + Vector3.up * up + Vector3.right * over;
      Vector3 delta = tpos - transform.position;
      if( delta.sqrMagnitude < small*small )
        velocity = Vector3.zero;
      else if( delta.sqrMagnitude < sightRange * sightRange )
      {
        velocity = (tpos-transform.position).normalized * flySpeed;
        // todo guns
        // todo missles
        // todo drop wheels
      }
      transform.rotation = Quaternion.RotateTowards( transform.rotation, Quaternion.Euler( 0, 0, velocity.x * -rot ), rotspeed * Time.deltaTime );
    }
  }

}
