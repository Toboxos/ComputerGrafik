using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inspired by: https://www.youtube.com/watch?v=rDJOilo4Xrg
public class CameraOrbitController : MonoBehaviour
{
    public Transform target;

    private Vector3 prevPos;
    private float zoom = 20;

    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;

        // Handle zooming
        zoom += -Input.mouseScrollDelta.y * 2;
        if( zoom < 1 ) {
            zoom = 1;
        }

        // Left mouse button is pressed
        if( Input.GetMouseButtonDown(0) ) {

            // Viewport space: (0, 0) - (1, 1) is later mapped to to rotation angles 0 - 180
            prevPos = cam.ScreenToViewportPoint( Input.mousePosition );

            Ray ray = cam.ScreenPointToRay( Input.mousePosition );
            RaycastHit hit;

            if( Physics.Raycast( ray, out hit, 100000 ) ) {
                target = hit.transform;
            }
        }

        cam.transform.position = target.position;

        if( Input.GetMouseButton(0) ) {
            Vector3 direction = cam.ScreenToViewportPoint( Input.mousePosition ) - prevPos;

            cam.transform.Rotate( new Vector3(1, 0, 0), direction.y * 180 );
            cam.transform.Rotate( new Vector3(0, 1, 0), -direction.x * 180, Space.World );



            prevPos = cam.ScreenToViewportPoint( Input.mousePosition );
        }

        cam.transform.Translate( new Vector3(0, 0, -zoom) );
    }
}
