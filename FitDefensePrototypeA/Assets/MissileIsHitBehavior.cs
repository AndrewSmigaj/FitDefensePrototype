using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileIsHitBehavior : MonoBehaviour
{

    public GameObject explosion;
    public bool alreadyHit = false;
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

    }

    public void runExplosionAnim()
    {
        if (!alreadyHit)
        {
            alreadyHit = true;
            Instantiate(explosion, this.transform.position, this.transform.rotation);
            Destroy(this.transform.parent.gameObject);
        }

    }
}
