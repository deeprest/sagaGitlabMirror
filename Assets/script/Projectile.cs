using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  public Animator animator;
  public CircleCollider2D circle;

  public Weapon weapon;
  public Character instigator;

  public Damage ContactDamage;
  public float speed = 1;
  public float raycastDistance = 0.2f;
  public float timeout = 2;
  public Vector2 velocity;

  public List<Transform> ignore = new List<Transform>();

  // cache
  // changing the array size from 1 will change logic
  public RaycastHit2D[] RaycastHits = new RaycastHit2D[1];
  public int hitCount;
  public RaycastHit2D hit;
}
