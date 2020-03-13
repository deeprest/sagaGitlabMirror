using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NodeLink : MonoBehaviour
{
  /*public enum NodeLinkType
  {
    None,
    Road,
    Decoration

  }
  public NodeLinkType linkType;*/

  // A prefab cannot have a reference to itself! It gets serialized internally as a local reference.
  // So I keep a list of prefabs in a ScriptedObject (NodeLinkSet).
  // https://forum.unity.com/threads/prefab-with-reference-to-itself.412240/
  public NodeLinkSet linkSet;
}

