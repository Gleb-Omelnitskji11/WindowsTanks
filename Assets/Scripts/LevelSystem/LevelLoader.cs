using System.Collections.Generic;
using Game.Bullet;
using Game.Enemy;
using Game.Tanks;
using Game.Player;
using UnityEngine;

namespace Game.LevelSystem
{
    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader LevelLoaderInstance;

        [SerializeField]
        private TankSpawner m_TankSpawner;

        private PlayerController m_Player;
        private readonly List<EnemyController> m_Enemies = new List<EnemyController>();

        private void Start()
        {
            LevelLoaderInstance = this;
            NextLevel();
            Subscribe();
        }

        public Level GetLevelData()
        {
            Level level = new Level();

            var transform1 = m_Player.transform;

            level.PlayerSaveModel = new TransformModel()
            {
                Position = transform1.position,
                Rotation = transform1.rotation
            };

            level.EnemySaveModels = new EnemySaveModel[m_Enemies.Count];

            for (int i = 0; i < m_Enemies.Count; i++)
            {
                EnemySaveModel enemySaveModel = m_Enemies[i].GetEnemySaveModel();
                level.EnemySaveModels[i] = enemySaveModel;
            }

            level.BulletModels = BulletSpawner.Instance.GetBulletsData();

            return level;
        }

        public void DownloadLevel(Level level)
        {
            LoadLevel(level);
            SpawnBullets(level.BulletModels);
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
            SpawnPlayer(level.PlayerSaveModel);
            SpawnEnemies(level.EnemySaveModels);
        }

        private void SpawnPlayer(TransformModel playerTransformModel)
        {
            m_Player = m_TankSpawner.InitPlayer(playerTransformModel);
            m_Player.Destroying += m_TankSpawner.SpawnPlayerInRandomCorner;
        }

        private void SpawnEnemies(EnemySaveModel[] enemySaveModels)
        {
            m_TankSpawner.ClearData();

            for (int i = 0; i < enemySaveModels.Length; i++)
            {
                EnemyController enemy = m_TankSpawner.SpawnEnemy(enemySaveModels[i]);
                m_Enemies.Add(enemy);
                enemy.Destroying += OnEnemyDestroy;
            }
        }

        private void SpawnBullets(BulletSaveModel[] bulletSaveModels)
        {
            foreach (var bulletModel in bulletSaveModels)
            {
                BulletSpawner.Instance.Shoot(bulletModel.TransformModel, bulletModel.Tag);
            }
        }

        private void OnEnemyDestroy(EnemyController enemyController)
        {
            m_Enemies.Remove(enemyController);
        }

        private void ClearLevel()
        {
            if (m_Player != null)
            {
                m_Player.Destroying -= m_TankSpawner.SpawnPlayerInRandomCorner;
                Destroy(m_Player.gameObject);
            }

            for (int i = 0; i < m_Enemies.Count; i++)
            {
                if (m_Enemies[i] != null)
                {
                    Destroy(m_Enemies[i].gameObject);
                }
            }

            m_Enemies.Clear();
            BulletSpawner.Instance.Clear();
        }

        private Level GetDefaultLevel()
        {
            Level defaultLevel = new Level
            {
                EnemySaveModels = new EnemySaveModel[5]
            };

            for (int i = 0; i < defaultLevel.EnemySaveModels.Length; i++)
            {
                defaultLevel.EnemySaveModels[i] = new EnemySaveModel()
                {
                    EnemyType = EnemyType.SimpleEnemy
                };
            }

            return defaultLevel;
        }
    }
}