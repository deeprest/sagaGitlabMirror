using UnityEngine;
using System.Collections;

public class SceneHome :  MonoBehaviour
{
  public CharacterIdentity drcain;
  public JabberPlayer drcainJabber;
  public SpriteAnimator drcainAnimator;
  public bool runPlayer;

  void Start()
  {
    //player.playerInput = false;
    drcainAnimator.Play( "drcain-talk" );
    Timer talk = new Timer( 3, null, delegate { drcainAnimator.Play( "drcain-idle" ); } );
    drcainJabber.Play( "the city is being attacked!" );
    Global.instance.Speak( drcain, "the city is being attacked!", 3 );
  }

}
