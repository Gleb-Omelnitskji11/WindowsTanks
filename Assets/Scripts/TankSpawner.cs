using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TankSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] borders;
    [SerializeField] private Player playerPrefab;
    [SerializeField] private EnemyConfig enemyConfig;
    private List<EnemyController> enemies = new List<EnemyController>();
    private Player player;
    private (float, float, float, float) fieldForSpawn;

    public void LoadLevel(Level level)
    {
        fieldForSpawn = GetSpawnField();
        InitPlayer(level.playerSaveModel);
        StartCoroutine(SpawnEnemy(level.enemySaveModels));
    }

    private void InitPlayer(TransformModel playerPosition)
    {
        if (player != null)
        {
            player.Destroying -= RespawnPlayer;
            Destroy(player);
        }

        player = Instantiate(playerPrefab);
        player.Destroying += RespawnPlayer;

        if (playerPosition == null)
        {
            RespawnPlayer();
            return;
        }

        var playerTransform = player.transform;
        playerTransform.position = playerPosition.position;
        playerTransform.rotation = playerPosition.rotation;
    }

    private void RespawnPlayer()
    {
        int placeToSpawn = Random.Range(0, borders.Length);

        if (!IsSpawnPossible(borders[placeToSpawn].position))
        {
            RespawnPlayer();
            return;
        }

        var playerTransform = player.transform;
        playerTransform.position = borders[placeToSpawn].position;
        playerTransform.rotation = borders[placeToSpawn].rotation;
        player.gameObject.SetActive(true);
    }

    private IEnumerator SpawnEnemy(EnemySaveModel[] enemiesModels)
    {
        Vector2 position;
        Quaternion rotation;
        TransformModel enemyTransformModel;

        foreach (var enemy in enemiesModels)
        {
            EnemyModel enemyPrefab = enemyConfig.GetEnemyModel(enemy.enemyType);
            IEnemyBehavior enemyBehavior = enemyConfig.GetEnemyBehavior(enemy.enemyType);
            enemyTransformModel = enemy.transformModel;
            if (enemyTransformModel == null)
            {
                enemyTransformModel = GetRandomTransformModel();
            }

            EnemyController newEnemy = Instantiate(enemyPrefab.tankPrefab,
                enemyTransformModel.position, enemyTransformModel.rotation);

            newEnemy.Destroying += ClearEnemy;
            newEnemy.SetBehavior(enemyBehavior);
            newEnemy.Init(enemy.PatrollingData);
            enemies.Add(newEnemy);
            yield return null;
        }
    }

    private TransformModel GetRandomTransformModel()
    {
        TransformModel transformModel = new TransformModel()
        {
            rotation = Quaternion.identity,
            position = new Vector2(Random.Range(fieldForSpawn.Item1, fieldForSpawn.Item2),
                Random.Range(fieldForSpawn.Item3, fieldForSpawn.Item4))
        };

        while (!IsSpawnPossible(transformModel.position))
        {
            transformModel.position = new Vector2(Random.Range(fieldForSpawn.Item1, fieldForSpawn.Item2),
                Random.Range(fieldForSpawn.Item3, fieldForSpawn.Item4));
        }

        return transformModel;
    }

    private void ClearEnemy(EnemyController transform)
    {
        enemies.Remove(transform);
        if (enemies.Count == 0)
        {
            DestroyedAllEnemies?.Invoke();
        }
    }

    private bool IsSpawnPossible(Vector2 placeToSpawn)
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

    private (float, float, float, float) GetSpawnField()
    {
        Vector2[] corners = new Vector2[borders.Length];
        for (int i = 0; i < borders.Length; i++)
        {
            corners[i] = borders[i].position;
        }

        float minX = corners.Min(x => x.x);
        float maxX = corners.Max(x => x.x);
        float minY = corners.Min(x => x.y);
        float maxY = corners.Max(x => x.y);
        return (minX, maxX, minY, maxY);
    }

    public event Action DestroyedAllEnemies;
}