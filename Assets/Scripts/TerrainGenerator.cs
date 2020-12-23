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

        ds = new DiamondSquare();


        renderer.material = newMat;
        renderer.material.SetFloat( "_Albedo", 0.0f );
        renderer.material.SetFloat( "_DisplacementScale", 0.5f );
        renderer.material.SetFloat( "_DisplacementOffset", 0.5f );
        renderer.material.SetFloat( "_Shininess", 1.0f );

        int seed = Random.Range( 1, 200000 );
        Debug.Log( "Random: " + seed );

        Texture2D tex = ds.diamondSquare(8, seed);
        renderer.material.SetTexture( "_DisplacementTexture", tex );
        renderer.material.SetTexture( "_ColorTexture", tex );

        tex.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
