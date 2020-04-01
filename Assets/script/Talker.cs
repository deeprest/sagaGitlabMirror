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
  Timer talk = new Timer();

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
    talk.Start( 4, null, delegate { animator.Play( "idle" ); } );
    jabber.Play( say );

  }

  public override void Unselect()
  {

  }
}
