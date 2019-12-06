using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class SceneIntro : SceneScript
{
  bool introFlag = false;
  [SerializeField] Animator animator;
  [SerializeField] AudioLoop musicIntro;

  public override void StartScene()
  {
    Global.instance.Controls.GlobalActions.Any.performed += StopIntro;
    animator.Play( "intro" );
    Global.instance.PlayMusic( music );
  }

  private void OnDestroy()
  {
    Global.instance.Controls.GlobalActions.Any.performed -= StopIntro;
  }

  void StopIntro( InputAction.CallbackContext ctx )
  {
    StopIntro();
  }

  public void StopIntro()
  {
    if( introFlag )
      return;
    introFlag = true;
    Global.instance.LoadScene( "home", false, true, true );
  }
}