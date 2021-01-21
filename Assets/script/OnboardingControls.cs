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
    public ControlCounter( InputAction atn )
    {
      count = 3;
      action = atn;
    }
    public int count;
    public InputAction action;
    public System.Action<InputAction.CallbackContext> iacc;
    public GameObject go;
    public Text txt;
  }

  void Start()
  {
    AddInputAction( Global.instance.Controls.GlobalActions.Menu );
    AddInputAction( Global.instance.Controls.GlobalActions.DEVRespawn );
    IEnumerator<InputAction> enumerator = Global.instance.Controls.BipedActions.Get().GetEnumerator();
    while( enumerator.MoveNext() )
      AddInputAction( enumerator.Current );


    //updateTextTimer.Start( int.MaxValue, 1, delegate ( Timer obj ) { UpdateText(); }, null );
  }

  private void LateUpdate()
  {
    parent.GetComponent<RectTransform>().sizeDelta = new Vector2( 200, parent.childCount * 20 + 20 );
  }

  public void UpdateText()
  {
    for( int i = 0; i < parent.childCount; i++ )
      parent.GetChild( i ).gameObject.SetActive( false );
    foreach( var pair in map )
    {
      if( pair.Value.count > 0 )
      {
        pair.Value.go.SetActive( true );
        pair.Value.txt.color = Color.Lerp( Color.white, Color.grey, 1.0f / pair.Value.count );
      }
    }
    foreach( var obj in Removal )
    {
      obj.performed -= map[obj].iacc;
      map.Remove( obj );
    }
    Removal.Clear();

    int activeCount=0;
    for( int i = 0; i < parent.childCount; i++ )
      if( parent.GetChild( i ).gameObject.activeSelf )
        activeCount++;
    if( activeCount == 0 )
    {
      gameObject.SetActive( false );
      updateTextTimer.Stop( false );
    }
  }

  void AddInputAction( InputAction inputAction )
  {
    ControlCounter cc = new ControlCounter( inputAction );
    cc.iacc = ( x ) => {
      map[cc.action].count--;
      UpdateText();
    };
    map.Add( inputAction, cc );
    inputAction.performed += cc.iacc;
    cc.go = Instantiate( template, parent );
    cc.txt = cc.go.GetComponent<Text>();
    cc.txt.text = Global.instance.ReplaceWithControlNames( inputAction.name + " [" + inputAction.name + "]" );
  }



}
