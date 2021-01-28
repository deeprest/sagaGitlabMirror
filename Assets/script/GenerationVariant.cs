using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationVariant : MonoBehaviour
{
  public GameObject[] random;

  // Start is called before the first frame update
  public GameObject Generate()
  {
    if( random.Length > 0 )
    {
      if( Application.isPlaying )
      {
        GameObject prefab = random[Random.Range( 0, random.Length )];
        if( prefab != null )
          return Global.instance.Spawn( prefab, transform.position, Quaternion.identity );
      }
    }
    return null;
  }


}
