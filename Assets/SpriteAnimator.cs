//#define USE_SPRITE_RENDERER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor( typeof( SpriteAnimator ) )]
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
    if( GUI.Button( EditorGUILayout.GetControlRect(), "Update Children List" ) )
    {
      sa.sac = sa.GetComponentsInChildren<SpriteAnimationChild>();
    }
    int frame = EditorGUILayout.IntField( "Current Frame Index", sa.CurrentFrameIndex );
    if( frame != sa.CurrentFrameIndex )
    {
      sa.CurrentFrameIndex = frame;
      sa.UpdateFrame();
    }
    DrawDefaultInspector();
  }
}
#endif

[System.Serializable]
public class MountPointBinding
{
  public string mount;
  public Transform transform;
}

[ExecuteInEditMode]
public class SpriteAnimator : MonoBehaviour
{
  [HideInInspector]
  public int CurrentFrameIndex = 0;
  public SpriteAnimationChild[] sac;

#if SPRITE_MESH_RENDERER
  [Header( "Mesh Renderer" )]
  [SerializeField] MeshRenderer mr;
  [SerializeField] MeshFilter mf;
  Vector2[] uvs = new Vector2[4];
#if UNITY_EDITOR
  [SerializeField] Mesh originalSharedMesh;
  [SerializeField] Material originalMaterial;
#endif
  public float depthIncrement = 0.01f;
  public int spriteLayer = 0;
#else
  [HideInInspector] bool UseSpriteRenderer = true;
#endif
  [Header( "Sprite Renderer" )]
  [SerializeField] SpriteRenderer sr;

  public Material material
  {
    get
    {
#if SPRITE_MESH_RENDERER
  if( UseSpriteRenderer ) return sr.material; else return mr.material;  
#else
      return sr.material;
#endif
    }
  }
  public bool flipX = false;
  public AnimSequence CurrentSequence;
  public AnimSequence startAnim;

  public bool isPlaying = false;
  public bool playAtAStart = true;
  public AnimSequence[] anims = new AnimSequence[1];
  public Dictionary<string, AnimSequence> animLookup;

  Rect frame;
  float animStart = 0;

  void Awake()
  {
    if( UseSpriteRenderer )
    {
      if( sr == null )
        sr = GetComponent<SpriteRenderer>();
    }
#if SPRITE_MESH_RENDERER
    else
    {
      if( mr == null )
        mr = GetComponent<MeshRenderer>();
      if( mf == null )
        mf = GetComponent<MeshFilter>();
    }
#endif
  }

  void Start()
  {
    sac = GetComponentsInChildren<SpriteAnimationChild>();

    animLookup = new Dictionary<string, AnimSequence>();
    foreach( var a in anims )
      animLookup[a.name] = a;

    if( playAtAStart )
      Play( startAnim );
  }

  public void Play( AnimSequence a )
  {
    if( a == null || (a.UseFrames && a.frames.Length == 0) || (!a.UseFrames && a.sprites.Length == 0) )
      return;

    isPlaying = true;
    CurrentSequence = a;
    CurrentFrameIndex = 0;

#if UNITY_EDITOR
    if( Application.isPlaying )
      animStart = Time.time;
    else
      animStart = (float)EditorApplication.timeSinceStartup;
#else
    if( Application.isPlaying )
    animStart = Time.time;
#endif

    if( UseSpriteRenderer )
    {
      if( !sr.enabled )
        sr.enabled = true;
    }
    else
    {
#if SPRITE_MESH_RENDERER
      if( !mr.enabled )
        mr.enabled = true;
      if( Application.isPlaying )
      {
        mr.material.mainTexture = CurrentSequence.sprites[0].texture;
      }
      else
      {
#if UNITY_EDITOR
        originalSharedMesh = mf.sharedMesh;
        mf.sharedMesh = Instantiate<Mesh>( mf.sharedMesh );
        originalMaterial = mr.sharedMaterial;
        mr.sharedMaterial = Instantiate<Material>( mr.sharedMaterial );
        mr.sharedMaterial.mainTexture = CurrentSequence.sprites[0].texture;
#endif
      }
#endif
    }
  }

  public void Play( string animName )
  {
    if( animLookup.ContainsKey( animName ) )
      Play( animLookup[animName] );
    else
    {
      Debug.LogError( "Anim sequence " + animName + " does not exist on animator", gameObject );
    }
  }

  public void Stop()
  {
    isPlaying = false;
#if UNITY_EDITOR
#if SPRITE_MESH_RENDERER
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
        CurrentFrameIndex = CurrentSequence.loopStartIndex + CurrentFrameIndex % (length - CurrentSequence.loopStartIndex);
    }
    else
    {
      CurrentFrameIndex = Mathf.Min( Mathf.FloorToInt( Mathf.Max( 0, time - animStart ) * (float)CurrentSequence.fps ), length - 1 );
      if( CurrentFrameIndex == length )
        isPlaying = false;
    }
  }

  public void UpdateFrame()
  {
    if( CurrentSequence == null )
      return;
    Sprite sprite;
    if( CurrentSequence.UseFrames )
    {
      CurrentFrameIndex = Mathf.Clamp( CurrentFrameIndex, 0, CurrentSequence.frames.Length - 1 );
      sprite = CurrentSequence.frames[CurrentFrameIndex].sprite;
    }
    else
    {
      CurrentFrameIndex = Mathf.Clamp( CurrentFrameIndex, 0, CurrentSequence.sprites.Length - 1 );
      sprite = CurrentSequence.sprites[CurrentFrameIndex];
    }

    if( UseSpriteRenderer )
    {
      sr.sprite = sprite;
      sr.flipX = flipX;

      if( Application.isPlaying )
        sr.material.SetInt( "_FlipX", flipX ? 1 : 0 );
      else
        sr.sharedMaterial.SetInt( "_FlipX", flipX ? 1 : 0 );
      /*Vector3 angles = transform.localRotation.eulerAngles;
      angles.y = Util.NormalizeAngle( flipX? 180 : 0 );
      transform.localRotation = Quaternion.Euler( angles );*/


      if( CurrentSequence.UseFrames )
      {
        AnimFrame af = CurrentSequence.frames[CurrentFrameIndex];
        foreach( var afp in af.point )
        {
          Transform child = transform.Find( afp.name );
          if( child != null )
          {
            if( flipX )
            {
              Vector3 lpos = afp.point;
              lpos.x = -lpos.x;
              child.localPosition = lpos;
            }
            else
              child.localPosition = afp.point;
          }
        }
      }

    }
#if SPRITE_MESH_RENDERER
    else
    {
      if( sprite == null )
      {
        Debug.LogWarning( "null sprite", this );
        return;
      }

      if( CurrentSequence.UseFrames )
      {
        //update this
      }

      // higher sprite layer equals more negative z
      Vector3 pos = transform.position;
      pos.z = depthIncrement * -spriteLayer;
      transform.position = pos;

      if( mr.sharedMaterial.mainTexture != null )
      {
        float w = sprite.texture.width;
        float h = sprite.texture.height;
        frame = new Rect( sprite.rect.x, sprite.rect.y + sprite.rect.height, sprite.rect.width, sprite.rect.height );
        uvs[0].x = frame.x / w;
        uvs[0].y = frame.y / h;
        uvs[1].x = (frame.x + frame.width) / w;
        uvs[1].y = frame.y / h;
        uvs[2].x = frame.x / w;
        uvs[2].y = (frame.y - frame.height) / h;
        uvs[3].x = (frame.x + frame.width) / w;
        uvs[3].y = (frame.y - frame.height) / h;

        float sw = 1f / sprite.pixelsPerUnit;

        mr.sharedMaterial.SetInt( "_FlipX", flipX ? 1 : 0 );
        int a, b, c, d;
        if( flipX )
        {
          a = 1;
          b = 0;
          c = 3;
          d = 2;
        }
        else
        {
          a = 0;
          b = 1;
          c = 2;
          d = 3;
        }
        if( Application.isPlaying )
        {
          Vector3[] v = mf.mesh.vertices;
          v[a] = (new Vector3( -sprite.pivot.x, sprite.rect.height - sprite.pivot.y, 0 )) * sw;
          v[b] = (new Vector3( sprite.rect.width - sprite.pivot.x, sprite.rect.height - sprite.pivot.y, 0 )) * sw;
          v[c] = (new Vector3( -sprite.pivot.x, -sprite.pivot.y, 0 )) * sw;
          v[d] = (new Vector3( sprite.rect.width - sprite.pivot.x, -sprite.pivot.y, 0 )) * sw;
          mf.mesh.vertices = v;
          mf.mesh.uv = uvs;
        }
#if UNITY_EDITOR
        else
        {
          Vector3[] v = mf.sharedMesh.vertices;
          v[a] = (new Vector3( -sprite.pivot.x, sprite.rect.height - sprite.pivot.y, 0 )) * sw;
          v[b] = (new Vector3( sprite.rect.width - sprite.pivot.x, sprite.rect.height - sprite.pivot.y, 0 )) * sw;
          v[c] = (new Vector3( -sprite.pivot.x, -sprite.pivot.y, 0 )) * sw;
          v[d] = (new Vector3( sprite.rect.width - sprite.pivot.x, -sprite.pivot.y, 0 )) * sw;
          mf.sharedMesh.vertices = v;
          mf.sharedMesh.uv = uvs;
        }
#endif
      }
    }
#endif
  }

  void Update()
  {
    if( CurrentSequence == null )
      return;

    if( isPlaying )
    {
#if UNITY_EDITOR
      if( Application.isPlaying )
        AdvanceFrame( Time.time );
      else
        AdvanceFrame( (float)EditorApplication.timeSinceStartup );
#else
      AdvanceFrame( Time.time );
#endif
      UpdateFrame();
    }

  }
}
