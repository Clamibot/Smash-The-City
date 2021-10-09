using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [Header("Building Spawner")]
    public List<GameObject> buildingPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBuilding();
    }

    public void SpawnBuilding()
    {
        if(buildingPrefabs.Count > 0)
            Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Count)]);
    }
}
