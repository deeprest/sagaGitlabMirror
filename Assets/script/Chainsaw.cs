using UnityEngine;
using UnityEngine.U2D;

public class Chainsaw : Weapon
{
  [Header( "Chainsaw" )]
  [SerializeField] SpriteShapeRenderer chainRenderer;
  [SerializeField] ParticleSystem sparks;
  Material ChainMaterial;
  float offset;
  public float Speed = 1;
  float speed;
  public float Radius = 0.2f;
  public float Distance = 0.5f;
  AudioSource[] source;
  [SerializeField] AudioClip soundActive;
  [SerializeField] AudioClip soundGrind;
  public Damage ContactDamage;

  public override void Equip( Transform parentTransform )
  {
    base.Equip( parentTransform );

    chainRenderer = go.transform.GetChild( 0 ).GetComponent<SpriteShapeRenderer>();
#if UNITY_EDITOR
    if( !Application.isPlaying )
      ChainMaterial = chainRenderer.sharedMaterials[1];
    else
      ChainMaterial = chainRenderer.materials[1];
#else
    ChainMaterial = chainRenderer.materials[1];
#endif
    sparks = go.GetComponentInChildren<ParticleSystem>();
    source = go.GetComponents<AudioSource>();
  }

  public override void Activate( Vector2 origin, Vector2 aim )
  {
    IsActive = true;
    speed = Speed;
    source[0].clip = soundActive;
    source[0].loop = true;
    source[0].Play();
  }

  public override void Deactivate()
  {
    IsActive = false;
    speed = 0;
    sparks.Stop();
    source[0].Stop();
    source[1].Stop();
  }

  public override void UpdateAbility()
  {
    if( IsActive )
    {
      offset += Time.deltaTime * speed;
      offset = Mathf.Repeat( offset, 1 );
      ChainMaterial.mainTextureOffset = new Vector2( offset, 0 );
      
      bool AtLeastOneHit = false;
      Vector2 origin = go.transform.position;
      if( biped != null )
        origin = biped.GetShotOriginPosition();
      int hitCount = Physics2D.CircleCastNonAlloc( origin, Radius, go.transform.up, Global.RaycastHits, Distance, Global.DefaultProjectileCollideLayers );
      for( int i = 0; i < hitCount; i++ )
      {
        RaycastHit2D hit = Global.RaycastHits[i];
        if( hit.transform.root == go.transform.root )
          continue;
        AtLeastOneHit = true;
        IDamage dam = hit.transform.GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = pawn;
          dmg.damageSource = go.transform;
          dmg.point = hit.point;
          dam.TakeDamage( dmg );
        }

        sparks.Play();
        
        source[1].clip = soundGrind;
        source[1].loop = true;
        source[1].Play();
        /*ParticleSystem.EmissionModule emit = sparks.emission;
        ParticleSystem.MinMaxCurve rate = emit.rateOverTime;
        rate.constant = 300;
        emit.rateOverTime = rate;
        ParticleSystem.VelocityOverLifetimeModule volm = sparks.velocityOverLifetime;
        volm.speedModifierMultiplier = 1;*/
        
        // We don't want the saw to cut through walls/doors and hit objects on the other side.
        break;
      }
      if( !AtLeastOneHit )
      {
        sparks.Stop();
        source[1].Stop();
      }
    }
  }
}