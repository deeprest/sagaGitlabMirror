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

  void OnEnable()
  {
    localStart = target.localPosition;
    startTime = Time.time;
  }
  void OnDisable()
  {
    target.localPosition = localStart;
  }

  void Update()
  {
    float localTime = ( Time.time - startTime );
    Vector3 pos = Vector3.zero;
    pos.x = localShakeAmount.x * Mathf.Sin( localTime * rate ) * intensityCurve.Evaluate( localTime / duration );
    pos.y = localShakeAmount.y * Mathf.Cos( localTime * rate ) * intensityCurve.Evaluate( localTime / duration );
    pos.z = 0;
    target.localPosition = localStart + (pos * amplitude); 
    if( localTime > duration )
      enabled = false;
  }
}

