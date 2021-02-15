using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof(Rain) )]
public class RainEditor : Editor
{
  public override void OnInspectorGUI()
  {
    (target as Rain).UpdateRain();
    DrawDefaultInspector();
  }
}
#endif

public class Rain : MonoBehaviour
{
  [SerializeField] RainDropSplashMesh rdsm;
  [SerializeField] ParticleSystem rainFall;
  [SerializeField] ParticleSystem rainPops;
  public Color color;

  public Vector2 direction = Vector2.down;
  public float intensity = 1;
  
  void OnDestroy()
  {
    Util.Destroy( rainFall.gameObject );
    Util.Destroy( rainPops.gameObject );
  }
  
  public void UpdateRain()
  {
    rdsm.direction = direction;
    ParticleSystem.VelocityOverLifetimeModule volm = rainFall.velocityOverLifetime;
    volm.x = direction.x;
    volm.y = direction.y;
    volm.speedModifierMultiplier = intensity * 8;

    ParticleSystem.MainModule fallMain = rainFall.main;
    fallMain.startColor = color;

    ParticleSystem.MainModule popsMain = rainPops.main;
    popsMain.startColor = color;
    popsMain.startRotation = -Mathf.Atan2( direction.x, -direction.y );
    popsMain.startSize3D = true;
    popsMain.startSizeXMultiplier = 0.016f;
    popsMain.startSizeYMultiplier = intensity;
    popsMain.startSizeZMultiplier = 0;
  }
  
  
}