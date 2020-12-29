using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class MeshColliderController : MonoBehaviour
{
    private MeshCollider collider;
    private Mesh mesh;
    private Renderer renderer;

    Texture2D displacementTex = null;

    // Start is called before the first frame update
    void OnEnable()
    {

        collider = GetComponent<MeshCollider>();
        renderer = GetComponent<Renderer>();
        mesh = Instantiate<Mesh>( GetComponent<MeshFilter>().sharedMesh );
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = renderer.material.GetTextureOffset( "_DisplacementTexture" );
        float displacementScale = renderer.material.GetFloat( "_DisplacementScale" );

        if( displacementTex == null ) {
            displacementTex = (Texture2D) renderer.material.GetTexture( "_DisplacementTexture" );
        }

        Vector3[] verts = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        for( int i = 0; i < verts.Length; ++i ) {
            Vector3 vert = verts[i];
            if( Mathf.Abs( vert.x ) > 0.2 || Mathf.Abs( vert.z ) > 0.2 ) continue;

            Color displacementColor = displacementTex.GetPixelBilinear( uv[i].x + offset.x, uv[i].y + offset.y );
            verts[i] = new Vector3( vert.x, displacementColor.r * displacementScale , vert.z );
        }

        mesh.vertices = verts;
        stopwatch.Stop();

        UnityEngine.Debug.Log( stopwatch.ElapsedMilliseconds + "ms elapsed" );
        collider.sharedMesh = mesh;

    }
}
