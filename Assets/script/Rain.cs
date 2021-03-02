using UnityEngine;
using UnityEngine.Serialization;
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
  [FormerlySerializedAs( "rdsm" )]
  public RainDropSplashMesh rainDropSplashMesh;
  public ParticleSystem rainFall;
  public ParticleSystem rainPops;
  public Color color;

  public Vector2 direction = Vector2.down;
  public float intensity = 1;

  public void Initialize( Bounds bounds ) 
  {
    if( rainDropSplashMesh != null )
    {
      const float horOffset = 40;
      rainDropSplashMesh.transform.parent = null;
      rainDropSplashMesh.transform.position = new Vector2( bounds.center.x, bounds.size.y );
      rainDropSplashMesh.width = bounds.size.x + horOffset;
      //rainDropSplashMesh.maxDistance = bounds.size.y;
      //rainMaker.direction = Vector2.down;
      rainDropSplashMesh.Generate();
      // Generate() before setting to active, so the mesh exists beforehand.
      rainDropSplashMesh.gameObject.SetActive( true );
    }
    if( rainFall != null )
    {
      rainFall.transform.parent = Global.instance.CameraController.transform;
      rainFall.transform.localPosition = Vector3.zero;
    }
    if( rainPops != null )
    {
      rainPops.transform.parent = Global.instance.CameraController.transform;
      rainPops.transform.localPosition = Vector3.zero;
    }
  }

  //#if !UNITY_EDITOR
  void OnDestroy()
  {
    if( rainFall!=null && rainFall.gameObject!=null )
      Destroy( rainFall.gameObject );
    if( rainPops!=null && rainPops.gameObject!=null )
      Destroy( rainPops.gameObject );
  }
//#endif
  
  public void UpdateRain()
  {
    rainDropSplashMesh.direction = direction;
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