using UnityEngine;
using System.Collections;

public class SceneHome : SceneScript
{
  public CharacterIdentity drcain;
  public JabberPlayer drcainJabber;
  public Animator drcainAnimator;

  public override void StartScene()
  {
    if( Application.isEditor && !Global.instance.SimulatePlayer && Global.instance.CurrentPlayer == null )
    {
      Global.instance.SpawnPlayer();
      return;
    }

    drcainAnimator.Play( "drcain-talk" );
    Timer talk = new Timer( 3, null, delegate { drcainAnimator.Play( "drcain-idle" ); } );
    drcainJabber.Play( "the city is being attacked!" );
    Global.instance.Speak( drcain, "the city is being attacked!", 3 );

    // for return from other level
    if( Global.instance.CurrentPlayer != null )
      Global.instance.CurrentPlayer.transform.position = Global.instance.FindSpawnPosition();
  }

}
