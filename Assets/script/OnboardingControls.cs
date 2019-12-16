using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class OnboardingControls : MonoBehaviour
{
  [SerializeField] Transform parent;
  [SerializeField] GameObject template;
  Controls c;
  Timer updateTextTimer = new Timer();
  // action, text
  Dictionary<InputAction, ControlCounter> map = new Dictionary<InputAction, ControlCounter>();
  List<InputAction> Removal = new List<InputAction>();

  class ControlCounter
  {
    public ControlCounter( string txt, InputAction atn )
    {
      text = txt;
      count = 3;
      action = atn;
      //action.performed += iacc;
    }
    public string text;
    public int count;
    public InputAction action;
    public System.Action<InputAction.CallbackContext> iacc;
  }

  void UpdateText()
  {
    for( int i = 0; i < parent.childCount; i++ )
      Destroy( parent.GetChild( i ).gameObject );
    foreach( var pair in map )
    {
      if( pair.Value.count <= 0 )
      {
        Removal.Add( pair.Value.action );
      }
      else
      {
        GameObject go = Instantiate( template, parent );
        Text txt = go.GetComponent<Text>();
        txt.text = Global.instance.ReplaceWithControlNames( pair.Value.text );
        txt.color = Color.Lerp( Color.white, Color.grey, 1.0f / pair.Value.count );
      }
    }
    foreach( var obj in Removal )
    {
      obj.performed -= map[obj].iacc;
      map.Remove( obj );
    }
    Removal.Clear();

    if( parent.childCount == 0 )
    {
      gameObject.SetActive( false );
      updateTextTimer.Stop( false );
    }
  }

  private void LateUpdate()
  {
    parent.GetComponent<RectTransform>().sizeDelta = new Vector2( 200, parent.childCount * 20 + 20 );
  }

  void AddInputAction( InputAction inputAction )
  {
    ControlCounter cc = new ControlCounter( inputAction.name + " [" + inputAction.name + "]", inputAction );
    cc.iacc = ( x ) => { map[cc.action].count--; UpdateText(); };
    map.Add( inputAction, cc );
    inputAction.performed += cc.iacc;
  }

  void Start()
  {
    IEnumerator<InputAction> enumerator = Global.instance.Controls.BipedActions.Get().GetEnumerator();
    while( enumerator.MoveNext() )
      AddInputAction( enumerator.Current );

    AddInputAction( Global.instance.Controls.GlobalActions.Menu );
    AddInputAction( Global.instance.Controls.GlobalActions.DEVRespawn );

    updateTextTimer.Start( int.MaxValue, 1, delegate ( Timer obj ) { UpdateText(); }, null );
  }

}
