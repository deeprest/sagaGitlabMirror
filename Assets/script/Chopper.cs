using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Chopper : MonoBehaviour {

  public Transform hangPoint;
  public Transform chopperStartPoint;
  public float speed = 5;
  Timer timer = new Timer();
  public float timeUntilDrop = 1;
  float timeStart;

  public void StartDrop( Entity ent )
  {
    transform.position = chopperStartPoint.position;
    ent.hanging = true;
    ent.velocity = Vector3.zero;
    ent.transform.parent = hangPoint;
    ent.transform.localPosition = Vector3.zero;


    timer.Start( timeUntilDrop, null, delegate
    {
      if( ent != null )
      {
        ent.transform.parent = null;
        ent.hanging = false;
        ent.inertia = Vector2.right * speed;
        //ent.OverrideVelocity( Vector2.right * speed, 2.8f );
        ent = null;
      }
    } );
  }

	void Update () 
  {
    transform.position += Vector3.right * speed * Time.deltaTime;
	}
}
