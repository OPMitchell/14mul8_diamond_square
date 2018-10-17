using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour 
{
	[SerializeField] int n;
	[SerializeField] float spread;
	[SerializeField] float spreadReductionRate;
	[SerializeField] int seed;
	[SerializeField] int height;
	public void Generate() 
	{
		GameObject terrain = new GameObject("Terrain");
		Terrain t = terrain.AddComponent<Terrain>();
		TerrainData td = t.terrainData = new TerrainData();
		td.heightmapResolution = (int)Mathf.Pow(2, n);
		td.alphamapResolution = (int)Mathf.Pow(2, n);
		t.heightmapPixelError = 0;
		td.SetHeights(0, 0, DiamondSquare.CreateHeightmap(n, seed, spread, spreadReductionRate));
		td.size = new Vector3(td.heightmapWidth, height, td.heightmapHeight);
		terrain.AddComponent<TerrainCollider>();
		GetComponent<TextureTerrain>().DoItBaby();
	}
}
