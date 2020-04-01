using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using deeprest;

[SelectionBase]
//[RequireComponent( typeof(Tags) )]
public class SpawnPoint : SerializedComponent //, IAction, ITeam
{
  //[Serialize] public string TeamName;
  //public Team MyTeam;

  public Transform SpawnPointLocal;
  public Animation Animation;
  public AudioSource AudioSource;
  public AudioClip CannonSound;
  public ParticleSystem CannonPuff;

  public float CannonSpeed = 10;
  //[Serialize] 
  public float SpawnRadius = 0f;
  //[Serialize] 
  public int TargetQuota = 3;
  public float StartDelay = 1f;
  public float UnderRepeatRate = 1f;
  public float MaxedRepeatRate = 5f;
  public bool ChooseRandomPrefab = false;
  public List<GameObject> SpawnPrefab;
  public System.Action<GameObject> OnSpawn;
  public List<GameObject> SpawnedCharacters = new List<GameObject>();

  float lastTime = 0;
  int index = 0;
  //System.Random randy;

  [HideInInspector]
  [Serialize] public List<int> SpawnedCharactersID = new List<int>();

  public override void BeforeSerialize()
  {
    SpawnedCharactersID.Clear();
    // serialize the SO ids for SpawnedCharacters, and resolve after deserialization.
    foreach( var obj in SpawnedCharacters )
      if( obj != null )
        SpawnedCharactersID.Add( obj.GetComponent<SerializedObject>().id );
  }

  public override void AfterDeserialize()
  {
    SpawnedCharacters.Clear();
    foreach( var sf in SpawnedCharactersID )
    {
      SerializedObject so = SerializedObject.ResolveObjectFromId( sf );
      if( so != null )
        SpawnedCharacters.Add( so.gameObject );
    }
    SpawnedCharactersID.Clear();

    //Team team = Global.instance.gameData.FindTeam( TeamName );
    //if( team != null )
    //Global.instance.AssignTeam( gameObject, team );
  }
  
  void OnEnable()
  {
    if( SpawnPrefab.Count == 0 )
      return;
    /*if( ChooseRandomPrefab )
      randy = new System.Random( GetInstanceID() );*/
  }

  void Update()
  {
    if( SpawnedCharacters.Count < TargetQuota )
    {
      if( Time.time - lastTime > UnderRepeatRate )
      {
        lastTime = Time.time;
        SpawnLimited();
      }
    }
    else
    {
      if( Time.time - lastTime > MaxedRepeatRate )
      {
        lastTime = Time.time;
        SpawnLimited();
      }
    }
  }

  public void SpawnMultiple( int count )
  {
    // ignore quota
    for( int i = 0; i < count; i++ )
      SpawnThatThing();
  }

  public void SpawnLimited()
  {
    if( SpawnPrefab.Count == 0 )
      return;
    ClearNull();
    if( SpawnedCharacters.Count < TargetQuota )
      SpawnThatThing();
  }

  public void SpawnThatThing()
  {
    if( ChooseRandomPrefab )
      index = Random.Range(0, SpawnPrefab.Count );
    //index = randy.Next( 0, SpawnPrefab.Count );
    else
      index = (index + 1 >= SpawnPrefab.Count) ? 0 : index + 1;

    GameObject prefab = SpawnPrefab[index];
    if( prefab == null )
    {
      Debug.LogError( "Spawn point has null objects in list " + name );
    }
    else
    {
      /*if( prefab.GetComponent<SerializedObject>() == null )
      {
        Debug.LogWarning( "SpawnPoint can only spawn prefabs with a SerializedObject component" );
        return;
      }*/

      // obey limits by doing a check before spawn
      ILimit[] limits = prefab.GetComponentsInChildren<ILimit>();
      foreach( var cmp in limits )
      {
        if( !cmp.IsUnderLimit() )
          return;
      }

      Vector3 pos = new Vector3();
      if( SpawnPointLocal != null )
        pos = SpawnPointLocal.position;
      else
        pos = transform.position;
      pos += Random.insideUnitSphere * SpawnRadius;
      pos.z = 0;
      GameObject go = Global.instance.Spawn( prefab, pos, Quaternion.identity, null, true, true );

      /*if( MyTeam != null )
        Global.Instance.AssignTeam( go, MyTeam );
*/
      if( Animation != null )
      {
        Animation.Stop();
        Animation.Play();
      }
      if( CannonPuff != null )
        CannonPuff.Play();
      if( AudioSource != null && CannonSound != null )
        AudioSource.PlayOneShot( CannonSound );
      //Rigidbody rb = go.GetComponent<Rigidbody>();
      //if( rb != null )
      //  rb.velocity = transform.forward * CannonSpeed;

      SpawnedCharacters.Add( go );

      if( OnSpawn != null )
        OnSpawn( go );
    }
  }

  void ClearNull()
  {
    List<GameObject> destroy = SpawnedCharacters.FindAll( x => x == null );
    foreach( var obj in destroy )
    {
      SpawnedCharacters.Remove( obj );
      Destroy( obj );
    }
  }

  /*
  public void GetActionContext( ref ActionData actionData )
  {
    actionData.actions.Add( "claim" );
  }

  public void OnAction( Character instigator, string action = "default" )
  {
    if( action == "claim" )
    {
      instigator.HideContextMenu();
      Global.Instance.AssignTeam( gameObject, instigator.Team );
    }
  }
  */

  /*
  public void SetTeam( Team team )
  {
    MyTeam = team;
    TeamName = MyTeam.name;
    Color tc = Color.white;
    if( team != null )
      tc = team.Color;
    TextMesh tm = GetComponentInChildren<TextMesh>();
    if( tm != null )
      tm.color = tc;
  }
  */
}
