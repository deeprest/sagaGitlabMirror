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
[SelectionBase]
public class Talker : WorldSelectable
{
  public CharacterIdentity identity;
  public JabberPlayer jabber;
  public Animator animator;
  Timer talkTimer = new Timer();

  JsonData json;
  
  void Start()
  {
    if( identity == null )
    {
      Debug.LogWarning( "null identity for " + name, gameObject );
      return;
    }
    if( identity.TextAsset == null )
    {
      Debug.LogWarning( "identity has no text asset " + name, gameObject );
      return;
    }
    if( identity.TextAsset.text.Length == 0 )
    {
      Debug.LogWarning( "identity text asset is empty " + name, gameObject );
      return;
    }
    json = JsonMapper.ToObject( new JsonReader( identity.TextAsset.text ) );
  }
  
  private void OnDestroy()
  {
    talkTimer.Stop( false );
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
    int randomCount = json["random"].Count;
    string say = json["random"][Random.Range( 0, randomCount )].GetString();
    //
    Say( say );
    // face the player when talking to them
    transform.localScale = new Vector3( -Mathf.Sign( Global.instance.CurrentPlayer.transform.position.x - transform.position.x ), 1, 1);
  }

  public override void Unselect() { }

  public void Say( string say )
  {
    animator.Play( "talk" );
    talkTimer.Start( 4, null, delegate { animator.Play( "idle" ); } );
    Global.instance.Speak( identity, say, 4 );
    jabber.Play( say );
  }
}