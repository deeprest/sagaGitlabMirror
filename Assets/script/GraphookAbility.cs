﻿using UnityEngine;

// todo support grapping to moving objects

// [CreateAssetMenu]
public class GraphookAbility : Ability
{
  // properties
  public float grapDistance = 10;
  public float grapSpeed = 5;
  public float grapTimeout = 5;
  public float grapPullSpeed = 10;
  public float grapStopDistance = 0.1f;
  public float grapReverseThreshold = 0.5f;
  public float inertiaCarryOver = 0.5f;

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
  Vector2 graphitpos;
  Transform parent;
  bool grapShooting;
  bool grapPulling;
  float grapDeltaMagnitude;
  float grapDeltaMagnitudePrevious;
  Vector2 lastGoodDelta;

  bool IsActive
  {
    get { return grapShooting || grapPulling; }
  }

  public override void Equip( Transform parentTransform )
  {
    parent = parentTransform;
    graphookTip = Instantiate( graphookTipPrefab, parentTransform.position, Quaternion.LookRotation( Vector3.forward, parentTransform.up ), parentTransform );
    grapCableRenderer = Instantiate( grapCablePrefab, parentTransform.position, Quaternion.identity, parentTransform ).GetComponent<SpriteRenderer>();
    graphookTip.SetActive( true );
    grapCableRenderer.gameObject.SetActive( false );
  }

  public override void Unequip()
  {
    StopGrap( true );
    Destroy( graphookTip );
    if( grapCableRenderer != null )
      Destroy( grapCableRenderer.gameObject );
  }

  private void OnDestroy()
  {
    Unequip();
  }

  public override void Activate( Vector2 origin, Vector2 aim )
  {
    if( IsActive )
      StopGrap( true );
    else
      ShootGraphook( origin, aim );
  }
  
  public override void Deactivate()
  {
    base.Deactivate();
    StopGrap( false );
  }

  public override void UpdateAbility()
  {
    if( !IsActive )
      return;

    Vector3 armpos = pawn.GetShotOriginPosition();
    grapCableRenderer.transform.position = armpos;
    grapCableRenderer.transform.rotation = Quaternion.LookRotation( Vector3.forward, graphookTip.transform.position - armpos );

    if( grapPulling )
    {
      grapSize = grapCableRenderer.size;
      grapSize.y = Vector2.Distance( graphookTip.transform.position, armpos );
      grapCableRenderer.size = grapSize;
      // parent is guarded by conditional above. If the parent object is destroyed be sure to Deactivate()
      Vector2 grapDelta = graphitpos - (Vector2) parent.position;
      grapDeltaMagnitudePrevious = grapDeltaMagnitude;
      grapDeltaMagnitude = grapDelta.magnitude;
      // if no forward progess, deactivate
      if( grapDeltaMagnitude - grapDeltaMagnitudePrevious > grapReverseThreshold * Time.smoothDeltaTime )
        StopGrap( false );
      // the pawn will stop short of the destination, so keep the inertia.
      if( grapDeltaMagnitude < grapStopDistance )
      {
        StopGrap( true );
      }
      else
      {
        pawn.velocity = grapDelta.normalized * grapPullSpeed;
        lastGoodDelta = grapDelta;
      }
    }
  }

  public override void PreSceneTransition()
  {
    StopGrap( false );
  }

  public override void PostSceneTransition() { }

  void ShootGraphook( Vector2 origin, Vector2 direction )
  {
    Vector3 pos = origin;
    if( !Physics2D.Linecast( origin, pos, Global.ProjectileNoShootLayers ) )
    {
      RaycastHit2D hit = Physics2D.Raycast( pos, direction, grapDistance, Global.DefaultProjectileCollideLayers );
      if( hit )
      {
        //Debug.DrawLine( pos, hit.point, Color.red );
        grapShooting = true;
        graphitpos = hit.point;
        Vector2 grapDelta = graphitpos - (Vector2) parent.position;
        grapDeltaMagnitude = grapDelta.magnitude;
        grapDeltaMagnitudePrevious = grapDeltaMagnitude;

        graphookTip.transform.parent = null;
        graphookTip.transform.localScale = Vector3.one;
        graphookTip.transform.position = pos;
        graphookTip.transform.rotation = Quaternion.LookRotation( Vector3.forward, graphitpos - origin );

        grapTimer.Start( grapTimeout, delegate
        {
          pos = origin;
          graphookTip.transform.position = Vector3.MoveTowards( graphookTip.transform.position, graphitpos, grapSpeed * Time.deltaTime );
          //grap cable
          grapCableRenderer.gameObject.SetActive( true );
          grapCableRenderer.transform.parent = null;
          grapCableRenderer.transform.localScale = Vector3.one;
          grapCableRenderer.transform.position = pos;
          grapCableRenderer.transform.rotation = Quaternion.LookRotation( Vector3.forward, graphookTip.transform.position - pos );
          grapSize = grapCableRenderer.size;
          grapSize.y = Vector3.Distance( graphookTip.transform.position, pos );
          grapCableRenderer.size = grapSize;

          if( Vector3.Distance( graphookTip.transform.position, graphitpos ) < 0.01f )
          {
            grapShooting = false;
            grapPulling = true;
            grapTimer.Stop( false );
            grapTimer.Start( grapTimeout, null, delegate { StopGrap( true ); } );
            Global.instance.AudioOneShot( grapHitSound, origin );
          }
        }, delegate { StopGrap( false ); } );
        Global.instance.AudioOneShot( grapShotSound, origin );
      }
    }
  }

  void StopGrap( bool useInertia )
  {
    grapShooting = false;
    grapPulling = false;
    if( parent != null )
    {
      graphookTip.transform.parent = parent;
      grapCableRenderer.transform.parent = parent;
    }
    graphookTip.transform.localPosition = Vector3.zero;
    graphookTip.transform.localRotation = Quaternion.identity;
    grapCableRenderer.gameObject.SetActive( false );
    // avoid grapsize.y == 0 if StopGrap is called before grapSize is assigned
    grapSize.x = grapCableRenderer.size.x;
    grapSize.y = 0;
    grapCableRenderer.size = grapSize;
    grapTimer.Stop( false );
    if( useInertia )
    {
      pawn.inertia = lastGoodDelta.normalized * grapPullSpeed * inertiaCarryOver;
    }
  }
}