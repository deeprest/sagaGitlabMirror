using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneIntro : MonoBehaviour
{
  PlayerController player;
  public CharacterIdentity drcain;
  public JabberPlayer drcainJabber;
  public SpriteAnimator drcainAnimator;

  public bool runPlayer;

  // Start is called before the first frame update
  void Start()
  {
    player = Global.instance.CurrentPlayer;
    //player.playerInput = false;

    drcainAnimator.Play( "drcain-talk" );
    Timer talk = new Timer( 3, null, delegate { drcainAnimator.Play( "drcain-idle" ); } );
    drcainJabber.Play( "the city is being attacked!" );
    Global.instance.Speak( drcain, "the city is being attacked!", 3 );
  }

}
