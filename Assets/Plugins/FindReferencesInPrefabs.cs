﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/**
 * Helper class to find MonoBehaviours that exist in Prefabs (not in the scene).
 * Useful to figure out if you can remove a script from your Project.
 */
public static class FindReferencesInPrefabs
{
	[MenuItem("Assets/Find In Prefabs",true)]
	static bool CanExecute()
	{
		MonoScript monoScript = Selection.activeObject as MonoScript;
		return monoScript != null;
	}

	[MenuItem("Assets/Find In Prefabs")]
	static void Execute()
	{
		// Make sure we have a MonoScript selected
		MonoScript monoScript = Selection.activeObject as MonoScript;
		if ( monoScript == null )
		{
			Debug.LogError( "Need to select a MonoScript for this function to work" );
			return;
		}

		// GetComponent only works on MonoBehaviour-derived classes
		System.Type scriptType = monoScript.GetClass();
		if ( !typeof(MonoBehaviour).IsAssignableFrom(scriptType) )
		{
			Debug.LogError( "Selected script must derive from MonoBehaviour" );
			return;
		}

		foreach( var obj in Resources.FindObjectsOfTypeAll<GameObject>() )
		{
			// Persistent (Prefabs/Assets) only
			if ( !EditorUtility.IsPersistent(obj) )
				continue;

			// Top-level only
			if ( obj.transform.parent != null )
				continue;

			// Does it contain a reference?
			Component[] allComponents = obj.GetComponentsInChildren( scriptType, true );
			if ( allComponents == null || allComponents.Length < 1 )
				continue;

			// List all of the references
			foreach( var comp in allComponents )
				Debug.Log( "Reference Found: " + FullPath(comp.transform), obj );
		}
	}

	/**
	 * Print out the full path in "Game Object/Child" format
	 */
	private static string FullPath( Transform transform )
	{
		var sb = new System.Text.StringBuilder();

		while ( transform != null )
		{
			sb.Insert(0, transform.name);
			transform = transform.parent;

			if ( transform != null )
				sb.Insert(0, '/');
		}

		return sb.ToString();
	}
}

#endif