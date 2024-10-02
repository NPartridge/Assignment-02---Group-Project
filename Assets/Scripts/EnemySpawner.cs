using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public int maxEnemies = 5;
    public float spawnInterval = 5f;
    private float enemyTimer;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public float bossSpawnInterval = 60f;
    private float bossTimer;

    [Header("Spawn Settings")]
    public Transform player;
    
    // These are the radii around the player that enemies will spawn under. We don't want enemies spawning either
    // too close or too far away from the player.
    public float maximumSpawnDistance = 30f;
    public float minSpawnDistance = 20f;

    void Update()
    {
        enemyTimer += Time.deltaTime;
        bossTimer += Time.deltaTime;

        // Spawn regular enemies
        if (enemyTimer >= spawnInterval)
        {
            enemyTimer = 0f;
            SpawnEnemy();
        }

        // Spawn boss enemy
        if (bossTimer >= bossSpawnInterval)
        {
            bossTimer = 0f;
            SpawnBoss();
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    void SpawnEnemy()
    {
        // Checks how many enemies we have in the scene
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Currently bosses will spawn repeatedly which looks quite odd if the spawn timer is low but this will make more
    // sense if the spawner timer is higher e.g. every 60 seconds or more
    void SpawnBoss()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        //Debug.Log("Boss incoming!!");
    }

    Vector3 GetRandomSpawnPosition()
    {
        float minDistance = minSpawnDistance;
        float maxDistance = maximumSpawnDistance;
        
        // A random angle between 0-360 degrees
        float angle = Random.Range(0f, Mathf.PI * 2);

        // A random distance between the min and max distance
        float distance = Random.Range(minDistance, maxDistance);

        // The x and z offsets of the players pos
        float xOffset = Mathf.Cos(angle) * distance;
        float zOffset = Mathf.Sin(angle) * distance;

        Vector3 position = player.position;
        Vector3 spawnPos = new Vector3(position.x + xOffset, position.y, position.z + zOffset);
        return spawnPos;
    }
}