#define DEBUG_LINES
// mesh must be "quad"
// use the "indexed-character" material
// deadbody must be updated when this is changed!
#define BILLBOARD
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using System.Reflection;
using LitJson;


public enum CharacterEventEnum : uint
{
  DebugAddAffinity,
  DebugSubAffinity,
  Damage,
  Death,
  Burial,
  DeclareAttack,
  Birth,
  FoodAdded
}

public class CharacterInfo
{
  public int id;
  public string name = "unknown";
  public bool alive = true;
  public bool female = true;
}

public class CharacterEvent
{
  // serialized
  public int id;
  public CharacterEventEnum type;
  public Vector3 position;
  public float time;
  public int CharacterIdA;
  public int CharacterIdB;
  // transient
  public Character CharacterA;
  public Character CharacterB;

  public CharacterEvent( CharacterEventEnum evtType, Vector3 evtPosition, float evtTime, Character evtCharacterA, Character evtCharacterB )
  {
    id = ++Global.Instance.GlobalEventID;
    Global.Instance.CharacterEventRecord[ id ] = this;
    type = evtType;
    position = evtPosition;
    time = evtTime;
    CharacterIdA = evtCharacterA.id;
    CharacterIdB = evtCharacterB.id;
    // assumed to be non-null
    CharacterA = evtCharacterA;
    CharacterB = evtCharacterB;
  }

  public CharacterEvent( JsonData ob )
  {
    Deserialize( ob );
  }

  void Deserialize( JsonData ob )
  {
    id = (int)ob[ "id" ].GetNatural();
    type = (CharacterEventEnum)ob[ "type" ].GetNatural();
    position = JsonUtil.ReadVector( ob[ "position" ] );
    CharacterIdA = (int)ob[ "cidA" ].GetNatural();
    CharacterIdB = (int)ob[ "cidB" ].GetNatural();
  }

  public void Serialize( JsonWriter writer )
  {
    writer.WriteObjectStart();
    writer.WritePropertyName( "id" );
    writer.Write( id );
    writer.WritePropertyName( "type" );
    writer.Write( (uint)type );
    writer.WritePropertyName( "position" );
    JsonUtil.WriteVector( writer, position );
    writer.WritePropertyName( "time" );
    writer.Write( time );
    writer.WritePropertyName( "cidA" );
    writer.Write( CharacterIdA );
    writer.WritePropertyName( "cidB" );
    writer.Write( CharacterIdB );
    writer.WriteObjectEnd();
  }
}

[SelectionBase]
[RequireComponent( typeof(Ball), typeof(Tags), typeof(AudioSource) )]
public partial class Character : SerializedComponent, IDamage, IAction, ILimit, ITeam
{

  #region Variables

  public bool IsUnderLimit()
  {
    return Limit.IsUnderLimit();
  }

  public static Limit<Character> Limit = new Limit<Character>();

  public delegate void ConsiderStateChange( Interest Interest );
  // serialized only to view in inspector
  [System.Serializable]
  public class CharacterState
  {
    public CharacterState( string name, int priority, ConsiderStateChange consider, System.Action push, System.Action update, System.Action pop, System.Action resume, System.Action suspend, bool isPlayerState = false )
    {
      Name = name;
      Priority = priority;
      Consider = consider;
      Update = update;
      Push = push;
      Pop = pop;
      Suspend = suspend;
      Resume = resume;
      IsPlayerState = isPlayerState;
    }

    public string Name;
    public int Priority;
    public ConsiderStateChange Consider;
    public System.Action Push;
    public System.Action Update;
    public System.Action Pop;
    public System.Action Suspend;
    public System.Action Resume;
    public bool IsPlayerState;

    //[System.NonSerialized]
    // assigned when state is active. Used to resolve state change when priority is equal.
    public Interest SourceInterest = null;
    // potential improvements:
    // flag to enforce a single instance on the stack
    // differentiate between successful state completion and cancellation.
    // Additive/Persistent states that are not on the stack
  }

  [System.Serializable]
  public class CharacterStateParameters
  {
    public string Name;
    public int Priority;
  }

  [System.Serializable]
  public class Interest
  {
    public enum Type
    {
      Tag,
      Instance
      //Type
    }

    // associated state name
    public string verb = null;

    public Type type = Type.Tag;
    // interest is triggered by object with this tag
    public Tag tag = default(Tag);
    public int InstanceID = 0;
    public string OtherType = null;

    public bool OverridePriority = false;
    // priority of this tag for the associated state
    public int Priority = 0;
    // extra conditions
    public string condition = null;

    public bool AlwaysConsider = false;

    [System.NonSerialized]
    // book-keeping. Index into the pool of interests.
    public int index;

    [Header( "Context" )]
    //[System.NonSerialized]
    public GameObject go;
    //[System.NonSerialized]
    public Vector3 objectPositionWhenSensed;

    public Interest()
    {
    }

    public Interest( string verb, Tag tag )
    {
      type = Type.Tag;
      this.verb = verb;
      this.tag = tag;
    }

    public Interest( string verb, int instanceID )
    {
      type = Type.Instance;
      this.verb = verb;
      this.InstanceID = instanceID;
    }

    public void AssignFrom( Interest rhs )
    {
      verb = rhs.verb;
      tag = rhs.tag;
      InstanceID = rhs.InstanceID;
      OverridePriority = rhs.OverridePriority;
      Priority = rhs.Priority;
      condition = rhs.condition;
      AlwaysConsider = rhs.AlwaysConsider;
      // context
      go = rhs.go;
      objectPositionWhenSensed = go.transform.position;
    }


  }

  public class InterestPool
  {
    int InterestPoolCount;
    bool[] freeslot;
    Interest[] Pool;

    public InterestPool( int count )
    {
      InterestPoolCount = count;
      freeslot = new bool[InterestPoolCount];
      Pool = new Interest[InterestPoolCount];
      for( int i = 0; i < InterestPoolCount; i++ )
      {
        freeslot[ i ] = true;
        Pool[ i ] = new Interest();
        Pool[ i ].index = i;
      }
    }

    public Interest GetFreeInterest()
    {
      for( int i = 0; i < InterestPoolCount; i++ )
      {
        if( freeslot[ i ] )
        {
          freeslot[ i ] = false;
          return Pool[ i ];
        }
      }
      //Debug.Log( "out of free interests" );
      return null;
    }

    public void RecycleInterest( Interest interest )
    {
      freeslot[ interest.index ] = true;
    }
  }

  // Base set of interests that all characters have in common.
  // Example: eat when hungry, sleep when tired.
  public List<Interest> IntrinsicInterests;

  // Interests associated with a job title, such as Soldier, Farmer.
  // Example: soldiers have a higher priority of attack and patrol, farmers prefer to gather.
  public List<Interest> VocationInterests;
  // These interests accumulate during the lifetime of the character, and are created from
  // personal experience. Example: affinity for specific individuals.
  public List<Interest> PersonalInterests;
  // The operative list of interests. A composite of all categories of interests.
  public List<Interest> Interests;

  // When processed, these are considered and acted upon, or disregarded.
  public List<Interest> ActiveInterests;
  InterestPool interestPool;
  int MaxActiveInterests;


  [System.Serializable]
  public class AffinityData
  {
    public int CharacterId;
    public int Value;
  }
  // key is instance ID
  public Dictionary<int,AffinityData> Affinity = new Dictionary<int, AffinityData>();
  // exists purely to view in inspector
  public List<AffinityData> DebugAffinity = new List<AffinityData>();
  // serialize pairs as separate lists
  [Serialize] public List<int> SerializedAffinity = new List<int>();

  [Serialize] public List<int> KnownEventID = new List<int>();
  List<CharacterEvent> KnownEvents = new List<CharacterEvent>();

  const int AffModDeath = -10;
  const int AffModDamage = -3;
  const int AffModFoodAdded = 3;
  const int AffModTalk = 1;

  const int AffPeaceful = -20;
  const int AffTalk = -10;
  const int AffShare = 5;
  const int AffFollow = 10;
  const int AffLoyal = 20;

  public List<Tag> AttackTags;

  bool Initialized = false;
  Collider[] SensorColliders;
  RaycastHit[] RaycastHits;

  [HideInInspector] UnityEngine.Networking.NetworkIdentity nid;

  [Header( "Identity" )]

  [Serialize] public string CharacterName;
  [Serialize] public string IdentityName;
  [Serialize] public int SkinIndex;
  [System.NonSerialized] public Identity identity = null;
  [System.NonSerialized] public Team Team;
  [Serialize] public string TeamName;

  public Vocation Vocation;
  [Serialize] public string VocationName = "Soldier";

  public TextMesh speechTextMesh;
  public MeshRenderer textRenderer;
  public Timeout speechTimeout;
  public SpriteAnimation FaceAnim;
  float faceAnimTime;
  float FaceResetInterval = 3f;

  [Header( "State" )]
  public string CurrentStateName;
  string InitialState = "Idle";
  public List<CharacterStateParameters> StateParameters;
  List<CharacterState> states = new List<CharacterState>();
  public List<CharacterState> Stack = new List<CharacterState>();
  public CharacterState CurrentState;



  [Header( "Movement" )]
  public Vector3 MoveDirection;
  public Vector3 FaceDirection = Vector3.forward;
  public bool FaceDirectionOfMovement = true;
  public float TurnSpeed = 180;
  public float WalkSpeed = 15;
  float SprintSpeed;
  public float SprintSpeedLow = 25;
  public float SprintSpeedHigh = 30;
  float CurrentMoveSpeed;
  public float DebugVelocity;
  public float runBounceRate = 0.4f;
  public float minRunRate = 0f;
  public float maxRunRate = 10f;
  public float moveThreshold = 0.1f;
  float lastMoveMag = 0f;
  // sidestep
  public bool SidestepAvoidance = true;
  bool DefaultSidestepAvoidance = true;
  float SidestepLast;
  Vector3 Sidestep;
  // pathing
  public bool HasPath = false;
  public float WaypointRadii = 0.1f;
  public float DestinationRadius = 0.3f;
  public Vector3 DestinationPosition;
  public System.Action OnPathEnd;
  public System.Action OnPathCancel;

  public NavMeshPath nvp;
  float PathEventTime;
  public List<Vector3>.Enumerator waypointEnu;
  public List<Vector3> waypoint = new List<Vector3>();
  public List<LineSegment> debugPath = new List<LineSegment>();

  public struct LineSegment
  {
    public Vector3 a;
    public Vector3 b;
  }

  [Header( "Health and Damage" )]
  [Serialize] public int Health = 3;
  int cachedHealth;
  // full health can increase
  int HealthFull = 3;

  public float HealthNormalized{ get { return (float)Health / (float)HealthFull; } }
  // full health start value
  public int HealthFullLow = 3;
  // full health limit
  public int HealthFullHigh = 10;
  [Range( 0, 1 )]
  public float HealthRegenAmount = 0.1f;
  protected bool Dead = false;
  bool IsTakingDamage = false;
  bool TakingDamageToggle;
  float TakingDamageStartTime;
  float TakingDamageLastToggleTime;
  float TakingDamageDuration = 1f;
  float TakingDamageToggleDuration = 0.05f;
  public AudioClip TakeDamageSound;
  public List<DamageType> DamageIgnore = new List<DamageType>();
  bool IsBleeding = false;
  float BleedStartTime = 0f;
  float BleedDuration = 5f;
  public BlendShape healthBar;

  [Header( "Destination" )]
  [Serialize] public Destination Home;
  [Serialize] public Bed Bed;
  public List<Destination> KnownDestinations = new List<Destination>();
  [Serialize] public List<int> KnownDestinationsID = new List<int>();


  [Header( "Age" )]
  public bool CanAge = true;
  [Serialize] public float Age = 0;
  public float AgeUpdateInterval = 10;
  float LastAgeUpdate;
  public float AgeMaxSeconds = 300;
  public AnimationCurve BallSizeNormal;
  public float WalkerOffsetMax = 0;
  public AnimationCurve SpriteScale;
  // youngest / smallest
  public float AudioPitchMin = 1.6f;
  // oldest
  public float AudioPitchMax = 0.8f;
  public AnimationCurve AudioPitchNormal;

  [Header( "Death" )]
  public GameObject DeadBodyPrefab;
  // used for effects
  public GameObject DeathPrefab;
  public AudioClip DeathSound;
  Vector3 lastHitOnMe = Vector3.zero;

  // kills
  [SerializeField] public int KillCount = 0;
  public BlendShape killBar;
  public float lastKillTime;


  [Header( "Stamina" )]
  public float StaminaFullLow = 3;
  public float StaminaFullHigh = 10;
  public float StaminaLossRate = -1;
  public float StaminaRecoverRate = 1;
  public BlendShape StaminaBar;
  // stamina affects movement speed
  public float Stamina = 3;
  // full stamina lowers with age
  public float StaminaFull = 1;

  [Header("Wake")]
  public float Wake = 360;
  public float WakeFull = 100;
  public float WakeLossRate = -1;
  public float WakeGainRate = 3;


  [Header( "Misc" )]
  // affects your ability to command others
  [Serialize] public int Rank = 0;
  int cachedRankForPlayerState = 0;
  // bats and "flying" creatures would not have footprints.
  public bool Footprints = true;
  public BlendShape Mouth;
  // can hide in bushes, etc
  public bool IsHidden = false;

  [Header( "Component" )]
  public Tags tags;
  public Rigidbody body;
  public Renderer myRenderer;
  public Transform moveTransform;
  #if BILLBOARD
  public Billboard billboard;
  #endif
  public Walker walk;
  public AudioSource audioSource;
  public Ball ball;
  public ColorPulse colorPulse;

  [Header( "Prototype" )]
  public SpriteAnimationDirectional sad;
  public bool meshOriginBottom = false;
  const float meshOriginBottomFudge = 0.0001f;
  public JabberPlayer JabberPlayer;


  [Header( "Carry / Hold" )]
  public float HoldRadius = 0.3f;
  public Transform HeldObject = null;
  Quaternion HeldObjectInitialRotationOffset;

  #endregion


  public override void BeforeSerialize()
  {
    if( isPlayerControlled )
    {
      PopState();
    }

    if( Team != null )
      TeamName = Team.name;
    if( identity != null )
      IdentityName = identity.name;

    swag.Clear();
    for( int i = 0; i < RightHandItemMount.childCount; i++ )
      swag.Add( RightHandItemMount.GetChild( i ).gameObject.name );
    for( int i = 0; i < LeftHandItemMount.childCount; i++ )
      swag.Add( LeftHandItemMount.GetChild( i ).gameObject.name );

    if( KnownDestinations.Count > 0 )
    {
      foreach( Destination dst in KnownDestinations )
        if( dst != null )
          KnownDestinationsID.Add( dst.id );
    }

    if( KnownEvents.Count > 0 )
    {
      foreach( CharacterEvent evt in KnownEvents )
        KnownEventID.Add( evt.id );
    }

    SerializedAffinity.Clear();
    if( Affinity.Count > 0 )
    {
      foreach( var pair in Affinity )
      {
        if( pair.Value.CharacterId != 0 )
        {
          SerializedAffinity.Add( pair.Key );
          SerializedAffinity.Add( pair.Value.Value );
        }
      }
    }
  }

  public override void AfterDeserialize()
  {
    cachedHealth = Health;

    foreach( string resourceName in swag )
    {
      GameObject resourcePrefab = Global.Instance.ItemPrefabs.Find( x => x.name == resourceName );
      if( resourcePrefab == null )
      {
        Debug.LogWarning( "failed to deserialize inventory item " + resourceName );
      }
      else
      {
        GameObject go = GameObject.Instantiate( resourcePrefab, Vector3.zero, Quaternion.identity, null );
        go.name = resourceName;
        if( !AcquireObject( go ) )
          GameObject.Destroy( go );
      }
    }
    swag.Clear();

    KnownDestinations.Clear();
    foreach( var sf in KnownDestinationsID )
    {
      Destination dst = SerializedObject.ResolveComponentFromId<Destination>( sf );
      if( dst != null )
        KnownDestinations.Add( dst );
    }
    KnownDestinationsID.Clear();

    KnownEvents.Clear();
    foreach( var evtid in KnownEventID )
    {
      if( Global.Instance.CharacterEventRecord.ContainsKey( evtid ) )
      {
        CharacterEvent evt = Global.Instance.CharacterEventRecord[ evtid ];
        /*SerializedComponent sca = SerializedObject.ResolveComponentFromId( evt.CharacterIdA );
        if( sca!=null )
          evt.CharacterA = (Character)sca;
        SerializedComponent scb = SerializedObject.ResolveComponentFromId( evt.CharacterIdB );
        if( scb!=null )
          evt.CharacterB = (Character)scb;*/
        KnownEvents.Add( evt );
      }
      else
        Debug.LogError( "event id not found when deserializing known events", this );
    }
    KnownEventID.Clear();

    Affinity.Clear();
    for( int i = 0; i < SerializedAffinity.Count; i += 2 )
    {
      CharacterInfo info = null;
      if( Global.Instance.CharacterInfoLookup.ContainsKey( SerializedAffinity[ i ] ) )
      {
        info = Global.Instance.CharacterInfoLookup[ SerializedAffinity[ i ] ];
        ModifyAffinity( info.id, SerializedAffinity[ i + 1 ] );
      }
      else
        Debug.LogError( "char id not found when deserializing affinity", this );
    }

    if( IsPregnant )
    {
      PregnantTimer = new Timer( GestationDuration, GestateUpdate, SpawnOffspring );
      PregnantTimer.SetProgress( GestationProgress );
    }

    Initialize();
  }

  void Awake()
  {
    if( !Limit.OnCreate( this ) )
    {
      return;
    }

    if( tags == null )
      tags = GetComponent<Tags>();
    if( ball == null )
      ball = GetComponent<Ball>();
    if( moveTransform == null )
      moveTransform = transform;
    if( body == null )
      body = GetComponent<Rigidbody>();
    if( audioSource == null )
      audioSource = GetComponent<AudioSource>();

    nvp = new NavMeshPath();

    //player states
    AddState( new CharacterState( "Construction", 200, null, PushConstruction, UpdateConstruction, PopConstruction, null, null, true ) );
    AddState( new CharacterState( "DiageticMenu", 160, null, PushDiageticMenu, UpdateDiageticMenu, PopDiageticMenu, null, null, true ) );
    AddState( new CharacterState( "ContextMenu", 150, null, PushContextMenu, UpdateContextMenu, PopContextMenu, PushContextMenu, null, true ) );
    AddState( new CharacterState( "PlayerControlled", 100, null, PushPlayer, UpdatePlayer, PopPlayer, ResumePlayer, SuspendPlayer, true ) );
    // non player
    AddState( new CharacterState( "Flee", 90, ConsiderFlee, PushFlee, UpdateFlee, null, PushFlee, null ) );
    AddState( new CharacterState( "Attack", 80, ConsiderAttack, PushAttack, UpdateAttack, PopAttack, PushAttack, null ) );
    AddState( new CharacterState( "Pickup", 70, ConsiderPickup, PushPickup, UpdatePickup, PopPickup, null, null ) );
    AddState( new CharacterState( "Follow", 60, ConsiderFollow, PushFollow, UpdateFollow, PopFollow, PushFollow, PopFollow ) );
    AddState( new CharacterState( "Investigate", 30, ConsiderInvestigate, PushInvestigate, UpdateInvestigate, null, PushInvestigate, null ) );
    AddState( new CharacterState( "Sleep", 25, ConsiderSleep, PushSleep, UpdateSleep, PopSleep, null, null ) );
    AddState( new CharacterState( "Gather", 20, ConsiderGather, PushGather, UpdateGather, PopGather, PushGather, null ) );
    AddState( new CharacterState( "Wander", 10, null, PushWander, UpdateWander, PopWander, PushWander, PopWander ) );
    AddState( new CharacterState( "Converse", 5, ConsiderConverse, PushConverse, UpdateConverse, PopConverse, null, null ) );
    AddState( new CharacterState( "Idle", 0, null, PushIdle, UpdateIdle, null, PushIdle, null ) );

    HealthFull = HealthFullLow;
    Health = HealthFull;
    if( CanAge )
      UpdateAge();

  }

  void OnDestroy()
  {
    Limit.OnDestroy( this );
    if( !Global.IsQuiting )
    {
      if( InteractIndicator != null )
        GameObject.Destroy( InteractIndicator );
    }
    if( isPlayerControlled )
    {
      // avoid serializing characters with the player's special temporary rank
      Rank = cachedRankForPlayerState;
      CleanupPlayerState();
    }
  }


  public void SetTeam( Team team )
  {
    // be sure to call Configure()
    tags.tags.Remove( Team.Tag );

    Team = team;
    TeamName = team.name;
    myRenderer.material.SetColor( "_IndexColor", Team.Color );

    if( !tags.HasTag( Team.Tag ) )
      tags.tags.Add( Team.Tag );

    // if enemy is converted, avoid attacking friends
    if( AttackTags.Contains( Team.Tag ) )
      AttackTags.Remove( Team.Tag );
  }

  public void AssignVocation( Vocation voc )
  {
    // be sure to call Configure()
    Vocation = voc;
    VocationName = Vocation.name;
  }

  void Configure()
  {
    Interests.Clear();
    Interests.AddRange( IntrinsicInterests );

    if( Team != null )
      Interests.AddRange( Team.Interests );

    if( Vocation != null )
      Interests.AddRange( Vocation.Interests );

    // (Modify Affinity adds attack interest)
    Interests.AddRange( PersonalInterests );

    // after all interests have been added
    AttackTags.Clear();
    foreach( var i in Interests )
      if( i.type == Interest.Type.Tag && i.verb == "Attack" )
        AttackTags.Add( i.tag );


    // apply intrinsic state priorities
    foreach( CharacterStateParameters sp in StateParameters )
    {
      CharacterState state = states.Find( x => x.Name == sp.Name );
      if( state == null )
        Debug.LogWarning( "stateparameter: state param not found: " + sp.Name );
      else
        state.Priority = sp.Priority;
    }

    if( Vocation != null )
    {
      foreach( CharacterStateParameters sp in Vocation.StateParameters )
      {
        CharacterState state = states.Find( x => x.Name == sp.Name );
        if( state == null )
          Debug.LogWarning( "stateparameter: state param not found: " + sp.Name );
        else
          state.Priority = sp.Priority;
      }
    }
  }

  //  void Start()
  //  {
  //    Initialize();
  //  }

  void Initialize()
  {
    if( Initialized )
      return;
    Initialized = true;
    MaxActiveInterests = 100;
    interestPool = new InterestPool( 50 );
    SensorColliders = new Collider[ MaxActiveInterests ];
    RaycastHits = new RaycastHit[ 4 ];

    // Team
    if( TeamName.Length > 0 )
      Team = Global.Instance.gameData.FindTeam( TeamName );
    if( Team != null )
      SetTeam( Team );

    // Vocation
    if( VocationName == "random" )
      Vocation = Global.Instance.gameData.Vocation[ Random.Range( 0, Global.Instance.gameData.Vocation.Count ) ];
    else
      Vocation = Global.Instance.gameData.Vocation.Find( x => x.name == VocationName );
    if( Vocation != null )
      AssignVocation( Vocation );

    // Personal
    // Affinity => personal interests

    // Identity
    // if serialized, the identity name should have a length
    if( IdentityName.Length > 0 )
    {
      identity = Global.Instance.gameData.FindIdentity( IdentityName );
      // random skin for random identity
      if( IdentityName == "human" || IdentityName == "animal" )
        SkinIndex = Random.Range( 0, identity.sprites.Count );
    }
    if( identity != null )
    {
      if( CharacterName.Length == 0 )
        CharacterName = Global.Instance.GenerateRandomName();
      #if BILLBOARD
      if( billboard != null )
      {
        billboard.sprite = identity.sprites[ SkinIndex ];
        billboard.UpdateMesh();
      }
      #endif
      JabberPlayer.jabber = identity.jabber;
    }

    Configure();


    PlayFaceAnimation( "idle" );
    
    int[] patience = new int[]{ 5, 15, 30 };
    IdlePatience = patience[ Random.Range( 0, patience.Length ) ];

    if( CanAge )
    {
      Age = Mathf.Min( Age, Mathf.Max( 1, AgeMaxSeconds ) );
      UpdateAge();
    }

    healthBar.alpha = (float)Health / (float)HealthFull;
    killBar.alpha = Mathf.Min( 1f, Mathf.Sqrt( (float)KillCount ) / 10f );

    if( !isPlayerControlled )
    {
      PushState( InitialState );
      // set follow character and state to "Follow" after the initial state is pushed
      if( FollowCharacter != null )
        Follow( FollowCharacter, null );
    }

    InvestigatePosition = moveTransform.position;

    if( CurrentWeaponPrefab == null && AmmoPrefab.Count > 0 )
      CurrentWeaponPrefab = AmmoPrefab[ 0 ].GetComponent<Attack>();
    SelectLeftHand( 0 );
    SelectRightHand( 0 );
    if( ShieldTransform != null )
      ShieldTransform.localPosition = ShieldInactiveLocal;

    if( !Global.Instance.CharacterInfoLookup.ContainsKey( id ) )
    {
      CharacterInfo info = new CharacterInfo();
      info.id = id;
      info.name = CharacterName;
      if( identity != null )
        info.female = identity.female;
      Global.Instance.CharacterInfoLookup[ id ] = info;
    }
  }

  public void UpdateSensor()
  {
    if( isPlayerControlled )
      return;

    {
      int count = Physics.OverlapSphereNonAlloc( moveTransform.position, Global.Instance.GlobalSightDistance, 
        SensorColliders, LayerMask.GetMask( Global.Instance.gameData.TopPrioritySensorLayers ), QueryTriggerInteraction.Ignore );
      for( int i = 0; i < count; i++ )
      {
        if( ActiveInterests.Count >= MaxActiveInterests )
          break;
        if( SensorColliders[ i ].gameObject != gameObject )
          OnProximity( SensorColliders[ i ].gameObject );
      }
    }

    {
      int count = Physics.OverlapSphereNonAlloc( moveTransform.position, Global.Instance.GlobalSightDistance, 
        SensorColliders, LayerMask.GetMask( Global.Instance.gameData.LowPrioritySensorLayers ), QueryTriggerInteraction.Ignore );
      for( int i = 0; i < count; i++ )
      {
        if( ActiveInterests.Count >= MaxActiveInterests )
          break;
        if( SensorColliders[ i ].gameObject != gameObject )
          OnProximity( SensorColliders[ i ].gameObject );
      }
    }
  }

  void OnProximity( GameObject go )
  {
    Destination des = go.GetComponent<Destination>();
    if( des != null )
    {
      if( !KnownDestinations.Contains( des ) )
        KnownDestinations.Add( des );
      /*if( Home == null && !des.IsOwned() && tags.HasAnyTag( des.HomeForTags ) )
      {
        des.AssignOwner( this );
        Home = des;
      }*/
    }

    if( Bed == null )
    {
      Bed bed = go.GetComponent<Bed>();
      if( bed != null )
      {
        if( !bed.IsOwned() && tags.HasAnyTag( bed.BedForTags ) )
        {
          bed.AssignOwner( this );
          Bed = bed;
          bed.destination.AssignOwner( this );
          Home = bed.destination;
        }
      }
    }

    // all objects of interest are assumed to have a tags component
    Tags objectTags = go.GetComponent<Tags>();
    if( objectTags == null )
      return;

    Character cha = go.GetComponent<Character>();

    foreach( var i in Interests )
    {
      if( i.type == Interest.Type.Instance )
      {
        if( i.InstanceID != 0 && cha != null && cha.id == i.InstanceID )
        {
          AddActiveInterest( i, go );
        }
      }
      else
      if( i.type == Interest.Type.Tag )
      {
        if( objectTags.HasTag( i.tag ) )
        {
          // TODO optimize
          if( !ActiveInterests.Exists( x => x.verb == i.verb && x.go == go && x.tag == i.tag ) )
          {
            AddActiveInterest( i, go );
          }
        }
      }
      /*else
      if( i.type == Interest.Type.Type )
      {
            if( go.GetComponent( i.OtherType ) )
        {
          AddActiveInterest( i, go );
        }
      }*/
    }
  }

  void AddActiveInterest( Interest i, GameObject go )
  {
    Interest interest = interestPool.GetFreeInterest();
    if( interest != null )
    {
      i.go = go;
      interest.AssignFrom( i );
      ActiveInterests.Add( interest );
    }
  }

  void ProcessInterests()
  {
    // could possibly sort interests here, based on priority
    List<Interest> remove = new List<Interest>();
    foreach( var i in ActiveInterests )
      if( i.go == null )
        remove.Add( i );
    foreach( var i in remove )
      ActiveInterests.Remove( i );

    if( ActiveInterests.Count > 0 )
    {
      // process a single interest per update
      Interest interest = ActiveInterests[ 0 ];
      ActiveInterests.RemoveAt( 0 );
      interestPool.RecycleInterest( interest );

      // associated state's priority
      CharacterState state = states.Find( x => x.Name == interest.verb );
      if( state == null )
      {
        Debug.LogError( "cannot find state for Interest (" + interest.tag + ", " + interest.verb + ") ", this );
        return;
      }
      // check conditions
      if( interest.condition != null && interest.condition.Length > 0 )
      {
        string[] tokens = interest.condition.Split( ';' );
        foreach( var token in tokens )
          if( !IsConditionSatisfied( token, interest ) )
            return;
      }
      // state priority is default
      int priority = state.Priority;
      // override with priority from interest
      if( interest.OverridePriority )
        priority = interest.Priority;

      // use the source interest priority if applicable
      int currentStatePriority = CurrentState.Priority;
      if( CurrentState.SourceInterest != null && CurrentState.SourceInterest.OverridePriority )
        currentStatePriority = CurrentState.SourceInterest.Priority;

      // only change state if priority is greater.
      if( priority > currentStatePriority || interest.AlwaysConsider )
        state.Consider( interest );
    }
  }


  public void BroadcastEvent( CharacterEvent evt, float radius )
  {
    // is it faster to broadcast to all, or to use OvelapSphere?
    Character cha = null;
    for( int i = 0; i < Character.Limit.All.Count; i++ )
    {
      cha = Character.Limit.All[ i ];
      if( cha != this && !cha.isPlayerControlled && Vector3.SqrMagnitude( cha.moveTransform.position - evt.position ) < radius * radius )
        cha.ReceiveEvent( evt );
    }
    /*
    string[] BroadcastLayers = new string[] { "Character" };
    int BroadcastLayerMask = LayerMask.GetMask(BroadcastLayers);
    Vector3 halfExtents = Vector3.one * World.Instance.GlobalSightDistance;
    int count = Physics.OverlapSphereNonAlloc( evt.A.transform.position, World.Instance.GlobalSightDistance, SensorColliders, BroadcastLayerMask );
    if( count > 0 )
    {
      for( int i = 0; i < count; i++ )
      {
        Character cha = SensorColliders[i].gameObject.GetComponent<Character>();
        if( cha==null )
          continue;
        cha.ReceiveEvent( evt );
      }
    }
    */
  }

  AffinityData GetAffinity( int characterId )
  {
    if( Affinity.ContainsKey( characterId ) )
      return Affinity[ characterId ];
    AffinityData aff = new AffinityData();
    aff.CharacterId = characterId;
    aff.Value = 0;
    Affinity.Add( characterId, aff );
    DebugAffinity.Add( aff );
    return aff;
  }

  void ModifyAffinity( int characterId, int value )
  {
    AffinityData aff;
    int previousValue = 0;
    if( Affinity.ContainsKey( characterId ) )
    {
      aff = Affinity[ characterId ];
      if( aff.CharacterId == characterId )
      {
        previousValue = aff.Value;
        aff.Value += value;
        Affinity[ characterId ] = aff;
      }
      else
      {
        Debug.LogWarning( "character/instanceid mismatch" );
        return;
      }
    }
    else
    {
      aff = new AffinityData();
      aff.CharacterId = characterId;
      aff.Value = value;
      Affinity.Add( characterId, aff );
      DebugAffinity.Add( aff );
    }

    if( aff.Value > previousValue )
    {
      PlayFaceAnimation( "happy" );
    }

    if( previousValue > AffPeaceful && aff.Value <= AffPeaceful )
    {
      Interest i = new Interest( "Attack", characterId );
      PersonalInterests.Add( i );
      Interests.Add( i );
    }
    if( previousValue > AffFollow && aff.Value < AffFollow )
    {
      Follow( null, null );
    }
    
  }

  public void ReceiveEvent( CharacterEvent evt, bool inConversation = false )
  {
    int CharacterLayerMask = LayerMask.GetMask( new string[] { "Character" } );

    string myName = gameObject.name;
    if( CharacterName.Length > 0 )
      myName = CharacterName;

    CharacterInfo cia = Global.Instance.CharacterInfoLookup[ evt.CharacterIdA ];
    CharacterInfo cib = Global.Instance.CharacterInfoLookup[ evt.CharacterIdB ];
      
    switch( evt.type )
    {
      case CharacterEventEnum.DebugAddAffinity:
        Debug.Log( "debug add affinity" );
        ModifyAffinity( cia.id, 1 );
        break;

      case CharacterEventEnum.DebugSubAffinity:
        Debug.Log( "debug sub affinity" );
        ModifyAffinity( cia.id, -1 );
        break;

      case CharacterEventEnum.Death:
        if( inConversation )
        {
          ModifyAffinity( cib.id, AffModDamage );
          Debug.Log( myName + " witnessed " + cia.name + " killed by " + cib.name );           
        }
        else
        if( evt.CharacterA != null && CanSeeObject( evt.CharacterA.gameObject, false, CharacterLayerMask ) )
        {
          Debug.Log( myName + " witnessed " + cia.name + " killed by " + cib.name );
          if( evt.CharacterA.Team == Team )
          {
            // Make sure to care about the deceased before lowering affinity for the instigator.
            // Otherwise, there will be a chain reaction of bloody retribution.
            AffinityData aff = GetAffinity( evt.CharacterA.id );
            if( aff.Value > AffPeaceful )
            {
              if( evt.CharacterB != null )
              {
                ModifyAffinity( evt.CharacterB.id, AffModDeath );
                Speak( "negative" );
                KnownEvents.Add( evt );
              }
            }
          }
        }
        break;

      case CharacterEventEnum.Damage:
        if( inConversation )
        {
          //if( evt.CharacterA == null || evt.CharacterB == null )
          ModifyAffinity( cib.id, AffModDamage );
          Debug.Log( myName + " dislikes " + cib.name + " for damaging " + cia.name + " (told in conversation)" );
        }
        else
        if( evt.CharacterA != null && evt.CharacterB != null && ( evt.CharacterA.Team == null || !AttackTags.Contains( evt.CharacterA.Team.Tag ) ) )
        {
          // witnessed damage of a non-enemy
          if( CanSeeObject( evt.CharacterA.gameObject, false, CharacterLayerMask ) )
          {
            if( CanSeeObject( evt.CharacterB.gameObject, true, CharacterLayerMask ) )
            {
              Debug.Log( myName + " dislikes " + cib.name + " for damaging " + cia.name );
              ModifyAffinity( evt.CharacterB.id, AffModDamage );
              Speak( "negative" );
              PlayFaceAnimation( "unhappy" );
              KnownEvents.Add( evt );
            }
          }
        }
        break;

      case CharacterEventEnum.Burial:
        Speak( ":|" );
        break;

      case CharacterEventEnum.Birth:
        {
          // chance to react at all
          if( Random.Range( 0, 3 ) > 0 )
          {
            new Timer( Random.Range( 0.3f, 3.0f ), null, delegate
            {
              Speak( "witnessbirth" );
            } );
          }
          break;
        }

    /*case CharacterEventEnum.FoodAdded:
        Debug.Log( myName + " likes " + instigatorName + " for adding food " + evt.Swag.name );
        ModifyAffinity( evt.CharacterA.id, AffModFoodAdded );
        break;*/

      default:
        break;
    }

  }

  bool IsConditionSatisfied( string key, Interest interest )
  {
    switch( key.ToLower() )
    {
      case "unarmed":
        return IsUnarmed();
      case "age<other":
        return Age < interest.go.GetComponent<Character>().Age;
      default:
        break;
    }
    return false;
  }

  public void CharacterUpdate()
  {
    // This is prevent characters from updating during construction.
    // If characters are no longer constructed directly, this should be removed.
    if( !enabled )
      return;

    if( Dead )
      return;
    /*if( Health <= 0 )
    {
      Die();
      return;
    }*/

    //UpdateTimers();
      
    if( IsBleeding && Time.time - BleedStartTime > BleedDuration )
      IsBleeding = false;

    if( IsTakingDamage && Time.time - TakingDamageStartTime > TakingDamageDuration )
      IsTakingDamage = false;

    if( Time.time - faceAnimTime > FaceResetInterval )
      PlayFaceAnimation( "idle" );
    
    // health regen
    HealthRegenAccumulator += HealthRegenAmount * Time.deltaTime;
    if( HealthRegenAccumulator > 1f )
    {
      HealthRegenAccumulator = 0f;
      Health += 1;
    }
    // health update
    if( Health != cachedHealth )
    {
      Health = Mathf.Clamp( Health, 0, HealthFull );

      if( healthBar != null )
        healthBar.alpha = (float)Health / (float)HealthFull;

      if( isPlayerControlled )
      {
        Color screenColor = Color.red;
        screenColor.a = 1f - (float)Health / (float)HealthFull;
        screenColor.a = screenColor.a * screenColor.a;
        Global.Instance.cameraController.ScreenFadeColor( screenColor );
      }
      cachedHealth = Health;
    }
      
    if( CanAge )
    {
      if( Time.time - LastAgeUpdate > AgeUpdateInterval )
      {
        LastAgeUpdate = Time.time;
        Age = Mathf.Min( Age + AgeUpdateInterval, AgeMaxSeconds );
        UpdateAge();
      }
    }

    if( CurrentState.Update != null )
      CurrentState.Update.Invoke();

    if( !isPlayerControlled )
    {
      ProcessInterests();

      if( HasPath )
      {
        if( waypoint.Count > 0 )
        {
          if( Time.time - PathEventTime > Global.Instance.RepathInterval )
          {
            PathEventTime = Time.time;
            SetPath( DestinationPosition, OnPathEnd );
          }
          // follow path if waypoints exist
          Vector3 waypointFlat = waypointEnu.Current;
          waypointFlat.y = moveTransform.position.y;
          if( Vector3.SqrMagnitude( moveTransform.position - waypointFlat ) > ( Time.timeScale * WaypointRadii ) * ( WaypointRadii * Time.timeScale ) )
          {
            MoveDirection = waypointEnu.Current - moveTransform.position;
            MoveDirection.y = 0;
          }
          else
          if( waypointEnu.MoveNext() && Vector3.SqrMagnitude( moveTransform.position - DestinationPosition ) > DestinationRadius * DestinationRadius )
          {
            MoveDirection = waypointEnu.Current - moveTransform.position;
            MoveDirection.y = 0;
          }
          else
          {
            // destination reached
            // clear the waypoints before calling the callback because it may set another path and you do not want them to accumulate
            waypoint.Clear();
            debugPath.Clear();
            HasPath = false;
            DestinationPosition = moveTransform.position; //new Vector3( float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity );
            // do this to allow OnPathEnd to become null because the callback may set another path without a callback.
            System.Action temp = OnPathEnd;
            OnPathEnd = null;
            if( temp != null )
              temp.Invoke();
          }
        }

        #if DEBUG_LINES
        // draw path
        if( debugPath.Count > 0 )
        {
          Color pathColor = Color.white;
          if( nvp.status == NavMeshPathStatus.PathInvalid )
            pathColor = Color.red;
          if( nvp.status == NavMeshPathStatus.PathPartial )
            pathColor = Color.gray;
          foreach( var ls in debugPath )
          {
            Debug.DrawLine( ls.a, ls.b, pathColor );
          }
        }
        #endif
      }

      // keep the character facing in the direction of movement
      if( FaceDirectionOfMovement )
        FaceDirection = MoveDirection;
      if( FaceDirection.sqrMagnitude > 0 )
        moveTransform.rotation = Quaternion.RotateTowards( moveTransform.rotation, Quaternion.LookRotation( FaceDirection, Vector3.up ), Time.deltaTime * TurnSpeed );
      

      if( Global.Instance.GlobalSidestepping && SidestepAvoidance )
      {
        if( Time.time - SidestepLast > Global.Instance.SidestepInterval )
        {
          Sidestep = Vector3.zero;
          SidestepLast = Time.time;
          if( MoveDirection.magnitude > 0.001f )
          {
            float distanceToWaypoint = Vector3.Distance( waypointEnu.Current, moveTransform.position );
            if( distanceToWaypoint > Global.Instance.SidestepIgnoreWithinDistanceToGoal )
            {
              float raycastDistance = Mathf.Min( distanceToWaypoint, Global.Instance.SidestepRaycastDistance );
              int count = Physics.SphereCastNonAlloc( moveTransform.position, ball.Sphere.radius, MoveDirection.normalized, RaycastHits, raycastDistance );
              if( count > 0 )
              {
                for( int i = 0; i < count; i++ )
                {
                  RaycastHit hit = RaycastHits[ i ];
                  Character other = hit.transform.root.GetComponent<Character>();
                  if( other != null && other != this )
                  {
                    Vector3 delta = other.moveTransform.position - moveTransform.position;
                    Sidestep = ( ( moveTransform.position + Vector3.Project( delta, MoveDirection.normalized ) ) - other.moveTransform.position ).normalized * Global.Instance.SidestepDistance;
                    break;
                  }
                }
              }
            }
          }
        }
        MoveDirection += Sidestep;
      }
    }

    #if DEBUG_LINES
    Debug.DrawLine( moveTransform.position, moveTransform.position + MoveDirection.normalized * 0.5f, Color.magenta );
    Debug.DrawLine( moveTransform.position, moveTransform.position + FaceDirection.normalized, Color.red );
    #endif

    if( IsBleeding && Global.Instance.Bloodtrails )
    {
      if( Vector3.SqrMagnitude( moveTransform.position - lastPositionForFootprint ) > 0.25f * 0.25f )
      {
        Global.Instance.GetZone( gameObject ).PaintSingleGroundPixel( moveTransform.position, new Color( 1, 0, 0, 0.7f ), Global.Instance.footprintType );
        lastPositionForFootprint = moveTransform.position;
      }
    }
    else
    if( Footprints && Global.Instance.Footprints )
    {
      if( Vector3.SqrMagnitude( moveTransform.position - lastPositionForFootprint ) > 0.25f * 0.25f )
      {
        Color groundColor = Global.Instance.footprintColor;
        if( IsBleeding )
          groundColor = new Color( 1, 0, 0, 0.3f );

        Global.Instance.GetZone( gameObject ).PaintSingleGroundPixel( moveTransform.position, groundColor, Global.Instance.footprintType );
        lastPositionForFootprint = moveTransform.position;
      }
    }

    // optional
    if( sad != null )
      sad.SetDirection( Util.NormalizeAngle( moveTransform.rotation.eulerAngles.y - Camera.main.transform.rotation.eulerAngles.y ) );

    // walk bounce
    if( walk != null )
    {
      float mag = MoveDirection.magnitude * CurrentMoveSpeed; // body.velocity.magnitude
      if( mag < moveThreshold )
      {
        if( lastMoveMag > moveThreshold )
        {
          walk.Off();
          // optional
          if( sad != null )
            sad.Play( "idle" );
        }
      }
      else
      {
        if( lastMoveMag < moveThreshold )
        {
          walk.On();
          // optional
          if( sad != null )
            sad.Play( "walk" );
        }
      }
      lastMoveMag = mag;
    }

    if( Mouth != null )
    {
      if( audioSource.isPlaying )
      {
        Mouth.alpha = Mathf.Abs( Mathf.Sin( Time.time * 8 ) );
      }
    }

    if( EndAttackCooldown )
    {
      if( Time.time - EndAttackCooldownStart > EndAttackCooldownDuration )
      {
        EndAttackCooldown = false;
        if( Time.time - lastKillTime < 20f )
        {
          Speak( "yay" );
        }
      }
    }

    DebugVelocity = body.velocity.magnitude;

    float StaminaRate = 0;
    if( CurrentMoveSpeed > WalkSpeed && body.velocity.magnitude > 0 )
      StaminaRate = StaminaLossRate;
    else
      StaminaRate = StaminaRecoverRate;
    Stamina = Mathf.Clamp( Stamina + StaminaRate * Time.deltaTime, 0, StaminaFull );
    if( StaminaBar != null )
      StaminaBar.alpha = Stamina / StaminaFull;
    // stamina should affect speed if faster than a walk
    if( CurrentMoveSpeed > WalkSpeed )
      CurrentMoveSpeed = WalkSpeed + Mathf.Ceil( Stamina / StaminaFull ) * ( SprintSpeed - WalkSpeed );

    if( HeldObject != null )
    {
      HeldObject.rotation = moveTransform.rotation * HeldObjectInitialRotationOffset;
    }

    if( !InSleepCycle )
    {
      Wake = Mathf.Clamp( Wake + WakeLossRate * Time.deltaTime, 0, WakeFull );
      if( Wake <= 0 )
      {
        if( !isPlayerControlled )
        {
          if( CurrentState.Priority < states.Find( x => x.Name == "Sleep" ).Priority )
          {
            if( Bed != null )
              GoToBed( Bed );
            else
              SleepOnGround();
          }
        }
      }
    }

    if( isPlayerControlled )
      Global.Instance.ShowCharacterInfo( this );
  }

  public void CharacterFixedUpdate()
  {
    if( body == null )
      return;
    MoveDirection.y = 0f;
    MoveDirection.Normalize();
    Vector3 ActualMoveDirection = MoveDirection * CurrentMoveSpeed;
    float dot = Vector3.Dot( moveTransform.forward, ActualMoveDirection );
    // moving backward is slower
    if( dot < -0.7f )
      ActualMoveDirection *= 0.5f;
    body.AddForce( ActualMoveDirection * body.mass );
  }

  void OnCollisionEnter( Collision other )
  {
    lastHitOnMe = other.contacts[ 0 ].normal;
    if( body != null )
      body.AddForce( other.contacts[ 0 ].normal * Global.Instance.WallBounce * body.mass, ForceMode.Impulse );

    if( CanPickupItems && other.gameObject.GetComponent<CarryObject>() != null )
      AcquireObject( other.gameObject );

    /*if( UnarmedAttack )
    {
      // unarmed attacks are humping
      Tags tags = other.gameObject.GetComponent<Tags>();
      if( tags != null && tags.HasAnyTag( AttackTags.ToArray() ) )
      {
        IDamage dam = other.gameObject.GetComponent<IDamage>();
        if( dam != null )
          dam.TakeDamage( new DamageInfo( moveTransform, DamageType.Generic, 1 ) );
      }
    }*/
  }

  public void AddState( CharacterState newState )
  {
    CharacterState existing = states.Find( x => x.Name == newState.Name );
    if( existing != null )
    {
      Debug.LogWarning( "replacing existing state: " + newState.Name );
      states.Remove( existing );
    }
    states.Add( newState );
  }

  public void PopState()
  {
    if( CurrentState.Pop != null )
      CurrentState.Pop.Invoke();
    Stack.RemoveAt( Stack.Count - 1 );
    if( Stack.Count > 0 )
    {
      CurrentState = Stack[ Stack.Count - 1 ];
      CurrentStateName = CurrentState.Name;
      if( CurrentState.Resume != null )
        CurrentState.Resume.Invoke();
    }
    else
    {
      Debug.LogWarning( "Popped the init state!", this );
      PushState( InitialState );
    }
  }

  public void PushState( string stateName, Interest sourceInterest = null )
  {
    CharacterState state = states.Find( x => x.Name == stateName );
    if( state == null )
    {
      Debug.LogWarning( "state not found: " + stateName );
      return;
    }
    if( CurrentState.Suspend != null )
      CurrentState.Suspend.Invoke();
    Stack.Add( state );
    CurrentState = state;
    CurrentState.SourceInterest = sourceInterest;
    CurrentStateName = CurrentState.Name;
    if( CurrentState.Push != null )
      CurrentState.Push.Invoke();
  }




  // carry, hold, drop
  public void DropObject()
  {
    if( HeldObject == null )
      return;

    CarryObject swag = HeldObject.GetComponent<CarryObject>();
    Rigidbody rb = HeldObject.GetComponent<Rigidbody>();

    if( swag != null )
    {
      //HeldObject.transform.rotation = Quaternion.LookRotation( swag.GroundForward, swag.GroundUp );
      swag.HeldByCharacter = null;
      swag.IsHeld = false;
      // continuous lerp is used to hold the object.
      swag.lerp.enabled = false;
      if( swag.CreateRigidbodyWhenCarried )
      {
        Destroy( rb );
        rb = null;
      }

      if( rb == null )
      {
        Vector3 placeObjectPosition = HeldObject.transform.position;
        RaycastHit hit = new RaycastHit();
        if( Physics.Raycast( placeObjectPosition, Vector3.down, out hit, 1f ) )
          placeObjectPosition.y = hit.point.y + Global.Instance.GlobalSpriteOnGroundY;
        else
          placeObjectPosition.y = Global.Instance.GlobalSpriteOnGroundY;
        HeldObject.transform.position = placeObjectPosition;
      }
      else
      {
        if( swag.PreserveConstraintsWhenDropped )
        {
          rb.constraints = swag.cachedConstraints;
        }
        rb.useGravity = true;
      }
    }

    Collider coll = HeldObject.GetComponent<Collider>();
    Physics.IgnoreCollision( coll, ball.Sphere, false );
    //HeldObject.transform.position = placeObjectPosition;
    HeldObject.transform.parent = null;
    HeldObject = null;
  }

  bool AllowedToHoldObject( Transform obj )
  {
    if( obj == moveTransform )
      return false;
    CarryObject swag = obj.GetComponent<CarryObject>();
    if( swag == null || swag.IsHeld )
      return false;
    else
    if( obj.parent != null && obj.parent.GetComponent<Character>() != null )
      return false;
    return true;
  }

  bool WithinHoldRange( Transform obj )
  {
    Vector3 delta = obj.position - moveTransform.position;
    delta.y = 0;
    if( delta.magnitude < ActionRadius )
      return true;
    return false;
  }

  // carry, hold, drop
  public void HoldObject( Transform obj )
  {
    if( !AllowedToHoldObject( obj ) )
      return;
    if( !WithinHoldRange( obj ) )
      return;
    if( HeldObject != null )
      DropObject();

    CarryObject carry = obj.GetComponent<CarryObject>();
    carry.HeldByCharacter = this;

    HeldObject = obj;
    HeldObjectInitialRotationOffset = Quaternion.Inverse( moveTransform.rotation ) * HeldObject.rotation;
    //HeldObject.rotation = moveTransform.rotation * Quaternion.LookRotation( carry.HeldForward, carry.HeldUp );
    HeldObject.transform.position = moveTransform.position + moveTransform.forward * HoldRadius;

    Rigidbody rb = HeldObject.GetComponent<Rigidbody>();
    if( carry.CreateRigidbodyWhenCarried )
    {
      if( rb == null )
        rb = HeldObject.gameObject.AddComponent<Rigidbody>();
    }
    if( rb != null )
    {
      if( carry.PreserveConstraintsWhenDropped )
        carry.cachedConstraints = rb.constraints;
      rb.constraints = RigidbodyConstraints.None;
      rb.position = HeldObject.transform.position;
      //rb.rotation = HeldObject.rotation;
      rb.useGravity = false;
    }

    carry.IsHeld = true;

    if( carry.lerp == null )
      carry.lerp = carry.gameObject.AddComponent<LerpToTarget>();
    LerpToTarget lerp = carry.lerp;
    lerp.targetTransform = moveTransform;
    lerp.Continuous = true;
    lerp.Rigidbody = rb != null;
    lerp.force = 20;
    lerp.UseMaxDistance = true;
    lerp.MaxDistance = 1f;
    lerp.duration = 0.1f;
    lerp.Scale = false;
    lerp.lerpType = LerpToTarget.LerpType.Linear;
    lerp.localOffset = Vector3.forward * HoldRadius;
    lerp.enabled = true;

    Collider coll = HeldObject.GetComponent<Collider>();
    Physics.IgnoreCollision( coll, ball.Sphere );
  }



  bool CanSeeObject( GameObject go, bool flat = true, int layerMask = Physics.AllLayers )
  {
    Vector3 target = go.transform.position;
    // consider only the first collider found
    Collider col = go.GetComponent<Collider>();
    if( col != null )
      target = col.bounds.center;

    // all visibility raycasts should be done on the same plane
    Vector3 start = moveTransform.position;
    if( flat )
      start.y = Global.Instance.GlobalRaycastY;
    Vector3 delta = target - start;
    if( flat )
      delta.y = 0;
    RaycastHit hitInfo = new RaycastHit();
    if( Physics.Raycast( start, delta, out hitInfo, Global.Instance.GlobalSightDistance, layerMask ) )
    {
      if( hitInfo.transform == go.transform )
      {
        Character c = go.GetComponent<Character>();
        if( c != null && c.IsHidden && c.GetComponent<Rigidbody>().velocity.magnitude < Global.Instance.CamoflaugeMoveThreshold )
        {
          return false;
        }
        return true;
      }
    }
    return false;
  }


  public void CleanupPlayerState()
  {
    // This is meant to clean up Player and Construction states
    while( Stack.Count > 1 && Stack[ Stack.Count - 1 ].IsPlayerState )
      PopState();
  }

  public void PopAllStates()
  {
    // This is meant to clean up Player and Construction states
    while( Stack.Count > 1 )
      PopState();
  }

  public void Die()
  {
    if( Dead )
      return;
    Dead = true;
    if( isPlayerControlled )
    {
      //CleanupPlayerState();
      Global.Instance.RespawnAvatar();
      Global.Instance.cameraController.LookTarget = null;
      Global.Instance.cameraController.FreePosition = moveTransform.position + Vector3.up * 10;
      Global.Instance.cameraController.lookInput = new Vector2( 0, 90 );
    }

    Vector3 bodyPos = moveTransform.position;
    bodyPos.y = Global.Instance.GlobalSpriteOnGroundY;

    if( DeathPrefab != null )
      GameObject.Instantiate( DeathPrefab, moveTransform.position, Quaternion.LookRotation( lastHitOnMe, Vector3.up ) );
    if( DeathSound != null )
      Global.Instance.AudioOneShot( DeathSound, moveTransform.position );

    if( DeadBodyPrefab != null )
    {
      GameObject go = Global.Instance.Spawn( DeadBodyPrefab, bodyPos, Quaternion.LookRotation( Vector3.down, -MoveDirection ), null, true, true );
      if( go != null )
      {
        go.transform.localScale = myRenderer.transform.localScale;
        Deadbody db = go.GetComponent<Deadbody>();
        db.CharacterName = CharacterName;
        db.Kills = KillCount;
        #if BILLBOARD
        // temporary. remove when it's decided that characters will use sprites or not
        if( billboard != null )
          db.AssignSprite( billboard.sprite, myRenderer.transform.localScale.x );
        #endif
        if( Team != null )
        {
          db.TeamName = Team.name;
          go.GetComponent<MeshRenderer>().material.SetColor( "_IndexColor", Team.Color );
        }
        Tags deadTags = go.GetComponent<Tags>();
        //deadTags.tags.AddRange( tags.tags );
        if( tags.HasTag( Tag.Monster ) )
          deadTags.tags.Add( Tag.Food );
      }
    }

    if( Home != null )
      Home.ClearOwner();
    DropAllItems();
    if( HeldObject != null )
      DropObject();
    
//    if( LatestDamageInstigator != null )
//      LatestDamageInstigator.RewardForKill( this );
//    CharacterEvent evt = new CharacterEvent( CharacterEventEnum.Death, moveTransform.position, Time.time, this, LatestDamageInstigator );
//    BroadcastEvent( evt, Global.Instance.GlobalSightDistance );

    Global.Instance.CharacterInfoLookup[ id ].alive = false;

    GameObject.Destroy( gameObject );
  }

  public void RewardForKill( Character victim )
  {
    Debug.Log( CharacterName + " rewarded for killing " + victim.CharacterName );
    KillCount++;
    killBar.alpha = Mathf.Sqrt( (float)KillCount ) / 100f;
    lastKillTime = Time.time;
  }

  public void Speak( string tag, int priority = 0 )
  {
    // priority 0 = offhand remarks
    // priority 1 = unsolicted chat
    // priority 2 = player-engaged speech
    // priority 3 = mandatory message
    //string colorString = "#ffffff";
    /*Color color = Color.white;
    ColorUtility.TryParseHtmlString( colorString, out color );*/
    if( identity == null )
      return;
    Speech s = identity.speeches.Find( x => x.tag == tag );
    if( s == null )
    {
      Global.Instance.Speak( this, tag, 5, priority );
      JabberPlayer.Play( tag );
    }
    else
    {
      Global.Instance.Speak( this, s.text, 5/*s.duration*/, priority );
      //JabberPlayer.Play( s.text );
      if( audioSource != null && audioSource.enabled )
      {
        if( audioSource.isPlaying )
          audioSource.Stop();
        if( s.clips.Length > 0 )
          audioSource.PlayOneShot( s.clips[ Random.Range( 0, s.clips.Length ) ] );
      }
    }
  }

  void ClearPath()
  {
    HasPath = false;
    waypoint.Clear();
    debugPath.Clear();
    OnPathEnd = null;
    DestinationPosition = moveTransform.position;
  }

  public bool SetPath( Vector3 TargetPosition, System.Action onArrival = null )
  {
    OnPathEnd = onArrival;

    Vector3 EndPosition = TargetPosition;
    NavMeshHit hit;
    if( NavMesh.SamplePosition( TargetPosition, out hit, 5.0f, NavMesh.AllAreas ) )
      EndPosition = hit.position;
    DestinationPosition = EndPosition;

    Vector3 StartPosition = moveTransform.position;
    if( NavMesh.SamplePosition( StartPosition, out hit, 5.0f, NavMesh.AllAreas ) )
      StartPosition = hit.position;

    nvp.ClearCorners();
    if( NavMesh.CalculatePath( StartPosition, EndPosition, NavMesh.AllAreas, nvp ) )
    {
      if( nvp.status == NavMeshPathStatus.PathComplete || nvp.status == NavMeshPathStatus.PathPartial )
      {
        if( nvp.corners.Length > 0 )
        {
          Vector3 prev = StartPosition;
          debugPath.Clear();
          foreach( var p in nvp.corners )
          {
            LineSegment seg = new LineSegment();
            seg.a = prev;
            seg.b = p;
            debugPath.Add( seg );
            prev = p;
          }
          waypoint = new List<Vector3>( nvp.corners );
          waypointEnu = waypoint.GetEnumerator();
          waypointEnu.MoveNext();
          PathEventTime = Time.time;
          HasPath = true;
          return true;
        }
        else
        {
          Debug.Log( "corners is zero path to: " + TargetPosition );
        }
      }
      else
      {
        Debug.Log( "invalid path to: " + TargetPosition );
      }
    }
    return false;
  }



  void PlayFaceAnimation( string anim )
  {
    if( FaceAnim != null )
      FaceAnim.Play( anim );
    faceAnimTime = Time.time;
  }

  public void TakeDamage( Damage info )
  {
    if( Dead )
      return;
    if( InSleepCycle )
      SleepInterrupt = true;
    if( !DamageIgnore.Contains( info.type ) )
    {
      if( !IsTakingDamage )
      {
        Health -= info.amount;
        if( Health <= 0 )
        {
          if( info.instigator != null )
          {
            Character Instigator = info.instigator.GetComponent<Character>();
            if( Instigator != null )
            {
              Instigator.RewardForKill( this );
              CharacterEvent evt = new CharacterEvent( CharacterEventEnum.Death, moveTransform.position, Time.time, this, Instigator );
              BroadcastEvent( evt, Global.Instance.GlobalSightDistance );
            }
          }
          Die();
        }
        else
        if( Health < cachedHealth )
        {
          colorPulse.enabled = true;
          colorPulse.duration = TakingDamageDuration;
          colorPulse.flashDuration = TakingDamageToggleDuration;
          TakingDamageStartTime = Time.time;
          IsTakingDamage = true;
          IsBleeding = true;
          BleedStartTime = Time.time;

          if( TakeDamageSound != null )
          {
            if( audioSource.enabled )
              audioSource.PlayOneShot( TakeDamageSound );
          }
            
          if( info.instigator != null )
          {
            Character Instigator = info.instigator.GetComponent<Character>();
            if( Instigator != null )
            {
//              LatestDamageInstigator = Instigator;
              PlayFaceAnimation( "unhappy" );
              
              // do not modify affinity for teammates during attack. Accidents happen.
              if( Team == null || Instigator.Team != Team || ( Instigator.Team == Team && CurrentState.Name != "Attack" ) )
              {
                if( CanSeeObject( Instigator.gameObject, true ) )
                  ModifyAffinity( Instigator.id, AffModDamage );
                Speak( "negative" );
                
                CharacterEvent evt = new CharacterEvent( CharacterEventEnum.Damage, moveTransform.position, Time.time, this, Instigator );
                BroadcastEvent( evt, Global.Instance.GlobalSightDistance );
              }

              if( isPlayerControlled )
              {
                Global.Instance.ShowCharacterInfo( this );
              }
              else
              if( Vocation != null && Vocation.name == "Soldier" )
              {
                Interest interest = new Interest( "Investigate", Tag.Attack );
                interest.go = Instigator.gameObject;
                if( CanSeeObject( Instigator.gameObject, true, ~LayerMask.GetMask( new string []{ "Carry" } ) ) )
                  interest.objectPositionWhenSensed = info.instigator.position;
                else
                  interest.objectPositionWhenSensed = info.instigator.position + ( RandomFlatVector() * 4 );
                ActiveInterests.Add( interest );
                // TODO AddActiveInterest()
              }
              else
              {
                if( IsUnarmed() && !UnarmedAttack )
                {
                  FleeFrom = info.instigator;
                  PushState( "Flee" );
                }
              }
            
            }
          }

        }
      }
    }
  }

  Vector3 RandomFlatVector()
  {
    Vector3 v = Random.insideUnitCircle;
    //v.Normalize();
    v.z = v.y;
    v.y = 0;
    return v;
  }


  void UpdateAge()
  {
    float normalAge = Mathf.Clamp( Age / AgeMaxSeconds, 0, 1 );
    ball.Sphere.radius = BallSizeNormal.Evaluate( normalAge );
    walk.Offset = Vector3.up * BallSizeNormal.Evaluate( normalAge ) * WalkerOffsetMax;
    myRenderer.transform.localScale = Vector3.one * SpriteScale.Evaluate( normalAge );
    audioSource.pitch = AudioPitchNormal.Evaluate( normalAge );

    float relativeHealth = (float)Health / (float)HealthFull;
    HealthFull = HealthFullLow + Mathf.FloorToInt( normalAge * ( HealthFullHigh - HealthFullLow ) );
    // The value of (Health / HealthFull) lowers as HealthFull increases. Preserve the relative value.
    Health = Mathf.CeilToInt( relativeHealth * (float)HealthFull );
    Health = Mathf.Clamp( Health, 0, HealthFull );
    // assign cached value to avoid a value-change behavior
    cachedHealth = Health;

    SprintSpeed = SprintSpeedLow + Mathf.FloorToInt( ( 1f - normalAge ) * ( SprintSpeedHigh - SprintSpeedLow ) );
    StaminaFull = StaminaFullLow + Mathf.FloorToInt( normalAge * ( StaminaFullHigh - StaminaFullLow ) );

    if( meshOriginBottom )
      walk.Offset = Vector3.down * (ball.Sphere.radius - meshOriginBottomFudge);
  }

  public void GetActionContext( ref ActionData actionData )
  {
    Vector3 pos = transform.position;
    pos.y = Global.Instance.GlobalSpriteOnGroundY;
    actionData.indicator.position = pos;

    actionData.actions.Add( "command" );
    actionData.actions.Add( "talk" );
    actionData.actions.Add( "switch to character" );
    actionData.actions.Add( "misc" );
  }


  public void OnAction( Character instigator, string action = "default" )
  {
    if( action == "switch to character" )
    {
      Global.Instance.SetPlayerCharacter( this );
    }

    if( action == "misc" )
    {
      List<string> actions = new List<string>();
      actions.Add( "generate name" );
      if( CanMateWith( instigator ) )
        actions.Add( "romance" );
      instigator.ShowContextMenu( moveTransform, actions.ToArray() );
    }

    if( action == "command" )
    {
      List<string> actions = new List<string>();
      actions.Add( "command team join" );
      actions.Add( "change job" );
      if( FollowCharacter != null && FollowCharacter == instigator )
        actions.Add( "command unfollow" );
      else
        actions.Add( "command follow" );
      instigator.ShowContextMenu( moveTransform, actions.ToArray() );
    }

    if( action == "romance" )
    {
      instigator.HideContextMenu();
      if( identity.female )
        OnMate( instigator );
      else
        instigator.OnMate( this );
    }

    if( action == "command follow" )
    {
      instigator.HideContextMenu();
      // follow player when idle
      if( ShouldFollow( instigator ) )
      {
        Follow( instigator, null );
        DropObject();
      }
    }

    if( action == "command unfollow" )
    {
      instigator.HideContextMenu();
      FollowCharacter = null;
      instigator.Followers.Remove( this );
      Speak( "bye" );
      PushState( "Wander" );
    }

    if( action == "talk" )
    {
      instigator.HideContextMenu();

      TalkShareEvent( instigator );

    }// > AffNoTalk

    if( action == "command team join" )
    {
      instigator.HideContextMenu();
      SetTeam( instigator.Team );
      Configure();
    }

    if( action == "change job" )
      instigator.ShowContextMenu( moveTransform, new string[] {
        "job soldier",
        "job civilian"
      } );

    // assign vocation
    if( action.StartsWith( "job " ) )
    {
      Speak( "ok" );
      AssignVocation( Global.Instance.gameData.Vocation.Find( x => x.name.ToLower() == action.Substring( 4 ).ToLower() ) );
      Configure();
      instigator.HideContextMenu();
    }

    if( action == "generate name" )
    {
      CharacterName = Global.Instance.GenerateRandomName();
      Global.Instance.CharacterInfoLookup[ id ].name = CharacterName;
      Global.Instance.ShowCharacterInfo( this );
      Color gold = new Color( (float)0xF8 / 255f, (float)0xCF / 255f, (float)0x62 / 255f );
      instigator.ShowPositionalText( moveTransform.position, CharacterName, ball.Sphere.radius, gold );
    }

    // instigator == this
    if( action == "follow all" )
    {
      instigator.HideContextMenu();
      FollowAll();
    }
    
    if( action == "unfollow all" )
    {
      instigator.HideContextMenu();
      UnfollowAll();
    }
    
    if( action == "talk all" )
    {
      Character[] chas = GetCharactersWithinRadius( 8 );
      foreach( var cha in chas )
        if( cha.identity != null && cha.identity.speeches.Count > 0 )
          cha.Speak( cha.identity.speeches[ Random.Range( 0, cha.identity.speeches.Count - 1 ) ].tag );
    }
        
    if( action == "mode build" )
    {
      instigator.HideContextMenu();
      PushState( "Construction" );
    }
    if( action == "build quit" )
    {
      instigator.HideContextMenu();
      PopState();
    }
    if( action == "build build" )
    {
      instigator.HideContextMenu();
      ChangeMode( Mode.Build );
    }
    if( action == "build modify" )
    {
      instigator.HideContextMenu();
      ChangeMode( Mode.Modify );
    }
    if( action == "build block" )
    {
      instigator.HideContextMenu();
      ChangeMode( Mode.Block );
    }

    if( action == "sleep" )
    {
      instigator.HideContextMenu();
      SleepOnGround();
    }
  }


  void CleanKnownDestinations()
  {
    // remove null destinations
    List<Destination> remove = new List<Destination>();
    foreach( var i in KnownDestinations )
      if( i == null )
        remove.Add( i );
    foreach( var i in remove )
      KnownDestinations.Remove( i );
  }


  #region Timers
/*
  List<Timer> ActiveTimers = new List<Timer>();
  List<Timer> RemoveTimers = new List<Timer>();

  void UpdateTimers()
  {
    foreach( var timer in ActiveTimers )
    {
      if( timer.IsActive )
        timer.Update();
      else
        RemoveTimers.Add( timer );

    }
    foreach( var timer in RemoveTimers )
      ActiveTimers.Remove( timer );
    RemoveTimers.Clear();
  }

*/
  #endregion

  [Header( "Mate" )]
  [Serialize] public bool IsPregnant;
  [Serialize] public float GestationProgress;
  [Serialize] public Character MateInstigator;
  public Timer PregnantTimer;
  public GameObject RomanceEffect;
  const float GestationDuration = 30;
  public float SpawnRadius = 0.5f;


  string[] AgeLabels = new string[] {
    "Infant",
    "Toddler",
    "Child",
    "Adolescent",
    "Youth",
    "Adult"
  };
  float[] AgeLabelPercentages = new float[]{ 0, 5, 10, 15, 30, 50 };

  public string GetAgeLabel()
  {
    string age = "";
    for( int i = 0; i < AgeLabelPercentages.Length; i++ )
    {
      if( ( Age / AgeMaxSeconds ) * 100f < AgeLabelPercentages[ i ] )
        break;
      else
        age = AgeLabels[ i ];
    }
    return age;
  }

  public bool IsMatingAge()
  {
    // must be adult to mate
    return ( Age / AgeMaxSeconds ) * 100f >= AgeLabelPercentages[ 5 ];
  }

  public bool CanMateWith( Character other )
  {
    return !IsPregnant && IsMatingAge() && other.IsMatingAge() && other.identity != null && identity != null && other.identity.female != identity.female;
  }

  public void OnMate( Character instigator )
  {
    if( !identity.female || IsPregnant )
      return;

    IsPregnant = true;
    MateInstigator = instigator;
    PregnantTimer = new Timer( GestationDuration, GestateUpdate, SpawnOffspring );

    if( RomanceEffect != null )
      GameObject.Instantiate( RomanceEffect, moveTransform.position, Quaternion.LookRotation( Vector3.up ) );
  
  }

  void GestateUpdate( Timer obj )
  {
    if( isPlayerControlled )
      Global.Instance.ShowCharacterInfo( this );
    GestationProgress = obj.ProgressSeconds;
  }

  void SpawnOffspring()
  {
    // TODO: number of offspring is affected by amount of food eaten during pregnancy?
    IsPregnant = false;
    PregnantTimer = null;

    // update status
    Global.Instance.ShowCharacterInfo( this );

    Vector3 pos = transform.position + RandomFlatVector() * SpawnRadius;
    GameObject go = Global.Instance.Spawn( Global.Instance.gameData.CharacterPrefab, pos, Quaternion.identity, null, true, true );
    if( Team != null )
      Global.Instance.AssignTeam( go, Team );

    Character baby = go.GetComponent<Character>();
    ModifyAffinity( baby.id, AffLoyal );
    baby.Follow( this, null );
    baby.ModifyAffinity( this.id, AffLoyal );
    // father might be dead before birth of child
    if( MateInstigator != null )
    {
      MateInstigator.ModifyAffinity( baby.id, AffLoyal );
      baby.ModifyAffinity( MateInstigator.id, AffLoyal );
    }

    Speak( "givebirth" );
    if( DeathPrefab != null )
      GameObject.Instantiate( DeathPrefab, moveTransform.position, Quaternion.LookRotation( Vector3.up ) );

    /*CharacterEvent evt = new CharacterEvent();
    evt.type = CharacterEventEnum.Birth;
    evt.position = moveTransform.position;
    evt.radius = World.Instance.GlobalSightDistance;
    evt.A = cha;
    evt.B = this;
    BroadcastEvent( evt );*/
  }

  string Cap( string str )
  {
    return str.Substring( 0, 1 ).ToUpper() + str.Substring( 1 );
  }


}
