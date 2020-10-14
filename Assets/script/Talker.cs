using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof(Talker) )]
public class TalkerEditor : Editor
{
  public override void OnInspectorGUI()
  {
    if( Application.isPlaying )
    {
      Talker obj = target as Talker;
      if( GUI.Button( EditorGUILayout.GetControlRect(), "Test" ) )
        obj.Select();
    }
    DrawDefaultInspector();
  }
}
#endif

[RequireComponent( typeof(JabberPlayer), typeof(Animator) )]
public class Talker : WorldSelectable
{
  public CharacterIdentity identity;
  public JabberPlayer jabber;
  public Animator animator;
  Timer talk = new Timer();

  private void OnDestroy()
  {
    talk.Stop( false );
  }

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
    Say( say );
  }

  public override void Unselect() { }

  public void Say( string say )
  {
    animator.Play( "talk" );
    Global.instance.Speak( identity, say, 4 );
    talk.Start( 4, null, delegate { animator.Play( "idle" ); } );
    jabber.Play( say );
  }
}