using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO turret, airbot, wheelbot, hornet, mech

[CreateAssetMenu]
public class AIController : Controller
{
  [SerializeField] float sightRange = 6;
  [SerializeField] float sightStartRadius = 0.5f;
  [SerializeField] Transform sightOrigin;
  public float maxShootAngle = 5;
  
  // sight, target
  int hitCount;
  //RaycastHit2D[] RaycastHits = new RaycastHit2D[10];
  bool toggleShoot;
  
  //Collider2D[] results = new Collider2D[16];
  [SerializeField] Entity PotentialTarget;
  Transform targetPrev;
  Timer SightPulseTimer = new Timer();

  void OnDestroy()
  {
    if( Global.IsQuiting )
      return;
    SightPulseTimer.Stop( false );
  }

  public override void AssignPawn( Pawn pwn )
  {
    base.AssignPawn( pwn );

    SightPulseTimer.Start( int.MaxValue, 1, ( x ) => {
      // reaffirm target
      PotentialTarget = null;
      List<Entity> entities = new List<Entity>();
      int count = Physics2D.OverlapCircleNonAlloc( pawn.transform.position, sightRange, Global.ColliderResults, Global.EnemyInterestLayers );
      for( int i = 0; i < count; i++ )
      {
        Collider2D cld = Global.ColliderResults[i];
        Entity character = cld.GetComponent<Entity>();
        if( character != null && pawn.IsEnemyTeam( character.Team ) )
          entities.Add( character  );
      }
      PotentialTarget = (Entity)Util.FindClosest( pawn.transform.position, entities.ToArray() );
    }, null );

    sightOrigin = pawn.transform;
  }
  
  public override void Update()
  {
    if( pawn == null )
      return;
    if( PotentialTarget == null )
    {
      // animator.Play( "idle" );
    }
    else
    {
      Vector2 pos = pawn.transform.position;
      Vector2 potentialTargetPos = PotentialTarget.transform.position;
      Vector2 delta = potentialTargetPos - pos;
      if( delta.sqrMagnitude < sightRange * sightRange )
      {
        Transform target = null;
        hitCount = Physics2D.LinecastNonAlloc( (Vector2)sightOrigin.position + delta.normalized * sightStartRadius, potentialTargetPos, Global.RaycastHits, Global.SightObstructionLayers );
        if( hitCount == 0 )
          target = PotentialTarget.transform;
        if( target == null )
        {
          // animator.Play( "idle" );
        }
        else
        {
          toggleShoot = !toggleShoot;
          if( toggleShoot )
            input.Fire = true;
          input.Aim = delta;
        }
        targetPrev = target;
      }
      else
      {
        //animator.Play( "idle" );
      }
    }
    
    if( pawn != null )
      pawn.ApplyInput( input );
    input = default;
  }

}
