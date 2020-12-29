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
        zoom += -Input.mouseScrollDelta.y;
        if( zoom < 1 ) {
            zoom = 1;
        }

        // Left mouse button is pressed
        if( Input.GetMouseButtonDown(0) ) {

            // Set prevPos to current mouse position, otherwise mouse position from last mouse drag would be used
            prevPos = cam.ScreenToViewportPoint( Input.mousePosition );

            // Shoot ray and test if a planet has been clicked
            Ray ray = cam.ScreenPointToRay( Input.mousePosition );
            RaycastHit hit;

            if( Physics.Raycast( ray, out hit, 100000 )) {
                target = hit.transform;
            }
        }

        // Place camera into target
        cam.transform.position = target.position;

        if( Input.GetMouseButton(0) ) {

            // Difference of mouse movement between frames
            Vector3 direction = cam.ScreenToViewportPoint( Input.mousePosition ) - prevPos;

            // Handle longitude rotation
            cam.transform.Rotate( new Vector3(1, 0, 0), direction.y * 180 );

            // Handle latitude rotation (but in reference to World system, otherwise rotation would depend on longitude rotation)
            cam.transform.Rotate( new Vector3(0, 1, 0), -direction.x * 180, Space.World );

            // Save current position as previous position for next frame
            prevPos = cam.ScreenToViewportPoint( Input.mousePosition );
        }

        // Translate camera out of target based on zoom
        cam.transform.Translate( new Vector3( 0, 0, -zoom ) );
    }
}
