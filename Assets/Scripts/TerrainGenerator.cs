using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Start is called before the first frame update

    public Material matPrefab;
    private DiamondSquare ds;
    private MoistureGenerator mg;
    public Transform Player;

    //Texture2D defaultTexture;
    void Start()
    {
        Material newMat = Object.Instantiate( matPrefab );
        Renderer renderer = GetComponent<Renderer>();

        int Size = 8;

        // Generate Displacement Texture with Diamond Square
        ds = new DiamondSquare();
        Texture2D displacementTex = ds.diamondSquare( Size, Random.Range( 1, 200000 ) );

        // Generate moisture texture with perlin noise
        mg = new MoistureGenerator();
        int pixelSize = (int) Mathf.Pow(2, Size);
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
        renderer.material.SetFloat( "_WaterLevel", Random.Range(0.0f, 0.7f) );
        renderer.material.SetColor( "_WaterColor", new Color( Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f) ) );
    }

    // Update is called once per frame
    void Update()
    {
        // Renderer renderer = GetComponent<Renderer>();

        // float pixelWidth = 1 / Mathf.Pow(2, 8);

        // Vector2 PlayerOffset = new Vector2(Player.position.x/renderer.bounds.size.x, Player.position.z/ renderer.bounds.size.z);
        
        // PlayerOffset.x = Mathf.Ceil( PlayerOffset.x / pixelWidth ) * pixelWidth;
        // PlayerOffset.y = Mathf.Ceil( PlayerOffset.y / pixelWidth ) * pixelWidth;

        // renderer.material.SetTextureOffset( "_DisplacementTexture",  PlayerOffset);
        // renderer.material.SetTextureOffset( "_MoistureTexture", PlayerOffset);
    }
}
