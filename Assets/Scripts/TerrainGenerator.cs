using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Start is called before the first frame update

    public int GenerateTextureSize = 8;
    public float MaxWaterLevel = 0.7f;

    public Material matPrefab;
    private DiamondSquare ds;
    private MoistureGenerator mg;

    //Texture2D defaultTexture;
    void Start()
    {
        Material newMat = Object.Instantiate( matPrefab );
        Renderer renderer = GetComponent<Renderer>();

        // Generate Displacement Texture with Diamond Square
        ds = new DiamondSquare();
        Texture2D displacementTex = ds.diamondSquare( GenerateTextureSize, Random.Range( 1, 200000 ));

        // Generate moisture texture with perlin noise
        mg = new MoistureGenerator();
        int pixelSize = (int) Mathf.Pow( 2, GenerateTextureSize );
        mg.Init( pixelSize, pixelSize, 0, 0, 10 );
        mg.CalcNoise();
        Texture2D moistureTex = mg.NoiseTex;

        // Set textures for shader
        renderer.material = newMat;
        renderer.material.SetTexture( "_DisplacementTexture", displacementTex );
        renderer.material.SetTexture( "_MoistureTexture", moistureTex );

        // Apply textures
        displacementTex.Apply();
        moistureTex.Apply();

        // Set random terrain values
        renderer.material.SetFloat( "_WaterLevel", Random.Range( 0.0f, MaxWaterLevel ));
        renderer.material.SetColor( "_WaterColor", new Color( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f )));
    }

    // Update is called once per frame
    void Update()
    {       
    }
}
