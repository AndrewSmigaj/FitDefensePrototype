using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEnterTest : MonoBehaviour
{
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
        if (other.CompareTag("Missile"))
        {
            Debug.Log("TEST HANDTRIGGER");

            //explode
            if(other.gameObject != null)
            {
                other.gameObject.GetComponent<MissileIsHitBehavior>().runExplosionAnim();

            }

            // Destroy(other.gameObject.transform.parent.gameObject);


        }

    }
}
