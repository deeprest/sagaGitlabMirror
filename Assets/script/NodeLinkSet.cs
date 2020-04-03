using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A prefab cannot have a reference to itself! It gets serialized internally as a local reference.
// So I keep a list of prefabs in a ScriptedObject (NodeLinkSet).
[CreateAssetMenu]
public class NodeLinkSet : ScriptableObject
{
  public GameObject[] AllowedToLink;
}