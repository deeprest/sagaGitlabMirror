using UnityEngine;
using System.Collections;

public class SceneHome :  SceneScript
{
  public CharacterIdentity drcain;
  public JabberPlayer drcainJabber;
  public SpriteAnimator drcainAnimator;
  public bool runPlayer;

  public override void StartScene()
  {
    //player.playerInput = false;
    drcainAnimator.Play( "drcain-talk" );
    Timer talk = new Timer( 3, null, delegate { drcainAnimator.Play( "drcain-idle" ); } );
    drcainJabber.Play( "the city is being attacked!" );
    Global.instance.Speak( drcain, "the city is being attacked!", 3 );

    // for return from other level
    if( Global.instance.CurrentPlayer != null )
      Global.instance.CurrentPlayer.transform.position = Global.instance.FindSpawnPosition();
  }

}
