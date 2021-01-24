using UnityEngine;
using System.Collections;

public class SceneHome : SceneScript
{
  public override void StartScene()
  {
    base.StartScene();
    // for return from other level
    if( Global.instance.CurrentPlayer != null )
    {
      Global.instance.CurrentPlayer.transform.position = Global.instance.FindSpawnPosition();
      Global.instance.CurrentPlayer.velocity = Vector2.zero;
    }

    Global.instance.CameraController.orthoTarget = 2;
  }

  #region Skins
  [System.Serializable]
  public struct ImageVariant
  {
    public Material sharedMaterial;
    public Texture2D regular;
    public Texture2D alternate;
  }
  public ImageVariant[] variants;

  public void Flatten()
  {
    foreach( var vnt in variants )
    {
      Texture2D tex = ((Texture2D)vnt.sharedMaterial.GetTexture( "_MainTex" ));
      tex.SetPixels32( vnt.alternate.GetPixels32(), 0 );
      tex.Apply();
    }
  }

  public void Unflatten()
  {
    foreach( var vnt in variants )
    {
      Texture2D tex = ((Texture2D)vnt.sharedMaterial.GetTexture( "_MainTex" ));
      tex.SetPixels32( vnt.regular.GetPixels32(), 0 );
      tex.Apply();
    }
  }

  //#if UNITY_EDITOR
  private void OnDestroy()
  {
    Unflatten();
  }
  //#endif
  #endregion

}
