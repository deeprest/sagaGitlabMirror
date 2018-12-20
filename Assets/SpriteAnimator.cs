#define USE_SPRITE_RENDERER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


/*[System.Serializable]
public class AnimSequence
{
  public string name;
  public int fps = 16;
  public Sprite[] sprites;
}*/

//[ExecuteInEditMode]
public class SpriteAnimator : MonoBehaviour
{
  #if USE_SPRITE_RENDERER
  public SpriteRenderer sr;
  #else
  public MeshRenderer mr;
  public MeshFilter mf;
  Vector2[] uvs = new Vector2[4];
  #endif
  public AnimSequence CurrentSequence;
  public AnimSequence startAnim;

  public bool isPlaying = false;
  public bool playAtAStart = true;
  public AnimSequence[] anims = new AnimSequence[1];
  public Dictionary<string,AnimSequence> animLookup;

  Rect frame;
  float animStart = 0;
  int CurrentFrameIndex = 0;




  void Awake()
  {
    #if USE_SPRITE_RENDERER

    if( sr == null )
      sr = GetComponent<SpriteRenderer>();
    #else
    if( mr == null )
    mr = GetComponent<MeshRenderer>();
    if( mf == null )
    mf = GetComponent<MeshFilter>();

    #endif

    animLookup = new Dictionary<string, AnimSequence>();
    foreach( var a in anims )
      animLookup[ a.name ] = a;

    if( playAtAStart )
      Play( startAnim );
  }

  public void Play( AnimSequence a, bool restart = false )
  {
    if( CurrentSequence == a && !restart )
      return;
    if( a == null || a.sprites.Length == 0 )
      return;
    #if USE_SPRITE_RENDERER
    if( !sr.enabled )
      sr.enabled = true;
    #else

    if( !mr.enabled )
    mr.enabled = true;
    #endif
    isPlaying = true;
    CurrentSequence = a;
    CurrentFrameIndex = 0;

    if( Application.isPlaying )
      animStart = Time.time;
    #if UNITY_EDITOR    
    else
      animStart = (float)EditorApplication.timeSinceStartup;
    #endif

    #if USE_SPRITE_RENDERER

    #else
    if( Application.isPlaying )
    mr.material.mainTexture = CurrentSequence.sprites[0].texture;
    else
    mr.sharedMaterial.mainTexture = CurrentSequence.sprites[0].texture;

    #endif
  }

  public void Play( string animName, bool restart = false )
  {
    if( animLookup.ContainsKey( animName ) )
      Play( animLookup[ animName ], restart );
    else
    {
      Debug.LogError( "Anim sequence " + animName + " does not exist on animator", gameObject );
    }
  }

  void OnEnable()
  {
    #if UNITY_EDITOR
//    if( !EditorApplication.isPlaying )
//      SceneView.onSceneGUIDelegate += SceneGUI;

//    if( mesh==null )
//      mesh = GetComponent<MeshFilter>().mesh;

//    animStart = (float)EditorApplication.timeSinceStartup;
    #endif

  }

  void OnDisable()
  {
//    #if UNITY_EDITOR
//    if( !EditorApplication.isPlaying )
//      SceneView.onSceneGUIDelegate -= SceneGUI;
//    #endif
  }

  //  #if UNITY_EDITOR
  //  void SceneGUI( SceneView sv )
  //  {
  //    if( !Application.isPlaying )
  //    {
  //      UpdateFrame( (float)EditorApplication.timeSinceStartup );
  //    }
  //  }
  //  #endif


  void UpdateFrame( float time )
  {
    if( isPlaying &&
        CurrentSequence != null &&
        CurrentSequence.sprites.Length > 0 )
    {
      
      if( CurrentSequence.holdLastFrame )
        CurrentFrameIndex = Mathf.Min( Mathf.FloorToInt( Mathf.Max( 0, time - animStart ) * (float)CurrentSequence.fps ), CurrentSequence.sprites.Length - 1 );
      else
      {
        CurrentFrameIndex = Mathf.FloorToInt( Mathf.Max( 0, time - animStart ) * (float)CurrentSequence.fps );
        if( CurrentFrameIndex > CurrentSequence.sprites.Length - 1 )
          CurrentFrameIndex = CurrentSequence.loopStartIndex + CurrentFrameIndex % (CurrentSequence.sprites.Length - CurrentSequence.loopStartIndex );
      }
      Sprite sprite = CurrentSequence.sprites[ CurrentFrameIndex ];

      #if USE_SPRITE_RENDERER
      if( sprite != null )
        sr.sprite = sprite;
      #else
      if( sprite != null )
        frame = new Rect( sprite.rect.x, ( sprite.texture.height - sprite.rect.y - sprite.rect.height ), sprite.rect.width, sprite.rect.height );

      if( mr.sharedMaterial.mainTexture != null )
      {
        float w = sprite.texture.width;
        float h = sprite.texture.height;

        uvs[ 0 ].x = frame.x / w;
        uvs[ 0 ].y = ( h - frame.y ) / h;
        uvs[ 1 ].x = ( frame.x + frame.width ) / w;
        uvs[ 1 ].y = ( h - frame.y ) / h;
        uvs[ 2 ].x = frame.x / w;
        uvs[ 2 ].y = ( ( h - frame.y ) - frame.height ) / h;
        uvs[ 3 ].x = ( frame.x + frame.width ) / w;
        uvs[ 3 ].y = ( ( h - frame.y ) - frame.height ) / h;

        if( Application.isPlaying )
        {
          mf.mesh.uv = uvs;

        }
        else
        {
          mf.sharedMesh.uv = uvs;
        }
      }
      #endif
    }
  }

  void Update()
  {
    if( Application.isPlaying )
      UpdateFrame( Time.time );
    #if UNITY_EDITOR    
    else
      UpdateFrame( (float)EditorApplication.timeSinceStartup );
    #endif
  }
}
