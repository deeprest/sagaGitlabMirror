
using UnityEngine;

public class Timeout : MonoBehaviour
{
	public ParticleSystem ps = null;
	public float duration = 1;
	public UnityEngine.Events.UnityEvent OnTimeout;

	float timeStart;

	void OnEnable()
	{
		timeStart = Time.time;	
	}

	bool Check()
	{
		if( ps != null )
		{
			if (!ps.isPlaying)
			{
				return true;
			}
		}

		if( Time.time - timeStart >= duration)
		{
			return true;
		}
		return false;
	}

	void TimesUp()
	{
		OnTimeout.Invoke ();
		enabled = false;
	}

	void Update()
	{
		if( Check() )
		{
			TimesUp ();
		}
	}

	public void Die()
	{
		GameObject.Destroy (gameObject);
	}

	public void Die(float duration)
	{
		GameObject.Destroy (gameObject, duration);
	}

}


