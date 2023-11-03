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

    private void Awake()
    {
        fieldForSpawn = GetSpawnField();
    }

    public void ClearData()
    {
        enemies.Clear();
    }

    public Player InitPlayer(TransformModel playerPosition)
    {
        if (player != null)
        {
            player.Destroying -= SpawnPlayerInRandomCorner;
            Destroy(player);
        }

        player = Instantiate(playerPrefab);
        player.Destroying += SpawnPlayerInRandomCorner;

        if (playerPosition == null)
        {
            SpawnPlayerInRandomCorner();
            return player;
        }

        var playerTransform = player.transform;
        playerTransform.position = playerPosition.position;
        playerTransform.rotation = playerPosition.rotation;
        return player;
    }

    public void SpawnPlayerInRandomCorner()
    {
        int placeToSpawn = Random.Range(0, borders.Length);

        while(!IsSpawnPossible(borders[placeToSpawn].position, false))
        {
            SpawnPlayerInRandomCorner();
            return;
        }

        var playerTransform = player.transform;
        playerTransform.position = borders[placeToSpawn].position;
        playerTransform.rotation = borders[placeToSpawn].rotation;
        player.gameObject.SetActive(true);
    }

    public EnemyController SpawnEnemy(EnemySaveModel enemy)
    {
        TransformModel enemyTransformModel = enemy.transformModel;

        EnemyModel enemyPrefab = enemyConfig.GetEnemyModel(enemy.enemyType);
        IEnemyBehavior enemyBehavior = enemyConfig.GetEnemyBehavior(enemy.enemyType);
        if (enemyTransformModel == null)
        {
            enemyTransformModel = GetRandomTransformModel();
        }

        EnemyController newEnemy = Instantiate(enemyPrefab.tankPrefab,
            enemyTransformModel.position, enemyTransformModel.rotation);

        newEnemy.Destroying += ClearEnemy;
        newEnemy.Init(enemyBehavior, enemy.stateData, enemy.enemyType);
        enemies.Add(newEnemy);
        return newEnemy;
    }

    private TransformModel GetRandomTransformModel()
    {
        TransformModel transformModel = new TransformModel()
        {
            rotation = Quaternion.identity,
            position = new Vector2(Random.Range(fieldForSpawn.Item1, fieldForSpawn.Item2),
                Random.Range(fieldForSpawn.Item3, fieldForSpawn.Item4))
        };

        while (!IsSpawnPossible(transformModel.position, true))
        {
            transformModel.position = new Vector2(Random.Range(fieldForSpawn.Item1, fieldForSpawn.Item2),
                Random.Range(fieldForSpawn.Item3, fieldForSpawn.Item4));
        }

        return transformModel;
    }

    private void ClearEnemy(EnemyController enemyController)
    {
        enemies.Remove(enemyController);
        if (enemies.Count == 0)
        {
            DestroyedAllEnemies?.Invoke();
        }
    }

    private bool IsSpawnPossible(Vector2 placeToSpawn, bool playerCheck)
    {
        const int minAllowableDistance = 10;
        float distance;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (!IsDistanceSufficient(enemies[i].transform.position, placeToSpawn))
                return false;
        }

        if (playerCheck && !IsDistanceSufficient(player.transform.position, placeToSpawn))
        {
            return false;
        }

        return true;

        bool IsDistanceSufficient(Vector2 point1, Vector2 point2)
        {
            distance = Vector2.Distance(point1, point2);

            return distance >= minAllowableDistance;
        }
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