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

  [SerializeField] float min = -90;
  [SerializeField] float max = 90;
  public float maxShootAngle = 5;
  public bool debugShoot = false;

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
      Vector2 pos = cannon.position;
      Vector2 player = Global.instance.CurrentPlayer.transform.position;
      Vector2 delta = player - pos;
      Debug.DrawLine( player, pos, Color.white );
      if( delta.sqrMagnitude < sightRange * sightRange )
      {
        Vector2 local = transform.worldToLocalMatrix.MultiplyVector( delta );
        // keep cannon between -90 and 90 degrees relative to turret base
        float angle = Mathf.Clamp( Util.NormalizeAngle( Mathf.Rad2Deg * Mathf.Atan2( local.y, local.x ) - 90 ), min, max );
        // todo prevent cannon from rotating outside of 180 degree range 
        //cannon.rotation = Quaternion.RotateTowards( cannon.rotation, Quaternion.Euler( 0, 0, angle ) * transform.localToWorldMatrix.rotation, rotspeed * Time.deltaTime );
        cannon.rotation = Quaternion.Euler( 0, 0, angle ) * transform.localToWorldMatrix.rotation;
        Vector2 aim = cannon.transform.up;
        RaycastHit2D hit = Physics2D.Linecast( shotOrigin.position, player, LayerMask.GetMask( Global.DefaultProjectileCollideLayers ) );
        if( hit.transform != null && hit.transform.IsChildOf( Global.instance.CurrentPlayer.transform ) )
        {
          if( debugShoot )
            if( Vector2.Angle( delta, aim ) < maxShootAngle )
              if( !shootRepeatTimer.IsActive )
                Shoot( aim );
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
