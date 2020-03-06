using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationVariant : MonoBehaviour
{
  public GameObject[] random;

  // Start is called before the first frame update
  public void Generate()
  {
    if( random.Length > 0 )
    {
      if( Application.isPlaying )
      {
        GameObject go = random[Random.Range( 0, random.Length )];
        if( go != null )
          Global.instance.Spawn( go, transform.position, Quaternion.identity );
      }
    }
  }


}
