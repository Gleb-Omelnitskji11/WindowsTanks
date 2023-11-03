using System;
using System.Linq;
using UnityEngine;

namespace Game.Enemy
{
    [CreateAssetMenu(fileName = "EnemiesConfig", menuName = "ScriptableObjects/EnemiesConfig", order = 0)]
    public class EnemyConfig : ScriptableObject
    {
        [SerializeField]
        private EnemyModel[] m_EnemyModels;

        public IEnemyBehavior GetEnemyBehavior(EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.SimpleEnemy:
                    return new SimpleEnemy();
                default:
                    throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
            }
        }

        public EnemyModel GetEnemyModel(EnemyType enemyType)
        {
            return m_EnemyModels.FirstOrDefault(x => x.EnemyType == enemyType);
        }
    }

    [Serializable]
    public class EnemyModel
    {
        public EnemyType EnemyType;
        public EnemyController TankPrefab;
    }
}

public enum EnemyType
{
    SimpleEnemy
}
