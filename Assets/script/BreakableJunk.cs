using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BreakableJunk : Entity
{
  [Header("Breakable")]
  public float junkSpeed = 10;

  protected override void Die( Damage damage ) 
    {
      if( soundDeath != null )
        Global.instance.AudioOneShot( soundDeath, transform.position );
      
      if( explosion != null )
        Instantiate( explosion, transform.position, Quaternion.identity );
      
      GameObject prefab = GetDeathSpawnObject();
      if( prefab != null )
      {
        GameObject go = Instantiate( prefab, transform.position, Quaternion.identity );
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        if( ps != null )
        {
          ParticleSystem.VelocityOverLifetimeModule volm = ps.velocityOverLifetime;
          Vector2 vel = (Vector2)transform.position - damage.point;
          volm.x = vel.normalized.x * junkSpeed;
          volm.y = vel.normalized.y * junkSpeed;
          
          ParticleSystem.TextureSheetAnimationModule tsam = ps.textureSheetAnimation;
          tsam.SetSprite( 0, GetComponent<SpriteRenderer>().sprite );
        }
      }
      Destroy( gameObject );
      EventDestroyed?.Invoke();
    }
}
