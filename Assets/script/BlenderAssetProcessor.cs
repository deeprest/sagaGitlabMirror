#if false
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

// tested with Unity 5.5.4p4 and Blender 2.79
public class BlenderAssetProcessor : AssetPostprocessor
{
  /*void OnPreprocessAnimation()
  {
    var modelImporter = assetImporter as ModelImporter;
    foreach( var ca in modelImporter.clipAnimations )
    {
      string str = ca.name;
      Debug.Log( str );
    }
  }*/

	public void OnPostprocessModel( GameObject obj )
	{
		//only perform corrections with blender files
		ModelImporter importer = assetImporter as ModelImporter;
		if (Path.GetExtension(importer.assetPath) == ".blend")
		{
			ProcessObject(obj.transform);
		}
	}

	void ProcessObject( Transform obj )
	{
		obj.transform.rotation = Quaternion.identity;

		MeshFilter meshFilter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (meshFilter)
		{
			ProcessMesh(meshFilter.sharedMesh);
		}

		foreach( Transform child in obj )
		{
			ProcessObject(child);
		}
      
	}


	void ProcessMesh( Mesh mesh )
	{
		Vector3[] vertices = mesh.vertices;
		for( int index = 0; index < vertices.Length; index++)
		{
			vertices[index] = new Vector3( vertices[index].x, vertices[index].z, -vertices[index].y );
		}
		mesh.vertices = vertices;

		Vector3[] normals = mesh.normals;
		for(int i = 0; i < normals.Length; i++)
		{
			normals[i] = new Vector3( normals[i].x, normals[i].z, -normals[i].y );
		}
		mesh.normals = normals;

		mesh.RecalculateBounds();
	}
}

#endif