using UnityEngine;
using UnityEngine.U2D;

public class Monster : Entity
{
  [SerializeField] private DangerBall fist;
  [SerializeField] private SpriteShapeController sc;

  [SerializeField] float fistSpeed = 10;
  [SerializeField] private float fistRetractSpeed = 5;
  [SerializeField] float restOffset = 1;
  [SerializeField] private float shakeInterval = 0.3f;
  [SerializeField] private int shakeCount = 3;
  [SerializeField] private float tangentLength = 0.5f;
  [SerializeField] private Vector3 smashDirection;
  [SerializeField] private Timer timer = new Timer();
  [SerializeField] private Timer shakeTimer = new Timer();

  protected RaycastHit2D[] RaycastHits = new RaycastHit2D[4];

  [SerializeField] private bool devSmash;

  protected override void Start()
  {
    base.Start();
    UpdateLogic = LocalUpdate;
    UpdateHit = null;
    UpdateCollision = null;
    UpdatePosition = null;
    Physics2D.IgnoreCollision(circle, fist.circle);
    fist.OnHit = SomethingSolidWasHit;
  }

  protected virtual void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    base.OnDestroy();
    timer.Stop(false);
    shakeTimer.Stop(false);
  }

  void LocalUpdate()
  {
    if( devSmash )
    {
      devSmash = false;
      sc.spline.SetRightTangent(1, smashDirection);

      // straighten chain when firing
      shakeTimer.Start(0.2f, delegate(Timer timer3)
      {
        Vector2 startTangent = sc.spline.GetRightTangent(0);
        sc.spline.SetRightTangent(0, Vector3.Lerp(startTangent, smashDirection * tangentLength, shakeTimer.ProgressNormalized));
      }, null);

      fist.Launch(smashDirection, fistSpeed);
      // timeout
      timer.Start(1, null, delegate
      {
        fist.Stop();
        WaitToRetract();
      });
    }

    sc.spline.SetPosition(1, sc.transform.worldToLocalMatrix.MultiplyPoint(fist.transform.position));
  }

  public void SomethingSolidWasHit(RaycastHit2D hit)
  {
    WaitToRetract();
  }

  public void WaitToRetract()
  {
    timer.Start(1, null, () =>
    {
      // yank a few times
      Vector2 perp = new Vector2(-smashDirection.y, smashDirection.x);
      //sc.spline.SetRightTangent(0, smashDirection * tangentLength);
      bool toggle = true;
      timer.Start(shakeCount * 2, shakeInterval, delegate(Timer timer2)
      {
        if( toggle )
        {
          Vector2 startTangent = sc.spline.GetRightTangent(0);
          shakeTimer.Start(shakeInterval, delegate(Timer timer3) { sc.spline.SetRightTangent(0, Vector3.Lerp(startTangent, perp * tangentLength, shakeTimer.ProgressNormalized)); }, null);
        }
        else
        {
          Vector2 startTangent = sc.spline.GetRightTangent(0);
          shakeTimer.Start(shakeInterval, delegate(Timer timer3) { sc.spline.SetRightTangent(0, Vector3.Lerp(startTangent, smashDirection * tangentLength, shakeTimer.ProgressNormalized)); }, null);
        }
        toggle = !toggle;
      }, () =>
      {
        // retract
        // fist.UseGravity = false;

        // straighten chain out while retracting
        shakeTimer.Start(0.5f, delegate(Timer timer3)
        {
          Vector2 startTangent = sc.spline.GetRightTangent(0);
          sc.spline.SetRightTangent(0, Vector3.Lerp(startTangent, (transform.worldToLocalMatrix.rotation * smashDirection) * tangentLength, shakeTimer.ProgressNormalized));
        }, null);

        // bring the fist back to rest position
        Vector3 restTarget = transform.position + smashDirection.normalized * restOffset;
        timer.Start(3, delegate(Timer timer2)
        {
          fist.transform.position = Vector3.MoveTowards(fist.transform.position, restTarget, fistRetractSpeed * Time.deltaTime);
          if( (restTarget - fist.transform.position).magnitude < 0.1f )
            timer.Stop(true);
        }, delegate { fist.transform.position = restTarget; });
      });
    });
  }
}