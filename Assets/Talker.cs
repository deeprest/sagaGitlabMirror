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
    Timer talk = new Timer( 3, null, delegate { animator.Play( "idle" ); } );
    jabber.Play( "the city is being attacked!" );
    Global.instance.Speak( identity, "the city is being attacked!", 3 );
  }

  public override void Unselect()
  { 
  
  }
}
