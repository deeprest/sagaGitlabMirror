using UnityEngine;
using System.Collections;

public class LerpToTarget : MonoBehaviour
{
  public Transform moveTransform;
  public Transform targetTransform;
  //
  public bool WorldTarget = false;
  public Vector3 targetPositionWorld;
  public Vector3 targetRotationForward = Vector3.forward;
  public Vector3 targetRotationUp = Vector3.up;
  //
  public Vector3 localOffset;
  public float duration = 1f;
  public bool unscaledTime;
  public System.Action OnLerpEnd;

  float timeAccum = 0f;
  float alpha = 0f;

  public enum LerpType
  {
    Linear,
    Curve
  }

  public LerpType lerpType = LerpType.Linear;
  public AnimationCurve translateCurve;
  public AnimationCurve rotateCurve;

  public bool LerpRotation = false;
  public bool IgnoreYaw = true;
  public bool Scale = false;
  public float targetScale = 1f;
  public bool Continuous = false;
  public bool Rigidbody = false;
  public float force = 1f;
  public bool UseMaxDistance = false;
  public float MaxDistance = 2f;

  Vector3 startPosition;
  Quaternion startRotation;
  float startScale;
  Quaternion targetRotation;
  Rigidbody body = null;

  void OnEnable()
  {
    if( !WorldTarget && targetTransform == null )
    {
      enabled = false;
      return;
    }
    alpha = 0f;
    timeAccum = 0f;
    startPosition = moveTransform.position;
    startRotation = moveTransform.rotation;
    if( targetTransform != null )
      targetRotation = targetTransform.rotation;
    else
      targetRotation = Quaternion.LookRotation( targetRotationForward, targetRotationUp );

    if( Scale )
      startScale = moveTransform.localScale.x;

    if( Rigidbody )
      body = GetComponent<Rigidbody>();
  }

  void OnDisable()
  {
    WorldTarget = false;
  }

  void Awake()
  {
    if( moveTransform == null )
      moveTransform = transform;
  }

  void Update()
  {
    Vector3 position = targetPositionWorld;
    if( targetTransform != null )
      position = targetTransform.localToWorldMatrix.MultiplyPoint( localOffset );
    if( unscaledTime )
      timeAccum += Time.unscaledDeltaTime;
    else
      timeAccum += Time.deltaTime;
    alpha = Mathf.Clamp01( timeAccum / duration );
    float moveAlpha = alpha;
    if( lerpType == LerpType.Curve && translateCurve != null )
      moveAlpha = translateCurve.Evaluate( alpha );
    Vector3 delta = position - moveTransform.position;
    if( Rigidbody && body != null )
    {
      if( UseMaxDistance && delta.magnitude > MaxDistance )
      {
        body.MovePosition( position );
      }
      else
      {
        body.AddForce( -body.velocity + delta * force, ForceMode.VelocityChange );
      }
    }
    else
    {
      moveTransform.position = Vector3.Lerp( startPosition, position, moveAlpha );
    }

    if( Scale )
      moveTransform.localScale = Vector3.one * Mathf.Lerp( startScale, targetScale, moveAlpha );

    if( LerpRotation )
    {
      float rotAlpha = alpha;
      if( lerpType == LerpType.Curve && rotateCurve != null )
        rotAlpha = rotateCurve.Evaluate( alpha );
      Vector3 euler = Vector3.zero;
      euler.x = Mathf.LerpAngle( startRotation.eulerAngles.x, targetRotation.eulerAngles.x, rotAlpha );
      if( IgnoreYaw )
        euler.y = moveTransform.eulerAngles.y;
      else
        euler.y = Mathf.LerpAngle( startRotation.eulerAngles.y, targetRotation.eulerAngles.y, rotAlpha );
      euler.z = Mathf.LerpAngle( startRotation.eulerAngles.z, targetRotation.eulerAngles.z, rotAlpha );
      moveTransform.rotation = Quaternion.Euler( euler );
    }

    if( !Continuous )
    {
      if( Mathf.Approximately( alpha, 1f ) )
      {
        enabled = false;
        if( OnLerpEnd != null )
          OnLerpEnd.Invoke();
      }
    }

  }
}
