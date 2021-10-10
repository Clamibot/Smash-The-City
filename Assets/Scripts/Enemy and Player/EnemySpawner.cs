using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawner")]
    public GameObject enemyPrefab;
    public float spawnRate;
    private float nextSpawn;

    // Start is called before the first frame update
    private void Start()
    {
        nextSpawn = spawnRate + Random.Range(1, 5);
    }

    private void Update()
    {
        if(Time.timeScale == 0 && Time.time > nextSpawn)
        {
            SpawnEnemy();

            nextSpawn = Time.time + spawnRate + Random.Range(0, 2);
        }
    }

    public void SpawnEnemy()
    {
        if(enemyPrefab != null && Time.timeScale == 0)
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
