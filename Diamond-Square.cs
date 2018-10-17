using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiamondSquare
{
	private static int N;
	private static float Spread;
	private static float SpreadReductionRate;
	private static int edgeLength;
	private static float[,] heightmap;
	public static float[,] CreateHeightmap(int n, int seed, float spread, float spreadReductionRate)
	{
		N = n;
		Spread = spread;
		SpreadReductionRate = spreadReductionRate;
		
		Random.InitState(seed);
		edgeLength = CalculateLength(n);
		heightmap = new float[edgeLength, edgeLength];
		ClearHeightmap();
		RandomiseCorners();
		DSquare();
		NormaliseHeightmap();

		return heightmap;
	}

	private static int CalculateLength(int n)
	{
		return (int)Mathf.Pow(2, n) +1;
	}

	private static void ClearHeightmap()
	{
		for(int y = 0; y < edgeLength; y++)
		{
			for(int x = 0; x < edgeLength; x++)
			{
				heightmap[x,y] = 0.0f;
			}
		}
	}

	private static void NormaliseHeightmap()
	{
		float min = float.MaxValue;
		float max = float.MinValue;

		for(int y = 0; y < edgeLength; y++)
		{
			for(int x = 0; x < edgeLength; x++)
			{
				float current = heightmap[x,y];
				if(current < min)
					min = current;
				else if(current > max)
					max = current;
			}
		}

		for(int y = 0; y < edgeLength; y++)
		{
			for(int x = 0; x < edgeLength; x++)
			{
				heightmap[x,y] = Mathf.InverseLerp(min, max, heightmap[x,y]);
			}
		}
	}

	private static void RandomiseCorners()
	{
		heightmap[0, 0] = GetRandom();
		heightmap[0, edgeLength-1] = GetRandom();
		heightmap[edgeLength-1, 0] = GetRandom();
		heightmap[edgeLength-1, edgeLength-1] = GetRandom();
	}

	private static float GetRandom()
	{
		return Random.Range(-1.0f, 1.0f);
	}

	private static float GetOffset()
	{
		return GetRandom() * Spread;
	}

	private static int GetMidpoint(int a, int b)
	{
		return a+((b-a)/2);
	}

	private static float GetAverageOf3(float a, float b, float c)
	{
		return (a+b+c)/3.0f;
	}

	private static float GetAverageOf4(float a, float b, float c, float d)
	{
		return (a+b+c+d)/4.0f;
	}

	private static void DSquare()
	{
		int i = 0;
		while (i < N)
		{
			int numberOfQuads = (int)Mathf.Pow(4, i);
			int quadsPerRow = (int)Mathf.Sqrt(numberOfQuads);
			int quadLength = (edgeLength-1)/quadsPerRow;

			for(int y = 0; y < quadsPerRow; y++)
			{
				for(int x = 0; x < quadsPerRow; x++)
				{
					CalculateSquare(quadLength*x, quadLength*(x+1), quadLength*y, quadLength*(y+1));
					CalculateDiamond(quadLength*x, quadLength*(x+1), quadLength*y, quadLength*(y+1));
				}
			}
			Spread *= SpreadReductionRate;
			i++;
		}
	}

	private static void CalculateSquare(int x0, int x1, int y0, int y1)
	{
		int mx = GetMidpoint(x0, x1);
		int my = GetMidpoint(y0, y1);
		float tl = heightmap[x0, y0];
		float tr = heightmap[x1, y0];
		float bl = heightmap[x0, y1];
		float br = heightmap[x1, y1];
		heightmap[mx, my] = GetAverageOf4(tl, tr, bl, br) + GetOffset();
	}
	
	private static void CalculateDiamond(int x0, int x1, int y0, int y1)
	{
		int mx = GetMidpoint(x0, x1);
		int my = GetMidpoint(y0, y1);
		float tl = heightmap[x0, y0];
		float tr = heightmap[x1, y0];
		float bl = heightmap[x0, y1];
		float br = heightmap[x1, y1];
		float centre = heightmap[mx, my];

		heightmap[x0, my] = GetAverageOf3(tl, bl, centre) + GetOffset();
		heightmap[x1, my] = GetAverageOf3(tr, br, centre) + GetOffset();
		heightmap[mx, y0] = GetAverageOf3(tl, tr, centre) + GetOffset();
		heightmap[mx, y1] = GetAverageOf3(bl, br, centre) + GetOffset();
	}
}
