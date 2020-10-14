using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// follows the player around to give *super helpful* tips and musings!
public class ParadeFloater : MonoBehaviour
{
  public bool FollowPlayer;
  public Transform Target;
  public Transform LocalPosition;
  public Talker talker;
  Timer restPeriod = new Timer();
  public float speed = 1;
  public float targetRadius = 1;

  void OnDestroy()
  {
    restPeriod.Stop( false );
  }

  void Start()
  {
    if( talker != null )
      talker.animator.speed = 0.5f;
    if( FollowPlayer )
      Target = Global.instance.PlayerController.GetPawn().transform;
  }

  private Vector3 target;
  private Vector3 current;
  public int sayCount = 0;

  void Update()
  {
    if( !restPeriod.IsActive )
    {
      if( LocalPosition != null )
        target = Target.position - (transform.TransformPoint( LocalPosition.localPosition ) - transform.position);
      else
        target = Target.position;
      transform.position = Vector3.MoveTowards( transform.position, target, speed * Time.deltaTime );

      if( Vector3.Distance( target, transform.position ) < targetRadius )
      {
        restPeriod.Start( 10 );
        if( talker != null && sayCount == 0 )
          talker.Say( "Hello!" );
        else
          talker.Select();
        sayCount++;
      }
    }
  }
}