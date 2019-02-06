using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Rain : MonoBehaviour
{
  public Transform follow; 
  public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if( follow !=null )
      transform.position = follow.position + offset;
    }
}
