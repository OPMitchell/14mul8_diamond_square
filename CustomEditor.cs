using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (TerrainGenerator))]
public class ProcTerrGUI : Editor 
{
	public override void OnInspectorGUI()
	{
		TerrainGenerator c = (TerrainGenerator)target;

		DrawDefaultInspector ();
		if (GUILayout.Button ("Generate Terrain")) 
		{
			c.Generate();
		}
	}
}