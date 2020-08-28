using UnityEngine;

public class DangerBall : Entity
{
  [Header("DangerBall")] float fistSpeed = 10;
  [SerializeField] private AnimationCurve shakeCurve;
  [SerializeField] private Vector3 smashDirection;

  public System.Action<RaycastHit2D> OnHit;

  public void Launch(Vector2 direction, float speed)
  {
    UseGravity = false;
    fistSpeed = speed;
    velocity = direction.normalized * fistSpeed;
  }

  public void Stop()
  {
    velocity = Vector2.zero;
  }

  protected override void Start()
  {
    base.Start();
    UpdateLogic = null;
    UpdateHit = null;
    UpdateCollision = LocalCollision;
    UpdatePosition = BasicPosition;
  }

  void LocalCollision()
  {
    // hitCount = Physics2D.BoxCastNonAlloc( box.transform.position, box.size, 0, smashDirection, RaycastHits, Mathf.Max(0.01f, Time.deltaTime * fistSpeed), Global.DefaultProjectileCollideLayers );
    hitCount = Physics2D.CircleCastNonAlloc(transform.position, circle.radius + 0.002f, smashDirection, RaycastHits, Mathf.Max(0.005f, Time.deltaTime * fistSpeed), Global.DefaultProjectileCollideLayers);
    if( hitCount > 0 )
    {
      for( int i = 0; i < hitCount; i++ )
      {
        hit = RaycastHits[i];
        if( !hit.transform.gameObject.isStatic )
          continue;
        if( IgnoreCollideObjects.Count > 0 && IgnoreCollideObjects.Contains(hit.collider) )
          continue;

        IDamage dam = hit.transform.GetComponent<IDamage>();
        if( dam != null )
        {
          Damage dmg = Instantiate(ContactDamage);
          dmg.instigator = this;
          dmg.damageSource = transform;
          dmg.point = hit.point;
          dam.TakeDamage(dmg);
        }

        velocity = Vector2.zero;
        if( hit.transform.gameObject.isStatic )
        {
          CameraShake shaker = Global.instance.CameraController.GetComponent<CameraShake>();
          shaker.amplitude = 0.05f;
          shaker.duration = 0.4f;
          shaker.rate = 100;
          shaker.intensityCurve = shakeCurve;
          shaker.enabled = true;

          Stop();
          OnHit(hit);
          break;
        }
      }
    }
  }
}