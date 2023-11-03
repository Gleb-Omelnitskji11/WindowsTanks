using System;
using System.Collections.Generic;
using System.Linq;
using Game.Enemy;
using Game.LevelSystem;
using Game.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Tanks
{
    public class TankSpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform[] m_Borders;

        [SerializeField]
        private PlayerController m_PlayerPrefab;

        [SerializeField]
        private EnemyConfig m_EnemyConfig;

        private readonly List<EnemyController> m_Enemies = new List<EnemyController>();
        private PlayerController m_PlayerController;
        private (float, float, float, float) m_FieldForSpawn;

        private void Awake()
        {
            m_FieldForSpawn = GetSpawnField();
        }

        public void ClearData()
        {
            m_Enemies.Clear();
        }

        public PlayerController InitPlayer(TransformModel playerPosition)
        {
            if (m_PlayerController != null)
            {
                m_PlayerController.Destroying -= SpawnPlayerInRandomCorner;
                Destroy(m_PlayerController);
            }

            m_PlayerController = Instantiate(m_PlayerPrefab);
            m_PlayerController.Destroying += SpawnPlayerInRandomCorner;

            if (playerPosition == null)
            {
                SpawnPlayerInRandomCorner();
                return m_PlayerController;
            }

            var playerTransform = m_PlayerController.transform;
            playerTransform.position = playerPosition.Position;
            playerTransform.rotation = playerPosition.Rotation;
            return m_PlayerController;
        }

        public void SpawnPlayerInRandomCorner()
        {
            int placeToSpawn = Random.Range(0, m_Borders.Length);

            while (!IsSpawnPossible(m_Borders[placeToSpawn].position, false))
            {
                SpawnPlayerInRandomCorner();
                return;
            }

            var playerTransform = m_PlayerController.transform;
            playerTransform.position = m_Borders[placeToSpawn].position;
            playerTransform.rotation = m_Borders[placeToSpawn].rotation;
            m_PlayerController.gameObject.SetActive(true);
        }

        public EnemyController SpawnEnemy(EnemySaveModel enemy)
        {
            TransformModel enemyTransformModel = enemy.TransformModel;

            EnemyModel enemyPrefab = m_EnemyConfig.GetEnemyModel(enemy.EnemyType);
            IEnemyBehavior enemyBehavior = m_EnemyConfig.GetEnemyBehavior(enemy.EnemyType);

            if (enemyTransformModel == null)
            {
                enemyTransformModel = GetRandomTransformModel();
            }

            EnemyController newEnemy = Instantiate(enemyPrefab.TankPrefab, enemyTransformModel.Position,
                enemyTransformModel.Rotation);

            newEnemy.Destroying += ClearEnemy;
            newEnemy.Init(enemyBehavior, enemy.StateData, enemy.EnemyType);
            m_Enemies.Add(newEnemy);
            return newEnemy;
        }

        private TransformModel GetRandomTransformModel()
        {
            TransformModel transformModel = new TransformModel()
            {
                Rotation = Quaternion.identity,
                Position = new Vector2(Random.Range(m_FieldForSpawn.Item1, m_FieldForSpawn.Item2),
                    Random.Range(m_FieldForSpawn.Item3, m_FieldForSpawn.Item4))
            };

            while (!IsSpawnPossible(transformModel.Position, true))
            {
                transformModel.Position = new Vector2(Random.Range(m_FieldForSpawn.Item1, m_FieldForSpawn.Item2),
                    Random.Range(m_FieldForSpawn.Item3, m_FieldForSpawn.Item4));
            }

            return transformModel;
        }

        private void ClearEnemy(EnemyController enemyController)
        {
            m_Enemies.Remove(enemyController);

            if (m_Enemies.Count == 0)
            {
                DestroyedAllEnemies?.Invoke();
            }
        }

        private bool IsSpawnPossible(Vector2 placeToSpawn, bool playerCheck)
        {
            const int minAllowableDistance = 10;
            float distance;

            foreach (EnemyController enemy in m_Enemies)
            {
                if (!IsDistanceSufficient(enemy.transform.position, placeToSpawn))
                    return false;
            }

            if (playerCheck && !IsDistanceSufficient(m_PlayerController.transform.position, placeToSpawn))
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
            Vector2[] corners = new Vector2[m_Borders.Length];

            for (int i = 0; i < m_Borders.Length; i++)
            {
                corners[i] = m_Borders[i].position;
            }

            float minX = corners.Min(x => x.x);
            float maxX = corners.Max(x => x.x);
            float minY = corners.Min(x => x.y);
            float maxY = corners.Max(x => x.y);
            return (minX, maxX, minY, maxY);
        }

        public event Action DestroyedAllEnemies;
    }
}