using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Start is called before the first frame update

    public Material matPrefab;
    private DiamondSquare ds;
    private MoistureGenerator mg;

    //Texture2D defaultTexture;
    void Start()
    {
        Material newMat = Object.Instantiate( matPrefab );
        Renderer renderer = GetComponent<Renderer>();

        int Size = 8;

        // Generate Displacement Texture with Diamond Square
        ds = new DiamondSquare();
        Texture2D displacementTex = ds.diamondSquare( Size, Random.Range( 1, 200000 ) );

        // TODO: Generate moisture texture with perlin noise
        mg = new MoistureGenerator();
        int pixelSize = (int) Mathf.Pow(2, Size);
        mg.Init( pixelSize, pixelSize, 0, 0, 10 );
        mg.CalcNoise();
        Texture2D moistureTex = mg.NoiseTex;

        renderer.material = newMat;
        renderer.material.SetTexture( "_DisplacementTexture", displacementTex );
        renderer.material.SetTexture( "_MoistureTexture", moistureTex );

        displacementTex.Apply();
        moistureTex.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        // renderer.material.SetTextureOffset( "_DisplacementTexture", new Vector2(0, 0.2f * Time.realtimeSinceStartup) );
        // renderer.material.SetTextureOffset( "_MoistureTexture", new Vector2(0, 0.2f * Time.realtimeSinceStartup) );
    }
}
