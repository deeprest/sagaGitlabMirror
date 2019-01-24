//#define USE_SPRITE_RENDERER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor( typeof(SpriteAnimator) )]
public class SpriteAnimationEditor : Editor
{
  public override void OnInspectorGUI()
  {
    SpriteAnimator sa = target as SpriteAnimator;
    if( sa.isPlaying )
    {
      if( GUI.Button( EditorGUILayout.GetControlRect(), "Stop" ) )
        sa.Stop();
    }
    else
    {
      if( GUI.Button( EditorGUILayout.GetControlRect(), "Play" ) )
        sa.Play( sa.CurrentSequence );
    }
    DrawDefaultInspector();
  }
}
#endif


[ExecuteInEditMode]
public class SpriteAnimator : MonoBehaviour
{
  public bool UseSpriteRenderer = true;

  [Header( "Sprite Renderer" )]
  [SerializeField] SpriteRenderer sr;

  [Header( "Mesh Renderer" )]
  [SerializeField] MeshRenderer mr;
  [SerializeField] MeshFilter mf;
  Vector2[] uvs = new Vector2[4];

  public AnimSequence CurrentSequence;
  public AnimSequence startAnim;

  public bool isPlaying = false;
  public bool playAtAStart = true;
  public AnimSequence[] anims = new AnimSequence[1];
  public Dictionary<string,AnimSequence> animLookup;

  Rect frame;
  float animStart = 0;
  public int CurrentFrameIndex = 0;

  void Awake()
  {
    if( UseSpriteRenderer )
    {
      if( sr == null )
        sr = GetComponent<SpriteRenderer>();
    }
    else
    {
      if( mr == null )
        mr = GetComponent<MeshRenderer>();
      if( mf == null )
        mf = GetComponent<MeshFilter>();
    }
  }

  void Start()
  {
    animLookup = new Dictionary<string, AnimSequence>();
    foreach( var a in anims )
      animLookup[ a.name ] = a;

    if( playAtAStart )
      Play( startAnim );
  }

  #if UNITY_EDITOR
  public Mesh originalSharedMesh;
  public Material originalMaterial;
  #endif

  public void Play( AnimSequence a )
  {
    if( a == null || ( a.UseFrames && a.frames.Length == 0 ) || ( !a.UseFrames && a.sprites.Length == 0 ) )
      return;

    isPlaying = true;
    CurrentSequence = a;
    CurrentFrameIndex = 0;

    if( Application.isPlaying )
      animStart = Time.time;

    if( UseSpriteRenderer )
    {
      if( !sr.enabled )
        sr.enabled = true;
    }
    else
    {
      if( !mr.enabled )
        mr.enabled = true;
      if( Application.isPlaying )
      {
        mr.material.mainTexture = CurrentSequence.sprites[ 0 ].texture;
      }
      else
      {
        #if UNITY_EDITOR
        animStart = (float)EditorApplication.timeSinceStartup;
        originalSharedMesh = mf.sharedMesh;
        mf.sharedMesh = Instantiate<Mesh>( mf.sharedMesh );
        originalMaterial = mr.sharedMaterial;
        mr.sharedMaterial = Instantiate<Material>( mr.sharedMaterial );
        mr.sharedMaterial.mainTexture = CurrentSequence.sprites[ 0 ].texture;
        #endif
      }
    }
  }

  public void Play( string animName )
  {
    if( animLookup.ContainsKey( animName ) )
      Play( animLookup[ animName ] );
    else
    {
      Debug.LogError( "Anim sequence " + animName + " does not exist on animator", gameObject );
    }
  }

  public void Stop()
  {
    isPlaying = false;
    #if UNITY_EDITOR
    if( !UseSpriteRenderer )
    {
      if( mf.sharedMesh.GetInstanceID() != originalSharedMesh.GetInstanceID() )
      {
        DestroyImmediate( mf.sharedMesh );
        mf.sharedMesh = originalSharedMesh;
      }
      if( mr.sharedMaterial.GetInstanceID() != originalMaterial.GetInstanceID() )
      {
        DestroyImmediate( mr.sharedMaterial );
        mr.sharedMaterial = originalMaterial;
      }
    }
    #endif
  }

  void AdvanceFrame( float time )
  {
    int length = CurrentSequence.sprites.Length;
    if( CurrentSequence.UseFrames )
      length = CurrentSequence.frames.Length;
    if( length == 0 )
      return;

    if( CurrentSequence.loop )
    {
      CurrentFrameIndex = Mathf.FloorToInt( Mathf.Max( 0, time - animStart ) * (float)CurrentSequence.fps );
      if( CurrentFrameIndex > length - 1 )
        CurrentFrameIndex = CurrentSequence.loopStartIndex + CurrentFrameIndex % ( length - CurrentSequence.loopStartIndex );
    }
    else
    {
      CurrentFrameIndex = Mathf.Min( Mathf.FloorToInt( Mathf.Max( 0, time - animStart ) * (float)CurrentSequence.fps ), length - 1 );
    }
  }

  void UpdateFrame()
  {
    if( CurrentSequence == null )
      return;
    Sprite sprite;
    if( CurrentSequence.UseFrames )
    {
      CurrentFrameIndex = Mathf.Clamp( CurrentFrameIndex, 0, CurrentSequence.frames.Length - 1 ); 
      sprite = CurrentSequence.frames[ CurrentFrameIndex ].sprite;
    }
    else
    {
      CurrentFrameIndex = Mathf.Clamp( CurrentFrameIndex, 0, CurrentSequence.sprites.Length - 1 );
      sprite = CurrentSequence.sprites[ CurrentFrameIndex ];
    }

    if( UseSpriteRenderer )
    {
      sr.sprite = sprite;
    }
    else
    {
      if( sprite == null )
      {
        Debug.LogWarning( "null sprite", this );
        return;
      }

      if( mr.sharedMaterial.mainTexture != null )
      {
        float w = sprite.texture.width;
        float h = sprite.texture.height;
        frame = new Rect( sprite.rect.x, sprite.rect.y + sprite.rect.height, sprite.rect.width, sprite.rect.height );
        uvs[ 0 ].x = frame.x / w;
        uvs[ 0 ].y = frame.y / h;
        uvs[ 1 ].x = ( frame.x + frame.width ) / w;
        uvs[ 1 ].y = frame.y / h;
        uvs[ 2 ].x = frame.x / w;
        uvs[ 2 ].y = ( frame.y - frame.height ) / h;
        uvs[ 3 ].x = ( frame.x + frame.width ) / w;
        uvs[ 3 ].y = ( frame.y - frame.height ) / h;

        float sw = 1f / sprite.pixelsPerUnit;

        if( Application.isPlaying )
        {
          Vector3[] v = mf.mesh.vertices;
          v[ 0 ] = ( new Vector3( -sprite.pivot.x, sprite.rect.height - sprite.pivot.y, 0 ) ) * sw;
          v[ 1 ] = ( new Vector3( sprite.rect.width - sprite.pivot.x, sprite.rect.height - sprite.pivot.y, 0 ) ) * sw;
          v[ 2 ] = ( new Vector3( -sprite.pivot.x, -sprite.pivot.y, 0 ) ) * sw;
          v[ 3 ] = ( new Vector3( sprite.rect.width - sprite.pivot.x, -sprite.pivot.y, 0 ) ) * sw;
          mf.mesh.vertices = v;
          mf.mesh.uv = uvs;
        }
        #if UNITY_EDITOR
        else
        {
          Vector3[] v = mf.sharedMesh.vertices;
          v[ 0 ] = ( new Vector3( -sprite.pivot.x, sprite.rect.height - sprite.pivot.y, 0 ) ) * sw;
          v[ 1 ] = ( new Vector3( sprite.rect.width - sprite.pivot.x, sprite.rect.height - sprite.pivot.y, 0 ) ) * sw;
          v[ 2 ] = ( new Vector3( -sprite.pivot.x, -sprite.pivot.y, 0 ) ) * sw;
          v[ 3 ] = ( new Vector3( sprite.rect.width - sprite.pivot.x, -sprite.pivot.y, 0 ) ) * sw;
          mf.sharedMesh.vertices = v;
          mf.sharedMesh.uv = uvs;
        }
        #endif
      }
    }
  }

  void Update()
  {
    if( CurrentSequence == null )
      return;
    
    if( !Application.isEditor || Application.isPlaying )
    {
      if( isPlaying )
      {
        AdvanceFrame( Time.time );
        UpdateFrame();
      }
    }
    #if UNITY_EDITOR
    else
    {
      if( isPlaying )
        AdvanceFrame( (float)EditorApplication.timeSinceStartup );
      UpdateFrame();
    }
    #endif
  }
}
