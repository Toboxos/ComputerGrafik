using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointLightUpdater : MonoBehaviour {

    private Light pointLight;
    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        pointLight = gameObject.transform.parent.Find( "Sun" ).Find( "Point Light" ).GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.material.SetVector( "_LightSource", pointLight.transform.position );
        renderer.material.SetColor( "_LightColor", pointLight.color );
    }
}
