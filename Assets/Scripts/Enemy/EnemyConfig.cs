using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemiesConfig", menuName = "ScriptableObjects/EnemiesConfig", order = 0)]
public class EnemyConfig : ScriptableObject
{
    [SerializeField]
    private EnemyModel[] enemyModels;

    public IEnemyBehavior GetEnemyBehavior(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.simpleEnemy:
                return new SimpleEnemy();
            default:
                throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
        }
    }
    
    public EnemyModel GetEnemyModel(EnemyType enemyType)
    {
        return enemyModels.FirstOrDefault(x => x.enemyType == enemyType);
    }
}

[Serializable]
public class EnemyModel
{
    public EnemyType enemyType;
    public EnemyController tankPrefab;
    public Bullet projectPrefab;
}

public enum EnemyType
{
    simpleEnemy
}
