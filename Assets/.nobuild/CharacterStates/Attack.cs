using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Character
{
  [Header( "Weapon" )]
  public List<GameObject> AmmoPrefab;
  public int AmmoIndex = 0;
  [Range( 1, 5 )]
  public float FireIntervalDividen = 1f;
  public float shotSpawnDistance = 2;
  float lastFire = 0;
  public Attack CurrentWeaponPrefab;
  float StabbyStabBodyForce = 100;
  // Shield
  bool shielding = false;
  public float ShieldDownProximity = 2;
  Transform ShieldTransform;
  public Vector3 ShieldInactiveLocal = new Vector3( -0.2f, 0.05f, 0 );
  public Vector3 ShieldActiveLocal = new Vector3( -0.05f, 0.05f, 0.3f );


  [Header( "Attack" )]
  public bool UnarmedAttack = false;
  public Transform AttackTarget = null;
  bool HasNoVisual = false;
  float HasNoVisualStart;
  float HasNoVisualPatience = 5f;
  // if multiple, will choose random
  public AudioClip[] FoundTargetSound;
  // post-battle event
  bool EndAttackCooldown = false;
  float EndAttackCooldownStart;
  float EndAttackCooldownDuration = 1f;

  public bool IsUnarmed(){ return !HasItemType( "weapon" ); }

  public void Fire( Vector3 direction )
  {
    if( CurrentWeaponPrefab != null )
    {
      if( CurrentWeaponPrefab.RequireClearance )
      {
        Vector3 start = moveTransform.position; 
        Vector3 end = moveTransform.position + direction.normalized * CurrentWeaponPrefab.StartDistance;
        #if DEBUG_LINES
        Debug.DrawLine( start, end, Color.white );
        #endif
        RaycastHit info = new RaycastHit();
        if( Physics.Raycast( new Ray( start, end - start ), out info, ( end - start ).magnitude, ~LayerMask.GetMask( new string[]{ "Projectile" } ) ) )
        {
          Debug.Log( "weapon hit on raycast: " + name );
          return;
        }
      }
      if( CurrentWeaponPrefab.AttackClosestWithinRadius )
      {
        Collider[] cds = Physics.OverlapSphere( transform.position, CurrentWeaponPrefab.AttackDistance, LayerMask.GetMask( new string[]{ "Character" } ) );
        Transform closest = null;
        float shortestDistance = CurrentWeaponPrefab.AttackDistance;
        foreach( var coll in cds )
        {
          if( coll.transform == transform )
            continue;
          Tags tags = coll.GetComponent<Tags>();
          if( tags != null && tags.HasAnyTag( AttackTags.ToArray() ) )
          {
            // NOTE this ignores the size of the collider; only uses position
            float distance = Vector3.Distance( coll.transform.position, transform.position );
            if( distance < shortestDistance )
            {
              closest = coll.transform;
              shortestDistance = distance;
            }
          }
        }
        if( closest != null )
        {
          direction = closest.position - transform.position;
        }
      }

      if( CurrentWeaponPrefab.type == Attack.Type.Melee )
      {
        GameObject go = (GameObject)GameObject.Instantiate( CurrentWeaponPrefab.gameObject, moveTransform.position + direction.normalized * CurrentWeaponPrefab.StartDistance, Quaternion.LookRotation( direction ), moveTransform );
        go.name = CurrentWeaponPrefab.gameObject.name;
        if( go != null )
          CurrentWeaponPrefab.FireWeapon( go, direction, audioSource, moveTransform );
        body.AddForce( direction.normalized * StabbyStabBodyForce * body.mass );
        //RightHandAnimation.Play( "melee-0-attack" );
        //RightHandAnimation.PlayQueued( "melee-0-idle" );
      }

      if( CurrentWeaponPrefab.type == Attack.Type.Projectile )
      {
        GameObject go = (GameObject)GameObject.Instantiate( CurrentWeaponPrefab.gameObject, moveTransform.position + direction.normalized * CurrentWeaponPrefab.StartDistance, Quaternion.LookRotation( direction ) );
        go.name = CurrentWeaponPrefab.gameObject.name;
        if( go != null )
          CurrentWeaponPrefab.FireWeapon( go, direction, audioSource, moveTransform );
      }

      if( CurrentWeaponPrefab.type == Attack.Type.Spawner )
      {
        GameObject prefab = CurrentWeaponPrefab.SpawnerPrefabs[ Random.Range( 0, CurrentWeaponPrefab.SpawnerPrefabs.Length ) ]; 
        GameObject go = Global.Instance.Spawn( prefab, moveTransform.position + direction.normalized * CurrentWeaponPrefab.StartDistance, Quaternion.LookRotation( direction ) );
        if( go != null )
        {
          Character cha = go.GetComponent<Character>();
          if( cha != null )
          {
            cha.ModifyAffinity( id, 20 );
            //cha.Follow( this, null );
          }
          CurrentWeaponPrefab.FireWeapon( go, direction, audioSource, moveTransform );
        }
      }

    }
  }

  public void ShieldOn()
  {
    if( ShieldTransform != null && !shielding)
    {
      shielding = true;
      ShieldTransform.localRotation = Quaternion.identity;
      LerpToTarget lerp = ShieldTransform.GetComponent<LerpToTarget>();
      lerp.targetTransform = moveTransform;
      lerp.enabled = true;
      lerp.duration = 0.2f;
      lerp.localOffset = ShieldActiveLocal;
    }
  }

  public void ShieldOff()
  {
    if( ShieldTransform != null && shielding )
    {
      shielding = false;
      ShieldTransform.localRotation = Quaternion.identity;
      LerpToTarget lerp = ShieldTransform.GetComponent<LerpToTarget>();
      lerp.enabled = true;
      lerp.duration = 0.2f;
      lerp.localOffset = ShieldInactiveLocal;
      lerp.OnLerpEnd = null;
    }
  }



  void ConsiderAttack( Interest interest )
  {
    if( CurrentState.Name == "Attack" && AttackTarget != null )
    {
      if( interest.go == null )
        return;
      if( Vector3.SqrMagnitude( moveTransform.position - interest.go.transform.position ) < Vector3.SqrMagnitude( moveTransform.position - AttackTarget.position ) )
      {
        AttackTarget = interest.go.transform;
      }
    }
    else
    {
      // ignore swag otherwise shields make you effectively invisible by holding them in front of you.
      if( CanSeeObject( interest.go, true, ~LayerMask.GetMask( new string[]{ "Carry" } ) ) )
      { 
        Transform otherTransform = interest.go.transform; 
        if( CurrentWeaponPrefab != null )
        {
          if( AttackTarget == null )
          {
            if( identity != null )
              Speak( "attack" );
            else
            if( FoundTargetSound.Length > 0 )
              audioSource.PlayOneShot( FoundTargetSound[ Random.Range( 0, FoundTargetSound.Length ) ] );
          }
          AttackTarget = otherTransform;
          InvestigatePosition = otherTransform.position; 
          Rigidbody rb = otherTransform.GetComponent<Rigidbody>();
          if( rb != null )
            FinalInvestigateDirection = rb.velocity;
          if( CurrentState.Name != "Attack" )
            PushState( "Attack", interest );

          /*Character attackChar = AttackTarget.GetComponent<Character>();
          if( attackChar != null )
          {
            CharacterEvent evt = new CharacterEvent();
            evt.type = CharacterEventEnum.DeclareAttack;
            evt.position = moveTransform.position;
            evt.radius = World.Instance.GlobalSightDistance;
            evt.A = attackChar;
            evt.B = this;
            BroadcastEvent( evt );
          }*/
        }
        else
        if( CurrentState.Name != "Flee" )
        {
          FleeFrom = otherTransform;
          PushState( "Flee", interest );
        }
      }
    }
  }

  void PushAttack()
  {
    FaceDirectionOfMovement = false;
    CurrentMoveSpeed = SprintSpeed;
    SidestepAvoidance = false;
    EndAttackCooldown = false;
  }

  void UpdateAttack()
  {
    EndAttackCooldown = false;

    if( CurrentWeaponPrefab == null )
    {
      PopState();
      return;
    }
    if( AttackTarget == null )
    {
      PopState();
      return;
    }

    Vector3 delta = AttackTarget.position - transform.position;
    Transform visibleEnemy = null;
    // ignore swag otherwise shields make you effectively invisible by holding them in front of you.
    if( CanSeeObject( AttackTarget.gameObject, true, ~LayerMask.GetMask( new string[]{ "Carry" } ) ) )
      visibleEnemy = AttackTarget;

    if( visibleEnemy == null )
    {
      if( !HasNoVisual )
      {
        HasNoVisual = true;
        HasNoVisualStart = Time.time;
      }
      if( Time.time - HasNoVisualStart > HasNoVisualPatience )
      {
        HasNoVisual = true;
        PopState();
        // TODO if on guard duty, investigate
        if( CurrentState.Name != "Follow" )
          PushState( "Investigate" );
        return;
      }
    }
    else
    {
      FaceDirection = delta;
      HasNoVisual = false;
      InvestigatePosition = visibleEnemy.position;
      Rigidbody targetBody = visibleEnemy.GetComponent<Rigidbody>();
      if( targetBody != null )
        FinalInvestigateDirection = targetBody.velocity;

      if( delta.magnitude < ShieldDownProximity )
        ShieldOff();
      else
        ShieldOn();

      float distanceToTargetCollider;
      SphereCollider tsc = AttackTarget.GetComponent<SphereCollider>();
      if( tsc != null )
        distanceToTargetCollider = delta.magnitude - tsc.radius;
      else
        distanceToTargetCollider = delta.magnitude;

      if( distanceToTargetCollider < CurrentWeaponPrefab.AttackDistance )
      {
        ClearPath();
        MoveDirection = Vector3.zero;

        // Weapon
        if( Time.time - lastFire > CurrentWeaponPrefab.Interval / FireIntervalDividen )
        {
          lastFire = Time.time;
          // get shot direction
          Vector3 tVelocity;
          Rigidbody tBody = AttackTarget.GetComponent<Rigidbody>();
          if( tBody != null )
            tVelocity = Vector3.Project( tBody.velocity, Vector3.Cross( delta.normalized, Vector3.up ).normalized ) * 0.5f; // .5 fudge
            else
            tVelocity = Vector3.zero; //Random.onUnitSphere;
          tVelocity.y = 0f;
          Vector3 howMuchToLeadTarget = tVelocity * Random.value;
          if( Random.Range( 0, 2 ) == 1 )  // randomly lead target
              howMuchToLeadTarget = Vector3.zero;
          Vector3 shotDirection = ( AttackTarget.position + howMuchToLeadTarget ) - transform.position;
          shotDirection.y = 0f;
          shotDirection.Normalize();

          // make sure I don't shoot my friends
          // ignore swag otherwise shields make you effectively invisible by holding them in front of you.
          if( CanSeeObject( AttackTarget.gameObject, true, ~LayerMask.GetMask( new string[]{ "Carry" } ) ) )
            Fire( shotDirection );

          #if DEBUG_LINES
            Debug.DrawLine( AttackTarget.position, AttackTarget.position + tVelocity, Color.red, 1f );
            Debug.DrawLine( transform.position, transform.position + shotDirection, Color.green, 1f ); 
          #endif
        }

        // Movement
        if( CurrentWeaponPrefab.type == Attack.Type.Projectile )
        {
          float IdealAttackDistance = CurrentWeaponPrefab.AttackDistance - 1;
          float throttle = Mathf.Min( 1, ( delta.magnitude - IdealAttackDistance ) / IdealAttackDistance );
          CurrentMoveSpeed = SprintSpeed * throttle;
          if( Mathf.Abs( 1f - throttle ) < 0.2f )
            MoveDirection = Vector3.Cross( delta.normalized, Vector3.up ).normalized * Mathf.Sign( Random.value - 0.5f );
          else
            MoveDirection = delta.normalized;
        }

        
      }
      else
      {
        // Approach target
        Vector3 tVelocity = Vector3.zero;
        Rigidbody tBody = AttackTarget.GetComponent<Rigidbody>();
        if( tBody != null )
          tVelocity = Vector3.Project( tBody.velocity, Vector3.Cross( delta.normalized, Vector3.up ).normalized ) * 0.5f;
        Vector3 pos = AttackTarget.position + tVelocity; // * Random.value;
        SetPath( pos );
        if( Vector3.SqrMagnitude( DestinationPosition - pos ) > CurrentWeaponPrefab.AttackDistance * CurrentWeaponPrefab.AttackDistance )
        {
          // add a final waypoint in case the last waypoint is off-mesh and out of attack range
          // Commented out because it modifies the waypoint list and invalidates the enumerator.
          //waypoint.Add( pos );
          Debug.Log( "Added final attack waypoint" );
        }
      }
    }
  }

  void PopAttack()
  {
    FaceDirectionOfMovement = true;
    ShieldOff();
    EndAttackCooldown = true;
    EndAttackCooldownStart = Time.time;
    SidestepAvoidance = DefaultSidestepAvoidance;
  }

}

