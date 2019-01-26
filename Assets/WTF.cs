#if false
// For no apparent reason, Unity does not let you animate the sprite in a sprite
// renderer from a AnimationClip. We cannot access the time within the Animation
// Editor window, otherwise this script would suffice.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[ExecuteInEditMode]
public class WTF : MonoBehaviour
{
  public float time;
  Animation anim;

  // Update is called once per frame
  void Update ()
  {
    if (anim == null)
      anim = GetComponent<Animation> ();
    if (anim.clip != null) {
      EditorCurveBinding editorCurveBinding = EditorCurveBinding.PPtrCurve ("", typeof (SpriteRenderer), "m_Sprite");
      ObjectReferenceKeyframe [] objectReferenceKeyframes = AnimationUtility.GetObjectReferenceCurve (anim.clip, editorCurveBinding);
      if (objectReferenceKeyframes != null) {
        //sprites = objectReferenceKeyframes.Select (objectReferenceKeyframe => objectReferenceKeyframe.value).OfType<Sprite> ().ToArray ();
        //objectReferenceKeyframes.Select(x=>x.time >=)
        for (int i = 0; i < objectReferenceKeyframes.Length; i++) {
          //anim [anim.clip.name].time
          if (objectReferenceKeyframes [i].time > time) {
            GetComponent<SpriteRenderer> ().sprite = (objectReferenceKeyframes [i - 1].value as Sprite);
            break;
          }
        }
      }
    }

  }

  public void CHangeSprite (Sprite s)
  {

    GetComponent<SpriteRenderer> ().sprite = s;
  }
}

#endif