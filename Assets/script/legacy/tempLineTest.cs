using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class tempLineTest : MonoBehaviour
{
  public EdgeCollider2D a;
  public EdgeCollider2D b;

  void Update()
  {
    Debug.DrawLine( a.points[0], a.points[1], Color.grey );
    Debug.DrawLine( b.points[0], b.points[1], Color.grey );

    Vector2 intersection = Vector2.zero;
    if( Util.LineSegmentsIntersectionWithPrecisonControl( a.points[0], a.points[1], b.points[0], b.points[1], ref intersection) )
    {
      Debug.DrawLine( transform.position, intersection, Color.cyan );
    }

    if( Util.LineSegementsIntersect( a.points[0], a.points[1], b.points[0], b.points[1], ref intersection ) )
    {
      Debug.DrawLine( transform.position, intersection, Color.cyan );
    }

  }
}
