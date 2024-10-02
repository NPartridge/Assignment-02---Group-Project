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
    public float bossSpawnInterval = 30f;
    private float bossTimer;

    [Header("Spawn Settings")]
    public Transform player;
    public float playerSpawnRadius = 10f;

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
    // sense if the spawner timer is higher e.g. every 60 seconds or higher
    void SpawnBoss()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        //Debug.Log("Boss incoming!!");
    }

    Vector3 GetRandomSpawnPosition()
    {
        // A random point around the player that's within the spawn radiis
        Vector2 randomCircle = Random.insideUnitCircle * playerSpawnRadius;
        Vector3 position = player.position;
        Vector3 spawnPos = new Vector3(position.x + randomCircle.x, position.y, position.z + randomCircle.y);
        return spawnPos;
    }
}