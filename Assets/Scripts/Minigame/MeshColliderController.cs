using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;


public class MeshColliderController : MonoBehaviour
{
    private MeshCollider collider;
    private Mesh mesh;
    private Renderer renderer;

    private Texture2D displacementTex;

    private Vector2 prevOffset, currOffset;

    [Range(0.1f, 2.0f)]
    public float Factor = 1.0f;

    // Texture2D displacementTex = null;

    // Start is called before the first frame update
    void OnEnable()
    {

        collider = GetComponent<MeshCollider>();
        renderer = GetComponent<Renderer>();
        mesh = Instantiate<Mesh>( GetComponent<MeshFilter>().sharedMesh );
        collider.sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        // Start time measurement
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Get current offset of texture from shader
        prevOffset = currOffset;
        currOffset = renderer.material.GetTextureOffset( "_DisplacementTexture" );

        // Only recalculate mesh if offset has changed
        if( prevOffset != null && currOffset == prevOffset ) return;

        float displacementScale = renderer.material.GetFloat( "_DisplacementScale" );

        // Get the texture at the first update
        // (not possible before because it first have to be set by the TerrainGenerator)
        if( displacementTex == null ) {
            displacementTex = (Texture2D) renderer.material.GetTexture( "_DisplacementTexture" );
        }

        // Get mesh data
        Vector3[] verts = mesh.vertices;
        Vector2[] uv = mesh.uv;
        UnityEngine.Debug.Log( "Vertices: " + verts.Length );

        // For every vertex in mesh:
        for( int i = 0; i < verts.Length; ++i ) {
            Vector3 vert = verts[i];

            // If vertex is not near 0 (position of player) skip. Reduces computational need
            if( Mathf.Abs(vert.x) > 0.2 || Mathf.Abs(vert.z) > 0.2 ) continue;

            // Displacement from texture
            // TODO: BUG: Calculated displacement differs sometimes slightly from shader displacement. Might be difference in texture sampling between shader and unity
            float displacement = displacementTex.GetPixelBilinear( uv[i].x + currOffset.x, uv[i].y + currOffset.y, 0 ).r;
            verts[i] = new Vector3( vert.x, displacement * displacementScale * Factor, vert.z );
        }

        // Write updated changes back
        mesh.vertices = verts;
        collider.sharedMesh = mesh;

        // Stop time measurement
        stopwatch.Stop();
        UnityEngine.Debug.Log("Mesh updated time: " + stopwatch.ElapsedMilliseconds + "ms");
    }
}
