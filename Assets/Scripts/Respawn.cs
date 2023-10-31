using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform[] spawnPoints;
    public List<EnemyController> enemies = new List<EnemyController>();
    public PlayerController player;

    private void Awake()
    {
        SpawnPlayer();
        player.Destroying += SpawnPlayer;

        foreach (var enemy in enemies)
        {
            enemy.Destroying += ClearEnemy;
        }
    }

    private void ClearEnemy(EnemyController transform)
    {
        enemies.Remove(transform);
    }

    private void SpawnPlayer()
    {
        int placeToSpawn = Random.Range(0, spawnPoints.Length);
        if (!CheckForEnemies(spawnPoints[placeToSpawn].position))
        {
            SpawnPlayer();
            return;
        }

        var transform1 = player.transform;
        transform1.position = spawnPoints[placeToSpawn].position;
        transform1.rotation = spawnPoints[placeToSpawn].rotation;
        player.gameObject.SetActive(true);
    }

    private bool CheckForEnemies(Vector2 placeToSpawn)
    {
        const int minAllowableDistance = 10;
        float distance;
        
        for (int i = 0; i < enemies.Count; i++)
        {
            distance = Vector2.Distance(enemies[i].transform.position, placeToSpawn);

            if (distance < minAllowableDistance)
                return false;
        }
        
        return true;
    }
}
