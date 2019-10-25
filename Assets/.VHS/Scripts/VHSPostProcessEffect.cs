using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class VHSPostProcessEffect : UnityStandardAssets.ImageEffects.PostEffectsBase
{
	Material m;
	public Shader shader;
  new public Renderer renderer;
  public UnityEngine.Video.VideoClip clip;
	public UnityEngine.Video.VideoPlayer VHS;

	float yScanline, xScanline;

	protected override void Start() {
		//m = new Material(shader);
		//m.SetTexture("_VHSTex", VHS.texture );
    VHS.clip = clip;
    VHS.isLooping = true;
    VHS.playOnAwake = false;
    VHS.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
    VHS.targetMaterialRenderer = renderer;
    VHS.targetMaterialProperty = "_VHSTex";
		VHS.Play();
	}

  /*
	void OnRenderImage(RenderTexture source, RenderTexture destination){
		yScanline += Time.deltaTime * 0.1f;
		xScanline -= Time.deltaTime * 0.1f;

		if(yScanline >= 1){
			yScanline = Random.value;
		}
		if(xScanline <= 0 || Random.value < 0.05){
			xScanline = Random.value;
		}
    renderer.material.SetFloat("_yScanline", yScanline);
    renderer.material.SetFloat("_xScanline", xScanline);
		Graphics.Blit(source, destination, m);
	}
	*/
}