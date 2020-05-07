using UnityEngine;
using System.Collections;

public class Hornet : Entity
{
  [SerializeField] float sightRange = 6;
  [SerializeField] float flySpeed = 2;
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
  Timer wheelDropTimer = new Timer();
  [SerializeField] float wheelDropInterval = 3;
  [SerializeField] int wheelDropsRemaining = 6;
  [SerializeField] Transform drop;
  [SerializeField] GameObject dropPrefab;

  [SerializeField] Weapon weapon;
  Timer shootRepeatTimer = new Timer();
  [SerializeField] Transform shotOrigin;

  // sight, target
  Collider2D[] results = new Collider2D[8];
  int LayerMaskCharacter;
  [SerializeField] Entity Target;
  Timer SightPulseTimer = new Timer();

  protected override void Start()
  {
    base.Start();
    UpdateLogic = UpdateHornet;

    LayerMaskCharacter = LayerMask.GetMask( new string[] { "character" } );
    SightPulseTimer.Start( int.MaxValue, 3, ( x ) => {
      // reaffirm target
      Target = null;
      int count = Physics2D.OverlapCircleNonAlloc( transform.position, sightRange, results, LayerMaskCharacter );
      for( int i = 0; i < count; i++ )
      {
        Collider2D cld = results[i];
        //Character character = results[i].transform.root.GetComponentInChildren<Character>();
        Entity character = results[i].GetComponent<Entity>();
        if( character != null && IsEnemyTeam( character.Team ) )
        {
          Target = character;
          break;
        }
      }
    }, null );
  }

  protected override void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    base.OnDestroy();
    explosionTimer.Stop( false );
    shootRepeatTimer.Stop( false );
    wheelDropTimer.Stop( false );
    SightPulseTimer.Stop( false );
  }

  protected override void Die()
  {
    dying = true;
    foreach( var c in IgnoreCollideObjects )
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

    if( Target == null )
    {
      // slow to a stop
      velocity += -velocity * 0.5f * acc * Time.deltaTime;
      transform.rotation = Quaternion.RotateTowards( transform.rotation, Quaternion.identity, rotspeed * Time.deltaTime );
    }
    else
    {
      Vector2 pos = transform.position;
      Vector2 targetpos = Target.transform.position;
      Vector2 tpos = targetpos + Vector2.up * up + Vector2.right * over;
      // hover above surface
      Vector2 delta = tpos - pos;
      pathAgent.SetPath( tpos );
      // seek player
      if( delta.sqrMagnitude < small * small )
        tvel = Vector2.zero;
      else
      {
        pathAgent.UpdatePath();
        velocity += (pathAgent.MoveDirection.normalized * flySpeed - velocity) * acc * Time.deltaTime;
      }
      // guns
      RaycastHit2D hit = Physics2D.Linecast( shotOrigin.position, targetpos, Global.DefaultProjectileCollideLayers );
      if( hit.transform != null && hit.transform.IsChildOf( Target.transform ) )
      {
        // drop wheels
        if( wheelDropsRemaining > 0 && velocity.x > 0 && !wheelDropTimer.IsActive )
        {
          wheelDropsRemaining--;
          wheelDropTimer.Start( wheelDropInterval, null, null );
          GameObject go = Global.instance.Spawn( dropPrefab, drop.position, Quaternion.identity );
          Physics2D.IgnoreCollision( go.GetComponent<Collider2D>(), GetComponent<Collider2D>() );
          Wheelbot wheelbot = go.GetComponent<Wheelbot>();
          wheelbot.wheelVelocity = Mathf.Sign( targetpos.x - transform.position.x );
        }
        if( targetpos.x < transform.position.x && targetpos.y < transform.position.y )
        {
          if( !shootRepeatTimer.IsActive )
            Shoot( new Vector3( -1, -1, 0 ) );
        }
      }

      transform.rotation = Quaternion.RotateTowards( transform.rotation, Quaternion.Euler( 0, 0, Mathf.Clamp( velocity.x, -topspeedrot, topspeedrot ) * -rot ), rotspeed * Time.deltaTime );
    }
  }

  void Shoot( Vector3 shoot )
  {
    shootRepeatTimer.Start( weapon.shootInterval, null, null );
    Vector3 pos = shotOrigin.position;
    if( !Physics2D.Linecast( transform.position, pos, Global.ProjectileNoShootLayers ) )
      weapon.FireWeapon( this, pos, shoot );
  }
}
