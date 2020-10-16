using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Ability : ScriptableObject
{
  public bool IsActive;
  public Sprite icon;
  public Sprite cursor;
  public GameObject prefab;

  // transient
  [SerializeField] GameObject go;

  // when the ability is equipped
  public virtual void Equip( Transform parentTransform )
  {
    //Ability
    go = Instantiate( prefab, parentTransform.position, Quaternion.identity, parentTransform );
    go.transform.localRotation = Quaternion.identity;
  }

  // unequipped
  public virtual void Unequip()
  {
    Destroy( go );
  }

  public virtual void Activate( Vector2 origin, Vector2 aim )
  {
    IsActive = true;
  }

  public virtual void UpdateAbility( Pawn pawn ) { }

  public virtual void Deactivate()
  {
    IsActive = false;
  }

  public virtual void PreSceneTransition() { }
  public virtual void PostSceneTransition() { }
}