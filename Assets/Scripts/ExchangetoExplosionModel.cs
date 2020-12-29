using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangetoExplosionModel : MonoBehaviour
{
    public GameObject firstModel;
    public GameObject explosionModel;
  
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SpawnExplosionModel();
        }
    }
    public void SpawnExplosionModel()
    {
        Destroy(firstModel);
        GameObject explosionObject = Instantiate(explosionModel) as GameObject;
        explosionObject.GetComponent<ExplodeScript>().Explode();
    }
}
