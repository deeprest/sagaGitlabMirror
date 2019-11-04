using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Character
{
  [Header( "Turret" )]
  [SerializeField] Transform cannon;
  [SerializeField] float sightRange = 6;
  [SerializeField] float small = 0.1f;
  [SerializeField] float rotspeed = 20;
  Timer shootRepeatTimer = new Timer();
  [SerializeField] Transform shotOrigin;
  [SerializeField] Weapon weapon;

  void Start()
  {
    CharacterStart();
    UpdateLogic = UpdateTurret;
    UpdatePosition = null;
    UpdateCollision = null;
  }

  void UpdateTurret()
  {
    if( Global.instance.CurrentPlayer != null )
    {
      Vector2 pos = shotOrigin.position;
      Vector2 player = Global.instance.CurrentPlayer.transform.position;
      Vector2 delta = player - pos;
      Debug.DrawLine( player, pos, Color.white );
      if( delta.sqrMagnitude < sightRange * sightRange )
      {
        // orientation is facing right
        cannon.rotation = Quaternion.RotateTowards( cannon.rotation, Quaternion.Euler( 0, 0, Mathf.Rad2Deg * Mathf.Atan2( delta.y, delta.x ) ), rotspeed * Time.deltaTime );
        RaycastHit2D hit = Physics2D.Linecast( shotOrigin.position, player, LayerMask.GetMask( Global.DefaultProjectileCollideLayers ) );
        if( hit.transform != null && hit.transform.IsChildOf( Global.instance.CurrentPlayer.transform ) )
        {
          if( !shootRepeatTimer.IsActive )
            Shoot( delta );
        }
      }
    }
  }

  void Shoot( Vector3 shoot )
  {
    shootRepeatTimer.Start( weapon.shootInterval, null, null );
    Vector3 pos = shotOrigin.position;
    //if( !Physics2D.Linecast( transform.position, pos, LayerMask.GetMask( Global.ProjectileNoShootLayers ) ) )
    weapon.FireWeapon( this, pos, shoot );
  }
}
