using UnityEngine;

public class GenerationVariant : MonoBehaviour
{
  public GameObject[] random;
  
  public GameObject Generate()
  {
    if( random.Length > 0 )
    {
      // do not spawn variants when generating in editor
      if( Application.isPlaying )
      {
        GameObject prefab = random[Random.Range( 0, random.Length )];
        if( prefab != null )
          return Global.instance.Spawn( prefab, transform.position, Quaternion.identity, transform.parent );
      }
    }
    return null;
  }
}
