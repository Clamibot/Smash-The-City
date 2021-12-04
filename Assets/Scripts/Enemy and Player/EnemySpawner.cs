using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawner")]
    public GameObject enemyPrefab;
    public float spawnRate;
    private float nextSpawn = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        nextSpawn = spawnRate + Random.Range(5, 10);
        SpawnEnemy();
    }

    private void Update()
    {
        if(Time.timeScale == 1 && Time.time > nextSpawn)
        {
            SpawnEnemy();

            nextSpawn = Time.time + spawnRate + Random.Range(2, 8);
        }
    }

    public void SpawnEnemy()
    {
        if(enemyPrefab != null && Time.timeScale == 1)
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
