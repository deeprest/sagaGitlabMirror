using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

[RequireComponent( typeof( JabberPlayer ), typeof( Animator ) )]
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

    string say = "blah";
    JsonData json = new JsonData();
    string gameJson = identity.TextAsset.text;
    if( gameJson.Length > 0 )
    {
      JsonReader reader = new JsonReader( gameJson );
      json = JsonMapper.ToObject( reader );

      int count = json["randomdrcain"].Count;
      say = json["randomdrcain"][Random.Range( 0, count )].GetString();
    }

    Global.instance.Speak( identity, say, 4 );

  }

  public override void Unselect()
  {

  }
}
