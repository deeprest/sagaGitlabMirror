using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using Random = UnityEngine.Random;

 public enum Team
{
  None,
  GoodGuys,
  BadDudes,
  Hostile
}

public class Entity : MonoBehaviour, IDamage
{
  public static Limit<Entity> Limit = new Limit<Entity>();

  public PathAgent pathAgent;

  public Team Team;
  public Rigidbody2D body;
  public BoxCollider2D box;
  public CircleCollider2D circle;
  public SpriteRenderer renderer;
  public Animator animator;

  public List<Collider2D> IgnoreCollideObjects;
  public List<SpriteRenderer> spriteRenderers;
  // composite entities
  Entity rootEntity;

  public bool UseGravity = true;
  public bool IsStatic = false;
  public bool BoxCollisionOne = false;
  public Vector2 velocity;
  public Vector2 Velocity
  {
    get
    {
      if( carryCharacter == null )
        return velocity;
      else
        return velocity + carryCharacter.Velocity;
    }
  }
  public Vector2 inertia;
  public Vector2 overrideVelocity;
  public Timer overrideVelocityTimer = new Timer();
  public bool hanging { get; set; }
  // moving platforms / stacking characters
  public Entity carryCharacter;
  public float friction = 0.05f;
  
  public float raylength = 0.01f;
  public float contactSeparation = 0.01f;
  public float DownOffset = 0;

  public float Raydown 
  {
    get
    {
      return DownOffset + contactSeparation;
    }
  }

  
  // collision flags
  protected bool collideRight = false;
  protected bool collideLeft = false;
  protected bool collideTop = false;
  protected bool collideBottom = false;
  // cached for optimization - to avoid allocating every frame
  protected RaycastHit2D[] RaycastHits = new RaycastHit2D[4];
  // cached
  protected RaycastHit2D hit;
  protected int hitCount;
  protected Vector2 adjust;
  protected Vector2 boxOffset;

  [Header( "Pathing" )]
  public bool EnablePathing = false;
  public string AgentTypeName = "Small";

  [Header( "Damage" )]
  public bool CanTakeDamage = true;
  public int Health = 3;
  public int MaxHealth = 5;
  public GameObject explosion;
  public AudioClip soundHit;
  public GameObject spawnWhenDead;
  public int spawnChance = 1;
  // FLASH
  protected Timer flashTimer = new Timer();
  public float flashInterval = 0.05f;
  public int flashCount = 5;
  protected bool flip = false;
  protected readonly float flashOn = 1f;
  // deal this damage on collision
  public Damage ContactDamage;
  public UnityEvent EventDestroyed;

  // "collision" impedes this object's movement
  protected System.Action UpdateCollision;
  // "hit" inflicts damage on others
  protected System.Action UpdateHit;
  // integrate forces into position
  protected System.Action UpdatePosition;
  // brains!!
  protected System.Action UpdateLogic;

  public virtual void PreSceneTransition() { }
  public virtual void PostSceneTransition() { }

  protected virtual void Awake()
  {

    if( !Limit.OnCreate( this ) )
      return;
    EntityAwake();
  }
  
  public void EntityAwake()
  {
    RaycastHits = new RaycastHit2D[8];
  }

  protected virtual void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    Limit.OnDestroy( this );
    flashTimer.Stop( false );
    overrideVelocityTimer.Stop( false );
    if( rootEntity != null )
      rootEntity.RemoveChild( this );
    rootEntity = null;
  }

  protected virtual void Start()
  {
    if( transform.parent != null )
      rootEntity = transform.root.GetComponent<Entity>();
    IgnoreCollideObjects.AddRange( GetComponentsInChildren<Collider2D>() );
    spriteRenderers.AddRange( GetComponentsInChildren<SpriteRenderer>() );
    UpdateHit = BoxHit;
    if( !IsStatic )
    {
      if( BoxCollisionOne )
        UpdateCollision = BoxCollisionOneDown;
      else
        UpdateCollision = BoxCollisionFour;
      UpdatePosition = BasicPosition;
    }
    if( EnablePathing )
    {
      pathAgent = new PathAgent();
      pathAgent.Client = this;
      pathAgent.transform = transform;
      pathAgent.AgentTypeID = Global.instance.AgentType[AgentTypeName];
    }
    InitializeParts();
    
  }

  public virtual void EntityUpdate()
  {
    if( Global.Paused )
      return;

    if( UpdateLogic != null )
      UpdateLogic();

    if( UpdateHit != null )
      UpdateHit();

    if( UpdatePosition != null )
      UpdatePosition();

    if( UpdateCollision != null )
      UpdateCollision();

    //body.MovePosition( transform.position );
  }
  
  public void EntityLateUpdate()
  {
    if( !Global.instance.Updating )
      return;

    UpdateParts();
  }

  protected void BoxHit()
  {
    if( ContactDamage != null )
    {
      hitCount = Physics2D.BoxCastNonAlloc( box.transform.position, box.size, 0, velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
      for( int i = 0; i < hitCount; i++ )
      {
        hit = RaycastHits[i];

        IDamage dam = hit.transform.GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = this;
          dmg.damageSource = transform;
          dmg.point = hit.point;
          dam.TakeDamage( dmg );
        }
      }
    }
  }

  protected void CircleHit()
  {
    if( ContactDamage != null )
    {
      hitCount = Physics2D.CircleCastNonAlloc( circle.transform.position, circle.radius, velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
      for( int i = 0; i < hitCount; i++ )
      {
        hit = RaycastHits[i];

        IDamage dam = hit.transform.GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate( ContactDamage );
          dmg.instigator = this;
          dmg.damageSource = transform;
          dmg.point = hit.point;
          dam.TakeDamage( dmg );
        }

      }
    }
  }

  public void OverrideVelocity( Vector2 pVelocity, float duration )
  {
    overrideVelocityTimer.Stop( false );
    overrideVelocityTimer.Start( duration, delegate( Timer timer )
    {
      overrideVelocity = pVelocity;
    }, delegate
    {
      overrideVelocity = Vector2.zero;
    });
  }

  protected void BasicPosition()
  {

    if( overrideVelocityTimer.IsActive )
      velocity = overrideVelocity;

    if( UseGravity )
      velocity.y += -Global.Gravity * Time.deltaTime;

    if( collideTop )
    {
      velocity.y = Mathf.Min( velocity.y, 0 );
    }
    if( collideBottom )
    {
      velocity.x -= (velocity.x * friction) * Time.deltaTime;
      velocity.y = Mathf.Max( velocity.y, 0 );
    }
    if( collideRight )
    {
      velocity.x = Mathf.Min( velocity.x, 0 );
    }
    if( collideLeft )
    {
      velocity.x = Mathf.Max( velocity.x, 0 );
    }
    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );

    //velocity -= (velocity * airFriction) * Time.deltaTime;

    transform.position += (Vector3) Velocity * Time.deltaTime;
    carryCharacter = null;
  }

  protected void BoxCollisionVelocity()
  {
    // Do a single box cast in the direction of velocity
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    const float corner = 0.707f;
    boxOffset.x = box.offset.x * Mathf.Sign( transform.localScale.x );
    boxOffset.y = box.offset.y;
    adjust = (Vector2)transform.position + boxOffset;

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, velocity, RaycastHits, Mathf.Max( Raydown, -velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation + DownOffset;
        // moving platforms
        Entity cha = hit.transform.GetComponent<Entity>();
        if( cha != null )
        {
#if UNITY_EDITOR
          if( cha.GetInstanceID() == GetInstanceID() )
          {
            Debug.LogError( "character set itself as carry character", gameObject );
            Debug.Break();
          }
#endif
          carryCharacter = cha;
        }
        break;
      }
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        break;
      }
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        break;
      }
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        break;
      }
    }
    transform.position = adjust - boxOffset;
  }
  
  protected void CircleCollisionVelocity()
  {
    // Do a single circle cast in the direction of velocity
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    const float corner = 0.707f;
    adjust = transform.position;

    hitCount = Physics2D.CircleCastNonAlloc( adjust, circle.radius, velocity, RaycastHits, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
        continue;
      if( hit.normal.y > corner )
      {
        collideBottom = true;
        adjust.y = hit.point.y + circle.radius + contactSeparation + DownOffset;
        // moving platforms
        Entity cha = hit.transform.GetComponent<Entity>();
        if( cha != null )
        {
#if UNITY_EDITOR
          if( cha.GetInstanceID() == GetInstanceID() )
          {
            Debug.LogError( "character set itself as carry character", gameObject );
            Debug.Break();
          }
#endif
          carryCharacter = cha;
        }
        break;
      }
      if( hit.normal.y < -corner )
      {
        collideTop = true;
        adjust.y = hit.point.y - circle.radius - contactSeparation;
        break;
      }
      if( hit.normal.x > corner )
      {
        collideLeft = true;
        adjust.x = hit.point.x + circle.radius + contactSeparation;
        break;
      }
      if( hit.normal.x < -corner )
      {
        collideRight = true;
        adjust.x = hit.point.x - circle.radius - contactSeparation;
        break;
      }
    }
    transform.position = adjust;
  }
  
  protected void BoxCollisionOneDown()
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    const float corner = 0.707f;
    boxOffset.x = box.offset.x * Mathf.Sign( transform.localScale.x );
    boxOffset.y = box.offset.y;
    adjust = (Vector2)transform.position + boxOffset;

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.down, RaycastHits, Mathf.Max( Raydown, -velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.y > corner )
      {
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation + DownOffset;
        // moving platforms
        Entity cha = hit.transform.GetComponent<Entity>();
        if( cha != null )
        {
#if UNITY_EDITOR
        if( cha.GetInstanceID() == GetInstanceID() )
        {
          Debug.LogError( "character set itself as carry character", gameObject );
          Debug.Break();
        }
#endif
          carryCharacter = cha;
        }
        break;
      }
    }
    transform.position = adjust - boxOffset;
  }

  protected void BoxCollisionFour()
  {
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    const float corner = 0.707f;
    const float raydown = 0.2f;
    boxOffset.x = box.offset.x * Mathf.Sign( transform.localScale.x );
    boxOffset.y = box.offset.y;
    adjust = (Vector2)transform.position + boxOffset;

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.down, RaycastHits, Mathf.Max( raydown, -velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.y > corner )
      {
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
        collideBottom = true;

        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation + DownOffset;
        // moving platforms
        Entity cha = hit.transform.GetComponent<Entity>();
        if( cha != null )
        {
#if UNITY_EDITOR
          if( cha.GetInstanceID() == GetInstanceID() )
          {
            Debug.LogError( "character set itself as carry character", gameObject );
            Debug.Break();
          }
#endif
          carryCharacter = cha;
        }
        break;
      }
    }
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.up, RaycastHits, Mathf.Max( raylength, velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.y < -corner )
      {
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
        collideTop = true;
        adjust.y = hit.point.y - box.size.y * 0.5f - contactSeparation;
        break;
      }
    }
    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.left, RaycastHits, Mathf.Max( raylength, -velocity.x * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.x > corner )
      {
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
        collideLeft = true;
        adjust.x = hit.point.x + box.size.x * 0.5f + contactSeparation;
        break;
      }
    }

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.right, RaycastHits, Mathf.Max( raylength, velocity.x * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.x < -corner )
      {
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
        collideRight = true;
        adjust.x = hit.point.x - box.size.x * 0.5f - contactSeparation;
        break;
      }
    }

    transform.position = adjust - boxOffset;
  }

  protected virtual void Die()
  {
    Instantiate( explosion, transform.position, Quaternion.identity );
    if( spawnWhenDead != null && Random.Range( 0, spawnChance ) == 0 )
      Instantiate( spawnWhenDead, transform.position, Quaternion.identity );
    Destroy( gameObject );
    EventDestroyed?.Invoke();
  }

  public virtual bool TakeDamage( Damage d )
  {
    // dead characters will not absorb projectiles
    if( !CanTakeDamage || Health <= 0 )
      return false;
    if( d.instigator != null && d.instigator.Team == Team )
      return false;
    Health -= d.amount;
    if( Health <= 0 )
    {
      flashTimer.Stop( false );
      Die();
    }
    else
    {
      if( soundHit != null )
        Global.instance.AudioOneShot( soundHit, transform.position );
      // color pulse
      flip = false;
      
      foreach( var sr in spriteRenderers )
        sr.material.SetFloat( "_FlashAmount", flashOn );
      flashTimer.Start( flashCount * 2, flashInterval, delegate ( Timer t )
      {
        flip = !flip;
        foreach( var sr in spriteRenderers )
          sr.material.SetFloat( "_FlashAmount", flip? flashOn : 0 );
      }, delegate
      {
        foreach( var sr in spriteRenderers )
          sr.material.SetFloat( "_FlashAmount", 0 );
      } );
    }
    return true;
  }

  // Composite characters
  public void RemoveChild( Entity child )
  {
    Collider2D[] clds = IgnoreCollideObjects.ToArray();
    for( int i = 0; i < clds.Length; i++ )
    {
      if( clds[i].transform == child.transform )
      {
        IgnoreCollideObjects.Remove( clds[i] );
        break;
      }
    }

    // remove all sprite renderers from the child entity
    SpriteRenderer[] srs = spriteRenderers.ToArray();
    SpriteRenderer[] csrs = child.spriteRenderers.ToArray();
    for( int c = 0; c < csrs.Length; c++ )
    for( int i = 0; i < srs.Length; i++ )
    {
      if( srs[i].transform == csrs[c].transform )
      {
        spriteRenderers.Remove( srs[i] );
        break;
      }
    }
  }
  
  // for EventDestroyed unity events
  public void DestroyGameObject( GameObject go )
  {
    Destroy( go );
  }

  public virtual bool IsEnemyTeam( Team other )
  {
    return Team != Team.None && other != Team.None && other != Team;
  }
  
  
  [Header( "Character Parts" )]
  public int CharacterLayer;

  [System.Serializable]
  public class CharacterPart
  {
    public bool enabled = true;
    public Transform transform;
    public Animator animator;
    public Renderer renderer;
    public int layerAnimated;
  }
  
  public List<CharacterPart> CharacterParts;

  // Call from Awake()
  public virtual void InitializeParts()
  {
    // empty
  }

  // Call from LateUpdate()
  public void UpdateParts()
  {
    foreach( var part in CharacterParts )
      part.renderer.sortingOrder = CharacterLayer + part.layerAnimated;
  }
}
