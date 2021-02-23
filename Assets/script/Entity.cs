#define DEBUG_CHECKS

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

[SelectionBase]
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
  public Vector2 velocity;

  public Vector2 Velocity
  {
    get
    {
  #if DEBUG_CHECKS
      if( carryCharacter!=null && carryCharacter.GetInstanceID() == GetInstanceID() ) 
      {
        Debug.LogError("recursive Velocity", gameObject );
        return velocity;
      }
  #endif
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
    get { return DownOffset + contactSeparation; }
  }


  // collision flags
  public bool collideRight = false;
  public bool collideLeft = false;
  public bool collideTop = false;
  public bool collideBottom = false;
  const float corner = 0.707106769f;
  // cached for optimization - to avoid allocating every frame
  protected RaycastHit2D[] RaycastHits = new RaycastHit2D[4];
  protected RaycastHit2D[] bottomHits;
  protected RaycastHit2D[] topHits;
  protected RaycastHit2D[] leftHits;
  protected RaycastHit2D[] rightHits;
  protected int bottomHitCount = 0;
  protected int topHitCount = 0;
  protected int leftHitCount = 0;
  protected int rightHitCount = 0;
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
  public GameObject[] SpawnWhenDead;
  // FLASH
  protected Timer flashTimer = new Timer();
  public float flashInterval = 0.05f;
  public int flashCount = 5;
  protected bool flip = false;
  protected readonly float flashOn = 1f;
  // deal this damage on collision
  public Damage ContactDamage;
  public UnityEvent EventDestroyed;

  // "collision" impedes *this* object's movement
  // Only things that move need collision.
  protected System.Action UpdateCollision;
  // "hit" inflicts damage on others
  protected System.Action UpdateHit;
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
      UpdateCollision = BoxCollisionSingle;
    }
    if( EnablePathing )
    {
      pathAgent = new PathAgent();
      pathAgent.Client = this;
      pathAgent.transform = transform;
      pathAgent.AgentTypeID = Global.instance.AgentType[AgentTypeName];
    }
    InitializeParts();

    bottomHits = new RaycastHit2D[RaycastHits.Length];
    topHits = new RaycastHit2D[RaycastHits.Length];
    leftHits = new RaycastHit2D[RaycastHits.Length];
    rightHits = new RaycastHit2D[RaycastHits.Length];
  }

  public virtual void EntityUpdate()
  {
    if( Global.Paused )
      return;

    if( UpdateLogic != null )
      UpdateLogic();

    if( UpdateHit != null )
      UpdateHit();

    UpdatePosition();

    if( UpdateCollision != null )
      UpdateCollision();
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
    overrideVelocityTimer.Start( duration, delegate( Timer timer ) { overrideVelocity = pVelocity; }, delegate { overrideVelocity = Vector2.zero; } );
  }

  void UpdatePosition() 
  {
    if( overrideVelocityTimer.IsActive )
      velocity = overrideVelocity;

    if( UseGravity )
      velocity.y += -Global.Gravity * Time.deltaTime;
    velocity.y = Mathf.Max( velocity.y, -Global.MaxVelocity );
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

    transform.position += (Vector3) Velocity * Time.deltaTime;
  }

  protected void BoxCollisionSingle()
  {
    carryCharacter = null;
    
    float dT = Time.deltaTime;
    collideRight = false;
    collideLeft = false;
    collideTop = false;
    collideBottom = false;
    bottomHitCount = 0;
    topHitCount = 0;
    rightHitCount = 0;
    leftHitCount = 0;

    boxOffset.x = box.offset.x * Mathf.Sign( transform.localScale.x );
    boxOffset.y = box.offset.y;
    adjust = (Vector2) transform.position + boxOffset;

    float downOffset = Mathf.Max( DownOffset, -velocity.y * dT );
    Vector2 debugOrigin = adjust; // + Vector2.down * downOffset * 0.5f;
    Vector2 debugBox = box.size; // + Vector2.up * downOffset; // + Vector2.right *  Mathf.Max( 0, Mathf.Abs(velocity.x * dT) );

    Bounds bounds = new Bounds( debugOrigin, debugBox );
    Debug.DrawLine( new Vector3( bounds.min.x, bounds.min.y ), new Vector3( bounds.min.x, bounds.max.y ), Color.red );
    Debug.DrawLine( new Vector3( bounds.min.x, bounds.max.y ), new Vector3( bounds.max.x, bounds.max.y ), Color.red );
    Debug.DrawLine( new Vector3( bounds.max.x, bounds.max.y ), new Vector3( bounds.max.x, bounds.min.y ), Color.red );
    Debug.DrawLine( new Vector3( bounds.max.x, bounds.min.y ), new Vector3( bounds.min.x, bounds.min.y ), Color.red );

    hitCount = Physics2D.BoxCastNonAlloc( debugOrigin, debugBox, 0 /*transform.rotation.eulerAngles.z*/, Vector2.down, RaycastHits, Mathf.Max( Raydown, -velocity.y * dT ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( IgnoreCollideObjects.Contains( hit.collider ) )
        continue;

      if( hit.normal.y > 0 && hit.normal.y > corner )
      {
        collideBottom = true;
        bottomHits[bottomHitCount++] = hit;
        velocity.y = Mathf.Max( velocity.y, 0 );
        inertia.x = 0;
        adjust.y = hit.point.y + debugBox.y * 0.5f + Raydown;

        // moving platforms
        Entity cha = hit.transform.GetComponent<Entity>();
        if( cha != null )
        {
          if( cha.GetInstanceID() == GetInstanceID() )
            Debug.LogError( "character tried to set itself as carry character", gameObject );
          else
            carryCharacter = cha;
        }
      }

      if( hit.normal.y < -corner )
      {
        collideTop = true;
        topHits[topHitCount++] = hit;
        velocity.y = Mathf.Min( velocity.y, 0 );
        adjust.y = hit.point.y - debugBox.y * 0.5f - contactSeparation;
      }

      if( hit.normal.x > corner )
      {
        collideLeft = true;
        leftHits[leftHitCount++] = hit;
        velocity.x = Mathf.Max( velocity.x, 0 );
        inertia.x = Mathf.Max( inertia.x, 0 );
        adjust.x = hit.point.x + debugBox.x * 0.5f + contactSeparation;
        // prevent clipping through angled walls when falling fast.
        /*velocity.y -= Util.Project2D( velocity, hit.normal ).y;*/
      }

      if( hit.normal.x < -corner )
      {
        collideRight = true;
        rightHits[rightHitCount++] = hit;
        velocity.x = Mathf.Min( velocity.x, 0 );
        inertia.x = Mathf.Min( inertia.x, 0 );
        adjust.x = hit.point.x - debugBox.x * 0.5f - contactSeparation;
        // prevent clipping through angled walls when falling fast.
        /*velocity.y -= Util.Project2D( velocity, hit.normal ).y;*/
      }
    }
    transform.position = adjust - boxOffset;
  }

  protected virtual void Die()
  {
    if( explosion != null )
      Instantiate( explosion, transform.position, Quaternion.identity );
    if( SpawnWhenDead.Length > 0 )
    {
      GameObject prefab = SpawnWhenDead[Random.Range( 0, SpawnWhenDead.Length )];
      if( prefab != null )
        Instantiate( prefab, transform.position, Quaternion.identity );
    }
    Destroy( gameObject );
    EventDestroyed?.Invoke();
  }

  public virtual void AddHealth( int amount )
  {
    Health += amount;
  }

  public virtual bool TakeDamage( Damage damage )
  {
    // dead characters will not absorb projectiles
    if( !CanTakeDamage || Health <= 0 )
      return false;
    if( damage.instigator != null && damage.instigator.Team == Team )
      return false;
    AddHealth( -damage.amount );
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
      flashTimer.Start( flashCount * 2, flashInterval, delegate( Timer t )
      {
        flip = !flip;
        foreach( var sr in spriteRenderers )
          sr.material.SetFloat( "_FlashAmount", flip ? flashOn : 0 );
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