using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshColliderController : MonoBehaviour
{
    private MeshCollider collider;
    private Mesh mesh;

    // Start is called before the first frame update
    void OnEnable()
    {

        collider = GetComponent<MeshCollider>();
        mesh = Instantiate<Mesh>( GetComponent<MeshFilter>().sharedMesh );

        Vector3[] verts = mesh.vertices;
        Debug.Log( "Start" );
        for( int i = 0; i < 400; ++i ) {
            verts[i] += new Vector3(0, Random.Range(1.5f, 2.5f), 0);
        }
        mesh.vertices = verts;
        Debug.Log( "Stop" );

        collider.sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        // Mesh mesh = collider.sharedMesh;

        // Debug.Log( mesh.vertices.Length );

        // for( int i = 0; i < mesh.vertices.Length; ++i ) {
        //     mesh.vertices[i] += new Vector3(0, Random.Range(0, 10), 0);
        // }

    }
}
