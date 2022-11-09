using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondSuperbombTriggerzoneScript : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isTriggered;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        isTriggered = true;
        Debug.Log("SecondZone Triggered.");
    }

    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }
}
