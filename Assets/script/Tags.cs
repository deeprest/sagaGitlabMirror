using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tag
{
  None,
  Red,
  Blue,
  Swag,
  Attack,
  Dead,
  Monster,
  Combustible,
  Food,
  Weapon,
  Fire,
  Deer
}

public class Tags : MonoBehaviour 
{
  public List<Tag> tags;

  public bool HasTag( Tag tag )
  {
    return tags.Contains( tag );
  }

  public bool HasAnyTag( Tag[] tagList )
  {
    foreach( var tag in tagList )
      if( tags.Contains( tag ) )
        return true;
    return false;
  }

  public static bool HasTag( GameObject go, Tag tag )
  {
    return (GetString(tag) == go.tag);
  }
    
  public static string GetString( Tag tag )
  {
    string[] names = System.Enum.GetNames (typeof(Tag));
    Tag[] values = (Tag[])System.Enum.GetValues (typeof(Tag));
    for (int i = 0; i < values.Length; i++)
    {
      if (tag == values [i])
      return names[i];
    }
    return "";
  }
  /*
  public static Tag GetTag( string tag )
  {
    string[] names = System.Enum.GetNames (typeof(Tag));
    Tag[] values = (Tag[])System.Enum.GetValues (typeof(Tag));
    Tag found = Tag.None;
    for (int i = 0; i < names.Length; i++)
    {
      if (tag == names [i]) {
        found = values [i];
        break;
      }
    }
    return found;
  }
  */
}
  






