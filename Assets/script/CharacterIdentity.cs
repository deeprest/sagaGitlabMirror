using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class CharacterIdentity : ScriptableObject
{
  public string CharacterName;
  public Sprite Icon;
  public AnimatorOverrideController animationController;

  public TextAsset TextAsset;
  
}