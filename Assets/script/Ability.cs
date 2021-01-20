using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Ability : ScriptableObject
{
  public Pawn pawn;
  public bool IsActive;
  public Sprite icon;
  public Sprite cursor;
  public GameObject prefab;

  // transient
  [SerializeField] GameObject go;
  Collider2D[] clds;

  public virtual void OnAcquire( Pawn pawn )
  {
    this.pawn = pawn;
  }
  // when the ability is equipped
  public virtual void Equip( Transform parentTransform )
  {
    //Ability
    go = Instantiate( prefab, parentTransform.position, Quaternion.identity, parentTransform );
    go.transform.localRotation = Quaternion.identity;
    
    clds = go.GetComponentsInChildren<Collider2D>();
    for( int i = 0; i < clds.Length; i++ )
    {
      pawn.IgnoreCollideObjects.Add( clds[i] );
      if( pawn.circle != null )
        Physics2D.IgnoreCollision( pawn.circle, clds[i], true );
      if( pawn.box != null )
        Physics2D.IgnoreCollision( pawn.box, clds[i], true );
    }
  }

  // unequipped
  public virtual void Unequip()
  {
    for( int i = 0; i < clds.Length; i++ )
    {
      pawn.IgnoreCollideObjects.Remove( clds[i] );
      if( pawn.circle != null )
        Physics2D.IgnoreCollision( pawn.circle, clds[i], false );
      if( pawn.box != null )
        Physics2D.IgnoreCollision( pawn.box, clds[i], false );
    }
    Destroy( go );
  }

  public virtual void Activate( Vector2 origin, Vector2 aim )
  {
    IsActive = true;
  }

  public virtual void UpdateAbility() { }

  public virtual void Deactivate()
  {
    IsActive = false;
  }

  public virtual void PreSceneTransition() { }
  public virtual void PostSceneTransition() { }
}