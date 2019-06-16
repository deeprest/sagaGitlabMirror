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
      Vector2 pos = (Vector2)transform.position;
      Vector2 player = (Vector2)Global.instance.CurrentPlayer.transform.position;
      Vector2 tpos = player + Vector2.up * up + Vector2.right * over;
      Debug.DrawLine( player, tpos, Color.white );
      // hover above surface
      Vector2 delta = tpos - pos;
      SetPath( tpos );
      /*RaycastHit2D hit = Physics2D.Raycast( pos, Vector2.down, hoverDistance, LayerMask.GetMask( hoverLayers ) );
      if( hit.transform != null )
      {
        print( "down" );
        tpos = hit.point + Vector2.up * hoverDistance;
        delta = tpos - pos;
      }
      hit = Physics2D.Raycast( pos, Vector2.left, hoverDistance, LayerMask.GetMask( hoverLayers ) );
      if( hit.transform != null )
      {
        print( "left" );
        tpos = hit.point + Vector2.right * hoverDistance;
        delta = tpos - pos;
      }*/
      if( (player - pos).sqrMagnitude < sightRange * sightRange )
      {
        if( delta.sqrMagnitude < small * small )
          tvel = Vector2.zero;
        else
        {
          UpdatePath();
          velocity += (WaypointVector.normalized * flySpeed - velocity) * acc * Time.deltaTime;
          //tvel = delta.normalized * flySpeed;
          //velocity += (tvel - velocity) * acc * Time.deltaTime;
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
    if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Projectile.NoShootLayers ) ) )
      weapon.FireWeapon( this, pos, shoot );
  }
}
