using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeScript : MonoBehaviour
{

    public float force;
    public float radius;

    void Start()
    {
        Explode();
    }

    public void Explode()
    {
        foreach(Transform t in transform)
        {
            var rb = t.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
           
        }
    }
}