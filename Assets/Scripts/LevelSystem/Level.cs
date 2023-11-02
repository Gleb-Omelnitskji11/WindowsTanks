using System;
using UnityEngine;

[Serializable]
public class Level
{
    public EnemySaveModel[] enemySaveModels;
    public TransformModel playerSaveModel;
}

[Serializable]
public class EnemySaveModel
{
    public EnemyType enemyType;
    public TransformModel transformModel;
    public PatrollingData PatrollingData;
}

[Serializable]
public class TransformModel
{
    public Vector2 position;
    public Quaternion rotation;
}