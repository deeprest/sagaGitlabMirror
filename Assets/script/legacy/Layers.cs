using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Layers : ScriptableObject 
{
  public string[] _CharacterCollideLayers;
  public string[] _CharacterSidestepLayers;
  public string[] _CharacterDamageLayers;
  public string[] _TriggerLayers;
  public string[] _WorldSelectableLayers;
  public string[] _ProjectileNoShootLayers;
  public string[] _DefaultProjectileCollideLayers;
  public string[] _FlameProjectileCollideLayers;
  public string[] _BouncyGrenadeCollideLayers;
  public string[] _StickyBombCollideLayers;
  public string[] _TurretSightLayers;
  public string[] _EnemySightLayers;

  public static int CharacterCollideLayers;
  public static int CharacterSidestepLayers;
  public static int CharacterDamageLayers;
  public static int TriggerLayers;
  public static int WorldSelectableLayers;
  public static int ProjectileNoShootLayers;
  public static int DefaultProjectileCollideLayers;
  public static int FlameProjectileCollideLayers;
  public static int BouncyGrenadeCollideLayers;
  public static int StickyBombCollideLayers;
  public static int TurretSightLayers;
  public static int EnemySightLayers;

  public void Initialize()
  {
    /*
     *  CharacterCollideLayers = LayerMask.GetMask( new string[] { "Default", "destructible", "triggerAndCollision" } ); //, "character", "enemy" };
      CharacterSidestepLayers = LayerMask.GetMask( new string[] { "character", "enemy" } );
      CharacterDamageLayers = LayerMask.GetMask( new string[] { "character" } );
      TriggerLayers = LayerMask.GetMask( new string[] { "trigger", "triggerAndCollision" } );
      WorldSelectableLayers = LayerMask.GetMask( new string[] { "worldselect" } );
      ProjectileNoShootLayers = LayerMask.GetMask( new string[] { "Default" } );
      DefaultProjectileCollideLayers = LayerMask.GetMask( new string[] { "Default", "character", "triggerAndCollision", "enemy", "destructible", "bouncyGrenade", "flameProjectile" } );
      FlameProjectileCollideLayers = LayerMask.GetMask( new string[] { "Default", "character", "triggerAndCollision", "enemy", "destructible", "bouncyGrenade" } );
      BouncyGrenadeCollideLayers = LayerMask.GetMask( new string[] { "character", "triggerAndCollision", "enemy", "projectile", "destructible", "flameProjectile" } );
      StickyBombCollideLayers = LayerMask.GetMask( new string[] { "Default", "character", "triggerAndCollision", "enemy", "projectile", "destructible", "flameProjectile" } );
      TurretSightLayers = LayerMask.GetMask( new string[] { "Default", "character", "triggerAndCollision", "destructible" } );
      EnemySightLayers = LayerMask.GetMask( new string[] { "Default", "character", "triggerAndCollision", "destructible" } );

    */
  }

}

