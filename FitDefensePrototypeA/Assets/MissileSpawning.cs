using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSpawning : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] missileSpawners = new GameObject[4];
    public GameObject missilePrefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaunchMissile(int spawnIndex)
    {
        Instantiate(missilePrefab, missileSpawners[spawnIndex].transform.position, missileSpawners[spawnIndex].transform.rotation);
    }
}
