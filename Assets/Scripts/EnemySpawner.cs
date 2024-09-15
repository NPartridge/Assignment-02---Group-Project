using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float playerSpawnRadius = 10f;
    public int maxEnemies = 5;
    public float spawnInterval = 5f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
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

    Vector3 GetRandomSpawnPosition()
    {
        // A random point around the player that's within the spawn radiis
        Vector2 randomCircle = Random.insideUnitCircle * playerSpawnRadius;
        var position = player.position;
        Vector3 spawnPos = new Vector3(position.x + randomCircle.x, position.y, position.z + randomCircle.y);
        return spawnPos;
    }
}