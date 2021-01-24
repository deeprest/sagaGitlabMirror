using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof(PlayerController) )]
public class PlayerControllerEditor : Editor
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();
    PlayerController pc = target as PlayerController;
    if( GUI.Button( EditorGUILayout.GetControlRect(), "record" ) )
      pc.RecordBegin();
    if( GUI.Button( EditorGUILayout.GetControlRect(), "end" ) )
      pc.RecordEnd();
    if( GUI.Button( EditorGUILayout.GetControlRect(), "play recording" ) )
      pc.RecordPlayback();
  }
}
#endif

public class PlayerController : Controller
{
  Controls.BipedActionsActions BA;
  public float DirectionalCursorDistance = 3;
  // any persistent input variables
  Vector2 aimPosition = Vector2.zero;
  bool fire;
  public Vector2 aimDeltaSinceLastFrame;

  // init bindings to modify input struct
  public override void Awake()
  {
    base.Awake();
    BindControls();
    recordpath = Application.persistentDataPath + "/input.dat";
  }

  public void HACKSetSpeed( float speed )
  {
    if( pawn != null )
      pawn.OnControllerAssigned();
  }

  public override void AssignPawn( Pawn pwn )
  {
    base.AssignPawn( pwn );
    // I'm Mr. Meseeks, look at me!
    Global.instance.CameraController.LookTarget = this;
    Global.instance.CameraController.transform.position = pawn.transform.position;

    if( pawn is PlayerBiped )
      EnableBipedControls();
  }

  public override void RemovePawn( )
  {
    Global.instance.CameraController.LookTarget = null;
    pawn = null;
    Global.instance.Controls.BipedActions.Disable();
  }

  public void EnableBipedControls()
  {
    Global.instance.Controls.BipedActions.Enable();
  }

  private void BindControls()
  {
    BA = Global.instance.Controls.BipedActions;

    BA.Ability.performed += ( obj ) => input.Ability = true;
    BA.NextAbility.performed += ( obj ) => input.NextAbility = true;
    BA.NextWeapon.performed += ( obj ) => input.NextWeapon = true;
    //BA.Charge.started += ( obj ) => input.Charge = true;
    BA.Down.performed += ( obj ) => input.MoveDown = true;
    BA.Interact.performed += ( obj ) => { input.Interact = true; };
    //BA.Aim.performed += ( obj ) => { aimDeltaSinceLastFrame += obj.ReadValue<Vector2>(); };
  }

  // apply input to pawn
  public override void Update()
  {
    if( playback )
    {
      if( pawn == null || playbackIndex >= evt.Count )
        playback = false;
      else
      {
        pawn.transform.position = evt[playbackIndex].position;
        pawn.ApplyInput( evt[playbackIndex].input );
        playbackIndex++;
      }
    }
    else
    {
      if( Cursor.lockState != CursorLockMode.None )
      {
        if( Global.instance.UsingGamepad )
        {
          aimPosition = BA.Aim.ReadValue<Vector2>() * DirectionalCursorDistance;
        }
        else
        {
          aimDeltaSinceLastFrame = BA.Aim.ReadValue<Vector2>();
          aimPosition += aimDeltaSinceLastFrame * Global.instance.CursorSensitivity * Time.unscaledDeltaTime;
          aimDeltaSinceLastFrame = Vector2.zero;
          aimPosition = aimPosition.normalized * Mathf.Max( Mathf.Min( aimPosition.magnitude, Camera.main.orthographicSize * Camera.main.aspect * Global.instance.CursorOuter ), 0.01f );
        }
      }
      input.Aim = aimPosition;
      
      if( BA.MoveRight.ReadValue<float>() > 0.5f ) input.MoveRight = true;
      if( BA.MoveLeft.ReadValue<float>() > 0.5f ) input.MoveLeft = true;
      input.Jump = BA.Jump.phase == InputActionPhase.Started;
      input.Dash = BA.Dash.phase == InputActionPhase.Started;
      input.Fire = BA.Fire.phase == InputActionPhase.Started;
      input.Charge = BA.Charge.phase == InputActionPhase.Started;

      Vector2 move = BA.Move.ReadValue<Vector2>();
      if( move.x > 0.5f ) input.MoveRight = true;
      if( move.x < -0.5f ) input.MoveLeft = true;
      if( move.y > 0.5f ) input.MoveUp = true;
      if( move.y < -0.5f ) input.MoveDown = true;

      if( pawn != null )
        pawn.ApplyInput( input );

      /* // HACK
      foreach( var paw in minions )
        paw.ApplyInput( input );*/

      if( recording )
        evt.Add( new RecordState {input = input, position = pawn.transform.position} );
    }

    input = default;
  }

  public void FireOnlyInputMode()
  {
    Global.instance.Controls.BipedActions.Disable();
    Global.instance.Controls.BipedActions.Aim.Enable();
    Global.instance.Controls.BipedActions.Fire.Enable();
  }

  public void NormalInputMode()
  {
    Global.instance.Controls.BipedActions.Enable();
  }

  public void OnPauseMenu()
  {
    Global.instance.Controls.BipedActions.Disable();
  }

  public void OnUnpause()
  {
    Global.instance.Controls.BipedActions.Enable();
  }
  
  #region Record

  // todo record the initial state of all objects in the scene
  // store random seed / generation seed
  // store the scene index or sname

  // replace using frame index with timestamps. during playback, look ahead to the
  // state in the next keyframe and interpolate to it.

  // add chop drop event
  // store slo-motion begin/end events
  // store pause, menu active events (or ignore them during capture)

  struct RecordState
  {
    public InputState input;
    public Vector2 position;
  }

  // input state = 2 + 8 (position)
  const int stateSize = (2 + 8) + 8;

  struct RecordHeader
  {
    public Vector2 initialposition;
    // todo need all relevent pawn state
    // current weapon index
  }

  const int headerSize = 8;

  bool recording;
  bool playback;
  int playbackIndex = 0;
  RecordHeader header;
  List<RecordState> evt;
  string recordpath;

  public void RecordToggle()
  {
    if( recording )
      RecordEnd();
    else
      RecordBegin();
  }

  public bool IsRecording()
  {
    return recording;
  }

  public void RecordBegin()
  {
    recording = true;
    evt = new List<RecordState>();
    header = new RecordHeader
    {
      initialposition = pawn.transform.position
    };
  }

  public void RecordEnd()
  {
    recording = false;

    byte[] x;
    int p = 0;
    byte[] buffer = new byte[headerSize + evt.Count * stateSize];

    FileStream fs = File.Open( recordpath, FileMode.Create );
    x = System.BitConverter.GetBytes( header.initialposition.x );
    buffer[p] = x[0];
    buffer[p + 1] = x[1];
    buffer[p + 2] = x[2];
    buffer[p + 3] = x[3];
    p += 4;

    x = System.BitConverter.GetBytes( header.initialposition.y );
    buffer[p] = x[0];
    buffer[p + 1] = x[1];
    buffer[p + 2] = x[2];
    buffer[p + 3] = x[3];
    p += 4;

    for( int i = 0; i < evt.Count; i++ )
    {
      RecordState state = evt[i];
      InputState input = state.input;
      // 2 byte
      int value =
        (input.MoveLeft ? 0x1 : 0x0) << 0 |
        (input.MoveRight ? 0x1 : 0x0) << 1 |
        (input.MoveUp ? 0x1 : 0x0) << 2 |
        (input.MoveDown ? 0x1 : 0x0) << 3 |
        (input.Jump ? 0x1 : 0x0) << 4 |
        (input.Dash ? 0x1 : 0x0) << 5 |
        (input.Fire ? 0x1 : 0x0) << 6 |
        (input.Charge ? 0x1 : 0x0) << 7 |
        (input.Ability ? 0x1 : 0x0) << 6 |
        (input.Interact ? 0x1 : 0x0) << 8 |
        (input.NextWeapon ? 0x1 : 0x0) << 9 |
        (input.NextAbility ? 0x1 : 0x0) << 10 |
        (input.NOTUSED0 ? 0x1 : 0x0) << 11 |
        (input.NOTUSED0 ? 0x1 : 0x0) << 12 |
        (input.NOTUSED0 ? 0x1 : 0x0) << 13 |
        (input.NOTUSED0 ? 0x1 : 0x0) << 14;

      buffer[p] = (byte) value;
      buffer[p + 1] = (byte) (value >> 8);
      p += 2;

      x = System.BitConverter.GetBytes( input.Aim.x );
      buffer[p] = x[0];
      buffer[p + 1] = x[1];
      buffer[p + 2] = x[2];
      buffer[p + 3] = x[3];
      p += 4;

      x = System.BitConverter.GetBytes( input.Aim.y );
      buffer[p] = x[0];
      buffer[p + 1] = x[1];
      buffer[p + 2] = x[2];
      buffer[p + 3] = x[3];
      p += 4;

      // position
      x = System.BitConverter.GetBytes( state.position.x );
      buffer[p] = x[0];
      buffer[p + 1] = x[1];
      buffer[p + 2] = x[2];
      buffer[p + 3] = x[3];
      p += 4;

      x = System.BitConverter.GetBytes( state.position.y );
      buffer[p] = x[0];
      buffer[p + 1] = x[1];
      buffer[p + 2] = x[2];
      buffer[p + 3] = x[3];
      p += 4;
    }

    fs.Write( buffer, 0, buffer.Length );
    fs.Flush();
    fs.Close();
  }

  public void RecordPlayback()
  {
    evt = new List<RecordState>();
    playbackIndex = 0;

    byte[] buffer = File.ReadAllBytes( recordpath );
    // header
    pawn.transform.position = new Vector3( System.BitConverter.ToSingle( buffer, 0 ), System.BitConverter.ToSingle( buffer, 4 ), 0 );
    // frames
    for( int i = headerSize; i < buffer.Length; i += stateSize )
    {
      InputState state = new InputState
      {
        MoveLeft = ((buffer[i] >> 0) & 0x1) > 0,
        MoveRight = ((buffer[i] >> 1) & 0x1) > 0,
        MoveUp = ((buffer[i] >> 2) & 0x1) > 0,
        MoveDown = ((buffer[i] >> 3) & 0x1) > 0,
        Jump = ((buffer[i] >> 4) & 0x1) > 0,
        Dash = ((buffer[i] >> 5) & 0x1) > 0,
        Fire = ((buffer[i] >> 6) & 0x1) > 0,
        Charge = ((buffer[i] >> 7) & 0x1) > 0,

        Ability = ((buffer[i] >> 0) & 0x1) > 0,
        Interact = ((buffer[i + 1] >> 1) & 0x1) > 0,
        NextWeapon = ((buffer[i + 1] >> 2) & 0x1) > 0,
        NextAbility = ((buffer[i + 1] >> 3) & 0x1) > 0,
        // NOTUSED0 = ((buffer[i + 1] >> 4) & 0x1) > 0,
        // NOTUSED1 = ((buffer[i + 1] >> 5) & 0x1) > 0,
        // NOTUSED2 = ((buffer[i + 1] >> 6) & 0x1) > 0,
        // NOTUSED3 = ((buffer[i + 1] >> 7) & 0x1) > 0,
        //

        Aim = new Vector2( System.BitConverter.ToSingle( buffer, i + 2 ), System.BitConverter.ToSingle( buffer, i + 6 ) )
      };

      Vector2 pos = new Vector2( System.BitConverter.ToSingle( buffer, i + 10 ), System.BitConverter.ToSingle( buffer, i + 14 ) );
      evt.Add( new RecordState {input = state, position = pos} );
    }

    playback = true;
  }

  #endregion
}