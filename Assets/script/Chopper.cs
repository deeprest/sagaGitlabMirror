using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chopper : MonoBehaviour {

  public Transform hangPoint;
  public Transform chopperStartPoint;
  public PlayerController character;
  public float speed = 5;
  Timer timer = new Timer();
  public float timeUntilDrop = 3;
  float timeStart;

  public void StartDrop()
  {
    transform.position = chopperStartPoint.position;
    character.hanging = true;
    character.velocity = Vector3.zero;
    character.transform.parent = hangPoint;
    character.transform.localPosition = Vector3.zero;


    timer.Start( timeUntilDrop, null, delegate
    {
      if( character != null )
      {
        character.transform.parent = null;
        character.hanging = false;
        character.Push( Vector2.right * speed, 1 );
        character = null;
      }
    } );
  }

	void Update () 
  {
    transform.position += Vector3.right * speed * Time.deltaTime;
	}
}
