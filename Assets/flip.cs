using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flip : StateMachineBehaviour
{

  public SpriteChunk[] sac;

  // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    sac = animator.gameObject.GetComponentsInChildren<SpriteChunk>();   
  }

  // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
  override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {

  }

  // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
  //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  //{
  //    
  //}

    /*
  // OnStateMove is called right after Animator.OnAnimatorMove()
  override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    bool facingRight = animator.GetBool( "facingRight" );
    if( !facingRight )
    {
      foreach( var sa in sac )
      {
        if( sa.flipXPosition )
        {

          Vector3 pos = sa.transform.localPosition;
          pos.x = -pos.x;
          sa.transform.localPosition = pos;

        }
      }
    }
  }
    */

  // OnStateIK is called right after Animator.OnAnimatorIK()
  //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  //{
  //    // Implement code that sets up animation IK (inverse kinematics)
  //}
}
