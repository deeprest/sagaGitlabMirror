using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum Team
{
  None,
  GoodGuys,
  BadDudes,
  Hostile
}

public class Entity : MonoBehaviour, IDamage
{
  public PathAgent pathAgent;

  public Team Team;
  public Rigidbody2D body;
  public BoxCollider2D box;
  public SpriteRenderer renderer;
  public Animator animator;

  public List<Collider2D> IgnoreCollideObjects;
  public List<SpriteRenderer> spriteRenderers;
  // when Characters are composite under same transform root
  Entity parentCharacter;

  public bool UseGravity = true;
  public bool IsStatic = false;
  public bool BoxCollisionOne = false;
  public Vector2 velocity = Vector2.zero;
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
  public Vector2 pushVelocity = Vector2.zero;
  protected Timer pushTimer = new Timer();
  // basic support for moving platforms / stacking characters
  public Entity carryCharacter;
  public float friction = 0.05f;
  public float raylength = 0.01f;
  public float contactSeparation = 0.01f;
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
  public int health = 5;
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
    RaycastHits = new RaycastHit2D[8];
  }

  protected virtual void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    flashTimer.Stop( false );
    pushTimer.Stop( false );
    if( parentCharacter != null )
      parentCharacter.RemoveChild( this );
    parentCharacter = null;
  }

  protected virtual void Start()
  {
    if( transform.parent != null )
      parentCharacter = transform.parent.GetComponent<Entity>();
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
  }



  void Update()
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

  protected void BoxHit()
  {
    if( ContactDamage != null )
    {
      hitCount = Physics2D.BoxCastNonAlloc( body.position, box.size, 0, velocity, RaycastHits, raylength, Global.CharacterDamageLayers );
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

  public void Push( Vector2 pVelocity, float duration )
  {
    pushVelocity = pVelocity;
    pushTimer.Start( duration );
  }

  protected void BasicPosition()
  {
    if( pushTimer.IsActive )
      velocity = pushVelocity;

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
    transform.position += (Vector3)Velocity * Time.deltaTime;
    carryCharacter = null;
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

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.down, RaycastHits, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.y > corner )
      {
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation;
        // moving platforms
        Entity cha = hit.transform.GetComponent<Entity>();
        if( cha != null )
          carryCharacter = cha;
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
    boxOffset.x = box.offset.x * Mathf.Sign( transform.localScale.x );
    boxOffset.y = box.offset.y;
    adjust = (Vector2)transform.position + boxOffset;

    hitCount = Physics2D.BoxCastNonAlloc( adjust, box.size, 0, Vector2.down, RaycastHits, Mathf.Max( raylength, -velocity.y * Time.deltaTime ), Global.CharacterCollideLayers );
    for( int i = 0; i < hitCount; i++ )
    {
      hit = RaycastHits[i];
      if( hit.normal.y > corner )
      {
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains( hit.collider ) )
          continue;
        collideBottom = true;
        adjust.y = hit.point.y + box.size.y * 0.5f + contactSeparation;
        // moving platforms
        Entity cha = hit.transform.GetComponent<Entity>();
        if( cha != null )
          carryCharacter = cha;
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
        //hitLeft = hit;
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
        //hitRight = hit;
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
    if( !CanTakeDamage || health <= 0 )
      return false;
    if( d.instigator != null && d.instigator.Team == Team )
      return false;
    health -= d.amount;
    if( health <= 0 )
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
        if( flip )
          foreach( var sr in spriteRenderers )
            sr.material.SetFloat( "_FlashAmount", flashOn );
        else
          foreach( var sr in spriteRenderers )
            sr.material.SetFloat( "_FlashAmount", 0 );
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

    SpriteRenderer[] srs = spriteRenderers.ToArray();
    for( int i = 0; i < srs.Length; i++ )
    {
      if( srs[i].transform == child.transform )
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
}
