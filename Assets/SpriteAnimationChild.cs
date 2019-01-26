using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SpriteAnimationChild : MonoBehaviour
{
  public SpriteAnimator sa;
#if UNITY_EDITOR
  Vector3 lastLocalPosition;
  void Update()
  {
    if( Application.isPlaying )
      return;
    if( sa.isPlaying )
      return;
    if( Vector3.Distance( transform.localPosition, lastLocalPosition ) > Mathf.Epsilon )
    {
      lastLocalPosition = transform.localPosition;
      sa.CurrentFrameIndex = Mathf.Clamp( sa.CurrentFrameIndex, 0, sa.CurrentSequence.frames.Length - 1 );
      AnimFrame af = sa.CurrentSequence.frames[sa.CurrentFrameIndex];
      if( af.point.Count == 0 )
      {
        AnimFramePoint np = new AnimFramePoint();
        np.name = transform.name;
        np.point = transform.localPosition;
        af.point.Add( np );
      }
      else
      {
        AnimFramePoint afp = af.point.Find( x => x.name == transform.name );
        if( afp == null )
        {
          afp = new AnimFramePoint();
          afp.name = transform.name;
          af.point.Add( afp );
        }
        afp.point = transform.localPosition;
      }
    }
  }
#endif
}
