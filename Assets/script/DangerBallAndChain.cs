using UnityEngine;
using UnityEngine.U2D;

public class DangerBallAndChain : Entity
{
  [SerializeField] private DangerBall fist;
  [SerializeField] private SpriteShapeController sc;
  [SerializeField] private LineRenderer lineRenderer;

  [SerializeField] float fistSpeed = 10;
  [SerializeField] private float fistRetractSpeed = 5;
  [SerializeField] private float restDelay = 1;
  [SerializeField] float launchDelay = 1;
  [SerializeField] float restOffset = 0.5f;
  [SerializeField] private float shakeInterval = 0.3f;
  [SerializeField] private int shakeCount = 3;
  [SerializeField] private float tangentLength = 0.2f;
  [SerializeField] private Vector3 smashDirection;
  private Timer timer = new Timer();
  private Timer shakeTimer = new Timer();

  // sight, target
  [SerializeField] float sightRange = 6;
  Collider2D[] results = new Collider2D[8];
  int SightMask;
  [SerializeField] Entity Target;
  Timer SightPulseTimer = new Timer();
  public AudioClip soundTargetAcquired;

  protected override void Start()
  {
    base.Start();
    UpdateLogic = LocalUpdate;
    UpdateHit = null;
    UpdateCollision = null;
    UpdatePosition = null;
    Physics2D.IgnoreCollision( circle, fist.box );
    fist.OnHit = WaitToRetract;
    // sight pulse
    SightMask = LayerMask.GetMask( new string[] {"character"} );
    SightPulseTimer.Start( int.MaxValue, 3, ( x ) =>
    {
      // reaffirm target
      Target = null;
      int count = Physics2D.OverlapCircleNonAlloc( transform.position, sightRange, results, SightMask );
      for( int i = 0; i < count; i++ )
      {
        Collider2D cld = results[i];
        //Character character = results[i].transform.root.GetComponentInChildren<Character>();
        Entity character = results[i].GetComponent<Entity>();
        if( character != null && IsEnemyTeam( character.Team ) )
        {
          Target = character;
          break;
        }
      }
    }, null );
  }

  protected override void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    base.OnDestroy();
    timer.Stop( false );
    shakeTimer.Stop( false );
    SightPulseTimer.Stop( false );
  }

  void LocalUpdate()
  {
    if( !timer.IsActive && Target != null )
    {
      Global.instance.AudioOneShot( soundTargetAcquired, transform.position );

      timer.Start( launchDelay, null, delegate
      {
        if( Target == null )
          return;
        smashDirection = Target.transform.position - fist.transform.position;
        //sc.spline.SetRightTangent( 1, smashDirection );

        // straighten chain when firing
        shakeTimer.Start( 0.2f, delegate( Timer timer3 )
        {
          // Vector2 startTangent = sc.spline.GetRightTangent( 0 );
          // sc.spline.SetRightTangent( 0, Vector3.Lerp( startTangent, smashDirection * tangentLength, shakeTimer.ProgressNormalized ) );
        }, null );

        fist.transform.parent = null;
        fist.Launch( smashDirection, fistSpeed );
        // timeout
        timer.Start( 1, null, delegate
        {
          fist.transform.parent = transform;
          fist.Stop();
          fist.UseGravity = true;
          WaitToRetract();
        } );
      } );
    }

    //sc.spline.SetPosition( 1, sc.transform.worldToLocalMatrix.MultiplyPoint( fist.transform.position ) );
    lineRenderer.SetPosition( 1, lineRenderer.transform.worldToLocalMatrix.MultiplyPoint( fist.transform.position ) );
  }

  private bool retracting = false;
  
  public void WaitToRetract()
  {
    if( retracting )
      return;
    retracting = true;
    timer.Start( 1, null, () =>
    {
      // yank a few times
      Vector2 perp = new Vector2( -smashDirection.y, smashDirection.x );
      //sc.spline.SetRightTangent(0, smashDirection * tangentLength);
      bool toggle = true;
      timer.Start( shakeCount * 2, shakeInterval, delegate( Timer timer2 )
      {
        if( toggle )
        {
          // Vector2 startTangent = sc.spline.GetRightTangent( 0 );
          // shakeTimer.Start( shakeInterval, delegate( Timer timer3 ) { sc.spline.SetRightTangent( 0, Vector3.Lerp( startTangent, perp * tangentLength, shakeTimer.ProgressNormalized ) ); }, null );
        }
        else
        {
          // Vector2 startTangent = sc.spline.GetRightTangent( 0 );
          // shakeTimer.Start( shakeInterval, delegate( Timer timer3 ) { sc.spline.SetRightTangent( 0, Vector3.Lerp( startTangent, smashDirection * tangentLength, shakeTimer.ProgressNormalized ) ); }, null );
        }
        toggle = !toggle;
      }, () =>
      {
        // retract
        fist.UseGravity = false;

        // straighten chain out while retracting
        shakeTimer.Start( 0.5f, delegate( Timer timer3 )
        {
          // Vector2 startTangent = sc.spline.GetRightTangent( 0 );
          // sc.spline.SetRightTangent( 0, Vector3.Lerp( startTangent, (transform.worldToLocalMatrix.rotation * smashDirection) * tangentLength, shakeTimer.ProgressNormalized ) );
        }, null );

        // bring the fist back to rest position
        Vector3 restTarget = transform.position + smashDirection.normalized * restOffset;
        timer.Start( 3, delegate( Timer timer2 )
        {
          fist.velocity = (restTarget - fist.transform.position).normalized * fistRetractSpeed;
          // fist.transform.position = Vector3.MoveTowards(fist.transform.position, restTarget, fistRetractSpeed * Time.deltaTime);
          if( (restTarget - fist.transform.position).magnitude < 0.1f )
            timer.Stop( true );
        }, delegate
        {
          fist.transform.position = restTarget;
          fist.transform.parent = transform;
          fist.Stop();
          timer.Start( restDelay, null, null );
          retracting = false;
        } );
      } );
    } );
  }
}