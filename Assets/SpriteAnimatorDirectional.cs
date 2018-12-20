using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum AnimDirectionalEnum
{
  Towards,
  Away,
  Right,
  Left
}

[System.Serializable]
public class AnimSequenceDirectional
{
  public AnimDirectionalEnum direction;
  public Sprite[] sprites = new Sprite[1];
}


[System.Serializable]
public class AnimDirectional
{
  public string name;
  public int fps = 16;
  public AnimSequenceDirectional[] sequences = new AnimSequenceDirectional[4];
}

[ExecuteInEditMode]
public class SpriteAnimationDirectional : MonoBehaviour 
{
  public Texture2D texture;
  public MeshRenderer mr;
  public MeshFilter mf;
  public string startAnim;
  public bool isPlaying = false;
  public bool playAtAStart = true;
  public AnimDirectional[] anims = new AnimDirectional[1];
  Rect frame;
  AnimDirectional CurrentAnim;
  AnimSequenceDirectional CurrentSequence;
  Vector2[] uvs = new Vector2[4];
  float animStart = 0;
  int CurrentFrameIndex = 0;
  //

  public AnimDirectionalEnum direction;

  public void SetDirection( float yaw )
  {
    if( yaw >= -45 && yaw <= 45 ) 
      direction = AnimDirectionalEnum.Away;
    else if( yaw >= 45 && yaw <= 135 )
      direction = AnimDirectionalEnum.Right;
    else if( yaw <= -45 && yaw >= -135 )
      direction = AnimDirectionalEnum.Left;
    else 
      direction = AnimDirectionalEnum.Towards;

    CurrentSequence = CurrentAnim.sequences[ (int)direction ];
  }

  void Awake()
  {
    if( mr == null )
      mr = GetComponent<MeshRenderer>();
    if( mf == null )
      mf = GetComponent<MeshFilter>();

    if( playAtAStart )
      Play (startAnim, true);
    else
      CurrentAnim = anims[0];

    if( Application.isPlaying )
      mr.material.mainTexture = texture;
    else
      mr.sharedMaterial.mainTexture = texture;
  }

  public void Play( AnimDirectional a )
  {
    if( a == null )
      return;
    if( !mr.enabled )
      mr.enabled = true;
    isPlaying = true;
    CurrentAnim = a;
    CurrentSequence = CurrentAnim.sequences[ (int)direction ];
    CurrentFrameIndex = 0;
    if( Application.isPlaying )
      animStart = Time.time;
    #if UNITY_EDITOR    
    else
      animStart = (float)EditorApplication.timeSinceStartup;
    #endif

    //if( CurrentSequence != null )
      //mr.material.mainTexture = CurrentSequence.sprites[0].texture;
    
  }

  public void Play( string animSeq, bool restart=false )
  {
    foreach (var a in anims)
    {
      if (a.name == animSeq )
      {
        if( (CurrentAnim!=null && animSeq != CurrentAnim.name) || restart )
        {
          Play( a );
          break;
        }
      }
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
        CurrentSequence!=null && 
        CurrentSequence.sprites.Length>0 && 
        mr.material.mainTexture!=null  )
    {
      CurrentFrameIndex = Mathf.FloorToInt( ( Mathf.Max(0,time - animStart) ) * (float)CurrentAnim.fps ) % CurrentSequence.sprites.Length;
      Sprite sprite = CurrentSequence.sprites[ CurrentFrameIndex ];
      if( sprite != null )
      frame = new Rect( sprite.rect.x, (sprite.texture.height-sprite.rect.y-sprite.rect.height), sprite.rect.width, sprite.rect.height );

      if( mr.sharedMaterial.mainTexture!= null )
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
    }
  }

  void Update()
  {
    if( Application.isPlaying )
      UpdateFrame( Time.time );
    #if UNITY_EDITOR    
    else
      UpdateFrame( (float)EditorApplication.timeSinceStartup);
    #endif
  }
}
