using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
  public Transform target;
  public float amplitude = 1;
  public float duration = 1;
  public float rate = 64;
  public AnimationCurve intensityCurve;
  public Vector3 localShakeAmount;

  Vector3 localStart;
  float startTime;
  Vector3 localShakeOffset;

  void OnEnable()
  {
    localStart = target.localPosition;
    startTime = Time.time;
    localShakeOffset = Random.onUnitSphere;
  }
  void OnDisable()
  {
    target.localPosition = localStart;
  }

  void Update()
  {
    float localTime = ( Time.time - startTime );
    Vector3 pos = Vector3.zero;
    pos.x = localShakeAmount.x * Mathf.Sin( localShakeOffset.x + localTime * rate ) * intensityCurve.Evaluate( (localShakeOffset.x + localTime) / duration );
    pos.y = localShakeAmount.y * Mathf.Sin( localShakeOffset.y + localTime * rate ) * intensityCurve.Evaluate( (localShakeOffset.y + localTime) / duration );
    pos.z = localShakeAmount.z * Mathf.Sin( localShakeOffset.z + localTime * rate ) * intensityCurve.Evaluate( (localShakeOffset.z + localTime) / duration );
    target.localPosition = localStart + (pos * amplitude); 
    if( localTime > duration )
      enabled = false;
  }
}

