using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class HitEvent
{
	// tags
	public Tag[] Tags;
	public Tag[] ExcludeTags;
	// trigger or collision or both
	public enum HitType
	{
		Both,
		Trigger,
		Collision,
	}
	public HitType Type = HitType.Both;
  public Vector3 SpawnEffectOffset = Vector3.zero;
	public float DestroyDelay = 0f;
	public ForceMode ReflectForceMode = ForceMode.VelocityChange;
	public float ReflectIntensity = 1f;
	public UnityEngine.Events.UnityEvent evt;
}

public class OnHit : MonoBehaviour
{

	public Character Owner = null;
	public bool EnableTrigger = true;
	public bool EnableCollision = true;
	public HitEvent[] events;

	Transform CurrentHitObject;
	HitEvent CurrentHitEvent;
	Collision CurrentCollision;
	Collider CurrentCollider;

	void OnCollisionEnter( Collision other )
	{
    if( other.collider.isTrigger )
      return;

    if( EnableCollision )
		{
			CurrentCollision = other;
      foreach( var he in events )
      {
        if( he.Type == HitEvent.HitType.Collision || he.Type == HitEvent.HitType.Both )
        {
          CheckHit( he, other.transform );
        }
      }
			CurrentCollision = null;
		}
	}

	void OnTriggerEnter( Collider other )
	{
    if( other.isTrigger )
      return;

		if( EnableTrigger )
		{
			CurrentCollider = other;
      foreach( var he in events )
      {
        if( he.Type == HitEvent.HitType.Trigger || he.Type == HitEvent.HitType.Both )
        {
          CheckHit( he, other.transform );
        }
      }
			CurrentCollider = null;
		}
	}

  public void SendHit( RaycastHit info )
  {
    foreach( var he in events )
    {
      CheckHit( he, info.transform );
    }
  }
    
	void CheckHit( HitEvent he, Transform other )
	{
		bool check = false;
    // if hitevent tags is empty, it means this hit has no preference and will fire the event.
    if( he.Tags.Length == 0 )
      check = true;

    Tags tags = other.GetComponent<Tags>();
    if( tags != null )
    {
      if( he.Tags.Length > 0 )
        foreach( Tag tag in he.Tags )
          if( tags.HasTag( tag ) )
            check = true;

      if( he.ExcludeTags.Length > 0 )
        foreach( Tag tag in he.ExcludeTags )
          if( tags.HasTag( tag ) )
            check = false;
    }

    if( check )
    {
      CurrentHitEvent = he;
      CurrentHitObject = other;
      he.evt.Invoke();
      CurrentHitEvent = he;
      CurrentHitObject = null;
    }
	}

	public void ReflectOther()
	{
		Rigidbody body = CurrentHitObject.GetComponent<Rigidbody>();
		if( body != null )
			body.AddForce( -body.velocity * CurrentHitEvent.ReflectIntensity, CurrentHitEvent.ReflectForceMode );
//		if( CurrentHitEvent.ReflectChangeOtherTag.Length > 0 )
//			CurrentHitObject.tag = CurrentHitEvent.ReflectChangeOtherTag;
	}

	public void DisableBody()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if( rb != null )
			rb.isKinematic = true;

		Collider c = GetComponent<Collider>();
		if( c != null )
			c.enabled = false;
	}

  public void DestroyGameObject( )
	{
    if( CurrentHitEvent!=null && CurrentHitEvent.DestroyDelay > 0f )
			GameObject.Destroy( gameObject, CurrentHitEvent.DestroyDelay );
		else
			GameObject.Destroy( gameObject );
	}

  public void DestroyGameObject( GameObject go )
  {
    if( CurrentHitEvent!=null && CurrentHitEvent.DestroyDelay > 0f )
      GameObject.Destroy( go, CurrentHitEvent.DestroyDelay );
    else
      GameObject.Destroy( go );
  }

	public void SpawnEffect( GameObject prefab )
	{
    if( CurrentHitEvent == null )
      return;
		Quaternion rot = Quaternion.identity;
		Vector3 pos = transform.position + CurrentHitEvent.SpawnEffectOffset;
		if( CurrentCollision!=null )
		{
			ContactPoint contact = CurrentCollision.contacts[0];
			 rot = prefab.transform.rotation * Quaternion.LookRotation(contact.normal, Vector3.up);
			 pos = contact.point;
		}
    if( CurrentCollider != null )
    {
      pos = CurrentCollider.ClosestPoint( transform.position );
      if( Vector3.Distance(transform.position, pos) > 0 )
        rot = Quaternion.LookRotation( transform.position - pos );
    }
	  GameObject go = GameObject.Instantiate( prefab, pos , rot );
    go.name = prefab.name;
	}

  public void DamageOther( Damage info )
  {
    IDamage otherChar = CurrentHitObject.GetComponent<IDamage>();
    if( otherChar != null )
    {
      Attack weapon = GetComponent<Attack>();
      if( weapon != null )
        info.instigator = weapon.instigator;
      otherChar.TakeDamage( info );
    }
  }

  public void AudioOneShot( AudioClip clip )
  {
    if( CurrentHitEvent == null )
      return;
    
    Quaternion rot = Quaternion.identity;
    Vector3 pos = transform.position + CurrentHitEvent.SpawnEffectOffset;
    if( CurrentCollision!=null )
    {
      ContactPoint contact = CurrentCollision.contacts[0];
//      rot = prefab.transform.rotation * Quaternion.LookRotation(contact.normal, Vector3.up);
      // pos = transform.position + CurrentHitEvent.SpawnEffectOffset; //contact.point;
      pos = contact.point; // + contact.normal*0.1f;
    }
    GameObject go = GameObject.Instantiate( Global.instance.audioOneShotPrefab, pos , rot );
    go.GetComponent<AudioSource>().PlayOneShot( clip );
    GameObject.Destroy( go, clip.length );
  }

  public void ConsoleLog( string msg )
  {
    Debug.Log( msg );
  }

  public void GlobalMethod( string msg )
  {
    Global.instance.SendMessage( msg );
  }

  public void LoadScene( string msg )
  {
    Global.instance.LoadScene( msg );
  }
}
