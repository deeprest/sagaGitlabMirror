using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : WorldSelectable
{
  [SerializeField] Animator animator;
  public Weapon weapon;

  public Transform GetTransform() { return transform; }

  public override void Highlight()
  {
    animator.Play( "highlight" );
  }
  public override void Unhighlight()
  {
    animator.Play( "idle" );
  }
  public override void Select()
  {
    animator.Play( "selected" );
  }
  public override void Unselect()
  { }

  void Start()
  {
    animator.Play( "idle" );
  }


}
