using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character
{
  [Header( "Player" )]
  public bool isPlayerControlled = false;
  public bool PlayerStrafeMode = false;
  Vector2 moveRelative;
  // arms length
  public float ActionRadius = 1;
  public float ActionRaycastMinimumDot = 0.5f;
  public GameObject InteractIndicatorPrefab;
  GameObject InteractIndicator = null;
  IAction lastSelected;
  QueryTriggerInteraction qti = QueryTriggerInteraction.Ignore;
  List<IAction> acts;
  Transform selected;
  Character selectedCharacter;

  void PushPlayer()
  {
    if( nid != null )
      nid.localPlayerAuthority = true;

    isPlayerControlled = true;
    CurrentMoveSpeed = WalkSpeed;
    InteractIndicator = GameObject.Instantiate( InteractIndicatorPrefab, null );
    InteractIndicator.SetActive( false );

    cachedRankForPlayerState = Rank;
    Rank = 100;

    acts = new List<IAction>();
  }

  void ResumePlayer()
  {

  }

  void SuspendPlayer()
  {

  }

  void PopPlayer()
  {
    if( nid != null )
      nid.localPlayerAuthority = false;

    isPlayerControlled = false;

    if( InteractIndicator != null )
      GameObject.Destroy( InteractIndicator );
    selectedCharacter = null;
    HidePositionalText();
    
    Rank = cachedRankForPlayerState;

    Global.Instance.ContextMenu.Hide();
  }
    
 
  /*
  public IEnumerator SleepCycle( )
  {
    // sleep. animation. camera. fade out. advance time of day. recharge health. fade in. music.

    Zone zone = Global.Instance.GetZone( gameObject );
    musicSource = zone.GetComponent<AudioSource>();
    musicSource.clip = sleepyMusic;

//    Sleeping = true;
//    SleepInterrupt = false;

    musicSource.Play();
    Global.Instance.AudioGoSleep( 1 );

    cam = Global.Instance.cameraController;
    cam.LookControlEnabled = false;
    //cam.Automated = true;
    //cam.LerpTo( transform.position + Vector3.up * cam.cameraHeightMax, Vector3.down, cam.GetForwardDirection(), cameraLerpDuration );
    float start = Time.unscaledTime;
    float todStart = Global.Instance.CurrentTimeOfDay;
    //float lerpstart = cam.lookInput.y;
    Color cacheScreenFade = cam.FullScreenQuad.color;

    while( Time.unscaledTime - start < sleepCameraLerpDurationIn )
    {
      if( SleepInterrupt )
        break;
      float alpha = ( Time.unscaledTime - start ) / sleepCameraLerpDurationIn;
      cam.lookInput.y = Mathf.Lerp(cam.lookInput.y, 90, alpha );
      //cam.ScreenFadeColor( Color.Lerp( cacheScreenFade, fade, alpha) );
      yield return null;
    }
    if( SleepInterrupt )
    {
      yield return WakeUpSequence();
      yield break;
    }
      
    Time.timeScale = sleepTimescale;
    start = Time.unscaledTime;
    while( Time.unscaledTime - start < sleepTODWait )
    {
      if( SleepInterrupt )
        break;
      float alpha = ( Time.unscaledTime - start ) / sleepTODWait;
      Global.Instance.CurrentTimeOfDay = todStart + sleepTODDuration * alpha;
      Global.Instance.UpdateTimeOfDay();
      //cam.ScreenFadeColor( fade );
      yield return null;
    }
      
    SleepInterrupt = true;
    yield return WakeUpSequence();
  }

  IEnumerator WakeUpSequence()
  {
    Time.timeScale = 1;
    musicSource.Stop();
    Global.Instance.AudioWakeUp( 1 );
    cam.LookControlEnabled = true;

    float start = Time.unscaledTime;
    while( Time.unscaledTime - start < sleepCameraLerpDurationOut )
    {
      float alpha = ( Time.unscaledTime - start ) / sleepCameraLerpDurationOut;
      cam.lookInput.y = Mathf.Lerp( cam.lookInput.y, 15, alpha );
      //cam.ScreenFadeColor( Color.Lerp( fade, cacheScreenFade, alpha) );
      yield return null;
    }
    //Sleeping = false;
    //SleepInterrupt = false;

    sleepState = SleepState.None;
    body.isKinematic = false;
    ball.Sphere.enabled = true;
    ball.LockToGround = true;
    FaceDirectionOfMovement = true;
    moveTransform.parent = null;
    billboard.faceCamera = true;
    PlayFaceAnimation( "idle" );
  }
*/

  void UpdatePlayer()
  {
//    if( Sleeping )
//    {
//      if( Input.GetButtonDown( "Interact" ) )
//      {
//        SleepInterrupt = true;
//      }
//      return;
//    }
    UpdatePlayerMovementInput();
    UpdatePlayerWeaponInput();

    if( Input.GetKeyDown( KeyCode.Equals ) )
    {
      CharacterEvent evt = new CharacterEvent( CharacterEventEnum.DebugAddAffinity, moveTransform.position, Time.time, this, this );
      BroadcastEvent( evt, Global.Instance.GlobalSightDistance );
    }

    if( Input.GetKeyDown( KeyCode.Minus ) )
    {
      CharacterEvent evt = new CharacterEvent( CharacterEventEnum.DebugSubAffinity, moveTransform.position, Time.time, this, this );
      BroadcastEvent( evt, Global.Instance.GlobalSightDistance );
    }

    if( Input.GetKeyDown( KeyCode.H ) )
      Global.Instance.cameraController.Shake();
        
    /*if( Input.GetButtonDown( "Mode" ) )
    {
      PushState( "Construction" );
    }*/

    if( Input.GetButtonDown( "Action" ) )
    {
      selected = moveTransform;
      acts.Clear();
      acts.Add( this );
      List<string> actions = new List<string>();
      actions.Add( "mode build" );
      actions.Add( "follow all" );
      actions.Add( "unfollow all" );
      actions.Add( "sleep" );
      PushState( "ContextMenu" );
      Global.Instance.ContextMenu.Show( actions.ToArray() );
    }
    else
    if( HeldObject != null )
    {
      InteractIndicator.SetActive( false );
      if( Input.GetButtonDown( "Interact" ) )
      {
        DropObject();
        lastSelected = null;
      }
    }
    else
    if( Physics.CheckSphere( moveTransform.position, ActionRadius, LayerMask.GetMask( Global.Instance.gameData.InteractLayers ), qti ) )
    {
      selected = null;
      Vector3 start = moveTransform.position;
      start.y = Global.Instance.GlobalRaycastY;
      RaycastHit info = new RaycastHit();
      // line of sight
      string[] BlockingLineOfSightLayers = new string[]{ "Default" };
      if( Physics.Raycast( new Ray( start, moveTransform.forward ), out info, ActionRadius, LayerMask.GetMask( Global.Instance.gameData.InteractLayers ) | LayerMask.GetMask( BlockingLineOfSightLayers ), qti ) )
      {
        // prevent selecting objects through walls
        if( !( ( info.transform.gameObject.layer & LayerMask.GetMask( BlockingLineOfSightLayers ) ) > 0 ) )
        {
          List<IAction> iactions = new List<IAction>( info.transform.GetComponents<IAction>() );
          if( iactions.Count > 0 )
            selected = info.transform;
        }
      }
      
      if( selected == null )
      {
        // if no object is in direct line of sight, choose highest dot
        int count = Physics.OverlapSphereNonAlloc( start, ActionRadius, SensorColliders, LayerMask.GetMask( Global.Instance.gameData.InteractLayers ), qti );
        Collider closest = null;
        float largestDot = 0f;
        for( int i = 0; i < count; i++ )
        {
          Collider collider = SensorColliders[ i ];
          if( CanSeeObject( collider.gameObject, false ) )
          {
            Vector3 flat = collider.transform.position - moveTransform.position;
            flat.y = 0;
            float dot = Vector3.Dot( moveTransform.forward, flat.normalized );
            if( dot > ActionRaycastMinimumDot && dot > largestDot && collider.transform != moveTransform )
            {
              closest = collider;
              largestDot = dot;
            }
          }
        }
        if( closest != null )
        {
          //List<IAction> iactions = new List<IAction>( selected.GetComponents<IAction>() );
          selected = closest.transform;
        }
      }

      if( selected == moveTransform )
      {
        Debug.LogWarning( "tried to select self", this );
        return;
      }

      if( selected == null || Vector3.Distance( selected.transform.position, moveTransform.position ) > ActionRadius )
      {
        InteractIndicator.SetActive( false );
        lastSelected = null;
        if( Global.Instance.ContextMenu.gameObject.activeSelf )
          Global.Instance.ContextMenu.Hide();
        selectedCharacter = null;
        HidePositionalText();
        Global.Instance.HideCharacterInfo();
      }
      else
      {
        acts = new List<IAction>( selected.GetComponents<IAction>() );
        if( acts.Count > 0 )
        {
          // default indicator transform
          InteractIndicator.transform.position = selected.position + Vector3.up * Global.Instance.GlobalSpriteOnGroundY;
          InteractIndicator.transform.rotation = Quaternion.identity;
          InteractIndicator.transform.localScale = Vector3.one; 

          // only let a single IAction class position the indicator
          IAction firstAction = acts[ 0 ];
          if( firstAction != lastSelected )
          {
            lastSelected = firstAction;
            HidePositionalText();
            selectedCharacter = null;
            Global.Instance.HideCharacterInfo();
          }
          if( !InteractIndicator.activeSelf )
            InteractIndicator.SetActive( true );

          Character chr = selected.GetComponent<Character>();
          if( chr != null )
          {
            selectedCharacter = chr;
            // F8CF62
            Color gold = new Color( (float)0xF8 / 255f, (float)0xCF / 255f, (float)0x62 / 255f );
            ShowPositionalText( selected.position, selectedCharacter.CharacterName, selectedCharacter.ball.Sphere.radius, gold );
            Global.Instance.ShowCharacterInfo( selectedCharacter );
          }

          // provide context-sensitive actions to the player
          List<string> actions = new List<string>();
          // tags have categorical actions associated with them. Context is not considered, though.
          Dictionary<Tag,string> actionTagMap = new Dictionary<Tag, string>();
          if( WithinHoldRange( selected ) )
            actionTagMap[ Tag.Swag ] = "carry";
          actionTagMap[ Tag.Weapon ] = "keep";
          actionTagMap[ Tag.Food ] = "eat";
          foreach( var act in acts )
          {
            MonoBehaviour mono = (MonoBehaviour)act;
            Tags tags = mono.GetComponent<Tags>();
            if( tags != null )
            {
              // based on order of keys in dictionary
              foreach( var pair in actionTagMap )
                if( tags.HasTag( pair.Key ) && !actions.Contains( pair.Value ) )
                  actions.Add( pair.Value );
            }
          }

          ActionData actionData = new ActionData();
          actionData.instigator = this;
          actionData.actions = actions;
          actionData.indicator = InteractIndicator.transform;
          firstAction.GetActionContext( ref actionData );


          if( Input.GetButtonDown( "Interact" ) )
          {
            if( actions.Count > 0 )
            {
              PushState( "ContextMenu" );
              ShowContextMenu( selected, actions.ToArray() );

              if( selectedCharacter == null )
                HidePositionalText();
              else
                CenterContextText( selectedCharacter.CharacterName );
            }
            else
            {
              // there is only one action to take
              foreach( var a in acts )
                a.OnAction( this );
            }
          }
        }

      }
    }
    else
    {
      InteractIndicator.SetActive( false );
    }

    if( Input.GetButtonDown( "Next" ) )
      SelectNextRightHand();
    
    if( Input.GetButtonDown( "Previous" ) )
      SelectNextLeftHand();

    if( Input.GetButtonDown( "Shield" ) || Input.GetAxis( "Shield" ) > 0.5f )
      ShieldOn();
    
    if( Input.GetButtonUp( "Shield" ) || Input.GetAxis( "Shield" ) < 0.5f )
      ShieldOff();

  }

  void HidePositionalText()
  {
    Global.Instance.InteractCharacterName.gameObject.SetActive( false );
  }

  public void ShowPositionalText( Vector3 worldPosition, string text, float vertical, Color color )
  {
    Global.Instance.InteractCharacterName.gameObject.SetActive( true );
    Global.Instance.InteractCharacterName.color = color;
    Global.Instance.InteractCharacterName.text = text;
    Vector3 worldPos = worldPosition + Vector3.up * vertical;
    // This is accurate with arbitrary resolutions and aspect ratios because of the Canvas Scaler (set to Expand)
    Vector3 view = Camera.main.WorldToViewportPoint( worldPos );
    view.y += 0.1f;
    view.x *= Screen.width / Global.Instance.UI.transform.localScale.x;
    view.y *= Screen.height / Global.Instance.UI.transform.localScale.y;
    Global.Instance.InteractCharacterName.rectTransform.anchoredPosition = view;
  }

  public void CenterContextText( string text )
  {
    Global.Instance.InteractCharacterName.gameObject.SetActive( true );
    Vector3 view = new Vector3( 0.5f, 0.65f, 0 );
    view.x *= Screen.width / Global.Instance.UI.transform.localScale.x;
    view.y *= Screen.height / Global.Instance.UI.transform.localScale.y;
    Global.Instance.InteractCharacterName.rectTransform.anchoredPosition = view;
  }

  void UpdatePlayerMovementInput()
  {
    moveRelative.y = Mathf.Clamp( Input.GetAxis( "Vertical" ), -1f, 1f );
    moveRelative.x = Mathf.Clamp( Input.GetAxis( "Horizontal" ), -1, 1 );
    Vector3 forward = Global.Instance.cameraController.GetForwardDirection().normalized;
    Vector3 right = Global.Instance.cameraController.transform.right;
    right.y = 0;
    right.Normalize();
    MoveDirection = Vector3.zero;
    MoveDirection += forward * moveRelative.y;
    MoveDirection += right.normalized * moveRelative.x;
    moveRelative = Vector2.zero;

    if( PlayerStrafeMode )
      FaceDirection = forward;
    else if( MoveDirection.sqrMagnitude > 0 )
        FaceDirection = MoveDirection;
    
    if( FaceDirection.sqrMagnitude > 0 )
      moveTransform.rotation = Quaternion.LookRotation( FaceDirection, Vector3.up );

    /*if( Input.GetButton( "Sprint" ) || Input.GetAxis( "Sprint" ) > 0.5f )
      CurrentMoveSpeed = SprintSpeed;
    else
      CurrentMoveSpeed = WalkSpeed;*/
    CurrentMoveSpeed = SprintSpeed;
  }

  void UpdatePlayerWeaponInput()
  {
    if( Input.GetButton( "Fire1" ) )
    {
      if( CurrentWeaponPrefab != null )
      {
        if( Time.time - lastFire > CurrentWeaponPrefab.Interval / FireIntervalDividen )
        {
          lastFire = Time.time;
          Fire( moveTransform.forward );
        }
      }
    }
  }


  public class ContextMenuLayer
  {
    public Transform transform;
    public string[] actions;
  }

  List<ContextMenuLayer> ContextMenuLayers = new List<ContextMenuLayer>();

  public void ShowContextMenu( Transform contextObject, string[] actions, bool addLayer = true )
  {
    if( !isPlayerControlled )
      return;
    acts = new List<IAction>( contextObject.GetComponents<IAction>() );
    Global.Instance.ContextMenu.Show( actions );
    if( addLayer )
    {
      ContextMenuLayer cml = new ContextMenuLayer();
      cml.transform = contextObject;
      cml.actions = actions;
      ContextMenuLayers.Add( cml );
    }
  }

  public void HideContextMenu()
  {
    if( CurrentState.Name == "ContextMenu" )
      PopState();
  }

  void PushContextMenu()
  {
    CurrentMoveSpeed = 0;
    Global.Instance.ShowControlInfo( "contextmenu" );
    ContextMenuLayers.Clear();
  }

  void PopContextMenu()
  {
    Global.Instance.ContextMenu.Hide();
    Global.Instance.HideControlInfo();
  }

  void UpdateContextMenu()
  {
    if( selected == null || Input.GetButtonDown( "Interact" ) || Input.GetButtonDown( "Action" ) )
    {
      PopState();
    }

    if( Input.GetButtonDown( "Previous" ) )
    {
      if( ContextMenuLayers.Count > 1 )
      {
        ContextMenuLayers.RemoveAt( ContextMenuLayers.Count - 1 );
        ShowContextMenu( ContextMenuLayers[ ContextMenuLayers.Count - 1 ].transform, ContextMenuLayers[ ContextMenuLayers.Count - 1 ].actions, false );
      }
    }

    if( selectedCharacter != null )
      InteractIndicator.transform.position = selectedCharacter.transform.position;

    if( Input.GetButtonDown( "Directional Up" ) )
    {
      if( Global.Instance.ContextMenu.CurrentActions.Count > 0 )
      {
        foreach( var a in acts )
          a.OnAction( this, Global.Instance.ContextMenu.CurrentActions[ 0 ].id );
      }
    }
    if( Input.GetButtonDown( "Directional Right" ) )
    {
      if( Global.Instance.ContextMenu.CurrentActions.Count > 1 )
      {
        foreach( var a in acts )
          a.OnAction( this, Global.Instance.ContextMenu.CurrentActions[ 1 ].id );
      }
    }
    if( Input.GetButtonDown( "Directional Down" ) )
    {
      if( Global.Instance.ContextMenu.CurrentActions.Count > 2 )
      {
        foreach( var a in acts )
          a.OnAction( this, Global.Instance.ContextMenu.CurrentActions[ 2 ].id );
      }
    }
    if( Input.GetButtonDown( "Directional Left" ) )
    {
      if( Global.Instance.ContextMenu.CurrentActions.Count > 3 )
      {
        foreach( var a in acts )
          a.OnAction( this, Global.Instance.ContextMenu.CurrentActions[ 3 ].id );
      }
    }
  }




  DiageticUI dui;

  public void StartDiageticUI( DiageticUI ui )
  {
    dui = ui;
    PushState( "DiageticMenu" );
    CurrentMoveSpeed = 0;

    Global.Instance.UI.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject( dui.InitiallySelected );
    Global.Instance.EnableRaycaster( false );
    dui.raycaster.enabled = true;
    // look at the menu
    Global.Instance.cameraController.Automated = true;
    Global.Instance.cameraController.LerpTo( ui.CameraLerpTarget );
    Cursor.lockState = CursorLockMode.None;
  }

  void PushDiageticMenu()
  {
  }

  void UpdateDiageticMenu()
  {
    if( Input.GetButtonDown( "Interact" ) )
    {
      PopState();
    }
  }

  void PopDiageticMenu()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Global.Instance.cameraController.Lerp.enabled = false;
    Global.Instance.cameraController.Automated = false;
    // avoid selecting things after we've left the menu
    Global.Instance.UI.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject( null );
    Global.Instance.EnableRaycaster( true );
    dui.raycaster.enabled = false;
  }

}