using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperbombGesturePrototypeScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject other;
    public GameObject shockWave;

    public bool isTriggered;

    public bool fireShockwave = true;

    public float rechargeTime = 5;
    public float currentTime = 0;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered && other.GetComponent<SecondSuperbombTriggerzoneScript>().isTriggered && fireShockwave)
        {
            Debug.Log("Got to trigger shockwave.");
            fireShockwave = false;
            shockWave.GetComponent<ShockwavePrototype>().TriggerWave();
            StartCoroutine(Recharge());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isTriggered = true;
        Debug.Log("First Zone Triggered");

    }

    private void OnTriggerExit(Collider other)
    {

        isTriggered = false;
    }


    IEnumerator Recharge()
    {
        while(currentTime < rechargeTime)
        {
            currentTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        fireShockwave = true;
        currentTime = 0;
    }
}
