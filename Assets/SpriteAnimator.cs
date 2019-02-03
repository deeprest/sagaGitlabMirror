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
      // avoid creating keyframes while scrubbing
      foreach( var sac in sa.sac )
        sac.ResetPosition();
    }
    if( GUI.Button( EditorGUILayout.GetControlRect(), "Delete Frame" ) )
    {
      sa.CurrentSequence.DeleteFrame( sa.CurrentFrameIndex );
      sa.UpdateFrame();
      foreach( var sac in sa.sac )
        sac.ResetPosition();
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

  [SerializeField] SpriteRenderer sr;

  public Material material
  {
    get
    {
      return sr.material;
    }
  }
  public bool flipX = false;
  public AnimSequence CurrentSequence;

  public bool isPlaying = false;
  public bool playAtAStart = true;
  public AnimSequence[] anims = new AnimSequence[1];
  public Dictionary<string, AnimSequence> animLookup;

  Rect frame;
  float animStart = 0;

  void Awake()
  {

    if( sr == null )
      sr = GetComponent<SpriteRenderer>();

  }

  void Start()
  {
    sac = GetComponentsInChildren<SpriteAnimationChild>();

    animLookup = new Dictionary<string, AnimSequence>();
    foreach( var a in anims )
      animLookup[a.name] = a;

    if( playAtAStart )
      Play( CurrentSequence );
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


    if( !sr.enabled )
      sr.enabled = true;


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
  }

  public void AdvanceFrame( float time )
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


    sr.sprite = sprite;
    sr.flipX = flipX;

    if( Application.isPlaying )
      sr.material.SetInt( "_FlipX", flipX ? 1 : 0 );
    else
      sr.sharedMaterial.SetInt( "_FlipX", flipX ? 1 : 0 );

    if( CurrentSequence.UseFrames )
    {
      AnimFrame af = CurrentSequence.GetKeyFrame (CurrentFrameIndex);
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
