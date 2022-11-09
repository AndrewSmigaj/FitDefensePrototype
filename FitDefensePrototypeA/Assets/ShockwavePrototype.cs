using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwavePrototype : MonoBehaviour
{
    // Start is called before the first frame update
    float maxScale = 7;
    float currentScale = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator testShockwaveLogic()
    {
        while(currentScale < maxScale)
        {
            currentScale += 0.1f;

            this.transform.localScale = Vector3.one * currentScale;
            yield return new WaitForSeconds(0.025f);
        }
        currentScale = 1;
        this.transform.localScale = Vector3.one * currentScale;
    }

    public void TriggerWave()
    {
        StartCoroutine(testShockwaveLogic());
    }
}
