using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectReplacement
{
  public string oldKey;
  public string newKey;
}

[CreateAssetMenu]
public class GameData : ScriptableObject 
{
  // necessary for characters to spawn their own prefab without Instantiating themselves.
  public List<ObjectReplacement> objectReplacementList;
  public Dictionary<string,string> replacements = new Dictionary<string,string>();
   
  [Header("Character")]
  public GameObject CharacterPrefab;


  [Header("Layers")]
  public string[] LowPrioritySensorLayers = new string[] {
    "Deadbody"
  };

  public string[] TopPrioritySensorLayers = new string[] {
    "Character",
    "Notable",
    "Interact",
    "Carry",
    "Weapon"
  };

  public string[] InteractLayers = new string[] {
    "Interact",
    "Notable",
    "Character",
    "Carry",
    "Deadbody"
  };

  public string[] IncludeLayers = new string[] {
    "Default",
    "CameraHide",
    "Character",
    "Carry",
    "Interact",
    "Notable"
  };


  public void Initialize()
  {
    // init object name-replacements
    replacements.Clear();
    foreach( var r in objectReplacementList )
      replacements.Add( r.oldKey, r.newKey );
  }

}

