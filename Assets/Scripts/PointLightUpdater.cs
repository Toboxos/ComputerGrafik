using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointLightUpdater : MonoBehaviour
{
    public Light pointLight;

    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.material.SetVector( "_LightSource", pointLight.transform.position );
        renderer.material.SetColor( "_LightColor", pointLight.color );
    }
}
