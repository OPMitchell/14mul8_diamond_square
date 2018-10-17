using System;
using System.Linq;
using UnityEngine;
 
[Serializable]
public class TextureTerrain : MonoBehaviour
{
    public TextureSetting[] Textures;
 
    public void DoItBaby()
    { 
        var terrain = GameObject.Find("Terrain").GetComponent<Terrain>();                                                              //create a neat reference to our terrain
        var terrainData = terrain.terrainData;                                                                              //create a neat reference to our terrain data
 
        terrainData.splatPrototypes = Textures.Select(s => new SplatPrototype { texture = s.Texture }).ToArray();                   //Get all the textures and assign it to the terrain's spaltprototypes
        terrainData.RefreshPrototypes();                                                                                            //gotta refresh my terraindata's prototypes after its manipulated
 
        int splatLengths = terrainData.splatPrototypes.Length;
        int alphaMapResolution = terrainData.alphamapResolution;
        int alphaMapHeight = terrainData.alphamapResolution;
        int alphaMapWidth = terrainData.alphamapResolution;
 
        var splatMap = new float[alphaMapResolution, alphaMapResolution, splatLengths];       //create a new splatmap array equal to our map's, we will store our new splat weights in here, then assight it to the map later
        var heights = terrainData.GetHeights(0, 0, alphaMapWidth, alphaMapHeight);                                 //get all the height points for the terrain... this will be where ware are going paint our textures on
 
        for (var zRes = 0; zRes < alphaMapHeight; zRes++)
        {
            for (var xRes = 0; xRes < alphaMapWidth; xRes++)
            {
                var splatWeights = new float[splatLengths];                                             //create a temp array to store all our 'none-normalised weights'
                var normalizedX = (float)xRes / (alphaMapWidth - 1);                        //gets the normalised X position based on the map resolution                     
                var normalizedZ = (float)zRes / (alphaMapHeight - 1);                       //gets the normalised Y position based on the map resolution 
 
                float angle = terrainData.GetSteepness(normalizedX, normalizedZ);                       //Get the ANGLE/STEEPNESS at this point: returns the angle between 0 and 90
 
                for (var i = 0; i < Textures.Length; i++)                                               //Loop through all our trextures and apply them accoding to the rules defined
                {
                    var weighting = 0f;                                                                 //set the default weighting to 0, this means that if the image does not meet any of the criteria, then it will have no impact
                    var textureSetting = Textures[i];                                                   //get the setting instance based on index

                    if (angle <= textureSetting.MaxAngle && angle >= textureSetting.MinAngle)                      //check if the specified angle is the same as the current angle (allow a variance based on the precision)
                    	weighting = 1.0f;
 
                    splatWeights[i] = weighting;
                }
 
                #region normalize
                //we need to make sure that the sum of our weights is not greater than 1, so lets normalise it
                var totalWeight = splatWeights.Sum();                               //sum all the splat weights,
                for (int i = 0; i < splatLengths; i++)        //Loop through each splatWeights
                {
                    splatWeights[i] /= totalWeight;                                 //Normalize so that sum of all texture weights = 1
                    splatMap[zRes, xRes, i] = splatWeights[i];                      //Assign this point to the splatmap array
                }
                #endregion
            }
        }
 
        terrainData.SetAlphamaps(0, 0, splatMap);
    }
}
 
[Serializable]
public class TextureSetting
{
    public Texture2D Texture;

    [Range(0, 90)]
    public float MinAngle;
	[Range(0, 90)]
	public float MaxAngle;
}