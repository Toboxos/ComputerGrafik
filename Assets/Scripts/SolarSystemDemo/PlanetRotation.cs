using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{

    public float Speed = 0.01f;
    GameObject sun;

    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        sun = transform.parent.Find("Sun").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown("space") ) {
            paused = !paused;
        }

        if( !paused ) {
            // Rotation arround sun
            transform.RotateAround( sun.transform.position, Vector3.up, Speed * Time.deltaTime );

            // Rotation arround itself
            transform.Rotate( Vector3.right, Speed * Time.deltaTime );
        }
    }
}
