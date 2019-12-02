using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLink : MonoBehaviour
{
  public enum NodeLinkType
  {
    None,
    Road,
    Decoration

  }
  public NodeLinkType linkType;

  public GameObject[] AllowedToLink;
}
