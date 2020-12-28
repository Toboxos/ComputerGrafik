using System.Collections;
using UnityEngine;

//Perlin Noise Moisture map generation
public class MoistureGenerator : MonoBehaviour
{
    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;

    public Texture2D NoiseTex { get{ return noiseTex; } }
    private Texture2D noiseTex;
    private Color[] pix;
    // private Renderer rend;

    public void Start()
    {

        // rend = GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        // rend.material.mainTexture = noiseTex;
    }

    public void Init( int pixWidth, int pixHeight, float xOrg, float yOrg, float scale ) {
        this.pixWidth = pixWidth;
        this.pixHeight = pixHeight;
        this.xOrg = xOrg;
        this.yOrg = yOrg;
        this.scale = scale;


        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
    }

    public void CalcNoise()
    {
        // For each pixel in the texture...
        for(float y = 0.0F; y < noiseTex.height; y++)
        {
            for(float x = 0.0F; x < noiseTex.width; x++)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                //einzelne Farbwerte werden an sample angepasst 1= weiß und 0= schwarz
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
            }
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    void Update()
    {
        CalcNoise();
    }
}