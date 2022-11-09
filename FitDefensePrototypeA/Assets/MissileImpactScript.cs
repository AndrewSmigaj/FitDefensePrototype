using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileImpactScript : MonoBehaviour
{

    public GameObject explosionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
    }
}
