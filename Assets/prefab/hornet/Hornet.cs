using UnityEngine;
using System.Collections;

public class Hornet : Character
{
  [SerializeField] float sightRange = 6;
  [SerializeField] float flySpeed = 2;
  [SerializeField] float hoverDistance = 0.5f;
  [SerializeField] string[] hoverLayers = new string[] { "Default" };
  [SerializeField] float up = 3;
  [SerializeField] float over = 2;
  [SerializeField] float small = 0.1f;
  [SerializeField] float rot = 15;
  [SerializeField] float rotspeed = 20;
  [SerializeField] float topspeedrot = 1;
  [SerializeField] float acc = 2;
  [SerializeField] float crashSpeed = 3;
  [SerializeField] float explosionInterval = 0.2f;
  [SerializeField] float explosionRange = 1;
  bool dying;
  Timer explosionTimer = new Timer();
  [SerializeField] GameObject junk;
  Vector2 tvel;
  Timer wheelDrop = new Timer();
  [SerializeField] float wheelDropInterval = 3;
  [SerializeField] int wheelDropsRemaining = 6;
  [SerializeField] Transform drop;
  [SerializeField] GameObject dropPrefab;

  [SerializeField] Weapon weapon;
  Timer shootRepeatTimer = new Timer();
  [SerializeField] Transform shotOrigin;

  void Start()
  {
    CharacterStart();
    UpdateLogic = UpdateHornet;
  }

  void OnDestroy()
  {
    explosionTimer.Stop( false );
  }

  protected override void Die()
  {
    dying = true;
    foreach( var c in colliders )
      c.enabled = false;
    UpdateHit = null;
    //UpdateCollision = null;
    explosionTimer.Start( 10, explosionInterval,
    delegate
    {
      Instantiate( explosion, transform.position + (Vector3)Random.insideUnitCircle * explosionRange, Quaternion.identity );
    },
    delegate
    {
      Destroy( gameObject );
      Instantiate( junk, transform.position, Quaternion.identity );
    } );
  }

  void UpdateHornet()
  {
    if( Global.instance.CurrentPlayer != null )
    {
      if( dying )
      {
        velocity += Vector2.down * crashSpeed * Time.deltaTime;
        if( collideBottom )
        {
          Destroy( gameObject );
          Instantiate( junk, transform.position, Quaternion.identity );
        }
        return;
      }
      Vector2 pos = transform.position;
      Vector2 player = Global.instance.CurrentPlayer.transform.position;
      Vector2 tpos = player + Vector2.up * up + Vector2.right * over;
      //Debug.DrawLine( player, tpos, Color.white );
      // hover above surface
      Vector2 delta = tpos - pos;
      SetPath( tpos );
      if( (player - pos).sqrMagnitude < sightRange * sightRange )
      {
        if( delta.sqrMagnitude < small * small )
          tvel = Vector2.zero;
        else
        {
          UpdatePath();
          velocity += (MoveDirection.normalized * flySpeed - velocity) * acc * Time.deltaTime;
          //tvel = delta.normalized * flySpeed;
          //velocity += (tvel - velocity) * acc * Time.deltaTime;
          transform.rotation = Quaternion.RotateTowards( transform.rotation, Quaternion.Euler( 0, 0, Mathf.Clamp( velocity.x, -topspeedrot, topspeedrot ) * -rot ), rotspeed * Time.deltaTime );
        }
        // drop wheels
        if( wheelDropsRemaining > 0 && velocity.x > 0 && !wheelDrop.IsActive )
        {
          wheelDropsRemaining--;
          wheelDrop.Start( wheelDropInterval, null, null );
          GameObject go = Global.instance.Spawn( dropPrefab, drop.position, Quaternion.identity );
          Physics2D.IgnoreCollision( go.GetComponent<Collider2D>(), GetComponent<Collider2D>() );
          Wheelbot wheelbot = go.GetComponent<Wheelbot>();
          wheelbot.wheelVelocity = Mathf.Sign( player.x - transform.position.x );
        }

        // guns
        RaycastHit2D hit = Physics2D.Linecast( shotOrigin.position, player, LayerMask.GetMask( Global.DefaultProjectileCollideLayers ) );
        if( hit.transform != null && hit.transform.IsChildOf( Global.instance.CurrentPlayer.transform ) )
        {
          if( player.x < transform.position.x && player.y < transform.position.y )
          {
            if( !shootRepeatTimer.IsActive )
              Shoot( new Vector3( -1, -1, 0 ) );
          }
        }

      }
    }
  }

  void Shoot( Vector3 shoot )
  {
    shootRepeatTimer.Start( weapon.shootInterval, null, null );
    Vector3 pos = shotOrigin.position;
    if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Global.ProjectileNoShootLayers ) ) )
      weapon.FireWeapon( this, pos, shoot );
  }
}
