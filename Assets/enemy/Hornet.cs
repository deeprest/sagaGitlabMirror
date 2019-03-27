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

  public Weapon weapon;
  Timer shootRepeatTimer = new Timer();
  public Transform shotOrigin;

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
      if( (player - transform.position).sqrMagnitude < sightRange * sightRange )
      {
        if( delta.sqrMagnitude < small * small )
          tvel = Vector3.zero;
        else
        {
          tvel = (tpos - transform.position).normalized * flySpeed;
          velocity += (tvel - velocity) * acc * Time.deltaTime;
          transform.rotation = Quaternion.RotateTowards( transform.rotation, Quaternion.Euler( 0, 0, Mathf.Clamp( velocity.x, -topspeedrot, topspeedrot ) * -rot ), rotspeed * Time.deltaTime );
        }

        // drop wheels
        if( tvel.x > 0 && !wheelDrop.IsActive )
        {
          wheelDrop.Start( wheelDropInterval, null, null );
          GameObject go = Global.instance.Spawn( dropPrefab, drop.position, Quaternion.identity );
          Physics2D.IgnoreCollision( go.GetComponent<Collider2D>(), GetComponent<Collider2D>() );
          Wheelbot wheelbot = go.GetComponent<Wheelbot>();
          wheelbot.wheelVelocity = Mathf.Sign( player.x - transform.position.x );
        }

        // guns
        //if( Physics2D.Linecast( shotOrigin.position, player, LayerMask.GetMask( Projectile.NoShootLayers )) )
        {
          if( player.x < transform.position.x && player.y < transform.position.y )
          {
            if( !shootRepeatTimer.IsActive )
              Shoot( new Vector3( -1, -1, 0 ) );  //player - transform.position );
          }
        }
        // todo missles
      }
    }
  }

  void Shoot( Vector3 shoot )
  {
    shootRepeatTimer.Start( weapon.shootInterval, null, null );
    Vector3 pos = shotOrigin.position;
    if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Projectile.NoShootLayers ) ) )
      weapon.FireWeapon( this, pos, shoot );
  }
}
