using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANIMTEST : MonoBehaviour
{
[SerializeField] Animator body;
[SerializeField] Animator head;
    public bool sub0;

    // Start is called before the first frame update
    void Start()
    {
        body.Play("base");
        head.Play("SUB0");
      cached = sub0;
    }

  bool cached;
    // Update is called once per frame
    void Update()
    {
      if( cached != sub0 )
      {
        cached = sub0;
      }
       if( sub0 )
         head.Play("SUB0", -1, body.GetCurrentAnimatorStateInfo( 0 ).normalizedTime );
        else
         head.Play("SUB1", -1, body.GetCurrentAnimatorStateInfo( 0 ).normalizedTime);
      
    }
}
