using System;
using UnityEngine;

// [CreateAssetMenu]
public class GraphookAbility : Ability
{
  // properties
  public float grapDistance = 10;
  public float grapSpeed = 5;
  public float grapTimeout = 5;
  public float grapPullSpeed = 10;
  public float grapStopDistance = 0.1f;

  // asset references
  public AudioClip grapShotSound;
  public AudioClip grapHitSound;
  public GameObject graphookTipPrefab;
  public GameObject grapCablePrefab;

  // transient, state
  [SerializeField] GameObject graphookTip;
  [SerializeField] SpriteRenderer grapCableRenderer;
  Timer grapTimer = new Timer();
  Vector2 grapSize;
  Vector3 graphitpos;
  public bool grapShooting;
  public bool grapPulling;

  public override void Equip( Transform parentTransform )
  {
    graphookTip = Instantiate( graphookTipPrefab, parentTransform.position, Quaternion.identity, parentTransform );
    grapCableRenderer = Instantiate( grapCablePrefab, parentTransform.position, Quaternion.identity, parentTransform ).GetComponent<SpriteRenderer>();
    graphookTip.SetActive( true );
    grapCableRenderer.gameObject.SetActive( false );
  }

  public override void Unequip()
  {
    Destroy( graphookTip );
    Destroy( grapCableRenderer.gameObject  );
  }

  private void OnDestroy()
  {
    Unequip();
  }

  public override void Activate( Vector2 origin, Vector2 aim )
  {
    ShootGraphook( origin, aim );
  }

  public override void UpdateAbility( Pawn pawn )
  {
    Vector3 armpos = pawn.GetShotOriginPosition();
    grapCableRenderer.transform.position = armpos;
    grapCableRenderer.transform.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, (graphookTip.transform.position - armpos) ) );
    
    if( grapPulling )
    {

      grapSize = grapCableRenderer.size;
      grapSize.x = Vector3.Distance( graphookTip.transform.position, armpos );
      grapCableRenderer.size = grapSize;

      Vector3 grapDelta = graphitpos - pawn.transform.position;
      if( grapDelta.magnitude < grapStopDistance )
        StopGrap();
      else if( grapDelta.magnitude > 0.01f )
        pawn.velocity = grapDelta.normalized * grapPullSpeed;
    }
    
    // grap force
    if( !grapPulling && pawn.pushTimer.IsActive )
      pawn.velocity.x = pawn.pushVelocity.x;
  }
  
  public override void Deactivate()
  {
    StopGrap();
  }

  public override void PreSceneTransition()
  {
    StopGrap();
    Unequip();
  }

  public override void PostSceneTransition()
  {
  }

  void ShootGraphook( Vector2 origin, Vector2 direction )
  {
    if( grapShooting )
      return;
    if( grapPulling )
      StopGrap();
    Vector3 pos = origin;
    if( !Physics2D.Linecast( origin, pos, Global.ProjectileNoShootLayers ) )
    {
      RaycastHit2D hit = Physics2D.Raycast( pos, direction, grapDistance, LayerMask.GetMask( new string[] {"Default", "triggerAndCollision", "enemy"} ) );
      if( hit )
      {
        //Debug.DrawLine( pos, hit.point, Color.red );
        grapShooting = true;
        graphitpos = hit.point;
        graphookTip.SetActive( true );
        graphookTip.transform.parent = null;
        graphookTip.transform.localScale = Vector3.one;
        graphookTip.transform.position = pos;
        graphookTip.transform.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, (graphitpos - (Vector3)origin ) ) );
        ;
        grapTimer.Start( grapTimeout, delegate
          {
            pos = origin;
            graphookTip.transform.position = Vector3.MoveTowards( graphookTip.transform.position, graphitpos, grapSpeed * Time.deltaTime );
            //grap cable
            grapCableRenderer.gameObject.SetActive( true );
            grapCableRenderer.transform.parent = null;
            grapCableRenderer.transform.localScale = Vector3.one;
            grapCableRenderer.transform.position = pos;
            grapCableRenderer.transform.rotation = Quaternion.LookRotation( Vector3.forward, Vector3.Cross( Vector3.forward, (graphookTip.transform.position - pos) ) );
            grapSize = grapCableRenderer.size;
            grapSize.x = Vector3.Distance( graphookTip.transform.position, pos );
            grapCableRenderer.size = grapSize;

            if( Vector3.Distance( graphookTip.transform.position, graphitpos ) < 0.01f )
            {
              grapShooting = false;
              grapPulling = true;
              grapTimer.Stop( false );
              grapTimer.Start( grapTimeout, null, StopGrap );
              Global.instance.AudioOneShot( grapHitSound, origin );
            }
          },
          StopGrap );
        Global.instance.AudioOneShot( grapShotSound, origin );
      }
    }
  }

  void StopGrap()
  {
    grapShooting = false;
    grapPulling = false;
    graphookTip.SetActive( false );
    grapCableRenderer.gameObject.SetActive( false );
    // avoid grapsize.y == 0 if StopGrap is called before grapSize is assigned
    grapSize.y = grapCableRenderer.size.y;
    grapSize.x = 0;
    grapCableRenderer.size = grapSize;
    grapTimer.Stop( false );
  }
}