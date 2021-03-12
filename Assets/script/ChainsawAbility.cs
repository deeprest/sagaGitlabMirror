using UnityEngine;

public class ChainsawAbility : Ability
{
  public float activeRate = 100;
  public float idleRate = 10;
  
  Animator animator;
  
  public Renderer chainRenderer;
  Material ChainMaterial;
  float offset;
  public float Speed = 1;
  float speed;

  // ParticleSystem smoke;
  AudioSource source;
  [SerializeField] AudioClip soundIdle;
  [SerializeField] AudioClip soundActive;

  public override void Equip( Transform parentTransform )
  {
    base.Equip( parentTransform );
    
    chainRenderer = go.transform.GetChild(0).GetComponent<Renderer>();
#if UNITY_EDITOR
    ChainMaterial = chainRenderer.sharedMaterial;
#else
        ChainMaterial = chainRenderer.material;
#endif
    
    // animator = go.GetComponent<Animator>();
    // animator.Play( "idle" );

    // smoke = go.GetComponent<ParticleSystem>();
    // smoke.Play();
    
    source = go.GetComponent<AudioSource>();
    source.clip = soundIdle;
    source.loop = true;
    source.Play();
  }
  
  public override void Activate( Vector2 origin, Vector2 aim )
  {
    IsActive = true;
    speed = Speed;
    
    /*ParticleSystem.EmissionModule emit = smoke.emission;
    ParticleSystem.MinMaxCurve rate = emit.rateOverTime;
    rate.constant = activeRate;
    emit.rateOverTime = rate;
    ParticleSystem.VelocityOverLifetimeModule volm = smoke.velocityOverLifetime;
    volm.speedModifierMultiplier = 3;*/

    source.clip = soundActive;
    source.Play();
  }

  public override void Deactivate()
  {
    IsActive = false;
    speed = 0; 
    
    /*ParticleSystem.EmissionModule emit = smoke.emission;
    ParticleSystem.MinMaxCurve rate = emit.rateOverTime;
    rate.constant = idleRate;
    emit.rateOverTime = rate;
    ParticleSystem.VelocityOverLifetimeModule volm = smoke.velocityOverLifetime;
    volm.speedModifierMultiplier = 1;*/
    
    source.clip = soundIdle;
    source.loop = true;
    source.Play();
  }

  public override void UpdateAbility()
  {
    base.UpdateAbility();
    offset += Time.deltaTime * speed;
    offset = Mathf.Repeat( offset, 1 );
    ChainMaterial.mainTextureOffset = new Vector2( offset, 0 );
  }
  
 
}