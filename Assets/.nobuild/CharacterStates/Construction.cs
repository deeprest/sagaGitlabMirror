using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecalSet
{
  public string name;
  public Sprite Diffuse;
  public Sprite Overlay;
  public Sprite BurnMask;
}

public partial class Character
{
  const float BuildDistance = 0.8f;
  Vector3 ModifyExtents = new Vector3( 0.4f, 4, 0.4f );
  const float fudge = 0.001f;

  public enum Mode : int
  {
    Menu,
    Build,
    Modify,
    Block
  }

  [Header( "Construction" )]
  public Mode BuildMode = Mode.Build;

  public int CurrentPrefabIndex = 0;
  GameObject CurrentBuildPrefab;
  public GameObject CurrentBuildObject;
  public Sprite DecalSprite;
  public int DecalIndex = 0;
  Vector3 snap;
  Vector3 lastSnapPos;
  Vector3 rotationAngles;
  Dictionary<Renderer,List<Material>> cachedMaterial = new Dictionary<Renderer, List<Material>>();
  List<Collider> disabledColliders = new List<Collider>();
  List<Rigidbody> constrainedRigidbodies = new List<Rigidbody>();
  List<MonoBehaviour> disabledBehaviours = new List<MonoBehaviour>();
  GameObject PreviouslySelectedObject = null;
  int SelectIndex = 0;
  float slidelast;
  float slideInterval = 0.15f;

  void ChangeMode( Mode newmode )
  {
    BuildMode = newmode;
    PopBuildMaterial();
    if( CurrentBuildObject != null )
      Destroy( CurrentBuildObject );
    CurrentBuildObject = null;
    snap = Vector3.zero;
    lastSnapPos = Vector3.zero;

    if( BuildMode == Mode.Build )
    {
      Global.Instance.ShowControlInfo( "construction" );
      Global.Instance.ConstructionLibrary.Show();
//      World.Instance.SelectedGroundDecal.transform.parent.gameObject.SetActive( false );
      Global.Instance.BlockSelector.SetActive( false );
    }

    if( BuildMode == Mode.Modify )
    {
      Global.Instance.ShowControlInfo( "modify" );
      Global.Instance.ConstructionLibrary.Hide();
//      World.Instance.SelectedGroundDecal.transform.parent.gameObject.SetActive( false );
      Global.Instance.BlockSelector.SetActive( true );
    }

    if( BuildMode == Mode.Block )
    {
      Global.Instance.ShowControlInfo( "ground" );
      Global.Instance.ConstructionLibrary.Hide();
//      World.Instance.SelectedGroundDecal.transform.parent.gameObject.SetActive( true );
      Global.Instance.BlockSelector.SetActive( true );
    }

  }

  void PushConstruction()
  {
    CurrentBuildPrefab = Global.Instance.BuildPrefabs[ CurrentPrefabIndex ];
    CurrentMoveSpeed = WalkSpeed;
    // unhide roofs
    Global.Instance.cameraController.EnableHide = false;
    //World.Instance.ConstructionLibrary.Show();
    //World.Instance.ShowControlInfo("construction");

    BuildMode = Mode.Menu;
    //ChangeMode( Mode.Build );

    PushState( "ContextMenu" );
    ShowContextMenu( moveTransform, new string[] {
      "build quit",
      "build build",
      "build modify",
      "build block"
    } );
    //World.Instance.ConstructionLibrary.Hide();
    Global.Instance.HideControlInfo();
  }

  void PopConstruction()
  {
    PopBuildMaterial();
    if( CurrentBuildObject != null )
      GameObject.DestroyImmediate( CurrentBuildObject );
    snap = Vector3.zero;
    lastSnapPos = Vector3.zero;

    // hide roofs
    Global.Instance.cameraController.EnableHide = true;
    Global.Instance.ConstructionLibrary.Hide();
    Global.Instance.HideControlInfo();
    Global.Instance.BlockSelector.SetActive( false );
//    World.Instance.SelectedGroundDecal.transform.parent.gameObject.SetActive( false );
  }

  void UpdateConstruction()
  {
    UpdatePlayerMovementInput();
    UpdatePlayerWeaponInput();

    if( BuildMode == Mode.Menu )
    {
      // this mode is reserved for the context menu. If the context menu state is popped and
      // execution reaches this point without changing the build mode, then leave construction model=.
      PopState();
      return;
    }

    if( Input.GetButtonDown( "Action" ) )
    {
      PushState( "ContextMenu" );
      ShowContextMenu( moveTransform, new string[] {
        "build quit",
        "build build",
        "build modify",
        "build block"
      } );
      Global.Instance.ConstructionLibrary.Hide();
      Global.Instance.HideControlInfo();
      return;
    }

    if( BuildMode == Mode.Build )
    {
      snap = SnapPos( moveTransform.position + moveTransform.forward * Global.Instance.BuildDistance );
      bool isNewCell = false;
      if( Vector3.Distance( snap, lastSnapPos ) > 1.5f )
      {
        lastSnapPos = snap;
        isNewCell = true;
      }

      if( CurrentBuildObject == null )
      { 
        CurrentBuildObject = Global.Instance.Spawn( CurrentBuildPrefab, snap, Quaternion.Euler( rotationAngles ), null, false, true );
        CurrentBuildObject.name = CurrentBuildPrefab.name;
        // do not allow serialization until this object is placed in level
        SerializedObject[] sos = CurrentBuildObject.GetComponentsInChildren<SerializedObject>();
        foreach( var so in sos )
          so.serialize = false;

        PushBuildMaterial( CurrentBuildObject, Global.Instance.BuildMaterial );
        disabledBehaviours = new List<MonoBehaviour>();
        MonoBehaviour[] mbs = CurrentBuildObject.GetComponentsInChildren<MonoBehaviour>();
        foreach( var c in mbs )
        {
          if( c == null )
            continue;
          if( c.GetType() == typeof(Ball) )
          {
            // exclude certain scripts
            continue;
          }
          if( c.enabled )
          {
            disabledBehaviours.Add( c );
            c.enabled = false;
          }
        }

        disabledColliders = new List<Collider>();
        Collider[] cols = CurrentBuildObject.GetComponentsInChildren<Collider>();
        foreach( var c in cols )
        {
          if( c.enabled )
          {
            disabledColliders.Add( c );
            c.enabled = false;
          }
        }
        // prevent bodies from falling through the ground before spawning the object
        constrainedRigidbodies = new List<Rigidbody>();
        Rigidbody[] rbs = CurrentBuildObject.GetComponentsInChildren<Rigidbody>();
        foreach( var rb in rbs )
        {
          rb.constraints = RigidbodyConstraints.FreezePosition;
          constrainedRigidbodies.Add( rb );
        }
      }
      CurrentBuildObject.transform.position = snap;

      if( Input.GetButton( "Next" ) )
      {
        if( Time.time - slidelast > slideInterval )
        {
          slidelast = Time.time;
          NextPrefab();
        }
      }

      if( Input.GetButton( "Previous" ) )
      {
        if( Time.time - slidelast > slideInterval )
        {
          slidelast = Time.time;
          PreviousPrefab();
        }
      }

      if( Input.GetButtonDown( "Rotate" ) )
      {
        rotationAngles.y += 90;
        CurrentBuildObject.transform.rotation = Quaternion.Euler( rotationAngles );
      }

      if( Input.GetButtonDown( "Interact" ) || ( Input.GetButton( "Interact" ) && isNewCell ) )
      {
        PopBuildMaterial();
        foreach( var c in disabledBehaviours )
        {
          if( c == null )
            continue;
          c.enabled = true;
        }
        disabledBehaviours.Clear();

        foreach( var c in disabledColliders )
        {
          if( c == null )
            continue;
          c.enabled = true;
        }
        disabledColliders.Clear();

        foreach( var rb in constrainedRigidbodies )
        {
          if( rb == null )
            continue;
          rb.constraints = RigidbodyConstraints.None;
        }
        constrainedRigidbodies.Clear();

        //World.Instance.AssignTeam( CurrentBuildObject, Team.Tag );

        SerializedObject[] sos = CurrentBuildObject.GetComponentsInChildren<SerializedObject>();
        foreach( var so in sos )
          so.serialize = true;

        CurrentBuildObject = null;
      }
    }
    else
    if( BuildMode == Mode.Modify )
    {
      if( Input.GetButtonDown( "Next" ) )
      {
        SelectIndex++;
      }
      if( Input.GetButtonDown( "Previous" ) )
      {
        SelectIndex--;
      }

      Vector3 snap = SnapPos( transform.position + transform.forward * Global.Instance.BuildDistance );
      bool isNewCell = false;
      if( Vector3.Distance( snap, lastSnapPos ) > 1.5f )
      {
        isNewCell = true;
      }
      if( isNewCell )
      {
        lastSnapPos = snap;
        SelectIndex = 0;
      }
      Global.Instance.BlockSelector.transform.position = snap;
      
        List<Collider> colliders = new List<Collider>( Physics.OverlapBox( snap, ModifyExtents, Quaternion.identity, LayerMask.GetMask( Global.Instance.gameData.IncludeLayers ), QueryTriggerInteraction.Collide ) );
      // ignore player collider
      if( colliders.Contains( ball.Sphere ) )
        colliders.Remove( ball.Sphere );
      if( colliders.Count > 0 )
      {
        colliders.Sort( delegate(Collider x, Collider y )
        {
          if( x.GetInstanceID() < y.GetInstanceID() )
            return -1;
          else
            return 1;
        } );

        if( SelectIndex < 0 )
          SelectIndex = colliders.Count + SelectIndex;
        SelectIndex = SelectIndex % colliders.Count;
        GameObject ModifyObject = null;
        foreach( var c in colliders )
        {
          //SelectIndex = SelectIndex % colliders.Count;
          GameObject check = colliders[ SelectIndex ].gameObject;
          // do not modify player character
          if( check == gameObject )
            continue;
          ModifyObject = check;
          break;
        }
        if( ModifyObject == null )
          return;

        // get the root level object
        SerializedObject so = ModifyObject.GetComponentInParent<SerializedObject>();
        if( so == null )
          ModifyObject = ModifyObject.transform.root.gameObject;
        else
          ModifyObject = so.gameObject;


        if( ModifyObject != PreviouslySelectedObject )
        {
          PopBuildMaterial();
          if( ModifyObject != null )
            PushBuildMaterial( ModifyObject, Global.Instance.DeleteMaterial );
          PreviouslySelectedObject = ModifyObject;
        }

        if( Input.GetButtonDown( "Interact" ) )
        {
          PopBuildMaterial();
          GameObject.Destroy( ModifyObject );
        }

        if( Input.GetButtonDown( "Rotate" ) )
        {
          if( ModifyObject != null )
          {
            ModifyObject.transform.Rotate( new Vector3( 0, 90, 0 ), Space.Self );
          }
        }
      }
      else
      {
        PopBuildMaterial();
        PreviouslySelectedObject = null;
      }

    }
    else
    if( BuildMode == Mode.Block )
    {
      snap = SnapPos( moveTransform.position + moveTransform.forward * Global.Instance.BuildDistance );
      Global.Instance.BlockSelector.transform.position = snap;

      if( Input.GetButtonDown( "Next" ) )
      {
        DecalIndex = Mathf.Max( ++DecalIndex % Global.Instance.Decals.Length, 0 );
        DecalSprite = Global.Instance.Decals[ DecalIndex ];
        lastSnapPos = Vector3.zero;
      }
      if( Input.GetButtonDown( "Previous" ) )
      {
        DecalIndex = Mathf.Max( --DecalIndex % Global.Instance.Decals.Length, 0 );
        DecalSprite = Global.Instance.Decals[ DecalIndex ];
        lastSnapPos = Vector3.zero;
      }

      Global.Instance.SelectedGroundDecal.sprite = DecalSprite;

      if( Input.GetKey( KeyCode.V ) )
      {
        //World.Instance.PaintDecalTile( snap + Vector3.up, DecalSprite );
        Vector2Int coord = new Vector2Int( Mathf.FloorToInt( snap.x ), Mathf.FloorToInt( snap.z ) );
        foreach( var go in gameObject.scene.GetRootGameObjects() )
        {
          Zone zone = go.GetComponent<Zone>();
          if( zone != null )
          {
            zone.SetStructureBitOn( coord.x, coord.y, PixelBit.Door );
            zone.UpdateStructure( coord.x, coord.y, 1, 1 );
            break;
          }
        }
      }

      if( Input.GetKey( KeyCode.W ) )
      {
        //World.Instance.PaintDecalTile( snap + Vector3.up, DecalSprite );
        Vector2Int coord = new Vector2Int( Mathf.FloorToInt( snap.x ), Mathf.FloorToInt( snap.z ) );
        foreach( var go in gameObject.scene.GetRootGameObjects() )
        {
          Zone zone = go.GetComponent<Zone>();
          if( zone != null )
          {
            zone.SetStructureBitOn( coord.x, coord.y, PixelBit.Wall );
            zone.UpdateStructure( coord.x, coord.y, 1, 1 );
            break;
          }
        }
      }

      if( Input.GetKey( KeyCode.Space ) )
      {
        Vector2Int coord = new Vector2Int( Mathf.FloorToInt( snap.x ), Mathf.FloorToInt( snap.z ) );
        foreach( var go in gameObject.scene.GetRootGameObjects() )
        {
          Zone zone = go.GetComponent<Zone>();
          if( zone != null )
          {
            zone.SetStructureData( coord.x, coord.y, PixelBit.None );
            zone.UpdateStructure( coord.x, coord.y, 1, 1 );
            break;
          }
        }
      }

      if( Input.GetKey( KeyCode.B ) )
      {
        //World.Instance.PaintDecalTile( snap + Vector3.up, DecalSprite );
        Vector2Int coord = new Vector2Int( Mathf.FloorToInt( snap.x ), Mathf.FloorToInt( snap.z ) );
        foreach( var go in gameObject.scene.GetRootGameObjects() )
        {
          Zone zone = go.GetComponent<Zone>();
          if( zone != null )
          {
            zone.SetStructureBitOn( coord.x, coord.y, PixelBit.Building );
            zone.UpdateStructure( coord.x, coord.y, 1, 1 );
            break;
          }
        }
      }
    }
  }

  float GetExtents( GameObject go, float radius = 1f )
  {
    if( go != null )
    {
      Collider[] collider = go.GetComponentsInChildren<Collider>();
      foreach( var col in collider )
      {
        if( col.bounds.extents.magnitude > radius )
          radius = col.bounds.extents.magnitude;
      }
    }
    return radius;
  }

  Vector3 SnapPos( Vector3 buildPos )
  {
    return new Vector3( Mathf.Floor( buildPos.x ) + 0.5f, 0, Mathf.Floor( buildPos.z ) + 0.5f );
  }

  void SelectPrefab( int index )
  {
    CurrentPrefabIndex = index;
    Destroy( CurrentBuildObject );
    CurrentBuildObject = null;
    CurrentBuildPrefab = Global.Instance.BuildPrefabs[ CurrentPrefabIndex ];
    Global.Instance.ConstructionLibrary.Select( CurrentPrefabIndex );
  }

  void NextPrefab()
  {
    CurrentPrefabIndex = ( CurrentPrefabIndex + 1 + Global.Instance.BuildPrefabs.Count ) % Global.Instance.BuildPrefabs.Count;
    CurrentBuildPrefab = Global.Instance.BuildPrefabs[ CurrentPrefabIndex ];
    Destroy( CurrentBuildObject );
    CurrentBuildObject = null;
    Global.Instance.ConstructionLibrary.Select( CurrentPrefabIndex );
  }

  void PreviousPrefab()
  {
    CurrentPrefabIndex = ( CurrentPrefabIndex - 1 + Global.Instance.BuildPrefabs.Count ) % Global.Instance.BuildPrefabs.Count;
    CurrentBuildPrefab = Global.Instance.BuildPrefabs[ CurrentPrefabIndex ];
    Destroy( CurrentBuildObject );
    CurrentBuildObject = null;
    Global.Instance.ConstructionLibrary.Select( CurrentPrefabIndex );
  }

  void PushBuildMaterial( GameObject go, Material mat )
  {
    Renderer[] renderer = go.GetComponentsInChildren<Renderer>();
    foreach( var r in renderer )
    {
      if( !cachedMaterial.ContainsKey( r ) )
      {
        List<Material> mats = new List<Material>( r.materials.Length );
        for( int i = 0; i < r.materials.Length; i++ )
          mats.Add( r.materials[ i ] );
        cachedMaterial[ r ] = mats;
      }
      Material[] buildmats = new Material[r.materials.Length];
      for( int i = 0; i < r.materials.Length; i++ )
      {
        buildmats[ i ] = mat;
      }
      r.materials = buildmats;
    }
  }

  void PopBuildMaterial()
  {
    if( cachedMaterial.Count > 0 )
    {
      foreach( var rm in cachedMaterial )
      {
        if( rm.Key != null )
        {
          rm.Key.materials = cachedMaterial[ rm.Key ].ToArray();
        }
      }
      cachedMaterial.Clear();
    }
  }
   

}
