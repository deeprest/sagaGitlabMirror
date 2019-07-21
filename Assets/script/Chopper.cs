using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chopper : MonoBehaviour {

  public Transform hangPoint;
  public Transform chopperStartPoint;
  public PlayerController character;
  public float speed = 5;
  public float timeUntilDrop = 3;
  float timeStart;

  public void StartDrop()
  {
    transform.position = chopperStartPoint.position;
    character.hanging = true;
    character.velocity = Vector3.zero;
    character.transform.parent = hangPoint;
    character.transform.localPosition = Vector3.zero;

    timeStart = Time.time;
  }

	// Update is called once per frame
	void Update () {
		
    transform.position += Vector3.right * speed * Time.deltaTime;

    if( character != null )
    {
      if( Time.time - timeStart > timeUntilDrop )
      {
        character.transform.parent = null;
        character.hanging = false;
        character.push.x = speed;
        character = null;
      }
    }
	}
}
