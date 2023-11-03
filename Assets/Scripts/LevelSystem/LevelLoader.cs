using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader LevelLoaderInstance;

    [SerializeField] private TankSpawner m_TankSpawner;

    private Player player;
    private List<EnemyController> enemies = new List<EnemyController>();

    private void Start()
    {
        LevelLoaderInstance = this;
        NextLevel();
        Subscribe();
    }

    public Level GetLevelData()
    {
        Level level = new Level();
        level.playerSaveModel = new TransformModel()
        {
            position = player.transform.position,
            rotation = player.transform.rotation
        };

        level.enemySaveModels = new EnemySaveModel[enemies.Count];
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemySaveModel enemySaveModel = enemies[i].GetEnemySaveModel();
            level.enemySaveModels[i] = enemySaveModel;
        }

        level.bulletModels = BulletSpawner.Instance.GetBulletsData();

        return level;
    }

    public void DownloadLevel(Level level)
    {
        LoadLevel(level);
        SpawnBullets(level.bulletModels);
    }

    private void NextLevel()
    {
        Level level = GetDefaultLevel();
        LoadLevel(level);
    }

    private void Subscribe()
    {
        m_TankSpawner.DestroyedAllEnemies += NextLevel;
    }

    private void LoadLevel(Level level)
    {
        ClearLevel();
        SpawnPlayer(level.playerSaveModel);
        SpawnEnemies(level.enemySaveModels);
    }

    private void SpawnPlayer(TransformModel playerTransformModel)
    {
        player = m_TankSpawner.InitPlayer(playerTransformModel);
        player.Destroying += m_TankSpawner.SpawnPlayerInRandomCorner;
    }

    private void SpawnEnemies(EnemySaveModel[] enemySaveModels)
    {
        m_TankSpawner.ClearData();
        for (int i = 0; i < enemySaveModels.Length; i++)
        {
            EnemyController enemy = m_TankSpawner.SpawnEnemy(enemySaveModels[i]);
            enemies.Add(enemy);
            enemy.Destroying += OnEnemyDestroy;
        }
    }

    private void SpawnBullets(BulletSaveModel[] bulletSaveModels)
    {
        foreach (var bulletModel in bulletSaveModels)
        {
            BulletSpawner.Instance.Shoot(bulletModel.transformModel, bulletModel.tag);
        }
    }

    private void OnEnemyDestroy(EnemyController enemyController)
    {
        enemies.Remove(enemyController);
    }

    private void ClearLevel()
    {
        if (player != null)
        {
            player.Destroying -= m_TankSpawner.SpawnPlayerInRandomCorner;
            Destroy(player.gameObject);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
            {
                Destroy(enemies[i].gameObject);
            }
        }

        enemies.Clear();
        BulletSpawner.Instance.Clear();
    }

    private Level GetDefaultLevel()
    {
        Level defaultLevel = new Level
        {
            enemySaveModels = new EnemySaveModel[5]
        };

        for (int i = 0; i < defaultLevel.enemySaveModels.Length; i++)
        {
            defaultLevel.enemySaveModels[i] = new EnemySaveModel()
            {
                enemyType = EnemyType.simpleEnemy
            };
        }

        return defaultLevel;
    }
}