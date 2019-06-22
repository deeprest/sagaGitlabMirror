using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GameInput
{
  public class InputControlScheme
  {
    public Dictionary<string, KeyCode> keyMap = new Dictionary<string, KeyCode>();
    public Dictionary<string, string> axisMap = new Dictionary<string, string>();
  }

  public static InputControlScheme icsCurrent { get; set; }
  static InputControlScheme icsKeyboard = new InputControlScheme();
  static InputControlScheme icsGamepad = new InputControlScheme();
  public static bool UsingKeyboard { get { return icsCurrent == icsKeyboard; } }
  static public bool remappingControls { get; set; }
  public static float deadZone = 0.3f;
  public static KeyCode[] allKeys;
  public static KeyCode[] activateKeyboardKeys;
  public GameInputRemapper gis;
  public static List<string> axes = new List<string>();
  public static System.Action RepopulateControlBindingList;
  public static System.Action OnUseKeyboard;
  public static System.Action OnUseGamepad;

  static GameInput()
  {
    allKeys = (KeyCode[])System.Enum.GetValues( typeof( KeyCode ) );
    List<KeyCode> temp = new List<KeyCode>( allKeys );
    temp.RemoveAll( x => x >= KeyCode.JoystickButton0 );
    // Escape is used to access keybind menu. 
    // This is intended to avoid changing current input scheme before remapping controls
    temp.RemoveAll( x => x == KeyCode.Escape || (x >= KeyCode.Mouse0 && x <= KeyCode.Mouse6) );
    activateKeyboardKeys = temp.ToArray();

    for( int i = 0; i < 20; i++ )
      axes.Add( "Joy0Axis" + i );
  }

  public static string[] GetAllActions()
  {
    List<string> str = new List<string>();
    foreach( var pair in icsCurrent.keyMap )
    {
      if( !str.Contains( pair.Key ) )
        str.Add( pair.Key );
    }
    foreach( var pair in icsCurrent.axisMap )
    {
      if( !str.Contains( pair.Key ) )
        str.Add( pair.Key );
    }
    return str.ToArray();
  }

  public static void UseKeyboard()
  {
    icsCurrent = icsKeyboard;
    RepopulateControlBindingList();
    Global.instance.CursorPlayerRelative = false;
    OnUseKeyboard();
  }

  public static void UseGamepad()
  {
    icsCurrent = icsGamepad;
    RepopulateControlBindingList();
    Global.instance.CursorPlayerRelative = true;
    OnUseGamepad();
  }

  public static void SetDefaultControls()
  {
    // keyboard
    icsKeyboard.keyMap["MoveRight"] = KeyCode.D;
    icsKeyboard.keyMap["MoveLeft"] = KeyCode.A;
    icsKeyboard.keyMap["Jump"] = KeyCode.W;
    icsKeyboard.keyMap["Down"] = KeyCode.S;
    icsKeyboard.keyMap["Dash"] = KeyCode.Space;
    icsKeyboard.keyMap["Fire"] = KeyCode.Mouse0;
    icsKeyboard.keyMap["graphook"] = KeyCode.Mouse1;
    icsKeyboard.keyMap["Pickup"] = KeyCode.E;

    icsKeyboard.keyMap["Charge"] = KeyCode.Mouse0;

    // gamepad
    icsGamepad.keyMap["MoveRight"] = KeyCode.JoystickButton8;
    icsGamepad.keyMap["MoveLeft"] = KeyCode.JoystickButton7;
    icsGamepad.keyMap["Jump"] = KeyCode.JoystickButton14;
    icsGamepad.keyMap["Down"] = KeyCode.JoystickButton0;
    icsGamepad.keyMap["Dash"] = KeyCode.JoystickButton13;
    icsGamepad.keyMap["Fire"] = KeyCode.None;
    icsGamepad.keyMap["graphook"] = KeyCode.None;
    icsGamepad.keyMap["Pickup"] = KeyCode.None;
    icsGamepad.keyMap["Charge"] = KeyCode.None;
    icsGamepad.axisMap["ShootX"] = "Joy0Axis2";
    icsGamepad.axisMap["ShootY"] = "Joy0Axis3";

  }

  public static float GetAxis( string key )
  {
    return Input.GetAxis( icsCurrent.axisMap[key] );
  }
  public static float GetAxisRaw( string key )
  {
    return Input.GetAxisRaw( icsCurrent.axisMap[key] );
  }
  public static bool GetKey( string key )
  {
    return Input.GetKey( icsCurrent.keyMap[key] );
  }
  public static bool GetKeyDown( string key )
  {
    return Input.GetKeyDown( icsCurrent.keyMap[key] );
  }
  public static bool GetKeyUp( string key )
  {
    return Input.GetKeyUp( icsCurrent.keyMap[key] );
  }

}

public class GameInputRemapper : MonoBehaviour
{


  [Header( "Control Remap" )]
  public GameObject controllerRemapScreen;
  public RectTransform controllerMapListParent;
  public GameObject controllerMapListItemTemplate;
  public Text remapInstruction;
  public Sprite keyboardSprite;
  public Sprite gamepadSprite;
  public Image controllerIndicator;


  public void InitializeControls()
  {
    // UI
    //controllerRemapScreen.SetActive( false );
    controllerMapListItemTemplate.SetActive( false );

    GameInput.RepopulateControlBindingList += RepopulateControlBindingList;
    GameInput.OnUseKeyboard += OnUseKeyboard;
    GameInput.OnUseGamepad += OnUseGamepad;

    GameInput.SetDefaultControls();
    GameInput.UseKeyboard();
  }

  void OnUseKeyboard()
  {
    controllerIndicator.sprite = keyboardSprite;
  }

  void OnUseGamepad()
  {
    controllerIndicator.sprite = gamepadSprite;
  }

  void Update()
  {
    if( !GameInput.remappingControls )
    {
      if( GameInput.UsingKeyboard )
      {
        for( int i = 0; i < 20; i++ )
        {
          if( Input.GetKey( "joystick button " + i ) )
          {
            GameInput.UseGamepad();
          }
        }
      }
      else
      {
        for( int i = 0; i < GameInput.activateKeyboardKeys.Length; i++ )
        {
          if( Input.GetKeyDown( GameInput.activateKeyboardKeys[i] ) )
          {
            GameInput.UseKeyboard();
          }
        }
      }
    }
  }


  void RepopulateControlBindingList()
  {
    // repopulate controller remap list
    for( int i = 0; i < controllerMapListParent.childCount; i++ )
    {
      Transform tf = controllerMapListParent.GetChild( i );
      if( tf.gameObject == controllerMapListItemTemplate )
        continue;
      Destroy( tf.gameObject );
    }
    foreach( var pair in GameInput.icsCurrent.keyMap )
      AddControlListItem( pair.Key );
    foreach( var pair in GameInput.icsCurrent.axisMap )
      AddControlListItem( pair.Key );

    SelectFirstButton();
  }

  void AddControlListItem( string key )
  {
    GameObject go = Instantiate( controllerMapListItemTemplate, controllerMapListParent );
    go.SetActive( true );
    go.name = key;
    Text[] ts = go.GetComponentsInChildren<Text>();
    ts[0].text = key;
    if( GameInput.icsCurrent.keyMap.ContainsKey( key ) )
      ts[1].text = GameInput.icsCurrent.keyMap[key].ToString();
    else if( GameInput.icsCurrent.axisMap.ContainsKey( key ) )
      ts[1].text = GameInput.icsCurrent.axisMap[key].ToString();
    Button btn = go.GetComponentInChildren<Button>();
    btn.onClick.AddListener( delegate
    {
      if( !GameInput.remappingControls )
        RemapSingleControl( key );
    } );
  }

  public void InitiateRemap()
  {
    controllerRemapScreen.SetActive( true );
    remapInstruction.text = "press desired button for each action";
    StartCoroutine( RemapRoutine() );
  }



  IEnumerator RemapRoutine()
  {
    GameInput.remappingControls = true;
    bool done = false;

    Dictionary<string, KeyCode> changeKey = new Dictionary<string, KeyCode>();
    Dictionary<string, string> changeAxis = new Dictionary<string, string>();

    string[] actions = GameInput.GetAllActions();
    for( int a = 0; !done && a < actions.Length; a++ )
    {
      string key = actions[a];
      GameObject go = controllerMapListParent.Find( key ).gameObject;
      Text[] ts = go.GetComponentsInChildren<Text>();
      ts[0].color = Color.red;
      remapInstruction.text = "press desired button for action: " + key;

      bool hit = false;
      while( !hit )
      {
        yield return null;
        if( Input.GetButtonDown( "Cancel" ) )
          break;
        for( int i = 0; i < GameInput.allKeys.Length; i++ )
        {
          if( Input.GetKeyDown( GameInput.allKeys[i] ) )
          {
            ts[1].text = i.ToString();
            hit = true;
            changeKey[key] = GameInput.allKeys[i];
            break;
          }
        }
        if( !hit )
        {
          foreach( var ax in GameInput.axes )
          {
            if( Mathf.Abs( Input.GetAxisRaw( ax ) ) > 0.5f )
            {
              changeAxis[key] = ax;
              hit = true;
              break;
            }
          }
        }
      }

      if( hit )
      {
        ts[0].color = Color.white;
        Input.ResetInputAxes();
        yield return new WaitForSecondsRealtime( 0.2f );
      }
    }

    foreach( var pair in changeKey )
      GameInput.icsCurrent.keyMap[pair.Key] = pair.Value;
    foreach( var pair in changeAxis )
      GameInput.icsCurrent.axisMap[pair.Key] = pair.Value;
      
    Input.ResetInputAxes();
    yield return null;
    GameInput.remappingControls = false;
  }

  public void RemapSingleControl( string key )
  {
    StartCoroutine( RemapSingleControlRoutine( key ) );
  }

  IEnumerator RemapSingleControlRoutine( string key )
  {
    GameInput.remappingControls = true;
    GameObject go = controllerMapListParent.Find( key ).gameObject;
    Text[] ts = go.GetComponentsInChildren<Text>();
    ts[0].color = Color.red;
    remapInstruction.text = "press desired button for action: " + key;

    KeyCode changeKey = KeyCode.None;
    string changeAxis = null;
    bool hit = false;
    while( !hit )
    {
      yield return null;
      if( Input.GetButtonDown( "Cancel" ) )
        break;
      for( int i = 0; i < GameInput.allKeys.Length; i++ )
      {
        if( Input.GetKeyDown( GameInput.allKeys[i] ) )
        {
          ts[1].text = GameInput.allKeys[i].ToString();
          hit = true;
          changeKey = GameInput.allKeys[i];
          break;
        }
      }
      if( !hit )
      {
        foreach( var ax in GameInput.axes )
        {
          if( Mathf.Abs( Input.GetAxisRaw( ax ) ) > 0.5f )
          {
            changeAxis = ax;
            hit = true;
            break;
          }
        }
      }
    }
    if( changeKey != KeyCode.None )
      GameInput.icsCurrent.keyMap[key] = changeKey;
    if( changeAxis != null )
      GameInput.icsCurrent.axisMap[key] = changeAxis;

    GameInput.remappingControls = false;
  }

  public void OnShow()
  {
    SelectFirstButton();
  }


  void SelectFirstButton()
  {
    GameObject go = null;
    for( int i = 0; i < controllerMapListParent.childCount; i++ )
    {
      if( controllerMapListParent.GetChild( i ).gameObject.activeSelf )
      {
        go = controllerMapListParent.GetChild( i ).gameObject;
        break;
      }
    }
    Button btn = go.GetComponentInChildren<Button>();
    if( EventSystem.current.currentSelectedGameObject != btn.gameObject )
      EventSystem.current.SetSelectedGameObject( btn.gameObject, null );
  }
}