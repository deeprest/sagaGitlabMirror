using UnityEngine;
using System.Collections;

public class BouncyGrenade : Projectile
{
  public override void OnFire()
  {
    GetComponent<Rigidbody2D>().velocity = new Vector2( velocity.x, velocity.y );
  }

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}

