using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSpawning : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] missileSpawners = new GameObject[4];
    public GameObject missilePrefab;
    public Transform railSystemPivot;
    public Transform HUDPivot;
    public GameObject dialogue;

    bool isLaunchingMissiles = true;

    void Start()
    {
        StartCoroutine(PivotControl());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaunchMissile(int spawnIndex)
    {
        if (isLaunchingMissiles)
        {
            Instantiate(missilePrefab, missileSpawners[spawnIndex].transform.position, missileSpawners[spawnIndex].transform.rotation);

        }
    }

    IEnumerator PivotControl()
    {
        yield return new WaitForSeconds(10);

        dialogue.SetActive(true);
        isLaunchingMissiles = false;
        //dialogue trigger

        yield return new WaitForSeconds(3);
        dialogue.SetActive(false);
        
        railSystemPivot.transform.Rotate(new Vector3(0, 180, 0));
        HUDPivot.transform.Rotate(new Vector3(0, 180, 0));
        isLaunchingMissiles = true;
    }
}
