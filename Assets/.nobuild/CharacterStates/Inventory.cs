using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class Character
{
  [Header("Inventory")]
  public bool CanPickupItems = true;
  public AnimationCurve AddTranslateCurve;
  public float ItemScale = 0.2f;

  public Transform RightHandMount;
  public Transform RightHandItemMount;
  public float ScaleItemsRight = -.2f;
  public int rightSelectedIndex = 0;
  public InventoryItem RightHandItem;
  public GameObject RightHandSwag;
  public Vector3 RightHandLocal = new Vector3( 0.12f, 0.0f, -0.04f );

  public Transform LeftHandItemMount;
  public float ScaleItemsLeft = 0.2f;
  public int leftSelectedIndex = 0;
  public InventoryItem LeftHandItem;

 

  [Serialize]
  public List<string> swag = new List<string>();

  public Animation RightHandAnimation;

  // keep, acquire, collect, possess, store on person
  public bool AcquireObject( GameObject obj )
  {
    CarryObject swg = obj.GetComponent<CarryObject>();
    if( swg != null && !swg.IsHeld )
    {
      if( swg.Item != null )
      {
        if( CanAcquireItem( swg.Item ) )
        {
          GameObject go = GameObject.Instantiate( swg.Item.gameObject, moveTransform.position + Vector3.up * 0.3f, Quaternion.identity, moveTransform );
          go.name = swg.Item.name;
          AcquireItem( go.GetComponent<InventoryItem>() );
          GameObject.Destroy( obj );
          return true;
        }
      }
    }
    InventoryItem item = obj.GetComponent<InventoryItem>();
    if( item != null )
    {
      if( CanAcquireItem( item ) )
      {
        AcquireItem( item );
        return true;
      }
    }
    return false;
  }

  public bool CanAcquireItem(  InventoryItem item )
  {
    if( (item.Type=="weapon"||item.Type=="shield") && HasItem( item ) )
      return false;
    return true;
  }

  public void AcquireItem( InventoryItem item )
  {
    LerpToTarget lerp = item.GetComponent<LerpToTarget>();
    lerp.targetTransform = item.transform.parent;
    lerp.duration = 1f; 
    lerp.Continuous = false;
    lerp.Scale = true;
    lerp.targetScale = ItemScale;
    lerp.lerpType = LerpToTarget.LerpType.Curve;
    lerp.translateCurve = AddTranslateCurve;
    lerp.enabled = true;
    lerp.OnLerpEnd = delegate{
      PositionItems();
    };

    if( item.Type == "weapon" )
    {
      item.transform.parent = RightHandItemMount;
      if( RightHandItemMount.childCount == 1 )
        SelectRightHand( 0 );
    }
    if( item.Type == "shield" )
    {
      item.transform.parent = LeftHandItemMount;
      if( LeftHandItemMount.childCount == 1 )
        SelectLeftHand( 0 );
    }
    if( item.Type == "health" )
    {
      Health += 1;
    }

    item.transform.localRotation = Quaternion.LookRotation( item.InInventoryForward, item.InInventoryUp );
  }

  public void SelectRightHand( int index )
  {
    if( RightHandItemMount.childCount == 0 )
    {
      if( RightHandSwag!=null)
        GameObject.Destroy( RightHandSwag );
      return;
    }
    rightSelectedIndex = index % RightHandItemMount.childCount;
    RightHandItem = RightHandItemMount.GetChild( rightSelectedIndex ).GetComponent<InventoryItem>();
    PositionItems();
    if( RightHandItem != null )
    {
      if( RightHandItem.Weapon != null )
        CurrentWeaponPrefab = RightHandItem.Weapon;
      else
        CurrentWeaponPrefab = null;

      if( RightHandSwag!=null)
        GameObject.Destroy( RightHandSwag );

      CarryObject.Limit.EnforceUpper = false;
      RightHandSwag = GameObject.Instantiate( RightHandItem.CarryObjectPrefab.gameObject );
      CarryObject.Limit.EnforceUpper = true;
      Destroy( RightHandSwag.GetComponent<SerializedObject>() );
      Rigidbody rb = RightHandSwag.GetComponent<Rigidbody>();
      if( rb != null )
        Destroy( rb );
      Collider collider = RightHandSwag.GetComponent<Collider>();
      if( collider != null )
        collider.enabled = false;
      RightHandSwag.transform.parent = RightHandMount;
      //RightHandSwag.transform.localRotation = Quaternion.identity;
      RightHandSwag.transform.localPosition = Vector3.zero;
      RightHandSwag.transform.localScale = Vector3.one;
//      RightHandSwag.transform.localPosition = RightHandLocal + RightHandItem.SwagPrefab.EquippedOffset;
      RightHandSwag.transform.localRotation = Quaternion.LookRotation( RightHandItem.CarryObjectPrefab.EquippedForward, RightHandItem.CarryObjectPrefab.EquippedUp );
      //RightHandAnimation.Play( "melee-0-idle" );
    }
  }

  public void SelectNextRightHand()
  {
    SelectRightHand( rightSelectedIndex + 1 );
  }

  public void SelectLeftHand( int index )
  {
    if( LeftHandItemMount.childCount == 0 )
      return;
    leftSelectedIndex = index % LeftHandItemMount.childCount;
    LeftHandItem = LeftHandItemMount.GetChild( leftSelectedIndex ).GetComponent<InventoryItem>();
    PositionItems();
    if( LeftHandItem != null && LeftHandItem.Shield != null )
    {
      if( ShieldTransform != null )
        GameObject.Destroy( ShieldTransform.gameObject );
      GameObject go = GameObject.Instantiate( LeftHandItem.Shield.gameObject, ShieldInactiveLocal, moveTransform.rotation, moveTransform );
      ShieldTransform = go.transform;
      ShieldTransform.localPosition = ShieldInactiveLocal;
      Physics.IgnoreCollision( ball.Sphere, ShieldTransform.GetComponent<Collider>() );
    }
  }

  public void SelectNextLeftHand()
  {
    SelectLeftHand( leftSelectedIndex + 1 );
  }

  public void PositionItems()
  {
    float unit = 1.0f / 5.0f;
    for( int i = 0; i < RightHandItemMount.childCount; i++ )
    {
      int index = ( rightSelectedIndex + i ) % RightHandItemMount.childCount;
      LerpToTarget lerp = RightHandItemMount.GetChild( index ).GetComponent<LerpToTarget>();
      lerp.lerpType = LerpToTarget.LerpType.Linear;
      lerp.duration = 0.2f;
      lerp.targetTransform = RightHandItemMount;
      lerp.localOffset = new Vector3( unit * (float)i, 0, 0 ) * ScaleItemsRight;
      lerp.OnLerpEnd = null;
      lerp.enabled = true;

      #if UNITY_EDITOR
      InventoryItem item = RightHandItemMount.GetChild( index ).GetComponent<InventoryItem>();
      InventoryItem prefab = item.CarryObjectPrefab.Item;
      RightHandItemMount.GetChild( index ).localRotation = Quaternion.LookRotation( prefab.InInventoryForward, prefab.InInventoryUp );
      #endif
    }
    for( int i = 0; i < LeftHandItemMount.childCount; i++ )
    {
      int index = ( leftSelectedIndex + i ) % LeftHandItemMount.childCount;
      LerpToTarget lerp = LeftHandItemMount.GetChild( index ).GetComponent<LerpToTarget>();
      lerp.lerpType = LerpToTarget.LerpType.Linear;
      lerp.duration = 0.2f;
      lerp.targetTransform = LeftHandItemMount;
      lerp.localOffset = new Vector3( unit * (float)i, 0, 0 ) * ScaleItemsLeft;
      lerp.OnLerpEnd = null;
      lerp.enabled = true;
    }
  }

  public void DropAllItems()
  {
    DropAllFromMount( RightHandItemMount );
    DropAllFromMount( LeftHandItemMount );
  }

  void DropAllFromMount( Transform mount )
  {
    for( int i = 0; i < mount.childCount; i++ )
    {
      if( mount.GetChild( i ) == null )
        continue;
      InventoryItem item = mount.GetChild( i ).GetComponent<InventoryItem>();
      if( item == null )
        continue;
      GameObject prefab = item.CarryObjectPrefab.gameObject;
      Vector3 pos = mount.position;
      pos.y = Global.Instance.GlobalSpriteOnGroundY;
      GameObject go = GameObject.Instantiate( prefab, pos, prefab.transform.rotation, transform.parent );
      go.name = prefab.name;
      go.transform.rotation = Quaternion.LookRotation( item.CarryObjectPrefab.GroundForward, item.CarryObjectPrefab.GroundUp );

    }
    for( int i = 0; i < mount.childCount; i++ )
      GameObject.Destroy( mount.GetChild( i ).gameObject );
  }

  public bool HasItem( InventoryItem item )
  {
    List<InventoryItem> items = new List<InventoryItem>( GetComponentsInChildren<InventoryItem>() );
    InventoryItem found = items.Find( x => x.gameObject.name.ToLower() == item.gameObject.name.ToLower() );
    return found != null;
  }

  public bool HasItemType( string type )
  {
    List<InventoryItem> items = new List<InventoryItem>( GetComponentsInChildren<InventoryItem>() );
    InventoryItem found = items.Find( x => x.Type.ToLower() == type.ToLower() );
    return found != null;
  }



  public void ReadyRightHand()
  {
    if( RightHandItem == null )
      return;
    if( RightHandItem.Weapon.type == Attack.Type.Melee )
    {
      //RightHandAnimation.Blend( "melee-0-ready", 1f, 0.5f );
      //RightHandAnimation.Play( "melee-0-ready" );
    }
  }

  public void BlendToAnimation( string name )
  {
    RightHandAnimation.Blend( name, 1f, 0.5f );
  }



}