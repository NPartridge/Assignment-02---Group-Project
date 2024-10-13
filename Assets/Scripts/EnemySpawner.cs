using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform player;

    // These are the radii around the player that enemies will spawn under. We don't want enemies spawning either
    // too close or too far away from the player.
    public float maximumSpawnDistance = 30f;
    public float minSpawnDistance = 20f;

    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public int maxEnemies;
        public float spawnInterval;
        [HideInInspector]
        public float spawnTimer;
        public Enemy.EnemyType enemyType;
    }

    public List<EnemySpawnData> enemySpawnDataList;

    void Update()
    {
        foreach (var data in enemySpawnDataList)
        {
            data.spawnTimer += Time.deltaTime;
            if (data.spawnTimer >= data.spawnInterval)
            {
                data.spawnTimer = 0f;
                SpawnEnemy(data);
            }
        }
    }

    void SpawnEnemy(EnemySpawnData data)
    {
        if (CountEnemiesOfType(data.enemyType) < data.maxEnemies)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject enemyObj = Instantiate(data.enemyPrefab, spawnPosition, Quaternion.identity);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.enemyType = data.enemyType;
        }
    }

    int CountEnemiesOfType(Enemy.EnemyType type)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int count = 0;
        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null && enemy.enemyType == type)
            {
                count++;
            }
        }
        return count;
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