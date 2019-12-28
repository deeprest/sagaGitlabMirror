using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JabberPlayer),typeof(Animator))]
public class Talker : WorldSelectable
{
  public CharacterIdentity identity;
  public JabberPlayer jabber;
  public Animator animator;

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
    animator.Play( "talk" );
    Timer talk = new Timer( 4, null, delegate { animator.Play( "idle" ); } );
    jabber.Play( "jibber jabber" );
    Global.instance.Speak( identity, "You! This struggle is philosophical!", 4 );
  }

  public override void Unselect()
  { 
  
  }
}
