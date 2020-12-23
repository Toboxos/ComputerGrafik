using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Start is called before the first frame update

    public Material matPrefab;
    private DiamondSquare ds;

    //Texture2D defaultTexture;
    void Start()
    {
        Material newMat = Object.Instantiate( matPrefab );
        Renderer renderer = GetComponent<Renderer>();

        // Generate Displacement Texture with Diamond Square
        ds = new DiamondSquare();
        int seed = Random.Range( 1, 200000 );
        Texture2D tex = ds.diamondSquare(8, seed);

        // TODO: Generate moisture texture with perlin noise

        renderer.material = newMat;
        renderer.material.SetTexture( "_DisplacementTexture", tex );
        // TODO: set moisture texture from perlin noise

        tex.Apply();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
