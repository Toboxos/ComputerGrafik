using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Transform PlayerPos;
    Rigidbody rigidbody;
    Animator animator;

    public float Speed = 1;
    private bool isUboat = true;
    public float PitchSpeed = 15;

    //Uboat
    public float ResetTurningRate = 50;
    public float UboatYawSpeed = 15;

    //Plane
    public float PlaneRollSpeed = 10;

    public GameObject Terrain;
    public Transform WaterPlane;

    private Vector2 LastPositon;
    private Vector2 Position;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPos = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    //Make Explosion on Collision
    void OnCollisionEnter( Collision collision )
    {
        Debug.Log( "Hit Something" );
        Debug.Log( "EXPLOSION BRIADSASDASDASFDASGF BUUUUUM" );
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidbody.velocity = transform.forward * Speed;
        animator.SetBool( "isUboat", isUboat );

        if( WaterPlane.position.y > transform.position.y ) {
            isUboat = true;
        } else {
            isUboat = false;
        }

        if( isUboat ) {
            Vector3 TargetRotation = transform.rotation.eulerAngles;

            //Angle Move Back to Z 90°
            TargetRotation.z = 90;

            rigidbody.MoveRotation( Quaternion.Lerp( transform.rotation, Quaternion.Euler( TargetRotation ), Time.deltaTime * ResetTurningRate ));
        }

        //Move Forward
        if( Input.GetKey( KeyCode.W )) {
            Quaternion deltaRotation = Quaternion.Euler( Vector3.up * -PitchSpeed * Time.deltaTime );
            rigidbody.MoveRotation( rigidbody.rotation * deltaRotation );
        }

        if( Input.GetKey( KeyCode.S )) {
            Quaternion deltaRotation = Quaternion.Euler( Vector3.up * PitchSpeed * Time.deltaTime );
            rigidbody.MoveRotation( rigidbody.rotation * deltaRotation );
        }

        if( Input.GetKey( KeyCode.A )){
            if( isUboat ) {
                Quaternion deltaRotation = Quaternion.Euler( Vector3.up * -UboatYawSpeed * Time.deltaTime );
                rigidbody.MoveRotation( deltaRotation * rigidbody.rotation );
            } else {
                Quaternion deltaRotation = Quaternion.Euler( Vector3.back * -PlaneRollSpeed * Time.deltaTime );
                rigidbody.MoveRotation( rigidbody.rotation * deltaRotation );
            }
        }

        if( Input.GetKey( KeyCode.D )) {
            if( isUboat ) {
                //TODO: Rotate to World Space instead of Local Space
                Quaternion deltaRotation = Quaternion.Euler( Vector3.up * UboatYawSpeed * Time.deltaTime );
                rigidbody.MoveRotation( deltaRotation * rigidbody.rotation );
            } else {
                Quaternion deltaRotation = Quaternion.Euler( Vector3.back * PlaneRollSpeed * Time.deltaTime );
                rigidbody.MoveRotation( rigidbody.rotation * deltaRotation );
            }
        }

        //Handle Movement of Terrain
        Renderer renderer = Terrain.GetComponent<Renderer>();
        float pixelWidth = 1 / Mathf.Pow( 2, Terrain.GetComponent<TerrainGenerator>().GenerateTextureSize );

        //Add Delta Movement to Positon
        Position += new Vector2( transform.position.x - LastPositon.x, transform.position.z - LastPositon.y );

        //Clamp Player Position Between Pixels via Float Modulo
        float xPos = Position.x - Mathf.Floor( Position.x / ( pixelWidth * renderer.bounds.size.x )) * ( pixelWidth * renderer.bounds.size.x );
        float yPos = Position.y - Mathf.Floor( Position.y / ( pixelWidth * renderer.bounds.size.z )) * ( pixelWidth * renderer.bounds.size.z );

        transform.position = new Vector3( xPos, transform.position.y, yPos );
        LastPositon = new Vector2( transform.position.x, transform.position.z );

        //Convert Player to Texture Coordinates with Scale
        Vector2 PlayerOffset = new Vector2( Position.x / renderer.bounds.size.x, Position.y / renderer.bounds.size.z );
        
        //Clamp Vertices to Pixels to avoid Wobelling
        //And move in Negative Offset to create the illusion of Movement
        PlayerOffset.x = -Mathf.Ceil( PlayerOffset.x / pixelWidth ) * pixelWidth;
        PlayerOffset.y = -Mathf.Ceil( PlayerOffset.y / pixelWidth ) * pixelWidth;

        renderer.material.SetTextureOffset( "_DisplacementTexture", PlayerOffset );
        renderer.material.SetTextureOffset( "_MoistureTexture", PlayerOffset );

    }
}
