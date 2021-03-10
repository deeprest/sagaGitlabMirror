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
      
      GameObject[] prefab = GetDeathSpawnObjects();
      for( int i = 0; i < prefab.Length; i++ )
      {
        GameObject go = Instantiate( prefab[i], transform.position, Quaternion.identity, null );
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        if( ps != null )
        {
          ParticleSystem.MainModule mainModule = ps.main;
          mainModule.startSize = 0.25f * transform.lossyScale.x;
          
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
